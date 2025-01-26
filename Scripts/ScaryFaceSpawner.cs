using Godot;
using System;

public partial class ScaryFaceSpawner : Node2D {

	[Export] private PackedScene scaryPrefab;

	private RandomNumberGenerator rng = new RandomNumberGenerator();

	private void Spawn() {
		Vector2 offset = new Vector2(rng.RandfRange(-256, 256), rng.RandfRange(-256, 256));

		Node2D newFace = scaryPrefab.Instantiate<Node2D>();
		newFace.GlobalPosition = this.GlobalPosition + offset;

		GetTree().CurrentScene.AddChild(newFace);
	}

}
