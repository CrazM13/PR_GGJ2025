using Godot;
using System;

public partial class LustBoss : Enemy {

	[Export] private Sprite2D sprite;
	[Export] private CanvasLayer bossHud;

	[ExportGroup("Attacks")]
	[Export] private PackedScene bulletPrefab;
	[Export] private float attackCooldown = 0.2f;
	[Export] private float attackPower = 1f;


	private float timeUntilAttack = 0;
	private float angle = 0;

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

	public override void _Ready() {
		base._Ready();

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
		}
	}

	private void UpdateAttacks(float delta) {
		if (timeUntilAttack > 0) {
			timeUntilAttack -= delta;

			if (timeUntilAttack <= 0) {
				timeUntilAttack = attackCooldown;

				angle += delta * Mathf.Tau;

				CreateProjectile(sprite.GlobalPosition, GameManager.Instance.Player.GlobalPosition - sprite.GlobalPosition);
			}
		}
	}

	public override void OnDamage() {
		sprite.SelfModulate = Colors.Red;
	}

	private void CreateProjectile(Vector2 position, Vector2 direction) {
		Bullet bubble = bulletPrefab.Instantiate<Bullet>();
		bubble.GlobalPosition = position;

		direction = direction.Normalized();

		bubble.Velocity = direction * attackPower;
		GetTree().CurrentScene.AddChild(bubble);
	}

}
