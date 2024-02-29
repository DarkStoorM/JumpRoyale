using System;

namespace JumpRoyale.Utils.Exceptions;

public class MissingJumperException : Exception
{
    public MissingJumperException()
        : base("Jumper was null. Check if the jumper you are trying to retrieve actually exists in PlayerStats.") { }

    public MissingJumperException(string message)
        : base(message) { }

    public MissingJumperException(string message, Exception innerException)
        : base(message, innerException) { }
}
