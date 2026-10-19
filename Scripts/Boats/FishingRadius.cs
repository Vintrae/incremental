using Godot;

namespace Incremental.Game.Scripts.Boats;

public partial class FishingRadius : Node2D
{
    [Export]
    public float Radius { get; set; } = 100f;

    [Export]
    public Color CircleColor { get; set; } = new Color(0.2f, 0.8f, 1f, 0.18f);

    [Export]
    public int Segments { get; set; } = 64;

    public override void _Draw()
    {
        DrawArc(
            Vector2.Zero,
            Radius,
            0f,
            Mathf.Tau,
            Segments,
            new Color(
                CircleColor.R,
                CircleColor.G,
                CircleColor.B,
                0.65f),
            2f,
            true);

        DrawCircle(Vector2.Zero, Radius, CircleColor);
    }

    public override void _Ready()
    {
        QueueRedraw();
    }
}
