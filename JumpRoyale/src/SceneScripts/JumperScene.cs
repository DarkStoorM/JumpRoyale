using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;
using JumpRoyale.Events;

namespace JumpRoyale;

public partial class JumperScene : CharacterBody2D
{
    /// <summary>
    /// Stores this jumper's position in the last frame. See <see cref="StorePosition"/>.
    /// </summary>
    private readonly HashSet<Vector2> _recentPosition = [];

    [AllowNull]
    private Jumper _jumper;

    private Label _namePlate = null!;
    private CpuParticles2D _glow = null!;
    private Vector2 _jumpVelocity;

    /// <summary>
    /// Previous X Velocity value of this jumper before MoveAndSlide was called. Used to bounce the jumper off the wall.
    /// </summary>
    private float _previousXVelocity;

    /// <summary>
    /// Indicated for how many frames this jumper has been in the same place under certain conditions. See <see
    /// cref="StorePosition"/>.
    /// </summary>
    private int _framesSincePositionChange;

    public void Init(Jumper jumper)
    {
        _namePlate = GetNode<Label>("NamePlate");
        _glow = GetNode<CpuParticles2D>("Glow");

        _jumper = jumper;

        // Listen to Command Execution events coming from twitch chat
        _jumper.JumperEventsManager.OnJumpCommandEvent += OnJumpCommandEvent;
        _jumper.JumperEventsManager.OnDisableGlowEvent += OnDisableGlowEvent;
        _jumper.JumperEventsManager.OnSetCharacterEvent += OnSetCharacterEvent;
        _jumper.JumperEventsManager.OnSetGlowColorEvent += OnSetGlowColorEvent;
        _jumper.JumperEventsManager.OnSetNameColorEvent += OnSetNameColorEvent;

        Name = _jumper.PlayerData.Name;
        _namePlate.Text = Name;

        // Remaining from the old codebase:
        // - Set character choice (sprite, requires the animation switcher class)
        HandleNameColorEvent();

        if (_jumper.PlayerData.IsPrivileged)
        {
            HandleGlowColorEvent();
        }
        else
        {
            // Disable the glow anyway in case the player lost his privileged in-between the game sessions
            HandleDisableGlowEvent();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // This order is important: apply gravity -> stop if touched the floor -> then apply the jump velocity
        ApplyInitialVelocity(delta);
        StopIfOnFloor();
        ApplyJumpVelocity();
        BounceOffWall();

        // Remaining from the old code:
        // Flip the sprite if necessary
        // Switch the Sprite animation if jumped
        _previousXVelocity = Velocity.X;

        MoveAndSlide();
        StorePosition();
    }

    private void OnJumpCommandEvent(object sender, JumpCommandEventArgs args)
    {
        HandleJumpEvent(args.JumpAngle, args.JumpPower);
    }

    private void OnDisableGlowEvent(object sender, DisableGlowEventArgs args)
    {
        HandleDisableGlowEvent();
    }

    private void OnSetCharacterEvent(object sender, SetCharacterEventArgs args)
    {
        GD.Print("OnSetCharacterEvent");
    }

    private void OnSetGlowColorEvent(object sender, SetGlowColorEventArgs args)
    {
        HandleGlowColorEvent();
    }

    private void OnSetNameColorEvent(object sender, SetNameColorEventArgs args)
    {
        HandleNameColorEvent();
    }

    private void HandleJumpEvent(int angle, int power)
    {
        if (IsOnFloor())
        {
            double normalizedPower = Math.Sqrt(power * 5 * GameConstants.Gravity);

            _jumpVelocity.X = Mathf.Cos(Mathf.DegToRad(angle + 180));
            _jumpVelocity.Y = Mathf.Sin(Mathf.DegToRad(angle + 180));
            _jumpVelocity = _jumpVelocity.Normalized() * (float)normalizedPower;
        }
    }

    private void HandleDisableGlowEvent()
    {
        DisableParticleEmitter();
    }

    private void HandleGlowColorEvent()
    {
        EnableParticleEmitter();

        _glow.Color = new Color(_jumper.PlayerData.GlowColor);
    }

    private void HandleNameColorEvent()
    {
        _namePlate.LabelSettings.FontColor = new Color(_jumper.PlayerData.PlayerNameColor);
    }

    private void DisableParticleEmitter()
    {
        _glow.Emitting = false;
    }

    private void EnableParticleEmitter()
    {
        _glow.Emitting = true;
    }

    private void ApplyInitialVelocity(double delta)
    {
        Vector2 velocity = Velocity;

        velocity.Y += IsOnFloor() ? 0 : GameConstants.Gravity * (float)delta;

        Velocity = velocity;
    }

    /// <summary>
    /// If the player has jumped recently, apply the Velocity calculated after executing the Jump command.
    /// </summary>
    private void ApplyJumpVelocity()
    {
        Velocity = _jumpVelocity != Vector2.Zero ? _jumpVelocity : Velocity;
        _jumpVelocity = Vector2.Zero;
    }

    private void StopIfOnFloor()
    {
        if (IsOnFloor())
        {
            Velocity = Vector2.Zero;
        }
    }

    private void BounceOffWall()
    {
        if (IsOnWall())
        {
            Velocity = new Vector2(_previousXVelocity * -0.5f, Velocity.Y);
        }
    }

    /// <summary>
    /// Attempts to store the recent position after MoveAndSlide() call. If the position was not added to the HashSet,
    /// it means that the position already existed and the jumper did not change his position, but this logic will only
    /// be executed if the jumper was constantly touching the wall, was not grounded and was not touching the ceiling.
    /// All this means that the jumper was stuck inside of a wall and his position has to be adjusted after x frames.
    /// </summary>
    private void StorePosition()
    {
        if (IsOnFloor() || !IsOnWall() || IsOnCeiling())
        {
            return;
        }

        bool added = _recentPosition.Add(Position);

        // If the position was different (the result of successful addition to the HashSet), any previous positions will
        // be cleared and the new position is stored. On a duplicate position, it could indicate the jumper got stuck.
        if (added)
        {
            _framesSincePositionChange = 0;

            // Remove the previous position and re-add the current
            _recentPosition.Clear();
            _recentPosition.Add(Position);
        }

        _framesSincePositionChange++;

        if (_framesSincePositionChange >= 60)
        {
            Position += Vector2.Up * 16;
        }
    }
}
