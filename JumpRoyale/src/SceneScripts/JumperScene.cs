using System.Diagnostics.CodeAnalysis;
using Godot;
using JumpRoyale.Events;

namespace JumpRoyale;

public partial class JumperScene : CharacterBody2D
{
    [AllowNull]
    private Jumper _jumper;

    private Label _namePlate = null!;
    private CpuParticles2D _glow = null!;

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
        ApplyInitialVelocity(delta);

        MoveAndSlide();
    }

    private void OnJumpCommandEvent(object sender, JumpCommandEventArgs args)
    {
        GD.Print("OnJumpCommandEvent");
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
}
