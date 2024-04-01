using System;
using System.Reflection;
using JumpRoyale.Utils.Attributes;

namespace JumpRoyale.Utils.Extensions;

public static class EnumExtensions
{
    public static string GetValue(this Enum value)
    {
        FieldInfo field = value?.GetType().GetField(value.ToString()) ?? throw new NullReferenceException();

        // Return the contents of "value" attribute or throw if there was no attribute assigned to the field/property
        ValueAttribute? attribute =
            Attribute.GetCustomAttribute(field, typeof(ValueAttribute)) as ValueAttribute
            ?? throw new NullReferenceException($"There was no attribute assigned to {value} enum.");

        return attribute.Value;
    }
}
