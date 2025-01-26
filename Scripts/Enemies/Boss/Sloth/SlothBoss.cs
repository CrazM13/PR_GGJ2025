using Godot;
using System;

public partial class SlothBoss : Enemy {

	private enum SlothState {
		IDLE,
		TAIL_ATTACK,
		FART_ATTACK,
		NOSE_ATTACK
	}

	[Export] private CanvasLayer bossHud;
	[Export] private Node2D bossBody;
	[Export] private AnimatedSprite2D sprite;
	[Export] private AnimatedSprite2D effects;
	[Export] private Node2D backEnd;

	[ExportGroup("Attacks")]
	[Export] private PackedScene attackPrefab;
	[Export] private float attackCooldown = 0.2f;
	[Export] private float power = 2;
	[Export] private float seperation = 32;
	[Export] private float speed = 50;
	[Export] private float runDistance = 256;

	private float timeUntilAttack;
	private SlothState state;

	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public override void _Ready() {
		base._Ready();

		timeUntilAttack = attackCooldown;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (!IsActive) return;

		sprite.Modulate = sprite.SelfModulate.Lerp(Colors.White, (float) delta * 10);

		if (CurrentHealth > 0) {
			UpdateAttacks((float) delta);
		}

	}

	private void UpdateAttacks(float delta) {
		if (state == SlothState.IDLE) UpdateIdle(delta);
		else if (state == SlothState.TAIL_ATTACK) UpdateTailAttack(delta);
		else if (state == SlothState.FART_ATTACK) UpdateFartAttack(delta);
		else if (state == SlothState.NOSE_ATTACK) UpdateNoseAttack(delta);
	}

	private void UpdateIdle(float delta) {

		bossBody.GlobalPosition = bossBody.GlobalPosition.MoveToward(this.GlobalPosition, delta * speed * 0.5f);
		bossBody.Rotation = this.GetAngleTo(GameManager.Instance.Player.GlobalPosition) + Mathf.Pi;

		if (timeUntilAttack > 0) {
			timeUntilAttack -= delta;

			if (timeUntilAttack <= 0) {
				timeUntilAttack = attackCooldown;

				int attack = rng.RandiRange(0, 2);
				switch (attack) {
					case 0:
						state = SlothState.TAIL_ATTACK;
						sprite.Play("tail_attack");
						effects.Play("tail_attack");
						timeUntilAttack = 1f;
						break;
					case 1:
						state = SlothState.FART_ATTACK;
						sprite.Play("fart");
						effects.Play("fart");
						timeUntilAttack = 0.001f;
						break;
					case 2:
						state = SlothState.NOSE_ATTACK;
						sprite.Play("idle");
						effects.Play("idle");
						break;
				}
			}
		}
	}

	private void UpdateTailAttack(float delta) {
		if (timeUntilAttack > 0) {
			timeUntilAttack -= delta;

			if (timeUntilAttack <= 0) {
				float angle = Mathf.Pi / 16f;
				for (int i = -4; i < 4; i++) {
					CreateProjectile(effects.GlobalPosition, Vector2.Left.Rotated(bossBody.Rotation + (i * angle)));
				}
			}
		} else {
			timeUntilAttack -= delta;

			if (timeUntilAttack < -2f) {
				state = SlothState.IDLE;
				sprite.Play("idle");
				effects.Play("idle");
				timeUntilAttack = attackCooldown;
			}
		}
	}

	private void UpdateFartAttack(float delta) {
		if (timeUntilAttack > 0) {
			timeUntilAttack -= delta;

			if (timeUntilAttack <= 0) {
				float angle = Mathf.Pi / 32f;
				for (int i = -4; i < 4; i++) {
					CreateProjectile(backEnd.GlobalPosition, Vector2.Right.Rotated(bossBody.Rotation + (i * angle)));
				}
			}
		} else {
			timeUntilAttack -= delta;

			bossBody.GlobalPosition += Vector2.Left.Rotated(bossBody.Rotation) * speed * delta;

			if (timeUntilAttack < -2) {
				state = SlothState.IDLE;
				sprite.Play("idle");
				effects.Play("idle");
				timeUntilAttack = attackCooldown;
			}
		}
	}

	private void UpdateNoseAttack(float delta) {
		if (timeUntilAttack > 0) {
			timeUntilAttack -= delta;

			if (timeUntilAttack <= 0) {
				float angle = Mathf.Pi / 64f;
				for (int i = -8; i < 8; i++) {
					CreateProjectile(effects.GlobalPosition, Vector2.Right.Rotated(bossBody.Rotation + (i * angle)));
				}
			}
			
		} else {
			timeUntilAttack -= delta;

			if (timeUntilAttack < -2) {
				state = SlothState.IDLE;
				sprite.Play("idle");
				effects.Play("idle");
				timeUntilAttack = attackCooldown;
			}
		}
	}

	public override void ActivateEnemy() {
		if (IsActive) return;

		GetTree().Paused = true;
		GameManager.Instance.Player.CameraLookAt(this.GlobalPosition);
		GetTree().CreateTimer(4).Timeout += () => {
			bossHud.Visible = true;
			GameManager.Instance.Player.CameraReset();
			GetTree().Paused = false;
		};

		IsActive = true;
	}

	public override void OnDeath() {
		sprite.Play("death");
		effects.Play("idle");
		GetTree().Paused = true;
		GameManager.Instance.Player.CameraLookAt(this.GlobalPosition);
		GameManager.Instance.ActivatedAbilities[GameManager.Instance.CurrentLevel + 2] = true;
		GetTree().CreateTimer(4).Timeout += () => {
			bossHud.Visible = false;
			GameManager.Instance.Player.CameraReset();
			GetTree().Paused = false;

			QueueFree();

			GameManager.Instance.WasSuccess = true;
			GameManager.Instance.Level = null;
			SceneManagement.SceneManager.Instance.LoadScene("res://Scenes/LobbyScene.tscn");
		};
	}

	public override void OnDamage() {
		sprite.Modulate = Colors.Red;
	}

	private void CreateProjectile(Vector2 position, Vector2 direction) {
		Bullet bubble = attackPrefab.Instantiate<Bullet>();
		bubble.GlobalPosition = position;

		direction = direction.Normalized();

		bubble.RandomizeScale(rng);
		bubble.Velocity = direction * power;
		GetTree().CurrentScene.AddChild(bubble);
	}
}
