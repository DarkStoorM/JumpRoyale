using System;
using System.Diagnostics.CodeAnalysis;

namespace Utils;

public static class NullGuard
{
    /// <summary>
    /// Throws <c>ArgumentNullException</c> if the provided argument is null. For throwing custom exception type, see
    /// overload <see cref="ThrowIfNull{TExceptionToThrow}(object?)"/>.
    /// </summary>
    /// <param name="argument">Argument to test.</param>
    public static void ThrowIfNull([NotNull] object? argument)
    {
        ArgumentNullException.ThrowIfNull(argument);
    }

    /// <summary>
    /// Overload method that throws custom exception type if the provided argument is null. Useful when we want to throw
    /// a different exception than ArgumentNullException.
    /// </summary>
    /// <param name="argument">Argument to test.</param>
    /// <typeparam name="TExceptionToThrow">Custom exception type to throw.</typeparam>
    public static void ThrowIfNull<TExceptionToThrow>([NotNull] object? argument)
        where TExceptionToThrow : Exception, new()
    {
        if (argument is null)
        {
            throw new TExceptionToThrow();
        }
    }
}
