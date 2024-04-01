using System;
using System.Reflection;
using JumpRoyale.Utils.Attributes;

namespace JumpRoyale.Utils.Extensions;

public static class EnumExtensions
{
    public static string GetValue(this Enum value)
    {
        FieldInfo field = value?.GetType().GetField(value.ToString()) ?? throw new NullReferenceException();

        ValueAttribute attribute = (ValueAttribute)Attribute.GetCustomAttribute(field, typeof(ValueAttribute))!;

        return attribute != null ? attribute.Value : value.ToString();
    }
}
