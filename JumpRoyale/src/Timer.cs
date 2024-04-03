using System;
using System.Threading;
using System.Threading.Tasks;
using JumpRoyale.Events;

namespace JumpRoyale;

/// <summary>
/// Reusable timer class for different Timed components. Note: this is not a realtime timer, the only purpose is to emit
/// events at set interval.
/// </summary>
/// <param name="runTimerForSeconds">Time in seconds this timer should run for unless manually stopped.</param>
/// <param name="eventEmissionInterval">Timer will emit an event after this many seconds.</param>
public class Timer(int runTimerForSeconds, int eventEmissionInterval) : IDisposable
{
    private int _elapsedTime;

    private CancellationTokenSource _cancellationTokenSource = new();

    /// <summary>
    /// Event emitted at set interval.
    /// </summary>
    public event EventHandler<TimerEventArgs>? OnCheckpointReached;

    /// <summary>
    /// Event emitted once the timer has finished running. Omitted when the timer was aborted.
    /// </summary>
    public event EventHandler<EventArgs>? OnFinished;

    /// <summary>
    /// Describes how many times the event was emitted.
    /// </summary>
    public int EventsEmittedCount { get; private set; }

    /// <summary>
    /// Event will be emitted every [x] seconds defined by this value.
    /// </summary>
    public int EventEmissionInterval { get; } = eventEmissionInterval;

    public bool IsStillRunning { get; private set; }

    /// <summary>
    /// Describes how long will this timer be allowed to run for.
    /// </summary>
    public int TotalTimerTime { get; } = runTimerForSeconds;

    /// <summary>
    /// Starts the timer on new cancellation token.
    /// </summary>
    public async Task Start()
    {
        if (IsStillRunning)
        {
            return;
        }

        _cancellationTokenSource = new();
        _elapsedTime = 0;
        IsStillRunning = true;
        EventsEmittedCount = 0;

        await RunTimer(_cancellationTokenSource.Token).ConfigureAwait(false);
    }

    /// <summary>
    /// Allows stopping the timer early.
    /// </summary>
    public void Stop()
    {
        IsStillRunning = false;

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        EmitEventOnTimerFinish();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            Dispose();
        }
    }

    private async Task RunTimer(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(EventEmissionInterval), cancellationToken).ConfigureAwait(false);

            EventsEmittedCount++;
            _elapsedTime += EventEmissionInterval;

            if (_elapsedTime >= TotalTimerTime)
            {
                EmitEventOnTimerFinish();
                Stop();
            }

            EmitEventOnCheckpoint();
        }
    }

    private void EmitEventOnTimerFinish() => OnFinished?.Invoke(this, new());

    private void EmitEventOnCheckpoint() => OnCheckpointReached?.Invoke(this, new(EventsEmittedCount));
}
