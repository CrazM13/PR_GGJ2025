using Godot;
using System;

public partial class GreedBoss : Enemy {

	[Export] private CanvasLayer bossHud;
	[Export] private AnimatedSprite2D[] sprites;
	[Export] private Node2D pearlSprite;
	[Export] private StaticBody2D[] protectors;

	[ExportGroup("Attacks")]
	[Export] private PackedScene attackPrefab;
	[Export] private float attackCooldown = 0.2f;
	[Export] private int enemiesPerAttack = 4;

	private RandomNumberGenerator rng = new RandomNumberGenerator();

	private float timeUntilAttack = 0;

	private uint protectorLayer;
	private uint hitboxLayer;

	public override void _Ready() {
		base._Ready();

		protectorLayer = protectors[0].CollisionLayer;
		hitboxLayer = this.CollisionLayer;

		timeUntilAttack = 1f;

		Open(0);

	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (!IsActive) return;

		if (sprites[0].Visible) {
			foreach (AnimatedSprite2D sprite in sprites) sprite.SelfModulate = sprite.SelfModulate.Lerp(Colors.White, (float) delta * 10);
		}

		if (CurrentHealth > 0) {
			UpdateAttacks((float) delta);
		}

	}

	private void UpdateAttacks(float delta) {
		if (timeUntilAttack > 0) {
			timeUntilAttack -= delta;

			if (timeUntilAttack <= 0) {

				int selected = rng.RandiRange(0, 3);
				Open(selected);

				timeUntilAttack = attackCooldown;

				for (int i = 0; i < 4; i++) {
					if (i == selected) continue;
					for (int _ = 0; _ < enemiesPerAttack; _++) {
						Enemy attack = attackPrefab.Instantiate<Enemy>();
						attack.GlobalPosition = sprites[i].GlobalPosition;

						GetTree().CurrentScene.AddChild(attack);
					}
				}

				GetTree().CreateTimer(attackCooldown * 0.9f).Timeout += () => {
					Close();
				};
				
			}
		}
	}

	private void Open(int selected) {
		foreach (AnimatedSprite2D sprite in sprites) {
			sprite.Play("open");
		}

		foreach (StaticBody2D body in protectors) {
			body.CollisionLayer = 1;
		}

		pearlSprite.GlobalPosition = sprites[selected].GlobalPosition + new Vector2(0, 32);
		pearlSprite.Visible = true;
		CollisionLayer = hitboxLayer;
	}

	private void Close() {
		foreach (AnimatedSprite2D sprite in sprites) {
			sprite.Play("closed");
		}

		foreach (StaticBody2D body in protectors) {
			body.CollisionLayer = protectorLayer;
		}

		pearlSprite.Visible = false;
		CollisionLayer = 0;
	}

	public override void ActivateEnemy() {
		if (IsActive) return;

		GetTree().Paused = true;
		GameManager.Instance.Player.CameraLookAt(this.GlobalPosition);
		GetTree().CreateTimer(4).Timeout += () => {
			Close();
			bossHud.Visible = true;
			GameManager.Instance.Player.CameraReset();
			GetTree().Paused = false;
		};

		IsActive = true;
	}

	public override void OnDeath() {
		GetTree().Paused = true;
		GameManager.Instance.Player.CameraLookAt(this.GlobalPosition);
		GameManager.Instance.ActivatedAbilities[GameManager.Instance.CurrentLevel + 2] = true;
		GetTree().CreateTimer(4).Timeout += () => {
			bossHud.Visible = false;
			GameManager.Instance.Player.CameraReset();
			GetTree().Paused = false;

			GameManager.Instance.WasSuccess = true;
			GameManager.Instance.Level = null;
			SceneManagement.SceneManager.Instance.LoadScene("res://Scenes/LobbyScene.tscn");
		};
		Close();
	}

	public override void OnDamage() {
		foreach (AnimatedSprite2D sprite in sprites) {
			sprite.SelfModulate = Colors.Red;
			sprite.Scale = Vector2.One;
		}
	}
}
