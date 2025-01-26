using Godot;
using System;

public partial class SlothAirBubble : Area2D {

	[Export] private float speed = 10f;
	[Export] private float maxLifetime = 10f;
	[Export] private Curve healing;

	private float lifetime = 0;
	private bool isInRange;

	public override void _Ready() {
		base._Ready();

		this.BodyEntered += this.OnBodyEnter;
		this.BodyExited += this.OnBodyExit;

	}

	private void OnBodyExit(Node2D body) {
		if (body is Player) isInRange = false;
	}

	private void OnBodyEnter(Node2D body) {
		if (body is Player) isInRange = true;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		lifetime += (float) delta;

		this.GlobalPosition = this.GlobalPosition.MoveToward(GameManager.Instance.Player.GlobalPosition, (float) delta);

		float lifePercent = lifetime / maxLifetime;
		if (lifePercent < 1) {
			GameManager.Instance.CurrentAir += healing.Sample(lifePercent);
		} else {
			QueueFree();
		}

	}

}
