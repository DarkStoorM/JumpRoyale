using System;
using JumpRoyale.Events;
using JumpRoyale.Utils;

namespace JumpRoyale;

public class JumperEvents
{
    private static readonly object _lock = new();

    private static JumperEvents? _instance;

    private JumperEvents() { }

#pragma warning disable S3264 // Events should be invoked
    public event EventHandler<JumpCommandEventArgs>? OnJumpEvent;

    public event EventHandler<DisableGlowEventArgs>? OnDisableGlowEvent;

    public event EventHandler<CharacterChangeEventArgs>? OnSetCharacterEvent;

    public event EventHandler<GlowColorChangeEventArgs>? OnSetGlowColorEvent;

    public event EventHandler<NameColorChangeEventArgs>? OnSetNameColorEvent;
#pragma warning restore S3264 // Events should be invoked

    public static JumperEvents Instance
    {
        get
        {
            lock (_lock)
            {
                _instance ??= new JumperEvents();

                return _instance;
            }
        }
    }

    public void InvokeEvent<TEventArgs>(JumperEventTypes eventName, TEventArgs eventArgs)
        where TEventArgs : EventArgs
    {
        EventHandler<TEventArgs>? eventToInvoke = eventName switch
        {
            JumperEventTypes.OnJumpEvent => OnJumpEvent as EventHandler<TEventArgs>,
            JumperEventTypes.OnDisableGlow => OnDisableGlowEvent as EventHandler<TEventArgs>,
            JumperEventTypes.OnSetCharacter => OnSetCharacterEvent as EventHandler<TEventArgs>,
            JumperEventTypes.OnSetGlowColor => OnSetGlowColorEvent as EventHandler<TEventArgs>,
            JumperEventTypes.OnSetNameColor => OnSetNameColorEvent as EventHandler<TEventArgs>,
            _ => throw new NotImplementedException(),
        };

        eventToInvoke?.Invoke(this, eventArgs);
    }
}
