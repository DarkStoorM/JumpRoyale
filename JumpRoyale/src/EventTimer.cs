using System;
using System.Threading;
using System.Threading.Tasks;
using JumpRoyale.Events;
using JumpRoyale.Utils.Exceptions;

namespace JumpRoyale;

/// <summary>
/// Reusable timer class for different Timed components. Note: this is not a realtime timer, the only purpose is to
/// raise events at set interval or to call action after the timer is finished counting. See: <see cref="OnFinished"/> /
/// <see cref="OnInterval"/>.
/// </summary>
/// <param name="runTimerForSeconds">Time in seconds this timer should run for unless manually stopped.</param>
/// <param name="eventEmissionInterval">Timer will raise an event after this many seconds.</param>
public class EventTimer(int runTimerForSeconds, int eventEmissionInterval = 1) : IDisposable
{
    /// <summary>
    /// Elapsed time incremented by interval. This is not incremented at realtime by delta.
    /// </summary>
    private int _elapsedTime;

    private CancellationTokenSource _cancellationTokenSource = new();

    /// <summary>
    /// Event raised at set interval. Useful for calling external actions after certain amount time. Contains data
    /// about how many times the events were raised.
    /// </summary>
    public event EventHandler<EventTimerEventArgs>? OnInterval;

    /// <summary>
    /// Event raised when the timer is started by external components.
    /// </summary>
    public event EventHandler<EventArgs>? OnStart;

    /// <summary>
    /// Event raised once the timer has finished running. Omitted when the timer was aborted.
    /// </summary>
    public event EventHandler<EventArgs>? OnFinished;

    /// <summary>
    /// Describes how many times the event was raised.
    /// </summary>
    public int EventsRaisedCount { get; private set; }

    /// <summary>
    /// Flag preventing the Start method from firing if the Timer is already running.
    /// </summary>
    public bool IsStillRunning { get; private set; }

    /// <summary>
    /// Event will be raised every [x] seconds defined by this value. By default, this value is 1 unless specified
    /// otherwise in the constructor. This value is not allowed to be greater than <c>TotalTimerTime</c> and it will be
    /// equalized. This value also has to be a factor of <c>TotalTimerTime</c>.
    /// </summary>
    public int EventEmissionInterval { get; } =
        eventEmissionInterval > runTimerForSeconds ? runTimerForSeconds : eventEmissionInterval;

    /// <summary>
    /// Describes how long will this timer be allowed to run for.
    /// </summary>
    public int TotalTimerTime { get; } = runTimerForSeconds;

    /// <summary>
    /// Starts the timer on new cancellation token.
    /// </summary>
    public async Task Start()
    {
        // Disallow non-multiple intervals of the total timer value
        if (TotalTimerTime % EventEmissionInterval != 0)
        {
            throw new NonMultipleIntervalException();
        }

        if (IsStillRunning)
        {
            return;
        }

        ResetInternals();
        RaiseEventOnStart();

        IsStillRunning = true;

        await RunTimer(_cancellationTokenSource.Token).ConfigureAwait(false);
    }

    public void Restart()
    {
        Stop();
        ResetInternals();

        _ = Start();
    }

    /// <summary>
    /// Allows stopping the timer early.
    /// </summary>
    public void Stop(bool skipOnFinishEvent = false)
    {
        IsStillRunning = false;

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        if (skipOnFinishEvent)
        {
            return;
        }

        RaiseEventOnTimerFinish();
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

            EventsRaisedCount++;
            _elapsedTime += EventEmissionInterval;

            if (_elapsedTime >= TotalTimerTime)
            {
                RaiseEventOnTimerFinish();
                Stop();
            }

            RaiseEventOnInterval();
        }
    }

    private void ResetInternals()
    {
        _cancellationTokenSource = new();
        _elapsedTime = 0;
        EventsRaisedCount = 0;
    }

    private void RaiseEventOnStart() => OnStart?.Invoke(this, new());

    private void RaiseEventOnTimerFinish() => OnFinished?.Invoke(this, new());

    private void RaiseEventOnInterval() => OnInterval?.Invoke(this, new(EventsRaisedCount));
}
