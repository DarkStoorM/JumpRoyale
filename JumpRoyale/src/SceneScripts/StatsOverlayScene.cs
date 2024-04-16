using System;
using Godot;
using JumpRoyale.Events;

namespace JumpRoyale;

public partial class StatsOverlayScene : Control
{
    private CameraScene _parent = null!;

    private Label _playerCount = null!;
    private Label _firstPlaceJumperName = null!;
    private Label _firstPlaceHeight = null!;
    private Label _secondPlaceJumperName = null!;
    private Label _secondPlaceHeight = null!;
    private Label _gameTimer = null!;
    private Label _difficultyLevel = null!;

    private int _secondsRemaining = GameConstants.GameTime;
    private int _currentDifficultyLevel = 1;

    public override void _Ready()
    {
        _parent = GetParent<CameraScene>();

        _playerCount = GetNode<Label>("Value_PlayerCount");
        _firstPlaceJumperName = GetNode<Label>("Value_1stPlayerName");
        _firstPlaceHeight = GetNode<Label>("Value_1stPlayerHeight");
        _gameTimer = GetNode<Label>("Value_GameTimer");
        _difficultyLevel = GetNode<Label>("Value_Level");
        _secondPlaceJumperName = GetNode<Label>("Value_2ndPlayerName");
        _secondPlaceHeight = GetNode<Label>("Value_2ndPlayerHeight");

        // Immediately update the UI values to reflect the configuration through code
        UpdateLabelText(_gameTimer, _secondsRemaining.ToString());
        UpdateLabelText(_difficultyLevel, _parent.MovementMultiplier.ToString());

        _parent.Timers.GameTimer.OnInterval += UpdateTimer;
        _parent.Timers.GameTimer.OnFinished += HideUI;
        _parent.Timers.DifficultyTimer.OnInterval += UpdateDifficulty;

        PlayerStats.Instance.OnPlayerJoin += UpdatePlayerCount;
    }

    public override void _Process(double delta)
    {
        // For now, this will be a test code, because this runs every frame, but benchmark this somehow and check if
        // there is any impact on performance if we just look for maximum value in a dictionary of players.
        UpdateLeadingJumper();
    }

    /// <summary>
    /// Retrieves the current player count from <c>PlayerStats</c>, updated when PlayerStats raises an event on new
    /// player join.
    /// </summary>
    private void UpdatePlayerCount(object sender, PlayerJoinEventArgs args)
    {
        UpdateLabelText(_playerCount, PlayerStats.Instance.JumpersCount().ToString());
    }

    private void UpdateDifficulty(object sender, EventArgs args)
    {
        _currentDifficultyLevel++;

        UpdateLabelText(_difficultyLevel, _currentDifficultyLevel.ToString());
    }

    private void UpdateTimer(object sender, EventArgs args)
    {
        _secondsRemaining--;

        UpdateLabelText(_gameTimer, _secondsRemaining.ToString());
    }

    /// <summary>
    /// Changes the UI visibility through Deferred call.
    /// </summary>
    private void HideUI(object sender, EventArgs args)
    {
        CallDeferred(nameof(DeferredUIVisibilityUpdate));
    }

    /// <summary>
    /// Performs a deferred update on Label's Text component.
    /// </summary>
    /// <param name="label">Label to update.</param>
    /// <param name="value">Value to replace the Label's Text contents.</param>
    private void UpdateLabelText(Label label, string value)
    {
        CallDeferred(nameof(DeferredUpdateLabelText), label, value);
    }

    /// <summary>
    /// See <see cref="UpdateLabelText(Label, string)"/>.
    /// </summary>
    private void DeferredUpdateLabelText(Label label, string value)
    {
        label.Text = value;
    }

    /// <summary>
    /// Changes the UI visibility through Deferred call.
    /// </summary>
    private void DeferredUIVisibilityUpdate()
    {
        Visible = false;
    }

    /// <summary>
    /// Updates the UI portions displaying the leader.
    /// </summary>
    private void UpdateLeadingJumper()
    {
        Jumper? leader = PlayerStats.Instance.CurrentLeadingJumper();

        if (leader is null)
        {
            UpdateLabelText(_firstPlaceJumperName, string.Empty);
            UpdateLabelText(_firstPlaceHeight, string.Empty);

            return;
        }

        UpdateLabelText(_firstPlaceJumperName, leader.PlayerData.Name);
        UpdateLabelText(_firstPlaceHeight, leader.CurrentHeight.ToString());
    }
}
