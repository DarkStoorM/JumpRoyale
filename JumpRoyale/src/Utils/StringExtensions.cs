namespace JumpRoyale.Utils.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Extension method to replace variables in a string template.
    /// <example>
    /// <c>
    /// <para/>string test = "This is a test: {0}";
    /// <para/>test.ReplaceInTemplate("Zero"); // Outputs: "This is a test: Zero"
    /// </c>
    /// </example>
    /// </summary>
    /// <param name="thisString">String to apply the replacement to.</param>
    /// <param name="argument">Replacement string.</param>
    public static string ReplaceInTemplate(this string thisString, string argument)
    {
        return string.Format(thisString, argument);
    }

    /// <summary>
    /// Extension method to replace variables in a string template.
    /// <example>
    /// <c>
    /// <para/>string test = "This is a test: {0}, {1}";
    /// <para/>test.ReplaceInTemplate("Zero", "One"); // Outputs: "This is a test: Zero, One"
    /// </c>
    /// </example>
    /// </summary>
    /// <exception cref="System.FormatException">When not enough replacement strings were provided, etc.</exception>
    /// <param name="thisString">String to apply the replacement to.</param>
    /// <param name="arguments">Replacement strings.</param>
    public static string ReplaceInTemplate(this string thisString, params object?[] arguments)
    {
        return string.Format(thisString, arguments);
    }
}
