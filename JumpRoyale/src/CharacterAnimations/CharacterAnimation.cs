namespace JumpRoyale;

public abstract class CharacterAnimationData(int framesCount, int framerate, bool shouldLoop)
{
    public int FramesCount { get; } = framesCount;

    public int Framerate { get; } = framerate;

    public bool ShouldLoop { get; } = shouldLoop;
}
