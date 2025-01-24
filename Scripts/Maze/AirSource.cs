using Godot;
using System;

public partial class AirSource : Area2D {

	[Export] private float airRate = 0.01f;
	[Export] private GpuParticles2D particles;
	[Export] private AudioStreamPlayer2D audio;
	[Export] private float maxDistance = 1024;

	private bool isInSource = false;

	public override void _Ready() {
		base._Ready();

		this.BodyEntered += this.BodyEnteredArea;
		this.BodyExited += this.BodyExitedArea;
	}

	private void BodyExitedArea(Node2D body) {
		if (body is Player) {
			isInSource = false;
		}
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (GameManager.Instance.Player != null) {
			float distance = this.GlobalPosition.DistanceSquaredTo(GameManager.Instance.Player.GlobalPosition);
		
			particles.Emitting = distance < maxDistance * maxDistance;
			audio.Playing = distance < maxDistance * maxDistance;
		}

		if (isInSource) {
			GameManager.Instance.CurrentAir += airRate * (float) delta;
		}

	}

	private void BodyEnteredArea(Node2D body) {
		if (body is Player) {
			isInSource = true;
		}
	}
}
