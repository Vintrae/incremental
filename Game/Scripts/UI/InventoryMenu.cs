using Godot;
using System.Collections.Generic;
using Incremental.Game.Scripts.Services;

namespace Incremental.Game.Scripts.UI;

public partial class InventoryMenu : Control
{
	private IReadOnlyDictionary<string, int> _resources;
	private EconomyService _economyService;

	public override void _Ready()
	{
		_economyService = GetNode<EconomyService>("../../Services/EconomyService");
		_resources = _economyService.GetResourceInventory();
		BuildInventoryGrid();
	}

	private void BuildInventoryGrid()
	{
		var panel = GetNode<PanelContainer>("PanelContainer");
		var content = new VBoxContainer
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill
		};
		panel.AddChild(content);

		var header = new HBoxContainer();
		content.AddChild(header);

		var gridMargin = new MarginContainer
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill
		};
		gridMargin.AddThemeConstantOverride("margin_left", 24);
		gridMargin.AddThemeConstantOverride("margin_top", 24);
		gridMargin.AddThemeConstantOverride("margin_right", 24);
		gridMargin.AddThemeConstantOverride("margin_bottom", 24);
		content.AddChild(gridMargin);

		var scrollContainer = new ScrollContainer
		{
			HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
			VerticalScrollMode = ScrollContainer.ScrollMode.Auto,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill
		};
		gridMargin.AddChild(scrollContainer);

		var rows = new VBoxContainer
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill
		};
		scrollContainer.AddChild(rows);

		const int columnCount = 5;
		const float cellWidth = 112;
		const float cellHeight = 112;
		var resources = new List<KeyValuePair<string, int>>(_resources);
		var rowCount = Mathf.Max(1, (resources.Count + columnCount - 1) / columnCount);

		for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
		{
			var row = new HBoxContainer
			{
				SizeFlagsHorizontal = SizeFlags.ExpandFill
			};
			row.AddThemeConstantOverride("separation", 8);
			rows.AddChild(row);

			for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
			{
				var resourceIndex = rowIndex * columnCount + columnIndex;
				var cell = new PanelContainer
				{
					CustomMinimumSize = new Vector2(cellWidth, cellHeight),
					SizeFlagsHorizontal = SizeFlags.ExpandFill
				};
				row.AddChild(cell);

				var cellMargin = new MarginContainer
				{
					SizeFlagsHorizontal = SizeFlags.ExpandFill,
					SizeFlagsVertical = SizeFlags.ExpandFill
				};
				cellMargin.AddThemeConstantOverride("margin_top", 8);
				cellMargin.AddThemeConstantOverride("margin_bottom", 16);
				cell.AddChild(cellMargin);

				var cellContent = new VBoxContainer
				{
					SizeFlagsHorizontal = SizeFlags.ExpandFill,
					SizeFlagsVertical = SizeFlags.ExpandFill,
					Alignment = BoxContainer.AlignmentMode.Center
				};
				cellMargin.AddChild(cellContent);

				if (resourceIndex >= resources.Count)
					continue;

				var resource = resources[resourceIndex];
				var icon = _economyService.GetResourceIcon(resource.Key);
				var nameLabel = new Label
				{
					Text = resource.Key,
					HorizontalAlignment = HorizontalAlignment.Center,
					AutowrapMode = TextServer.AutowrapMode.Word
				};
				cellContent.AddChild(nameLabel);

				if (icon == null)
				{
					GD.PrintErr($"No icon is defined for inventory resource '{resource.Key}'.");
				}
				else
				{
					var iconTexture = new TextureRect
					{
						Texture = icon,
						CustomMinimumSize = new Vector2(72, 72),
						ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
						StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
						SizeFlagsHorizontal = SizeFlags.ShrinkCenter
					};
					iconTexture.TooltipText = resource.Key;
					cellContent.AddChild(iconTexture);
				}

				var amountLabel = new Label
				{
					Text = resource.Value.ToString(),
					HorizontalAlignment = HorizontalAlignment.Center
				};
				cellContent.AddChild(amountLabel);
			}
		}
	}
}
