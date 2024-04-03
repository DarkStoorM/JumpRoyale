using System;
using System.Threading;
using System.Threading.Tasks;
using JumpRoyale.Events;

namespace JumpRoyale;

/// <summary>
/// Reusable timer class for different Timed components.
/// </summary>
public class Timer(int checkpoint) : IDisposable
{
    private CancellationTokenSource _cancellationTokenSource = new();

    public event EventHandler<GameTimerEventArgs>? OnCheckpointReached;

    public int CheckpointsReached { get; private set; }

    /// <summary>
    /// Event will be emitted every [x] seconds defined by this value.
    /// </summary>
    public int SecondsPerCheckpoint { get; } = checkpoint;

    public async Task Start()
    {
        _cancellationTokenSource = new();

        await RunTimer(_cancellationTokenSource.Token).ConfigureAwait(false);
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
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
            await Task.Delay(TimeSpan.FromSeconds(SecondsPerCheckpoint), cancellationToken).ConfigureAwait(false);

            CheckpointsReached++;

            OnCheckpointReached?.Invoke(this, new(CheckpointsReached));
        }
    }
}
