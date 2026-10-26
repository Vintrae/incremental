using Godot;

public partial class InventoryMenu : Node
{
	private int _resourcesCount = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_resourcesCount = GetNode<EconomyService>("../../Services/EconomyService").GetResourceTypeCount();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
