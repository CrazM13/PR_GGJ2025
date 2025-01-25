using Godot;
using System;

public partial class AbilityWarp : BaseAbility {

	[Export] private PackedScene backfirePrefab;

	[Export] private ShapeCast2D shapeCast;
	public override void Initalize(Player player) { /* MT */ }

	protected override void OnUseAbility(Player player) {
		if (IsOnCooldown) return;

		Vector2 mousePos = player.GetGlobalMousePosition();

		shapeCast.GlobalPosition = mousePos;

		shapeCast.ForceShapecastUpdate();

		Node2D node = backfirePrefab.Instantiate<Node2D>();
		node.GlobalPosition = player.GlobalPosition;
		GetTree().CurrentScene.AddChild(node);

		if (!shapeCast.IsColliding()) {
			player.GlobalPosition = mousePos;
		}

		StartCooldown();
	}
}
