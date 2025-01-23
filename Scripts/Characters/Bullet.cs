using Godot;
using System;

public partial class Bullet : CharacterBody2D {

	[Export] private Node2D sprite;
	[Export] private AudioStreamPlayer2D popSound;

	[Export] private float maxLifetime = 10;
	[Export] private int damage = 1;

	[ExportGroup("Randomization")]
	[Export] private float scaleMax;
	[Export] private float scaleMin;

	private float lifetime = 0;
	public bool IsActive { get; set; } = true;

	public void RandomizeScale(RandomNumberGenerator rng) {
		float scale = (rng.Randf() * (scaleMax - scaleMin)) + scaleMin;
		this.Scale = new Vector2(scale, scale);
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);

		lifetime += (float) delta;

		if (!IsActive) return;

		KinematicCollision2D collision = this.MoveAndCollide(this.Velocity);

		if (collision != null) {
			IsActive = false;
			popSound?.Play();

			sprite.Visible = false;

			GetTree().CreateTimer(2).Timeout += () => {
				QueueFree();
			};

			if (collision.GetCollider() is Enemy enemy) {
				enemy.Damage(damage);
			}
		} else if (lifetime >= maxLifetime) {
			IsActive = false;
			popSound?.Play();

			sprite.Visible = false;

			GetTree().CreateTimer(2).Timeout += () => {
				QueueFree();
			};
		}
	}

}
