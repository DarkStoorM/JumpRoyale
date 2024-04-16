using System;
using System.Collections.Generic;
using Godot;
using JumpRoyale.Events;
using JumpRoyale.Utils;

namespace JumpRoyale;

public partial class StatsOverlayScene : Control
{
    /// <summary>
    /// Stores the labels related to the podium jumpers in ascending order [name label, height label].
    /// </summary>
    private readonly Dictionary<int, PodiumJumperLabels> _podiumJumperLabels = [];

    private CameraScene _parent = null!;

    private Label _playerCount = null!;
    private Label _firstPlaceName = null!;
    private Label _firstPlaceHeight = null!;
    private Label _secondPlaceName = null!;
    private Label _secondPlaceHeight = null!;
    private Label _thirdPlaceName = null!;
    private Label _thirdPlaceHeight = null!;
    private Label _gameTimer = null!;
    private Label _difficultyLevel = null!;

    private int _secondsRemaining = GameConstants.GameTime;
    private int _currentDifficultyLevel = 1;

    public override void _Ready()
    {
        _parent = GetParent<CameraScene>();

        _playerCount = GetNode<Label>("Value_PlayerCount");
        _gameTimer = GetNode<Label>("Value_GameTimer");
        _difficultyLevel = GetNode<Label>("Value_Level");
        _firstPlaceName = GetNode<Label>("Value_1stPlaceName");
        _firstPlaceHeight = GetNode<Label>("Value_1stPlaceHeight");
        _secondPlaceName = GetNode<Label>("Value_2ndPlaceName");
        _secondPlaceHeight = GetNode<Label>("Value_2ndPlaceHeight");
        _thirdPlaceName = GetNode<Label>("Value_3rdPlaceName");
        _thirdPlaceHeight = GetNode<Label>("Value_3rdPlaceHeight");

        // Immediately update the UI values to reflect the configuration through code
        UpdateLabelText(_gameTimer, _secondsRemaining.ToString());
        UpdateLabelText(_difficultyLevel, _parent.MovementMultiplier.ToString());

        _parent.Timers.GameTimer.OnInterval += UpdateTimer;
        _parent.Timers.GameTimer.OnFinished += HideUI;
        _parent.Timers.DifficultyTimer.OnInterval += UpdateDifficulty;

        _podiumJumperLabels.Add(0, new(_firstPlaceName, _firstPlaceHeight));
        _podiumJumperLabels.Add(1, new(_secondPlaceName, _secondPlaceHeight));
        _podiumJumperLabels.Add(2, new(_thirdPlaceName, _thirdPlaceHeight));

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
    /// Use <see cref="UpdateLabelText(Label, string)"/>.
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
        Jumper?[] podiumJumpers = PlayerStats.Instance.GetPodiumJumpers();

        for (int i = 0; i < podiumJumpers.Length; i++)
        {
            Jumper? jumper = podiumJumpers[i];

            // If there was no jumper, just reset the label back to empty in case we had some initial text, but there
            // are not enough players to fill all three slots yet
            UpdateLabelText(_podiumJumperLabels[i].PlayerNameLabel, jumper?.PlayerData.Name ?? string.Empty);
            UpdateLabelText(_podiumJumperLabels[i].PlayerHeightLabel, jumper?.CurrentHeight.ToString() ?? string.Empty);
        }
    }
}
