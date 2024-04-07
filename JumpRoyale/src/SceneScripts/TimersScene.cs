using System;
using Godot;

namespace JumpRoyale;

public partial class TimersScene : Node2D
{
    /// <summary>
    /// Timer controlling the lobby ceiling. Child nodes should be subscribed to this timer to call actions when this
    /// timer raises events.
    /// </summary>
    public EventTimer LobbyTimer { get; } = new(GameConstants.LobbyAwaitingTime);

    /// <summary>
    /// Timer started after the lobby countdown, raises an event at interval. Child nodes should be subscribed to this
    /// timer to call actions when this timer raises events.
    /// </summary>
    public EventTimer GameTimer { get; } = new(GameConstants.GameTime, GameConstants.ScrollSpeedChangeInterval);

    /// <summary>
    /// Timer started after the Game Timer is finished.
    /// </summary>
    public EventTimer PodiumTimer { get; } = new(GameConstants.PodiumTimeout);

    public override void _EnterTree()
    {
        // Force assignment inside the parent before any other component tries to access these timers
        GetParent<ArenaScene>().Timers = this;
    }

    public override void _Ready()
    {
        // The Lobby Timer automatically starts when the game is launched and starts the Game Timer after it's finished,
        // which will then trigger the jump timeout after the game is done.
        _ = LobbyTimer.Start();

        LobbyTimer.OnFinished += StartGameTimer;
        GameTimer.OnFinished += StartPodiumTimer;
    }

    private void StartGameTimer(object sender, EventArgs args)
    {
        _ = GameTimer.Start();
    }

    private void StartPodiumTimer(object sender, EventArgs args)
    {
        _ = PodiumTimer.Start();
    }
}
