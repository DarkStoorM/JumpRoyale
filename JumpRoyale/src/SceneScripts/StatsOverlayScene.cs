using Godot;
using JumpRoyale.Events;

namespace JumpRoyale;

public partial class StatsOverlayScene : Control
{
    /// <summary>
    /// Label displaying the live player count, updated on join.
    /// </summary>
    private Label _playerCount = null!;

    /// <summary>
    /// Label displaying the current maximum height achieved by someone.
    /// </summary>
    private Label _currentMaxHeight = null!;

    /// <summary>
    /// Label displaying the name of the current leader (highest jumper).
    /// </summary>
    private Label _highestPlayer = null!;

    /// <summary>
    /// Label displaying the game timer (this component is not related to the lobby timer).
    /// </summary>
    private Label _gameTimer = null!;

    public override void _Ready()
    {
        _playerCount = GetNode<Label>("PlayerCount");
        _currentMaxHeight = GetNode<Label>("CurrentMaxHeight");
        _highestPlayer = GetNode<Label>("HighestPlayer");
        _gameTimer = GetNode<Label>("GameTimer");

        PlayerStats.Instance.OnPlayerJoin += PollPlayerCountEvent;
    }

    public override void _Process(double delta)
    {
        // For now, this will be a test code, because this runs every frame, but benchmark this somehow and check if
        // there is any impact on performance if we just look for maximum value in a dictionary of players.
    }

    /// <summary>
    /// Retrieves the current player count from <c>PlayerStats</c>, updated when PlayerStats raises an event on new
    /// player join.
    /// </summary>
    private void PollPlayerCountEvent(object sender, PlayerJoinEventArgs args)
    {
        _playerCount.Text = PlayerStats.Instance.JumpersCount().ToString();
    }
}
