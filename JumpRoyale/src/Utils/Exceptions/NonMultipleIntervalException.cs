using System;

namespace JumpRoyale.Utils.Exceptions;

public class NonMultipleIntervalException : Exception
{
    public NonMultipleIntervalException()
        : base(
            "Provided interval is not a multiple of timer's value. This is required to ensure that all events are raised 'in time' without skipping events or raising to many."
        )
    {
        // .
    }

    public NonMultipleIntervalException(string message)
        : base(message) { }

    public NonMultipleIntervalException(string message, Exception innerException)
        : base(message, innerException) { }
}
