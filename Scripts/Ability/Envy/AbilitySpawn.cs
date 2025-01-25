using Godot;
using System;

public partial class AbilitySpawn : BaseAbility {

	[Export] private PackedScene prefab;
	[Export] private int spawnCount;

	public override void Initalize(Player player) { /*MT*/ }

	protected override void OnUseAbility(Player player) {
		if (IsOnCooldown) {
			return;
		}

		for (int i = 0; i < spawnCount; i++) {
			Node2D newNode = prefab.Instantiate<Node2D>();
			newNode.GlobalPosition = player.GlobalPosition;
			GetTree().CurrentScene.AddChild(newNode);
		}

		StartCooldown();
	}
}
