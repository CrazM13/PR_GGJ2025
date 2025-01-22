using Godot;
using System;

public partial class Bullet : CharacterBody2D {

	[Export] private Sprite2D sprite;
	[Export] private AudioStreamPlayer2D popSound;

	[ExportGroup("Randomization")]
	[Export] private float scaleMax;
	[Export] private float scaleMin;

	public bool IsActive { get; set; } = true;

	public void RandomizeScale(RandomNumberGenerator rng) {
		float scale = (rng.Randf() * (scaleMax - scaleMin)) + scaleMin;
		this.Scale = new Vector2(scale, scale);
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);

		if (!IsActive) return;

		KinematicCollision2D collision = this.MoveAndCollide(this.Velocity);

		if (collision != null) {
			IsActive = false;
			popSound.Play();

			sprite.Visible = false;

			GetTree().CreateTimer(2).Timeout += () => {
				QueueFree();
			};
		}
	}

}
