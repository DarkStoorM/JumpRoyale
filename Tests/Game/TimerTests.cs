#pragma warning disable CA2000 // Dispose objects before losing scope

using JumpRoyale;
using JumpRoyale.Events;
using JumpRoyale.Utils.Exceptions;

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
    public async Task CanRaiseEventOnStart()
    {
        bool trigger = false;

        void TickTrigger(object sender, EventArgs args)
        {
            trigger = true;
        }

        EventTimer timer = new(1, 1);

        timer.OnStart += TickTrigger;

        Assert.That(trigger, Is.False);

        await timer.Start().ConfigureAwait(false);

        Assert.That(trigger, Is.True);
    }

    [Test]
    public async Task CanRaiseEventAtInterval()
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
    public async Task CanRaiseFinishEvent()
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

    /// <summary>
    /// This test makes sure we get get stopped when trying to provide an interval that is not a multiple of the timer,
    /// because it wouldn't make sense to define a timer with irregular intervals that will not add up to the timer.
    /// This would result in not running the timer for the defined time, but for whatever "overflows".
    /// </summary>
    [Test]
    public void CanThrowOnNonMultipleInterval()
    {
        EventTimer timer = new(5, 2);

        Assert.ThrowsAsync<NonMultipleIntervalException>(timer.Start);
    }

    /// <summary>
    /// This test makes sure the provided Interval will not exceed the timer value itself, which prevents raising the
    /// events.
    /// </summary>
    [Test]
    public void CanEqualizeInterval()
    {
        EventTimer timer = new(5, 6);

        Assert.That(timer.EventEmissionInterval, Is.EqualTo(5));
    }
}
