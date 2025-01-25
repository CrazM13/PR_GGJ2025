using Godot;
using System;

public partial class EnvyAllyDetection : Area2D {

	[Export] private EnvyAlly ally;

	public override void _Ready() {
		base._Ready();

		this.BodyEntered += this.OnBodyEntered;
		this.AreaEntered += this.OnAreaEntered;

	}

	private void OnAreaEntered(Area2D area) {
		if (area is AirSource airSource) {
			ally.SetTarget(airSource);
		}
	}

	private void OnBodyEntered(Node2D body) {
		if (body is Enemy enemy) {
			ally.SetTarget(enemy);
		}
	}
}
