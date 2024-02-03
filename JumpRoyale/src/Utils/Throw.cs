using System;

namespace Utils;

public static class NullGuard
{
    public static void ThrowIfNull<TExceptionToThrow>(object? argument)
        where TExceptionToThrow : Exception, new()
    {
        if (argument is null)
        {
            throw new TExceptionToThrow();
        }
    }
}
