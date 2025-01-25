using Godot;
using System;

public partial class EnvyAlly : CharacterBody2D {

	private enum EelState {
		SCATTER,
		ATTACK,
		RETURN
	}

	private Enemy targetEnemy;
	private AirSource targetAirSource;

	[Export] private Node2D sprite;
	[Export] private float speed = 100;
	[Export] private int damage = 4;

	private float healing = 0;

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

		if (state == EelState.ATTACK) {

			if (targetEnemy != null) {
				sprite.LookAt(targetEnemy.GlobalPosition);
				this.GlobalPosition = this.GlobalPosition.MoveToward(targetEnemy.GlobalPosition, (float) delta * speed);

				if (this.GlobalPosition == targetEnemy.GlobalPosition) {
					targetEnemy.Damage(damage);
					targetEnemy = null;
					healing = damage * 0.1f;
					state = EelState.RETURN;
				}
			} else if (targetAirSource != null) {
				sprite.LookAt(targetAirSource.GlobalPosition);
				this.GlobalPosition = this.GlobalPosition.MoveToward(targetAirSource.GlobalPosition, (float) delta * speed);

				if (this.GlobalPosition == targetAirSource.GlobalPosition) {
					healing = 0.1f;
					state = EelState.RETURN;
				}
			} else {
				state = EelState.SCATTER;
			}
			
		} else if (state == EelState.RETURN) {

			sprite.LookAt(GameManager.Instance.Player.GlobalPosition);
			this.GlobalPosition = this.GlobalPosition.MoveToward(GameManager.Instance.Player.GlobalPosition, (float) delta * speed);

			if (this.GlobalPosition == GameManager.Instance.Player.GlobalPosition) {
				GameManager.Instance.CurrentAir += healing;
				QueueFree();
			}

		} else {
			if (!randomPosition.HasValue) {
				randomPosition = this.GlobalPosition + new Vector2(rng.RandfRange(-1, 1) * 64, rng.RandiRange(-1, 1) * 64);

				if (!this.TestMove(this.Transform, randomPosition.Value - this.GlobalPosition)) {
					state = EelState.ATTACK;
				}
			}

			sprite.LookAt(randomPosition.Value);
			this.GlobalPosition = this.GlobalPosition.MoveToward(randomPosition.Value, (float) delta * speed);

			if (this.GlobalPosition == randomPosition.Value) {
				randomPosition = null;
			}
		}
	}

	public void SetTarget(Enemy enemy) {
		this.targetEnemy ??= enemy;
	}

	public void SetTarget(AirSource airSource) {
		this.targetAirSource ??= airSource;
	}

}
