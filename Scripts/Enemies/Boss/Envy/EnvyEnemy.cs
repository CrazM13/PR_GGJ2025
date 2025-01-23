using Godot;
using System;

public partial class EnvyEnemy : Enemy {

	private enum EelState {
		SCATTER,
		COLLECT,
		RETURN
	}

	[Export] private Node2D sprite;
	[Export] private float speed = 100;
	[Export] private float airCost = 0.1f;
	[Export] private int healing = 1;

	public EnvyBoss Boss { get; set; }

	private EelState state;
	private Vector2? randomPosition;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public override void _Ready() {
		base._Ready();

		state = EelState.SCATTER;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (sprite.Visible) {
			sprite.SelfModulate = sprite.SelfModulate.Lerp(Colors.White, (float) delta * 10);
		}

		if (state == EelState.COLLECT) {
			Vector2 playerPos = GameManager.Instance.Player.GlobalPosition;
			sprite.LookAt(playerPos);
			this.GlobalPosition = this.GlobalPosition.MoveToward(playerPos, (float) delta * speed);

			if (this.GlobalPosition == playerPos) {
				GameManager.Instance.AirPercentage -= airCost;
				state = EelState.RETURN;
			}
		} else if (state == EelState.RETURN) {

			if (Boss != null) {
				sprite.LookAt(Boss.GlobalPosition);
				this.GlobalPosition = this.GlobalPosition.MoveToward(Boss.GlobalPosition, (float) delta * speed);

				if (this.GlobalPosition == Boss.GlobalPosition) {
					Boss.CurrentHealth += healing;
					state = EelState.SCATTER;
				}
			} else {
				state = EelState.SCATTER;
			}

		} else {
			if (!randomPosition.HasValue) {
				randomPosition = this.GlobalPosition + new Vector2(rng.RandfRange(-1, 1) * 64, rng.RandiRange(-1, 1) * 64);

				if (!this.TestMove(this.Transform, randomPosition.Value - this.GlobalPosition)) {
					state = EelState.COLLECT;
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
