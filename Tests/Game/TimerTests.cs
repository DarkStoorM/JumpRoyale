#pragma warning disable CA2000 // Dispose objects before losing scope

using JumpRoyale;
using JumpRoyale.Events;

namespace Tests.Game;

/// <summary>
/// This test class defines local functions and variables to let everything run in parallel, so there are no access
/// collisions to class fields.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.All)]
public class TimerTests
{
    [Test]
    public async Task CanEmitEventAtInterval()
    {
        bool trigger = false;

        void TickTrigger(object sender, EventArgs args)
        {
            trigger = true;
        }

        EventTimer timer = new(1, 1);

        timer.OnInterval += TickTrigger;

        await timer.Start().ConfigureAwait(false);

        Assert.That(trigger, Is.True);
    }

    [Test]
    public async Task CanEmitFinishEvent()
    {
        bool finishTriggered = false;

        void FinishTrigger(object sender, EventArgs args)
        {
            finishTriggered = true;
        }

        EventTimer timer = new(1, 1);

        timer.OnFinished += FinishTrigger;

        await timer.Start().ConfigureAwait(false);

        Assert.That(finishTriggered, Is.True);
    }

    /// <summary>
    /// Tests if after stopping the timer at first emission actually stops further execution, preventing any future
    /// event emissions.
    /// </summary>
    [Test]
    public async Task CanStopTimer()
    {
        int ticksCount = 0;
        bool finishTriggered = false;

        void TickIncrement(object sender, EventTimerEventArgs args)
        {
            ticksCount++;
        }

        void FinishTrigger(object sender, EventArgs args)
        {
            finishTriggered = true;
        }

        EventTimer timer = new(2, 1);

        timer.OnInterval += TickIncrement;
        timer.OnFinished += FinishTrigger;

        _ = timer.Start().ConfigureAwait(false);

        await Task.Delay(1100).ConfigureAwait(false);
        timer.Stop(true);
        await Task.Delay(1100).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(ticksCount, Is.EqualTo(1));
            Assert.That(finishTriggered, Is.False);
        });
    }
}
