using Godot;
using System;

public partial class LightningAttack : Node2D {

	[Export] private AnimatedSprite2D attackSprite;
	[Export] private GpuParticles2D staticParticles;
	[Export] private float warningDuration = 1;
	[Export] private float attackDuration = 1;
	[Export] private float attackRange = 16;
	[Export] private float attackDamage = 0.25f;

	float timer = 0;

	public override void _Ready() {
		base._Ready();

		timer = warningDuration;
	}

	public override void _Process(double delta) {
		base._Process(delta);
		if (timer > 0) {
			timer -= (float) delta;
			float newScale = ((timer / warningDuration) * 0.9f) + 0.1f;

			staticParticles.Scale = new Vector2(newScale, newScale);

			if (timer <= 0) {
				timer = 0f;
				attackSprite.Visible = true;

				Vector2 playerPos = GameManager.Instance.Player.GlobalPosition;

				if (playerPos.DistanceSquaredTo(GlobalPosition) < attackRange * attackRange) {
					GameManager.Instance.AirPercentage -= attackDamage;
				}

				GetTree().CreateTimer(attackDuration).Timeout += () => {
					QueueFree();
				};
			}
		}
	}

}
