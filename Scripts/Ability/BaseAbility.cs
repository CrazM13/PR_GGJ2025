using Godot;
using System;

public abstract partial class BaseAbility : Node {

	private bool initialized = false;

	public abstract void Initalize(Player player);
	protected abstract void OnUseAbility(Player player);

	public void UseAbility(Player player) {
		if (!initialized) Initalize(player);

		OnUseAbility(player);
	}

}
