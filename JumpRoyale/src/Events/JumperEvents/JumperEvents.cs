using System;
using JumpRoyale.Events;
using JumpRoyale.Utils;
using JumpRoyale.Utils.Exceptions;

namespace JumpRoyale;

public class JumperEventsManager
{
    public event EventHandler<JumpCommandEventArgs>? OnJumpCommandEvent;

    public event EventHandler<DisableGlowEventArgs>? OnDisableGlowEvent;

    public event EventHandler<SetCharacterEventArgs>? OnSetCharacterEvent;

    public event EventHandler<SetGlowColorEventArgs>? OnSetGlowColorEvent;

    public event EventHandler<SetNameColorEventArgs>? OnSetNameColorEvent;

    /// <summary>
    /// Invokes the event indexed by the given event type.
    /// </summary>
    /// <param name="eventName">Name (type) of the event to look match and invoke.</param>
    /// <param name="eventArgs">EventArgs to pass to the invoked event.</param>
    /// <typeparam name="TEventArgs">Type of the EventArgs passed to the event invocation.</typeparam>
    public void InvokeEvent<TEventArgs>(JumperEventTypes eventName, TEventArgs eventArgs)
        where TEventArgs : EventArgs
    {
        switch (eventName)
        {
            case JumperEventTypes.OnJumpCommandEvent:
                InvokeIfTypeMatches(OnJumpCommandEvent, eventArgs);
                break;
            case JumperEventTypes.OnDisableGlow:
                InvokeIfTypeMatches(OnDisableGlowEvent, eventArgs);
                break;
            case JumperEventTypes.OnSetCharacter:
                InvokeIfTypeMatches(OnSetCharacterEvent, eventArgs);
                break;
            case JumperEventTypes.OnSetGlowColor:
                InvokeIfTypeMatches(OnSetGlowColorEvent, eventArgs);
                break;
            case JumperEventTypes.OnSetNameColor:
                InvokeIfTypeMatches(OnSetNameColorEvent, eventArgs);
                break;
            default:
                throw new Exception("Unhandled event");
        }
    }

    /// <summary>
    /// Invokes the passed event only if the generic types are matching, ensuring the same type of event args is being
    /// passed to the EventHandler.
    /// </summary>
    /// <param name="eventToInvoke">Jumper Event to invoke.</param>
    /// <param name="eventArgs">EventArgs to pass to the invoked event.</param>
    /// <typeparam name="TEventHandlerArgs">Type of the EventArgs required by EventHandler on invoked event.</typeparam>
    /// <typeparam name="TEventArgs">Type of the EventArgs passed to the event invocation.</typeparam>
    private void InvokeIfTypeMatches<TEventHandlerArgs, TEventArgs>(
        EventHandler<TEventHandlerArgs>? eventToInvoke,
        TEventArgs eventArgs
    )
        where TEventHandlerArgs : EventArgs
    {
        NullGuard.ThrowIfNull(eventArgs);

        if (eventArgs.GetType() != typeof(TEventHandlerArgs))
        {
            throw new MismatchedGenericEventArgsTypeException(
                $"Mismatched generic types. Event handler: {eventArgs.GetType()}. Event Args passed: {typeof(TEventHandlerArgs)}"
            );
        }

        eventToInvoke?.Invoke(this, (eventArgs as TEventHandlerArgs)!);
    }
}
