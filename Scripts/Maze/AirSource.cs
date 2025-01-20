using Godot;
using System;

public partial class AirSource : Area2D {

	[Export] private float airRate = 0.01f;

	public override void _Ready() {
		base._Ready();

		this.BodyEntered += this.BodyEnteredArea;

	}

	private void BodyEnteredArea(Node2D body) {
		if (body is Player) {
			GameManager.Instance.AirPercentage += airRate;
		}
	}
}
