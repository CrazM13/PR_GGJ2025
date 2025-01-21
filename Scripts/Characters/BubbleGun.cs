using Godot;
using System;

public partial class BubbleGun : Node2D {

	[Export] private PackedScene bubblePrefab;
	[Export] private float power = 1f;
	[Export] private float cooldown = 1f;
	[Export] private float spread = 20f;

	private float remainingCooldown = 0;

	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public override void _Process(double delta) {
		base._Process(delta);

		if (remainingCooldown > 0) {
			remainingCooldown -= (float) delta;
		}
	}

	public bool Fire(Vector2 direction) {

		if (remainingCooldown > 0) {
			return false;
		}

		BubbleBullet bubble = bubblePrefab.Instantiate<BubbleBullet>();
		bubble.GlobalPosition = this.GlobalPosition;

		float angleOffset = rng.RandfRange(-spread, spread);
		direction = direction.Normalized().Rotated(Mathf.DegToRad(angleOffset));

		bubble.Velocity = direction * power;
		bubble.Randomize(rng);
		GetTree().Root.AddChild(bubble);

		remainingCooldown = cooldown;
		return true;
	}

}
