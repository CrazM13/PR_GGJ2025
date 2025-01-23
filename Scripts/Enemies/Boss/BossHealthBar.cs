using Godot;
using System;

public partial class BossHealthBar : TextureProgressBar {

	[Export] private Enemy boss;

	public override void _Ready() {
		base._Ready();

		this.MaxValue = boss.MaxHealth;

	}

	public override void _Process(double delta) {
		base._Process(delta);

		this.Value = boss.CurrentHealth;

	}

}
