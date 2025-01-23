using Godot;
using System;

public partial class BossActivator : Area2D {

	[Export] private Enemy boss;

	public override void _Ready() {
		base._Ready();

		this.BodyEntered += this.OnBodyEntered;

	}

	private void OnBodyEntered(Node2D body) {
		if (body is Player) {
			boss.ActivateEnemy();
		}
	}
}
