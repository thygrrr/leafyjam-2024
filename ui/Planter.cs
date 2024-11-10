using System;
using Godot;
using leafy.entities;

// ReSharper disable StringLiteralTypo

namespace leafy.ui;

public partial class Planter : Node2D
{
    [Export]
    private AudioStream[] _plantableSounds = [];

    [Export]
    private AudioStream[] _harvestSounds = [];

    [Export] private Vector2 _pitchRange = new(0.9f, 1.1f);

    private AnimatedSprite2D _sprite;

    private Ecosystem _ecosystem;

    private State _state;
    public override void _Ready()
    {
        base._Ready();
        _ecosystem = GetParent<Ecosystem>();
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _sound = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
    }


    private Mushroom _plantable;
    private AudioStreamPlayer2D _sound;

    private ShroomTraits _traits;
    private ShroomTraits _fusion;
    
    public override void _Input(InputEvent input)
    {
        if (_plantable is { Deleting: false })
        {
            _plantable.Modulate = Colors.White;
        }

        _plantable = null;
        _traits = default;

        if (input is InputEventMouse mouseMotion)
        {
            if (_ecosystem.ClosestPlantPosition(mouseMotion.Position, out var shroom, out var closestPoint))
            {
                Position = closestPoint;
                _traits = shroom.traits;

                _ecosystem.GatherTraits(closestPoint, out _fusion);
                _sprite.Animation = PickSpriteAnimation(_traits);

                _plantable = shroom;
                _state = State.Planting;
            }
            else if (_ecosystem.ClosestHarvestable(mouseMotion.Position, out var mushroom))
            {
                _plantable = mushroom;
                _plantable.Modulate = Colors.Red;
                _state = State.Harvesting;
                _sprite.Animation = "default";
            }
            else
            {
                Position = mouseMotion.Position;
                _state = State.Idle;
                _sprite.Animation = "default";
            }
        }

        if (input is InputEventMouseButton { ButtonIndex: MouseButton.Left } button && button.IsPressed())
        {
            switch (_state)
            {
                case State.Planting:
                {
                    var planted = ResourceLoader.Load<PackedScene>(_plantable?.SceneFilePath).Instantiate<Mushroom>();
                    planted.Position = Position;
                    planted.fusion = _fusion;
                    GetParent().AddChild(planted);
                    var sound = _plantableSounds[Random.Shared.Next(_plantableSounds.Length)];
                    _sound.Stream = sound;
                    _sound.PitchScale = _pitchRange.Remap(Random.Shared.NextSingle());
                    _sound.Play();
                    break;
                }

                case State.Harvesting:
                {
                    var sound = _harvestSounds[Random.Shared.Next(_harvestSounds.Length)];
                    _sound.Stream = sound;
                    _sound.PitchScale = _pitchRange.Remap(Random.Shared.NextSingle());
                    _sound.Play();

                    _plantable?.QueueFree();
                    break;
                }
            }
            
            _plantable = null;
            _state = State.Idle;
        }
    }

    private StringName PickSpriteAnimation(ShroomTraits traits)
    {
        // We select the shape based on the traits.
        if (traits.HasFlag(ShroomTraits.Honey))return "hint_honeymash";
        if (traits.HasFlag(ShroomTraits.Toad)) return "hint_toadstool";
        if (traits.HasFlag(ShroomTraits.Porc)) return "hint_porcini";
        
        return "default";
    }

    public enum State
    {
        Idle,
        Planting,
        Harvesting,
    }
}
