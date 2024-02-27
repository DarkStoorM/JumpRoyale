using System.Diagnostics.CodeAnalysis;
using Godot;
using JumpRoyale.Events;

namespace JumpRoyale;

public partial class JumperScene : CharacterBody2D
{
    [AllowNull]
    private Jumper _jumper;

    public void Init(Jumper jumper)
    {
        _jumper = jumper;

        // Listen to Command Execution events coming from twitch chat
        _jumper.JumperEventsManager.OnJumpCommandEvent += OnJumpCommandEvent;
        _jumper.JumperEventsManager.OnDisableGlowEvent += OnDisableGlowEvent;
        _jumper.JumperEventsManager.OnSetCharacterEvent += OnSetCharacterEvent;
        _jumper.JumperEventsManager.OnSetGlowColorEvent += OnSetGlowColorEvent;
        _jumper.JumperEventsManager.OnSetNameColorEvent += OnSetNameColorEvent;
    }

    public override void _PhysicsProcess(double delta)
    {
        ApplyInitialVelocity(delta);

        MoveAndSlide();
    }

    private void OnJumpCommandEvent(object sender, JumpCommandEventArgs args)
    {
        // TBA
    }

    private void OnDisableGlowEvent(object sender, DisableGlowEventArgs args)
    {
        // TBA
    }

    private void OnSetCharacterEvent(object sender, SetCharacterEventArgs args)
    {
        // TBA
    }

    private void OnSetGlowColorEvent(object sender, SetGlowColorEventArgs args)
    {
        // TBA
    }

    private void OnSetNameColorEvent(object sender, SetNameColorEventArgs args)
    {
        // TBA
    }

    private void ApplyInitialVelocity(double delta)
    {
        Vector2 velocity = Velocity;

        velocity.Y += IsOnFloor() ? 0 : GameConstants.Gravity * (float)delta;

        Velocity = velocity;
    }
}
