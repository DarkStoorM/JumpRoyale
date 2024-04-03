using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;
using JumpRoyale.Utils;

namespace JumpRoyale;

public class CharacterSpriteProvider
{
    private static readonly object _lock = new();
    private static CharacterSpriteProvider? _instance;

    private readonly string[] _genders = ["Male", "Female"];

    private readonly Dictionary<JumperAnimation, CharacterAnimationData> _animations =
        new()
        {
            { JumperAnimation.FALL, new FallAnimation() },
            { JumperAnimation.IDLE, new IdleAnimation() },
            { JumperAnimation.JUMP, new JumpAnimation() },
            { JumperAnimation.LAND, new LandAnimation() },
        };

    /// <summary>
    /// Map of string to SpriteFrames. Keys are hashed animation names.
    /// </summary>
    private readonly Dictionary<string, SpriteFrames> _allSpriteFrames = [];

    private CharacterSpriteProvider()
    {
        foreach (string gender in _genders)
        {
            for (int charNumber = 1; charNumber <= CharactersCount; charNumber++)
            {
                for (int clothingNumber = 1; clothingNumber <= ClothingsCount; clothingNumber++)
                {
                    Create(gender, charNumber, clothingNumber);
                }
            }
        }
    }

    public static CharacterSpriteProvider Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new();
            }
        }
    }

    public int CharactersCount { get; } = 3;

    public int ClothingsCount { get; } = 3;

    /// <summary>
    /// Forces the singleton instantiation, because it has to load the resources, so we don't want to initialize this
    /// "at some point" when the first character is created (in case there are lots of resources). This is not required,
    /// but might be in the future if there are more resources introduced.
    /// </summary>
    public static void Initialize()
    {
        // Force instantiation, it doesn't matter what we do here
        if (Instance is not null)
        {
            return;
        }
    }

    public SpriteFrames GetSpriteFrames(string gender, int charNumber, int clothingNumber)
    {
        return _allSpriteFrames[GetAnimationHash(gender, charNumber, clothingNumber)];
    }

    private void Create([NotNull] string gender, int charNumber, int clothingNumber)
    {
        SpriteFrames spriteFrames = new();

        foreach (JumperAnimation animation in Enum.GetValues(typeof(JumperAnimation)))
        {
            string animationName = animation.ToString().ToLower();
            CharacterAnimationData animationData = _animations[animation];

            spriteFrames.AddAnimation(animationName);

            for (int frameNumber = 0; frameNumber < animationData.FramesCount; frameNumber++)
            {
                Texture2D texture = ResourceLoader.Load<Texture2D>(
                    GetPathToCharacter(gender, charNumber, clothingNumber, animationName, frameNumber)
                );

                spriteFrames.AddFrame(animationName, texture, 1);
            }

            spriteFrames.SetAnimationSpeed(animationName, animationData.Framerate);
            spriteFrames.SetAnimationLoop(animationName, animationData.ShouldLoop);
        }

        _allSpriteFrames.Add(GetAnimationHash(gender, charNumber, clothingNumber), spriteFrames);
    }

    /// <summary>
    /// Constructs a full path to the existing resources (character sprite file) based on the provided parameters.
    /// </summary>
    private string GetPathToCharacter(
        string gender,
        int characterNumber,
        int clothingNumber,
        string animationName,
        int frameNumber
    )
    {
        string pathToFolder =
            $"res://assets/sprites/characters/{gender}/Character {characterNumber}/Clothes {clothingNumber}/";
        string fileName = $"Character{characterNumber}{gender[0]}_{clothingNumber}_{animationName}_{frameNumber}.png";

        return $"{pathToFolder}{fileName}";
    }

    /// <summary>
    /// Creates an access string for SpriteFrames retrieval.
    /// </summary>
    /// <param name="gender">Male or Female (case sensitive).</param>
    /// <param name="characterNumber">Character variant, range from 1 to 3.</param>
    /// <param name="clothingNumber">Clothing variant, range from 1 to 3.</param>
    private string GetAnimationHash(string gender, int characterNumber, int clothingNumber)
    {
        return $"{gender}_{characterNumber}_{clothingNumber}";
    }
}