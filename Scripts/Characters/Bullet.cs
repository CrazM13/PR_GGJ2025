using Godot;
using System;

public partial class Bullet : CharacterBody2D {

	[Export] protected Node2D sprite;
	[Export] protected AudioStreamPlayer2D popSound;

	[Export] private float maxLifetime = 10;
	[Export] protected float damage = 1f;

	[ExportGroup("Randomization")]
	[Export] private float scaleMax;
	[Export] private float scaleMin;

	private float lifetime = 0;
	public bool IsActive { get; set; } = true;

	public void RandomizeScale(RandomNumberGenerator rng) {
		float scale = (rng.Randf() * (scaleMax - scaleMin)) + scaleMin;
		this.Scale = new Vector2(scale, scale);
		this.lifetime += rng.Randf();
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);

		lifetime += (float) delta;

		if (!IsActive) return;

		KinematicCollision2D collision = this.MoveAndCollide(this.Velocity);

		if (collision != null) {
			OnCollision(collision);
		} else if (lifetime >= maxLifetime) {
			IsActive = false;
			popSound?.Play();

			sprite.Visible = false;

			GetTree().CreateTimer(2).Timeout += () => {
				QueueFree();
			};
		}
	}

	protected virtual void OnCollision(KinematicCollision2D collision) {
		IsActive = false;
		popSound?.Play();

		sprite.Visible = false;

		GetTree().CreateTimer(2).Timeout += () => {
			QueueFree();
		};

		if (collision.GetCollider() is Enemy enemy) {
			enemy.Damage((int)damage);
		}
	}

}
