using Godot;
using System;

public partial class AbilityFireProjectile : BaseAbility {

	[Export] private PackedScene bulletPrefab;
	[Export] private float power = 1f;
	[Export] private float spread = 20f;
	[Export] private float cost = 0f;
	[Export] private float recoil = 0f;

	public override void Initalize(Player player) {/* MT */}

	protected override void OnUseAbility(Player player) {
		if (IsOnCooldown) {
			return;
		}

		Vector2 mouseDirection = (player.GetGlobalMousePosition() - player.GlobalPosition).Normalized();

		CreateProjectile(player.GlobalPosition, mouseDirection);

		if (recoil != 0) player.MoveInDirection(-mouseDirection * recoil);

		GameManager.Instance.CurrentAir -= cost;

		StartCooldown();
	}

	private void CreateProjectile(Vector2 position, Vector2 direction) {
		Bullet bubble = bulletPrefab.Instantiate<Bullet>();
		bubble.GlobalPosition = position;

		float angleOffset = RNG.RandfRange(-spread, spread);
		direction = direction.Normalized().Rotated(Mathf.DegToRad(angleOffset));

		bubble.Velocity = direction * power;
		bubble.RandomizeScale(RNG);
		GetTree().CurrentScene.AddChild(bubble);
	}
}
