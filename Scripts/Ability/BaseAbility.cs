using Godot;
using System;

public abstract partial class BaseAbility : Node {

	private bool initialized = false;

	[Export] private float cooldown = 1f;
	public float RemainingCooldown { get; set; } = 0;

	public bool IsOnCooldown => RemainingCooldown > 0;
	public float CooldownPercentage => RemainingCooldown / cooldown;

	protected RandomNumberGenerator RNG { get; private set; } = new RandomNumberGenerator();

	public override void _Process(double delta) {
		base._Process(delta);

		if (RemainingCooldown > 0) {
			RemainingCooldown -= (float) delta;
		}
	}

	public abstract void Initalize(Player player);
	protected abstract void OnUseAbility(Player player);

	public void UseAbility(Player player) {
		if (!initialized) Initalize(player);

		OnUseAbility(player);
	}

	public void StartCooldown() {
		RemainingCooldown = cooldown;
	}

}
