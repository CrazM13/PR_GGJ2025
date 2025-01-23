using Godot;
using System;

public partial class EnvyBoss : Enemy {

	[Export] private AnimatedSprite2D sprite;
	[Export] private CanvasLayer bossHud;

	[ExportGroup("Attacks")]
	[Export] private PackedScene attackPrefab;
	[Export] private Vector2 attackSpawnpoint;
	[Export] private float attackCooldown = 0.2f;
	[Export] private float waveCooldown = 1f;
	[Export] private int attacksPerWave = 4;

	private float timeUntilWave = 0;
	private float timeUntilAttack = 0;
	private int attacksThisWave = 0;

	private float deathTime = 0;

	public override void _Ready() {
		base._Ready();

		timeUntilWave = waveCooldown;
		timeUntilAttack = attackCooldown;
	}

	public override void _Process(double delta) {
		base._Process(delta);
		if (!IsActive) return;

		if (sprite.Visible) {
			sprite.SelfModulate = sprite.SelfModulate.Lerp(Colors.White, (float) delta * 10);
		}

		if (CurrentHealth > 0) {
			UpdateAttacks((float) delta);
		} else if (sprite.Visible) {
			deathTime += (float) delta;

			sprite.SelfModulate = new Color(sprite.SelfModulate, 1 - (deathTime / 1));

			if (deathTime >= 1) {
				sprite.Visible = false;
			}
		}

	}

	private void UpdateAttacks(float delta) {
		if (timeUntilWave > 0) {
			timeUntilWave -= delta;
			if (timeUntilWave <= 0) {
				attacksThisWave = 0;
				sprite.Play("attack");
			}
		} else if (timeUntilAttack > 0) {
			timeUntilAttack -= delta;

			if (timeUntilAttack <= 0) {
				attacksThisWave++;
				timeUntilAttack = attackCooldown;

				if (attackPrefab != null) {
					EnvyEnemy attack = attackPrefab.Instantiate<EnvyEnemy>();
					attack.GlobalPosition = this.GlobalPosition + attackSpawnpoint;
					attack.Boss = this;

					GetTree().Root.AddChild(attack);
				}

				if (attacksThisWave >= attacksPerWave) {
					sprite.Play("idle");

					timeUntilWave = waveCooldown;
				}
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
		GetTree().Paused = true;
		GameManager.Instance.Player.CameraLookAt(this.GlobalPosition);
		GameManager.Instance.ActivatedAbilities[3] = true;
		GetTree().CreateTimer(4).Timeout += () => {
			bossHud.Visible = false;
			GameManager.Instance.Player.CameraReset();
			GetTree().Paused = false;
		};
	}

	public override void OnDamage() {
		sprite.SelfModulate = Colors.Red;
		sprite.Scale = Vector2.One;
	}
}
