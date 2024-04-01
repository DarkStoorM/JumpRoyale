using System;

namespace JumpRoyale.Utils.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class ValueAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}
