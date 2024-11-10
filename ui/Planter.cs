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

    public override void _Input(InputEvent input)
    {
        if (input is InputEventMouseMotion mouseMotion)
        {
            if (_ecosystem.ClosestPlantPosition(mouseMotion.Position, out var shroom, out var closestPoint))
            {
                Position = closestPoint;
                var traits = shroom.traits;

                _sprite.Animation = traits switch
                {
                    ShroomTraits.Trait1 => "hint_honeymash",
                    ShroomTraits.Trait2 => "hint_toadstool",
                    ShroomTraits.Trait3 => "hint_porcini",
                    _ => "default",
                };

                if (_plantable != null)
                {
                    _plantable.Modulate = Colors.White;
                    _plantable = null;
                }
                _plantable = shroom;
                _state = State.Planting;
            }
            else if (_ecosystem.ClosestHarvestable(mouseMotion.Position, out var mushroom))
            {
                _plantable = mushroom;
                _plantable.Modulate = Colors.Red;
                _state = State.Harvesting;
            }
            else
            {
                if (_plantable != null)
                {
                    _plantable.Modulate = Colors.White;
                    _plantable = null;
                }
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
                    var planted = ResourceLoader.Load<PackedScene>(_plantable.SceneFilePath).Instantiate<Mushroom>();
                    planted.Position = Position;
                    GetParent().AddChild(planted);

                {
                    var sound = _plantableSounds[Random.Shared.Next(_plantableSounds.Length)];
                    _sound.Stream = sound;
                    _sound.PitchScale = _pitchRange.Remap(Random.Shared.NextSingle());
                    _sound.Play();
                }
                break;

                case State.Harvesting:
                {
                    var sound = _harvestSounds[Random.Shared.Next(_harvestSounds.Length)];
                    _sound.Stream = sound;
                    _sound.PitchScale = _pitchRange.Remap(Random.Shared.NextSingle());
                    _sound.Play();

                    _plantable.QueueFree();
                    _plantable = null;
                    _state = State.Idle;
                }
                break;
            }
        }
    }

    public enum State
    {
        Idle,
        Planting,
        Harvesting,
    }
}
