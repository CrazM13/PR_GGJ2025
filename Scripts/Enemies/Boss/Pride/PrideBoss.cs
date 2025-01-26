using Godot;
using System;

public partial class PrideBoss : Enemy {

	[Export] private CanvasLayer bossHud;
	[Export] private Node2D sprite;
	[Export] private GpuParticles2D[] gibs;

	[ExportGroup("Attacks")]
	[Export] private PackedScene attackPrefab;
	[Export] private float attackCooldown = 0.2f;
	[Export] private float seperation = 32;
	[Export] private float speed = 50;
	[Export] private float runDistance = 256;

	private Vector2[] patrolPositions;
	private int patrolPos = 0;

	private Vector2? targetPosition;

	private float timeUntilAttack;

	private uint hitboxLayer;

	private RandomNumberGenerator rng = new RandomNumberGenerator();

	private Vector2 lastPos;

	public override void _Ready() {
		base._Ready();
		patrolPositions = new Vector2[8];
		patrolPositions[0] = this.GlobalPosition + (Vector2.Left * runDistance);
		patrolPositions[1] = this.GlobalPosition + (Vector2.Right * runDistance);
		patrolPositions[2] = this.GlobalPosition + (Vector2.Up * runDistance);
		patrolPositions[3] = this.GlobalPosition + (Vector2.Down * runDistance);
		patrolPositions[4] = this.GlobalPosition + (Vector2.Left * runDistance) + (Vector2.Up * runDistance);
		patrolPositions[5] = this.GlobalPosition + (Vector2.Right * runDistance) + (Vector2.Up * runDistance);
		patrolPositions[6] = this.GlobalPosition + (Vector2.Left * runDistance) + (Vector2.Down * runDistance);
		patrolPositions[7] = this.GlobalPosition + (Vector2.Right * runDistance) + (Vector2.Down * runDistance);

		lastPos = this.GlobalPosition;

		timeUntilAttack = attackCooldown;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (!IsActive) return;

		sprite.Modulate = sprite.SelfModulate.Lerp(Colors.White, (float) delta * 10);

		if (CurrentHealth > 0) {

			if (targetPosition.HasValue) {
				Vector2 distance = targetPosition.Value - this.GlobalPosition;
				if (distance.LengthSquared() > speed * speed) {
					distance = distance.Normalized();
					this.Velocity = distance * speed;
				} else {
					this.Velocity = Vector2.Zero;
					int newPatrolPosition = rng.RandiRange(0, patrolPositions.Length - 1);

					if (newPatrolPosition == patrolPos) {
						targetPosition = null;
					} else {
						targetPosition = patrolPositions[newPatrolPosition];
					}
				}

			} else {
				UpdateAttacks((float) delta);
			}


			this.MoveAndCollide(this.Velocity);

			if (this.GlobalPosition.DistanceSquaredTo(lastPos) > seperation * seperation) {
				Node2D attack = attackPrefab.Instantiate<Node2D>();
				attack.GlobalPosition = sprite.GlobalPosition;

				GetTree().CurrentScene.AddChild(attack);
				lastPos = this.GlobalPosition;
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

	private void UpdateAttacks(float delta) {
		if (timeUntilAttack > 0) {
			timeUntilAttack -= delta;

			if (timeUntilAttack <= 0) {
				timeUntilAttack = attackCooldown;

				targetPosition = patrolPositions[patrolPos];
				patrolPos = (patrolPos + 1) % patrolPositions.Length;
			}
		}
	}

	public override void OnDamage() {
		sprite.Modulate = Colors.Red;
	}
}
