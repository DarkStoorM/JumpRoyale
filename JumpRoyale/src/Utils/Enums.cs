using JumpRoyale.Utils.Attributes;

namespace JumpRoyale.Utils;

public enum JumperEventTypes
{
    OnJumpCommandEvent,
    OnDisableGlow,
    OnSetCharacter,
    OnSetGlowColor,
    OnSetNameColor,
}

/// <summary>
/// Available Jumper animations. Includes the actual animation names in the attributes. Note: the animation names in
/// attribute values have to match the animations created in the SpriteFrame editor of AnimatedSprite2D.
/// </summary>
public enum JumperAnimation
{
    [Value("fall")]
    FALL,

    [Value("idle")]
    IDLE,

    [Value("jump")]
    JUMP,

    [Value("land")]
    LAND,
}
