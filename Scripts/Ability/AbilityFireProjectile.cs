using Godot;
using System;

public partial class AbilityFireProjectile : BaseAbility {

	[Export] private PackedScene bulletPrefab;
	[Export] private float power = 1f;
	[Export] private float cooldown = 1f;
	[Export] private float spread = 20f;
	[Export] private float cost = 0f;
	[Export] private float recoil = 0f;

	private float remainingCooldown = 0;

	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public override void _Process(double delta) {
		base._Process(delta);

		if (remainingCooldown > 0) {
			remainingCooldown -= (float) delta;
		}
	}

	public override void Initalize(Player player) {/* MT */}

	protected override void OnUseAbility(Player player) {
		if (remainingCooldown > 0) {
			return;
		}

		Vector2 mouseDirection = (player.GetGlobalMousePosition() - player.GlobalPosition).Normalized();

		CreateProjectile(player.GlobalPosition, mouseDirection);

		if (recoil != 0) player.MoveInDirection(-mouseDirection * recoil);

		GameManager.Instance.AirPercentage -= cost;

		remainingCooldown = cooldown;
	}

	private void CreateProjectile(Vector2 position, Vector2 direction) {
		Bullet bubble = bulletPrefab.Instantiate<Bullet>();
		bubble.GlobalPosition = position;

		float angleOffset = rng.RandfRange(-spread, spread);
		direction = direction.Normalized().Rotated(Mathf.DegToRad(angleOffset));

		bubble.Velocity = direction * power;
		bubble.RandomizeScale(rng);
		GetTree().Root.AddChild(bubble);
	}
}
