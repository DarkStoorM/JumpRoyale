#pragma warning disable CA2000

using JumpRoyale;
using JumpRoyale.Events;

namespace Tests.Game;

[TestFixture]
public class TimerTests
{
    private bool _triggeredTick;
    private bool _triggeredFinish;
    private int _ticksCount;

    [SetUp]
    public void SetUp()
    {
        _triggeredTick = false;
        _triggeredFinish = false;
        _ticksCount = 0;
    }

    [Test]
    public async Task CanEmitEventAtInterval()
    {
        EventTimer timer = new(1, 1);
        timer.OnInterval += Tick;

        await timer.Start().ConfigureAwait(false);

        Assert.That(_triggeredTick, Is.True);
    }

    [Test]
    public async Task CanEmitFinishEvent()
    {
        EventTimer timer = new(1, 1);
        timer.OnFinished += Finish;

        await timer.Start().ConfigureAwait(false);

        Assert.That(_triggeredFinish, Is.True);
    }

    /// <summary>
    /// Tests if after stopping the timer at first emission actually stops further execution, preventing any future
    /// event emissions.
    /// </summary>
    [Test]
    public async Task CanStopTimer()
    {
        EventTimer timer = new(2, 1);
        timer.OnInterval += Tick;
        timer.OnFinished += Finish;

        _ = timer.Start().ConfigureAwait(false);

        await Task.Delay(1100).ConfigureAwait(false);
        timer.Stop(true);
        await Task.Delay(1100).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(_ticksCount, Is.EqualTo(1));
            Assert.That(_triggeredFinish, Is.False);
        });
    }

    private void Tick(object sender, TimerEventArgs args)
    {
        _triggeredTick = true;
        _ticksCount = args.CheckpointsCount;
    }

    private void Finish(object sender, EventArgs args)
    {
        _triggeredFinish = true;
    }
}
