using Godot;
using System;

public partial class GluttonyBoss : Enemy {

	[Export] private CanvasLayer bossHud;
	[Export] private AnimatedSprite2D[] sprites;
	[Export] private CollisionShape2D[] hitboxes;
	[Export] private GpuParticles2D[] gibs;

	[ExportGroup("Attacks")]
	[Export] private float attackCooldown = 0.2f;
	[Export] private float damageDelay = 0.2f;
	[Export] private float seperation = 32;
	[Export] private float speed = 50;

	private float[] timeUntilAttack;
	private float[] rotation;

	private uint hitboxLayer;

	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public override void _Ready() {
		base._Ready();

		timeUntilAttack = new float[sprites.Length];
		rotation = new float[sprites.Length];
		for (int i = 0; i < timeUntilAttack.Length; i++) {
			timeUntilAttack[i] = (rng.Randf() * 0.75f * attackCooldown) + (attackCooldown * 0.25f);
			rotation[i] = rng.Randf() * Mathf.Tau;
		}

	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (CurrentHealth > 0) {
			MoveComponents((float) delta);
		}

		if (!IsActive) return;

		if (sprites[0].Visible) {
			foreach (AnimatedSprite2D sprite in sprites) sprite.SelfModulate = sprite.SelfModulate.Lerp(Colors.White, (float) delta * 10);
		}

		if (CurrentHealth > 0) {
			UpdateAttacks((float) delta);
		}

	}

	private void UpdateAttacks(float delta) {
		for (int i = 0; i < timeUntilAttack.Length; i++) {
			if (timeUntilAttack[i] > 0) {
				timeUntilAttack[i] -= delta;

				if (timeUntilAttack[i] <= 0) {

					AnimatedSprite2D sprite = sprites[i];
					timeUntilAttack[i] = attackCooldown;
					sprite.Play("attack");
					sprites[i].Rotation = 0;
					GetTree().CreateTimer(damageDelay).Timeout += () => {

						if (IsInstanceValid(sprite)) CreateAttack(sprite.GlobalPosition);

						GetTree().CreateTimer(damageDelay).Timeout += () => {
							if (IsInstanceValid(sprite)) sprite.Play("idle");
						};
						
					};
					
				}
			}
		}
	}

	private void MoveComponents(float delta) {
		for (int i = 0; i < sprites.Length; i++) {
			if (sprites[i].Animation == "idle") {
				rotation[i] += delta * speed * (1f / (i + 1f));

				sprites[i].Position = Vector2.Right.Rotated(rotation[i]) * (i + 1) * seperation;
				sprites[i].Rotation = rotation[i] + (Mathf.Pi * 0.5f);
				hitboxes[i].GlobalPosition = sprites[i].GlobalPosition;
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
		GameManager.Instance.ActivatedAbilities[GameManager.Instance.CurrentLevel + 2] = true;
		foreach (GpuParticles2D particles in gibs) particles.Emitting = true;
		foreach (AnimatedSprite2D sprite in sprites) sprite.Visible = false;

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
		foreach (AnimatedSprite2D sprite in sprites) {
			sprite.SelfModulate = Colors.Red;
		}
	}

	private void CreateAttack(Vector2 position) {
		Area2D attack = new Area2D();
		attack.AddChild(new CollisionShape2D() {
			Shape = new CircleShape2D() {
				Radius = 64
			}
		});
		attack.GlobalPosition = position;

		attack.BodyEntered += (body) => {
			if (body is Player) {
				GameManager.Instance.CurrentAir -= 0.5f;
				GameManager.Instance.Player.OnDamage();
				attack.QueueFree();
			}
		};

		GetTree().CurrentScene.AddChild(attack);
		GetTree().CreateTimer(1).Timeout += () => {
			if (IsInstanceValid(attack)) attack.QueueFree();
		};
	}

}
