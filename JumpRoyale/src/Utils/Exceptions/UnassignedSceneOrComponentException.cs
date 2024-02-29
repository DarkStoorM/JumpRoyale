using System;

namespace JumpRoyale.Utils.Exceptions;

/// <summary>
/// When this exception is being thrown, this means that a Scene contains an [Export]ed field/property, which has
/// nothing assigned to it, but expects a reference in the Inspector (editor).
/// </summary>
public class UnassignedSceneOrComponentException : Exception
{
    public UnassignedSceneOrComponentException()
        : base() { }

    public UnassignedSceneOrComponentException(string message)
        : base(message) { }

    public UnassignedSceneOrComponentException(string message, Exception innerException)
        : base(message, innerException) { }
}
