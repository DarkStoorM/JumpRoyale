using System;

namespace JumpRoyale.Utils.Exceptions;

public class NullPlayerDataException : Exception
{
    public NullPlayerDataException()
        : base("You are trying to update a non-existing player. PlayerData was not provided.") { }

    public NullPlayerDataException(string message)
        : base(message) { }

    public NullPlayerDataException(string message, Exception innerException)
        : base(message, innerException) { }
}
