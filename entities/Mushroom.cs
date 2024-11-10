using System;
using Godot;

namespace leafy.entities;

[GlobalClass]
public partial class Mushroom : EntityNode2D
{
    private Node2D _navTarget;

    [Export]
    private Vector2 _jitterRange = new(-2f, 2f);
    
    [Export]
    private Vector2 _lumiRange = new(0.9f, 1.0f);
    [Export]
    private Vector2 _satuRange = new(0.0f, 0.1f);
    [Export]
    private Vector2 _hueRange = new(0f, 1.0f);
    
    
    [Export]
    private Vector2 _growTimes = new(4, 6);
    [Export]
    private Vector2 _angles = new(-3f, 3f);
    [Export]
    private Vector2 _scales = new(0.9f, 1.1f);

    [Export]
    private Vector2 _pitchRange = new(0.9f, 1.1f);

    [Export]
    public Vector2 plantRange = new(50, 100);

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
    
        _navTarget = GetNode<Node2D>("NavigationTarget");
        _navTarget.Connect("start_growing", Callable.From(StartGrowing));
        
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        _sprite.Modulate = Color.FromOkHsl(
            _hueRange.Remap(Random.Shared.NextSingle()),
            _satuRange.Remap(Random.Shared.NextSingle()), 
            _lumiRange.Remap(Random.Shared.NextSingle()));

        _sprite.Position = new(
            _jitterRange.Remap(Random.Shared.NextSingle()), 
            _jitterRange.Remap(Random.Shared.NextSingle())
        );

        _frames = _sprite.GetSpriteFrames().GetFrameCount(_sprite.Animation);

        _seed = Random.Shared.NextSingle();
        _speed = Random.Shared.NextSingle();

        _orientation = new(Random.Shared.NextSingle() > 0.5f ? 1f : -1f, 1f);

        Entity.Add(_sprite);

        //NOTE: This is something fennecs should do better, one of the overloads is broken in 0.5.x
        Entity.Add(new Position(Position.X, Position.Y));
        
        Grow(0);
    }


    public void Grow(float dt)
    {
        _growth += dt / _growTimes.Remap(_speed);

        if (_growth > 1f)
        {
            Entity.Remove<Growing>();
        }

        var frame = Mathf.RoundToInt(_growth * _frames);
        if (_sprite.Frame != frame)
        {
            _sprite.Frame = frame;
        }
        _sprite.Scale = _scales.Remap(Noise.GetNoise2Dv(scaleGene)) * _orientation;
        _sprite.RotationDegrees = _angles.Remap(Noise.GetNoise2Dv(angleGene));
    }

    public void Mature()
    {
        if (_growth > 0.7f)
        {
            Entity.Add<Mature>();
        }
    }

    public void Idle(float dt)
    {
        _growth += dt / _growTimes.Remap(_speed) * 0.5f;
        _sprite.RotationDegrees = _angles.Remap(Noise.GetNoise2Dv(angleGene)); 
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        Entity.Add(this);
    }

    private void StartGrowing()
    {
        Entity.Add<Growing>();
        var audio = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        audio.PitchScale = _pitchRange.Remap(Random.Shared.NextSingle());
        audio.Play();
    }
}
