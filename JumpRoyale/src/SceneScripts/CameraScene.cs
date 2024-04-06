using System;
using Godot;
using JumpRoyale.Events;

namespace JumpRoyale;

public partial class CameraScene : Node2D
{
    /// <summary>
    /// Timer started after the lobby countdown, raises an event at interval.
    /// </summary>
    private readonly EventTimer _gameTimer =
        new(GameConstants.GameTimeInSeconds, GameConstants.ScrollSpeedChangeInterval);

    /// <summary>
    /// Reference to the ArenaScene in the parent node - assuming this camera scene is correctly instantiated inside the
    /// arena scene.
    /// </summary>
    private ArenaScene _arena = null!;

    /// <summary>
    /// Defines the current camera movement speed multiplier, increased at interval by game timer.
    /// </summary>
    private int _movementMultiplier = 1;

    public bool CanMove { get; set; }

    public override void _Ready()
    {
        _arena = (ArenaScene)Owner;

        _gameTimer.OnInterval += IncreaseMovementMultiplier;
        _gameTimer.OnFinished += StopCamera;

        _arena.LobbyTimer.OnFinished += StartCamera;
    }

    public override void _Process(double delta)
    {
        if (CanMove)
        {
            Position = new(
                Position.X,
                Position.Y + (float)delta * -GameConstants.BaseCameraMovementSpeed * _movementMultiplier
            );
        }
    }

    /// <summary>
    /// Event handler called when the game timer raises an event at set interval - makes the camera scroll faster.
    /// </summary>
    private void IncreaseMovementMultiplier(object sender, EventTimerEventArgs args)
    {
        _movementMultiplier++;
    }

    /// <summary>
    /// Event handler called when the lobby timer is finished. Starts the game timer and initiates the camera scroll.
    /// </summary>
    private void StartCamera(object sender, EventArgs args)
    {
        CanMove = true;

        _ = _gameTimer.Start();
    }

    /// <summary>
    /// Event handler called when the game timer is finished. Prevents the camera from moving.
    /// </summary>
    private void StopCamera(object sender, EventArgs args)
    {
        CanMove = false;
    }
}
