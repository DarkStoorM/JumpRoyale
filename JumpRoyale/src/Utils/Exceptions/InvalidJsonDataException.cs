using System;

namespace JumpRoyale.Utils.Exceptions;

public class InvalidJsonDataException : Exception
{
    public InvalidJsonDataException()
        : base("No records returned, but the Json contains data. Make sure the deserialized type matches the file.") { }

    public InvalidJsonDataException(string message)
        : base(message) { }

    public InvalidJsonDataException(string message, Exception innerException)
        : base(message, innerException) { }
}
