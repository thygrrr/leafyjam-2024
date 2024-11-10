using System;
using System.Collections.Generic;
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
    public ShroomTraits fusion;

    private float _seed;
    private float _speed;
    private float _growth;

    private Vector2 _orientation;

    private Vector2 scaleGene => new(_seed * 100f, _growth);
    private Vector2 angleGene => new(_seed * 100f, _growth);

    private AnimatedSprite2D _sprite;
    private int _frames;

    private static Dictionary<ShroomTraits, (Vector4 pulse, float glow, float amplitude, Color color1, Color color2)> _traitColors;
    //private static Dictionary<ShroomTraits, Color> _traitColors = new();
    //private static Dictionary<ShroomTraits, float> _traitGlows = new();
    
    private static readonly FastNoiseLite Noise = new();
    static Mushroom()
    {
        Noise.SetNoiseType(FastNoiseLite.NoiseTypeEnum.Simplex);
        Noise.SetFrequency(.5f);
        Noise.SetFractalOctaves(3);
        Noise.SetFractalLacunarity(2.0f);
        Noise.SetFractalGain(0.5f);
        
        _traitColors = new()
        {
            #region Basic
            {
                ShroomTraits.Toad, 
                (
                    new(0, 0, 0, 0), 
                    0.0f, 0.0f, 
                    Color.FromHtml("#CEC8B6"), Color.FromHtml("#913426")
                )
            },
            {
                ShroomTraits.Honey, 
                (
                    new(0, 0, 0, 0), 
                    0.0f, 0.0f, 
                    Color.FromHtml("#B86D30"), Color.FromHtml("#00ff00")
                )
            },
            {
                ShroomTraits.Porc, 
                (
                    new(0, 0, 0, 0), 
                    0.0f, 0.0f, 
                    Color.FromHtml("#977743"), Color.FromHtml("#5F412C")
                )
            },
            #endregion
            
            #region 2-Traits
            {ShroomTraits.Toad | ShroomTraits.Honey, 
                (
                    new(7, 4, 7, 7), 
                    1.5f,  0.3f, 
                    Color.FromHtml("#D95763"), Color.FromHtml("#D95763")
                )
            },
            {ShroomTraits.Toad | ShroomTraits.Porc,                 (
                    new(5, 4, 6, 9), 
                    1.5f, 0.3f, 
                    Color.FromHtml("#5B6EE1"), Color.FromHtml("#306082")
                )
            },

            {ShroomTraits.Honey | ShroomTraits.Porc,                 (
                    new(4, 7, 12, 12), 
                    1.5f, 0.3f, 
                    Color.FromHtml("#FBF236"), Color.FromHtml("#DF7126")
                )
            },
            
            #endregion

            {ShroomTraits.Porc | ShroomTraits.Toad | ShroomTraits.Honey,                 (
                    new(3, 4, 2, 5), 
                    -0.5f, 0.5f, 
                    Color.FromHtml("#ffff00"), Color.FromHtml("#00ffff")
                )
            },
        };
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
        
        InitializeTraits();
    }

    private void InitializeTraits()
    {
        //GD.Print($"Traits: {traits}, Fusion: {fusion}");

        fusion |= traits;
        
        // Select animation if procedural
        if (traits != fusion)
        {
            var shaderParams = _traitColors[fusion];
            var material = (ShaderMaterial)_sprite.Material.Duplicate();
            material.SetShaderParameter("color_1", shaderParams.color1);
            material.SetShaderParameter("color_2", shaderParams.color2);

            // Phase shift the pulse
            var pulse = shaderParams.pulse;
            pulse.Z *= Random.Shared.NextSingle();
            pulse.W *= Random.Shared.NextSingle();
            
            material.SetShaderParameter("pulse", pulse);
            material.SetShaderParameter("glow", shaderParams.glow);
            material.SetShaderParameter("amplitude", shaderParams.amplitude);
            _sprite.Material = material;
            
            switch (traits)
            {
                case ShroomTraits.Toad:
                    _sprite.Animation = "hybrid_toadstool";
                    break;
                case ShroomTraits.Honey:
                    _sprite.Animation = "hybrid_honeymash";
                    break;
                case ShroomTraits.Porc:
                    _sprite.Animation = "hybrid_porcini";
                    break;
            }
        }
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
