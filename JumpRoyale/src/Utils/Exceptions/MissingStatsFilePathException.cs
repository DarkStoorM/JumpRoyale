using System;

namespace JumpRoyale.Utils.Exceptions;

public class MissingStatsFilePathException : Exception
{
    public MissingStatsFilePathException()
        : base("No stats file path provided. Make sure you set the path to stats file before loading players.") { }

    public MissingStatsFilePathException(string message)
        : base(message) { }

    public MissingStatsFilePathException(string message, Exception innerException)
        : base(message, innerException) { }
}
