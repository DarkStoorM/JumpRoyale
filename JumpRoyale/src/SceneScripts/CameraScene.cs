using System;
using Godot;
using JumpRoyale.Events;

namespace JumpRoyale;

public partial class CameraScene : Node2D
{
    /// <summary>
    /// Defines the current camera movement speed multiplier, increased at interval by game timer. This also describes
    /// the current arena difficulty level (related to speed, not to the current height).
    /// </summary>
    public int MovementMultiplier { get; private set; } = 1;

    public bool CanMove { get; set; }

    /// <summary>
    /// Reference to the Timers in the parent node - assuming this camera scene is correctly instantiated inside the
    /// arena scene and is located __under__ the Timers scene in the hierarchy.
    /// </summary>
    public TimersScene Timers { get; private set; } = null!;

    public override void _EnterTree()
    {
        // Since the camera contains the UI, and it has to access the timers, we have to cache them before the children
        // are ready to make sure the timers are actually accessible to children
        Timers = GetParent<ArenaScene>().Timers;
    }

    public override void _Ready()
    {
        Timers.DifficultyTimer.OnInterval += IncreaseMovementMultiplier;
        Timers.DifficultyTimer.OnFinished += StopCamera;
        Timers.LobbyTimer.OnFinished += StartCamera;
    }

    public override void _Process(double delta)
    {
        if (CanMove)
        {
            Position = new(
                Position.X,
                Position.Y + (float)delta * -GameConstants.BaseCameraMovementSpeed * MovementMultiplier
            );
        }
    }

    /// <summary>
    /// Event handler called when the game timer raises an event at set interval - makes the camera scroll faster.
    /// </summary>
    private void IncreaseMovementMultiplier(object sender, EventTimerEventArgs args)
    {
        MovementMultiplier++;
    }

    /// <summary>
    /// Event handler called when the lobby timer is finished. Starts the game timer and initiates the camera scroll.
    /// </summary>
    private void StartCamera(object sender, EventArgs args)
    {
        CanMove = true;
    }

    /// <summary>
    /// Event handler called when the game timer is finished. Prevents the camera from moving.
    /// </summary>
    private void StopCamera(object sender, EventArgs args)
    {
        CanMove = false;
    }
}
