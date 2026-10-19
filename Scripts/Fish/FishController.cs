using Godot;

namespace Incremental.Game.Scripts.Fish;

public partial class FishController : Node2D
{
    public Domain.Fish.Fish Fish { get; private set; } = null!;

    public void Initialize(Domain.Fish.Fish fish)
    {
        Fish = fish;
    }
}
