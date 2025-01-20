using Godot;
using System;

public partial class AirSource : Area2D {

	[Export] private float airRate = 0.01f;
	[Export] private GpuParticles2D particles;
	[Export] private float maxDistance = 1024;

	public override void _Ready() {
		base._Ready();

		this.BodyEntered += this.BodyEnteredArea;

	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (GameManager.Instance.Player != null) {
			float distance = this.GlobalPosition.DistanceSquaredTo(GameManager.Instance.Player.GlobalPosition);

			particles.Emitting = distance < maxDistance * maxDistance;
		}

	}

	private void BodyEnteredArea(Node2D body) {
		if (body is Player) {
			GameManager.Instance.AirPercentage += airRate;
		}
	}
}
