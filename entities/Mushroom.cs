using System;
using Godot;

namespace leafy.entities;

[GlobalClass]
public partial class Mushroom : EntityNode2D
{
    [Export]
    private Vector2 _growTimes = new(4, 6);
    [Export]
    private Vector2 _angles = new(-3f, 3f);
    [Export]
    private Vector2 _scales = new(0.9f, 1.1f);

    [Export]
    public Vector2 plantRange = new(20, 40);

    [Export]
    public float pickRange = 20;

    [Export(PropertyHint.Enum)]
    public ShroomTraits traits = default;

    private float _seed;
    private float _speed;
    private float _growth;

    private Vector2 _orientation;

    private Vector2 scaleGene => new(_seed * 100f, _growth);
    private Vector2 angleGene => new(_seed * 100f, _growth);

    private AnimatedSprite2D _sprite;
    private int _frames;


    private static readonly FastNoiseLite Noise = new();
    static Mushroom()
    {
        Noise.SetNoiseType(FastNoiseLite.NoiseTypeEnum.Simplex);
        Noise.SetFrequency(.5f);
        Noise.SetFractalOctaves(3);
        Noise.SetFractalLacunarity(2.0f);
        Noise.SetFractalGain(0.5f);
    }

    public override void _Ready()
    {
        base._Ready();

        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _frames = _sprite.GetSpriteFrames().GetFrameCount(_sprite.Animation);

        _seed = Random.Shared.NextSingle();
        _speed = Random.Shared.NextSingle();

        _orientation = new(Random.Shared.NextSingle() > 0.5f ? 1f : -1f, 1f);

        Entity.Add(_sprite);

        Entity.Add<Growing>();

        //NOTE: This is something fennecs should do better, one of the overloads is broken in 0.5.x
        Entity.Add(new Position(Position.X, Position.Y));
    }


    public void Grow(double delta)
    {
        _growth += (float)delta / _growTimes.Remap(_speed);

        if (_growth > 1f)
        {
            Entity.Remove<Growing>();
            Entity.Add<Mature>();
        }

        var frame = (int)Mathf.Floor(_growth * _frames);
        if (_sprite.Frame != frame)
        {
            _sprite.Frame = frame;
        }
        _sprite.Scale = _scales.Remap(Noise.GetNoise2Dv(scaleGene)) * _orientation;
        _sprite.RotationDegrees = _angles.Remap(Noise.GetNoise2Dv(angleGene));
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        Entity.Add(this);
    }

    public override void _ExitTree()
    {
        Entity.Despawn();
        base._ExitTree();
    }
}
