using Godot;
using System;

public partial class BossSpawner : Node2D {

	[Export] private PackedScene[] bossPrefabs;

	public override void _Ready() {
		base._Ready();

		Node2D boss = bossPrefabs[GameManager.Instance.CurrentLevel].Instantiate<Node2D>();
		boss.Name = "Boss";

		boss.GlobalPosition = this.GlobalPosition - new Vector2(16, 16);

		GetTree().CurrentScene.AddChild(boss);
	}

}
