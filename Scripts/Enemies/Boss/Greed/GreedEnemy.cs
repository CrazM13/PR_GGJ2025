using Godot;
using System;

public partial class GreedEnemy : Enemy {

	[Export] private Node2D sprite;
	[Export] private float speed = 100;
	[Export] private float airCost = 0.1f;

	private bool isHunting = false;
	private Vector2? randomPosition;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public override void _Process(double delta) {
		base._Process(delta);

		if (sprite.Visible) {
			sprite.SelfModulate = sprite.SelfModulate.Lerp(Colors.White, (float) delta * 10);
		}

		if (isHunting) {
			Vector2 playerPos = GameManager.Instance.Player.GlobalPosition;
			sprite.LookAt(playerPos);
			this.GlobalPosition = this.GlobalPosition.MoveToward(playerPos, (float) delta * speed);

			if (this.GlobalPosition == playerPos) {
				GameManager.Instance.AirPercentage -= airCost;
				isHunting = false;
			}
		} else {
			if (!randomPosition.HasValue) {
				randomPosition = this.GlobalPosition + new Vector2(rng.RandfRange(-1, 1) * 64, rng.RandiRange(-1, 1) * 64);

				if (!this.TestMove(this.Transform, randomPosition.Value - this.GlobalPosition)) {
					isHunting = true;
				}
			}

			sprite.LookAt(randomPosition.Value);
			this.GlobalPosition = this.GlobalPosition.MoveToward(randomPosition.Value, (float) delta * speed);

			if (this.GlobalPosition == randomPosition.Value) {
				randomPosition = null;
			}
		}
	}

	public override void ActivateEnemy() { /*MT*/ }

	public override void OnDamage() {
		sprite.SelfModulate = Colors.Red;
	}

	public override void OnDeath() {
		QueueFree();
	}
}