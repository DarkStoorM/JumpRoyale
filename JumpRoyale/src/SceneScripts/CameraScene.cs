using System;
using Godot;
using JumpRoyale.Events;

namespace JumpRoyale;

public partial class CameraScene : Node2D
{
    private readonly EventTimer _timer = new(GameConstants.GameTimeInSeconds, GameConstants.ScrollSpeedChangeInterval);

    private int _movementMultiplier = 1;

    public bool CanMove { get; set; } = true;

    public override void _Ready()
    {
        _timer.OnInterval += IncreaseMovementMultiplier;
        _timer.OnFinished += StopCamera;

        _ = _timer.Start();
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

    private void IncreaseMovementMultiplier(object sender, EventTimerEventArgs args) => _movementMultiplier++;

    private void StopCamera(object sender, EventArgs args) => CanMove = false;
}
