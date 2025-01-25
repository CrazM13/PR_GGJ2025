using Godot;
using System;
using System.Collections.Generic;

public partial class SharkAttack : Area2D {

	[Export] private float attackDelay;
	[Export] private int damage;

	private List<Enemy> isInRange = new List<Enemy>();

	private float timeUntilAttack;

	public override void _Ready() {
		base._Ready();

		this.BodyEntered += this.OnBodyEnter;
		this.BodyExited += this.OnBodyExit;

		timeUntilAttack = attackDelay;

	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (timeUntilAttack > 0) {
			timeUntilAttack -= (float) delta;
			if (timeUntilAttack <= 0) {
				foreach (Enemy enemy in isInRange) {
					if (IsInstanceValid(enemy)) {
						enemy.Damage(damage);
					}
				}
				GameManager.Instance.Player.Camera.Shake(damage);
			}
		} else {
			timeUntilAttack -= (float) delta;
			if (timeUntilAttack <= -1) {
				QueueFree();
			}
		}

	}

	private void OnBodyExit(Node2D body) {
		if (body is Enemy enemy) {
			isInRange.Remove(enemy);
		}
	}

	private void OnBodyEnter(Node2D body) {
		if (body is Enemy enemy) {
			isInRange.Add(enemy);
		}
	}
}
