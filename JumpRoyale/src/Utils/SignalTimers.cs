using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Godot;

namespace JumpRoyale;

public static class SignalTimers
{
    public static async Task StartTimer(
        [NotNull] Node source,
        [NotNull] Func<bool> lambda,
        float interval,
        StringName signalToEmit = null!
    )
    {
        await source.ToSignal(source.GetTree().CreateTimer(interval), SceneTreeTimer.SignalName.Timeout);

        if (lambda())
        {
            _ = StartTimer(source, lambda, interval);
            return;
        }

        if (signalToEmit is not null)
        {
            source.EmitSignal(signalToEmit);
        }
    }
}
