using System;
using Godot;

namespace leafy.entities;

[GlobalClass]
public partial class Mushroom : EntityNode2D
{
    [Export] public Vector2 GrowTimes = new(4, 6);
    [Export] public Vector2 Angles = new(-2f, 2f);
    [Export] public Vector2 Scales = new(0.9f, 1.1f);

    
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
        Noise.SetFrequency(.1f);
        Noise.SetFractalOctaves(3);
        Noise.SetFractalLacunarity(2.0f);
        Noise.SetFractalGain(0.5f);
    }
    
    public override void _EnterTree()
    {
        base._EnterTree();
        
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _frames = _sprite.GetSpriteFrames().GetFrameCount(_sprite.Animation);
        
        _seed = Random.Shared.NextSingle();
        _speed = Random.Shared.NextSingle();
        
        _orientation = new(Random.Shared.NextSingle() > 0.5f ? 1f : -1f, 1f);
        
        Entity.Add(this);
        Entity.Add(_sprite);
    }

    public void Grow(double delta)
    {
        _growth += (float) delta / GrowTimes.Remap(_speed);
        
        var frame = (int)Mathf.Floor(_growth * _frames);
        if (_sprite.Frame != frame)
        {
            _sprite.Frame = frame;
        }
        _sprite.Scale = Scales.Remap(Noise.GetNoise2Dv(scaleGene)) * _orientation;
        _sprite.RotationDegrees = Angles.Remap(Noise.GetNoise2Dv(angleGene));
    }
    
    public override void _ExitTree()
    {
        Entity.Remove<Mushroom>();
        base._ExitTree();
    }
}