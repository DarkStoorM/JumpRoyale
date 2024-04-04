#pragma warning disable CA2000

using JumpRoyale;
using JumpRoyale.Events;

namespace Tests.Game;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class TimerTests
{
    private bool _triggeredTick;
    private bool _triggeredFinish;

    [SetUp]
    public void SetUp()
    {
        _triggeredTick = false;
        _triggeredFinish = false;
    }

    [Test]
    public async Task CanEmitEventAtInterval()
    {
        EventTimer timer = new(1, 1);
        timer.OnCheckpointReached += Tick;

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

    private void Tick(object sender, TimerEventArgs args)
    {
        _triggeredTick = true;
    }

    private void Finish(object sender, EventArgs args)
    {
        _triggeredFinish = true;
    }
}
