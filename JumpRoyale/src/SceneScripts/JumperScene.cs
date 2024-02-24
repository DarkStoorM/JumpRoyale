using System.Diagnostics.CodeAnalysis;
using Godot;
using JumpRoyale.Events;
using JumpRoyale.Utils;
using JumpRoyale.Utils.Exceptions;

namespace JumpRoyale;

public partial class JumperScene : Node2D
{
    [AllowNull]
    private Jumper _jumper;

    public void Init(Jumper jumper)
    {
        NullGuard.ThrowIfNull<MissingJumperException>(jumper);

        _jumper = jumper;

        jumper.JumperEventsManager.OnJumpCommandEvent += OnJumpCommandEvent;
        jumper.JumperEventsManager.OnDisableGlowEvent += OnDisableGlowEvent;
        jumper.JumperEventsManager.OnSetCharacterEvent += OnSetCharacterEvent;
        jumper.JumperEventsManager.OnSetGlowColorEvent += OnSetGlowColorEvent;
        jumper.JumperEventsManager.OnSetNameColorEvent += OnSetNameColorEvent;
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
}
