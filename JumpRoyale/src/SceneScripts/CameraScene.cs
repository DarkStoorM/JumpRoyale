using Godot;

namespace JumpRoyale;

public partial class CameraScene : Node2D
{
    public override void _Process(double delta)
    {
        Position = new(Position.X, Position.Y + (float)delta * -50);
    }
}
