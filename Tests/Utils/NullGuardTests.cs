using System.Data;
using JumpRoyale.Utils;

namespace Tests.Utils;

[TestFixture]
public class NullGuardTests
{
    /// <summary>
    /// This test checks the overload without generic parameter, which should throw ArgumentNullException.
    /// </summary>
    [Test]
    public void TestsDefaultGuard()
    {
        object? test = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            NullGuard.ThrowIfNull(test);
        });
    }

    /// <summary>
    /// This test checks the overload with specific parameter, which should throw the specified exception type.
    /// </summary>
    [Test]
    public void TestsTypeSpecificGuard()
    {
        object? test = null;

        Assert.Throws<SyntaxErrorException>(() =>
        {
            NullGuard.ThrowIfNull<SyntaxErrorException>(test);
        });
    }

    /// <summary>
    /// This test checks the overload with nullable parameter for strings, which should throw custom exception if the
    /// argument was null or empty.
    /// </summary>
    [Test]
    public void TestsNullOrEmptyGuard()
    {
        string? test = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            NullGuard.ThrowIfNullOrEmpty<ArgumentNullException>(test);
        });

        // Empty string should also throw with this guard, because we sometimes need to force initialization of a
        // property, which is used e.g. for file paths.
        test = string.Empty;

        Assert.Throws<ArgumentNullException>(() =>
        {
            NullGuard.ThrowIfNullOrEmpty<ArgumentNullException>(test);
        });
    }
}
