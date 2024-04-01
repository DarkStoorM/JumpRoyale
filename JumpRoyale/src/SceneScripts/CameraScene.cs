using Godot;

namespace JumpRoyale;

public partial class CameraScene : Node2D
{
    public bool CanMove { get; set; }

    public override void _Process(double delta)
    {
        if (CanMove)
        {
            Position = new(Position.X, Position.Y + (float)delta * -50);
        }
    }
}
