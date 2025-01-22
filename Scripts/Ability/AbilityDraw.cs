using Godot;
using System;

public partial class AbilityDraw : BaseAbility {

	[Export] private Color colour = Colors.White;
	[Export] private Texture2D texture;
	[Export] private float scale = 0.1f;
	[Export] private float spacing = 4f;
	[Export] private bool showOnMinimap = false;

	private Node container;
	private Vector2 lastPosition = Vector2.Zero;

	public override void Initalize(Player player) {
		container = new Node() {
			Name = "Drawing Container"
		};

		GetTree().Root.AddChild(container);
	}

	protected override void OnUseAbility(Player player) {
		
		Vector2 target = player.GetGlobalMousePosition();

		if (lastPosition.DistanceSquaredTo(target) > spacing * spacing) {
			Sprite2D sprite = new Sprite2D() {
				Texture = texture,
				Scale = new Vector2(scale, scale),
				SelfModulate = colour,
				GlobalPosition = target
			};

			container.AddChild(sprite);

			lastPosition = target;

			if (showOnMinimap) UpdateMinimap(target);
		}
	}

	public void UpdateMinimap(Vector2 globalPosition) {
		Vector2I cellPosition = new Vector2I(Mathf.RoundToInt(globalPosition.X / 32), Mathf.RoundToInt(globalPosition.Y / 32)) + new Vector2I(16, 16);

		if (cellPosition.X < 0 || cellPosition.Y < 0) {
			return;
		}

		GameManager.Instance.Level.Minimap[cellPosition.X, cellPosition.Y] = colour;
	}
}
