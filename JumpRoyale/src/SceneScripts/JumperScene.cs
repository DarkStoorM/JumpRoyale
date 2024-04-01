using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;
using JumpRoyale.Events;
using JumpRoyale.Utils;

namespace JumpRoyale;

public partial class JumperScene : CharacterBody2D
{
    /// <summary>
    /// Stores this jumper's position in the last frame. See <see cref="StorePosition"/>.
    /// </summary>
    private readonly HashSet<Vector2> _recentPosition = [];

    [AllowNull]
    private Jumper _jumper;

    [AllowNull]
    private AnimatedSprite2D _animatedSprite2D = null!;

    [AllowNull]
    private Label _namePlate = null!;

    [AllowNull]
    private CpuParticles2D _glow = null!;

    private Vector2 _jumpVelocityFromCommand;

    /// <summary>
    /// Previous X Velocity value of this jumper before MoveAndSlide was called. Used to bounce the jumper off the wall.
    /// </summary>
    private float _xVelocityLastFrame;

    /// <summary>
    /// Indicates for how many frames this jumper has been in the same place under certain conditions. See <see
    /// cref="StorePosition"/>.
    /// </summary>
    private int _framesSincePositionChange;

    private int _lastJumpAngleFromCommand;

    /// <summary>
    /// Flag defining if we have jumped straight up. Used for the rotating sprite switch and sprite flipping if there
    /// was a non-zero angle applied.
    /// </summary>
    private bool HasRecentlyJumpedAtZeroAngle
    {
        get => _lastJumpAngleFromCommand == 90;
    }

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("Sprite2D");
        _namePlate = GetNode<Label>("NamePlate");
        _glow = GetNode<CpuParticles2D>("Glow");

        _animatedSprite2D.AnimationFinished += HandleAnimationFinishEvent;
    }

    public void Init(Jumper jumper)
    {
        _jumper = jumper;

        // Listen to Command Execution events coming from twitch chat
        _jumper.JumperEventsManager.OnJumpCommandEvent += OnJumpCommandEvent;
        _jumper.JumperEventsManager.OnDisableGlowEvent += OnDisableGlowEvent;
        _jumper.JumperEventsManager.OnSetCharacterEvent += OnSetCharacterEvent;
        _jumper.JumperEventsManager.OnSetGlowColorEvent += OnSetGlowColorEvent;
        _jumper.JumperEventsManager.OnSetNameColorEvent += OnSetNameColorEvent;

        Name = _jumper.PlayerData.Name;
        _namePlate.Text = Name;

        HandleCharacterSetEvent();
        HandleNameColorEvent();

        if (_jumper.PlayerData.IsPrivileged)
        {
            HandleGlowColorEvent();
        }
        else
        {
            // Disable the glow anyway in case the player lost his privileged in-between the game sessions
            HandleDisableGlowEvent();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // This order is important: apply gravity -> stop if touched the floor -> then apply the jump velocity
        ApplyInitialVelocity(delta);
        StopIfOnFloor();
        PlayNotGroundedAnimation();
        ApplyJumpVelocity();
        RotateInAir(delta);
        BounceOffWall();

        _xVelocityLastFrame = Velocity.X;

        MoveAndSlide();
        StorePosition();
    }

    private void OnJumpCommandEvent(object sender, JumpCommandEventArgs args)
    {
        HandleJumpEvent(args.JumpAngle, args.JumpPower);
    }

    private void OnDisableGlowEvent(object sender, DisableGlowEventArgs args)
    {
        HandleDisableGlowEvent();
    }

    private void OnSetCharacterEvent(object sender, SetCharacterEventArgs args)
    {
        HandleCharacterSetEvent();
    }

    private void OnSetGlowColorEvent(object sender, SetGlowColorEventArgs args)
    {
        HandleGlowColorEvent();
    }

    private void OnSetNameColorEvent(object sender, SetNameColorEventArgs args)
    {
        HandleNameColorEvent();
    }

    /// <summary>
    /// Switches the animation once the last animation to finish is determined to be "Land".
    /// </summary>
    private void HandleAnimationFinishEvent()
    {
        if (_animatedSprite2D.Animation == JumperAnimationNames.Land)
        {
            _animatedSprite2D.Play(JumperAnimationNames.Idle);
        }
    }

    private void HandleJumpEvent(int angle, int power)
    {
        if (IsOnFloor())
        {
            double normalizedPower = Math.Sqrt(power * 5 * GameConstants.Gravity);

            _jumpVelocityFromCommand.X = Mathf.Cos(Mathf.DegToRad(angle + 180));
            _jumpVelocityFromCommand.Y = Mathf.Sin(Mathf.DegToRad(angle + 180));
            _jumpVelocityFromCommand = _jumpVelocityFromCommand.Normalized() * (float)normalizedPower;
            _lastJumpAngleFromCommand = angle;

            _animatedSprite2D.Play(JumperAnimationNames.Jump);
        }
    }

    private void HandleDisableGlowEvent()
    {
        DisableParticleEmitter();
    }

    private void HandleCharacterSetEvent()
    {
        int choice = _jumper.PlayerData.CharacterChoice;
        int charactersCount = CharacterSpriteProvider.Instance.CharactersCount;
        int clothingsCount = CharacterSpriteProvider.Instance.ClothingsCount;
        int choicesPerGender = charactersCount * clothingsCount;
        string gender = choice > choicesPerGender ? "Female" : "Male";
        int charNumber = ((choice - 1) % choicesPerGender / charactersCount) + 1;
        int clothingNumber = ((choice - 1) % clothingsCount) + 1;

        GD.Print("Choice: " + choice + " Gender: " + gender + " Char: " + charNumber + " Clothing: " + clothingNumber);

        _animatedSprite2D.SpriteFrames = CharacterSpriteProvider.Instance.GetSpriteFrames(
            gender,
            charNumber,
            clothingNumber
        );

        // Force playing the Idle animation when we change our character while on the floor.
        if (IsOnFloor())
        {
            _animatedSprite2D.Play(JumperAnimationNames.Idle);
        }
    }

    private void HandleGlowColorEvent()
    {
        EnableParticleEmitter();

        _glow.Color = new Color(_jumper.PlayerData.GlowColor);
    }

    private void HandleNameColorEvent()
    {
        _namePlate.LabelSettings.FontColor = new Color(_jumper.PlayerData.PlayerNameColor);
    }

    private void DisableParticleEmitter()
    {
        _glow.Emitting = false;
    }

    private void EnableParticleEmitter()
    {
        _glow.Emitting = true;
    }

    /// <summary>
    /// Basically the gravity application before any other component affects the velocity.
    /// </summary>
    private void ApplyInitialVelocity(double delta)
    {
        float newY = IsOnFloor() ? 0 : GameConstants.Gravity * (float)delta;

        Velocity = new(Velocity.X, Velocity.Y + newY);
    }

    /// <summary>
    /// If the player has jumped recently, apply the Velocity calculated after executing the Jump command.
    /// </summary>
    private void ApplyJumpVelocity()
    {
        Velocity = !_jumpVelocityFromCommand.IsEqualApprox(Vector2.Zero) ? _jumpVelocityFromCommand : Velocity;

        // Flip the sprite base on our x velocity, but only if we recently jumped at a non-zero angle
        if (!Mathf.IsZeroApprox(Velocity.X) && !HasRecentlyJumpedAtZeroAngle)
        {
            _animatedSprite2D.FlipH = Velocity.X < 0;
        }

        // Reset the jump velocity to indicate that we should not continuously apply the calculated velocity
        _jumpVelocityFromCommand = Vector2.Zero;
    }

    /// <summary>
    /// Causes the character to rotate based on its X velocity, but only at non-zero angles.
    /// </summary>
    private void RotateInAir(double delta)
    {
        // We don't want to rotate if we just jumped straight up
        if (HasRecentlyJumpedAtZeroAngle)
        {
            // Edge case reset when we jump at the very moment we hit the floor and we keep the previous rotation
            _animatedSprite2D.RotationDegrees = 0;

            return;
        }

        // Formula to automatically calculate the rotation speed based on the character's x jump velocity. The maximum
        // velocity is 700, but it's a bit too fast, so we want to clamp it at around 600. The formula was shortened and
        // tweaked to always output 1 at non-zero angle with low value, with a maximum of 7 at angle of 60, which should
        // not increase linearly, but a bit slower at the start
        // Visualization, caps at J60. Approximately every +100 velocity on the plot is the next 10 angles:
        // https://www.wolframalpha.com/input?i=min%28max%28%284sin%28%28abs%28x%29%2F280%29+%2B+300%29%2B5%29%2C1%29%2C+7%29+%3Bx+from+0+to+700
        float velocity = Math.Abs(Velocity.X);
        float rotationSpeedFromVelocity = (float)(4 * Math.Sin((velocity / 280) + 300) + 5);
        float clampedRotation = Math.Clamp(rotationSpeedFromVelocity, 1, 10);

        // Calculate the rotation factor and flip the value if we are going left (rotating in the right direction)
        float rotationFactor = 200 * (float)delta * (Velocity.X < 0 ? -1 : 1) * clampedRotation;

        _animatedSprite2D.RotationDegrees = IsOnFloor() ? 0 : _animatedSprite2D.RotationDegrees + rotationFactor;
    }

    /// <summary>
    /// Stops this body completely once it touches the floor.
    /// </summary>
    private void StopIfOnFloor()
    {
        if (IsOnFloor())
        {
            Velocity = Vector2.Zero;
        }
    }

    /// <summary>
    /// Reflects some of the X velocity when touching the wall to achieve the "bounce" effect.
    /// </summary>
    private void BounceOffWall()
    {
        if (IsOnWall())
        {
            Velocity = new Vector2(_xVelocityLastFrame * -0.5f, Velocity.Y);
        }
    }

    /// <summary>
    /// Switches the character animations in certain conditions when in the air. Handles falling/landing.
    /// </summary>
    private void PlayNotGroundedAnimation()
    {
        // Going up -> Jump, down -> Fall. Removing the check causes infinite animation start
        if (!IsOnFloor())
        {
            string animation = Velocity.Y < 0 ? JumperAnimationNames.Jump : JumperAnimationNames.Fall;

            _animatedSprite2D.Play(animation);
        }

        // Defines a situation when we stopped, but the animation is still playing the Jump/Fall frames
        bool hasLandedButStillAnimating =
            (
                _animatedSprite2D.Animation == JumperAnimationNames.Fall
                || _animatedSprite2D.Animation == JumperAnimationNames.Jump
            ) && Velocity.IsEqualApprox(Vector2.Zero);

        if (hasLandedButStillAnimating)
        {
            _animatedSprite2D.Play(JumperAnimationNames.Land);
        }
    }

    /// <summary>
    /// Attempts to store the recent position after MoveAndSlide() call. If the position was not added to the HashSet,
    /// it means that the position already existed and the jumper did not change his position, but this logic will only
    /// be executed if the jumper was constantly touching the wall, was not grounded and was not touching the ceiling.
    /// All this means that the jumper was stuck inside of a wall and his position has to be adjusted after x frames.
    /// </summary>
    private void StorePosition()
    {
        if (IsOnFloor() || !IsOnWall() || IsOnCeiling())
        {
            return;
        }

        bool added = _recentPosition.Add(Position);

        // If the position was different (the result of successful addition to the HashSet), any previous positions will
        // be cleared and the new position is stored. On a duplicate position, it could indicate the jumper got stuck.
        if (added)
        {
            _framesSincePositionChange = 0;

            // Remove the previous position and re-add the current
            _recentPosition.Clear();
            _recentPosition.Add(Position);
        }

        _framesSincePositionChange++;

        if (_framesSincePositionChange >= 60)
        {
            Position += Vector2.Up * 16;
        }
    }
}
