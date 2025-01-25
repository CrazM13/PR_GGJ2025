using Godot;
using System;

public partial class AbilityFireSpreadProjectile : BaseAbility {

	[Export] private PackedScene bulletPrefab;
	[Export] private float power = 1f;
	[Export] private float spread = 20f;
	[Export] private int projectileCount = 4;
	[Export] private float cost = 0f;
	[Export] private float recoil = 0f;

	public override void Initalize(Player player) {/* MT */}

	protected override void OnUseAbility(Player player) {
		if (IsOnCooldown) {
			return;
		}

		Vector2 mouseDirection = (player.GetGlobalMousePosition() - player.GlobalPosition).Normalized();

		float spreadIterval = spread / projectileCount;
		for (int i = -(int)(projectileCount * 0.5f); i < (int) (projectileCount * 0.5f); i++) {
			CreateProjectile(player.GlobalPosition, mouseDirection.Rotated(Mathf.DegToRad(spreadIterval * i)));
		}

		if (recoil != 0) player.MoveInDirection(-mouseDirection * recoil);

		GameManager.Instance.CurrentAir -= cost;

		StartCooldown();
	}

	private void CreateProjectile(Vector2 position, Vector2 direction) {
		Bullet bubble = bulletPrefab.Instantiate<Bullet>();
		bubble.GlobalPosition = position;

		direction = direction.Normalized();

		bubble.Velocity = direction * power;
		bubble.RandomizeScale(RNG);
		GetTree().CurrentScene.AddChild(bubble);
	}

}
