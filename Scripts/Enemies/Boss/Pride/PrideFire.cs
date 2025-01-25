using Godot;
using System;
using System.Collections.Generic;

public partial class PrideFire : Area2D {

	[Export] private float attackDelay;
	[Export] private float damage;
	[Export] private float maxLifetime = 30;

	private bool isInRange;
	private float timeUntilAttack;
	private float lifetime;

	public override void _Ready() {
		base._Ready();

		this.BodyEntered += this.OnBodyEnter;
		this.BodyExited += this.OnBodyExit;

		timeUntilAttack = attackDelay;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		lifetime += (float) delta;

		if (isInRange) {
			timeUntilAttack -= (float) delta;
			if (timeUntilAttack <= 0) {
				timeUntilAttack = attackDelay;

				GameManager.Instance.CurrentAir -= damage;
				GameManager.Instance.Player.OnDamage();
			}
		}

		if (lifetime > maxLifetime) {
			QueueFree();
		}

	}

	private void OnBodyExit(Node2D body) {
		if (body is Player) {
			isInRange = false;
		}
	}

	private void OnBodyEnter(Node2D body) {
		if (body is Player) {
			isInRange = true;

			GameManager.Instance.CurrentAir -= damage;
			GameManager.Instance.Player.OnDamage();
		}
	}

}
