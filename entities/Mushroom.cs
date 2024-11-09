using System;
using Godot;

namespace leafy.entities;

[GlobalClass]
public partial class Mushroom : EntityNode2D
{
    [Export] public Vector2 ScaleRange = new(0.9f, 1.1f);
    [Export] public Vector2 AngleRange = new(-2f, 2f);

    private static readonly FastNoiseLite Noise = new();
    
    private float _seed;

    private float _growth;

    private Vector2 scaleGene => new(_seed * 100f, _growth);
    private Vector2 angleGene => new(_seed * 100f, _growth);
    
    private AnimatedSprite2D _sprite;
    static Mushroom()
    {
        Noise.SetNoiseType(FastNoiseLite.NoiseTypeEnum.Simplex);
        Noise.SetFrequency(.1f);
        Noise.SetFractalOctaves(3);
        Noise.SetFractalLacunarity(2.0f);
        Noise.SetFractalGain(0.5f);
    }
    
    public override void _Ready()
    {
        base._Ready();
    }
    
    public override void _EnterTree()
    {
        base._EnterTree();
        
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _seed = Random.Shared.NextSingle();

        Entity.Add(this);
        Entity.Add(_sprite);
    }

    public void Grow(double delta)
    {
        _growth += (float) delta;
        _sprite.Frame = (int) Mathf.Floor(_growth);
        _sprite.Scale = ScaleRange.MapMinMaxV(Noise.GetNoise2Dv(scaleGene));
        _sprite.RotationDegrees = AngleRange.MapMinMax(Noise.GetNoise2Dv(angleGene));
    }
    
    public override void _ExitTree()
    {
        Entity.Remove<Mushroom>();
        base._ExitTree();
    }
}