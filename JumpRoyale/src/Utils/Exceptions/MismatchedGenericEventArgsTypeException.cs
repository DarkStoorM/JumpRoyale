using System;

namespace JumpRoyale.Utils.Exceptions;

public class MismatchedGenericEventArgsTypeException : Exception
{
    public MismatchedGenericEventArgsTypeException()
        : base() { }

    public MismatchedGenericEventArgsTypeException(string message)
        : base(message) { }

    public MismatchedGenericEventArgsTypeException(string message, Exception innerException)
        : base(message, innerException) { }
}
