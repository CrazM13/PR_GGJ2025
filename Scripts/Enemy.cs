using Godot;
using System;

public abstract partial class Enemy : CharacterBody2D {
	[Export] public int MaxHealth { get; set; } = 100;
	public bool IsActive { get; set; } = false;

	public int CurrentHealth { get; set; }

	public override void _Ready() {
		base._Ready();

		CurrentHealth = MaxHealth;
	}

	public abstract void ActivateEnemy();
	public abstract void OnDeath();
	public abstract void OnDamage();

	public void Damage(int damage) {
		OnDamage();

		if (CurrentHealth > 0) {
			CurrentHealth -= damage;

			if (CurrentHealth <= 0) {
				OnDeath();
			}
		}

	}
}
