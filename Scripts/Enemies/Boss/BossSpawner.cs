using Godot;
using System;

public partial class BossSpawner : Node2D {

	[Export] private PackedScene bossPrefab;

	public override void _Ready() {
		base._Ready();

		Node2D boss = bossPrefab.Instantiate<Node2D>();
		boss.Name = "Boss";

		boss.GlobalPosition = this.GlobalPosition;

		GetTree().Root.AddChild(boss);
	}

}
