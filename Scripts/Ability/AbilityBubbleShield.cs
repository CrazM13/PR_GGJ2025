using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AbilityBubbleShield : BaseAbility {

	[Export] private PackedScene bulletPrefab;
	[Export] private float power = 1f;
	[Export] private int projectileCount = 8;
	[Export] private float cost = 0f;

	private Node2D[] shieldComponents;

	public override void _Process(double delta) {
		base._Process(delta);

		shieldComponents ??= new Node2D[16];

		for (int i = shieldComponents.Length - 1; i >= 0; i--) {
			if (shieldComponents[i] == null) continue;

			if (shieldComponents[i].GetChildCount() > 0) {

				shieldComponents[i].GlobalPosition = GameManager.Instance.Player.GlobalPosition;
				shieldComponents[i].Rotate((float) delta * power);

			} else {
				shieldComponents[i].QueueFree();
				shieldComponents[i] = null;
			}
		}

	}

	public override void Initalize(Player player) { /*MT*/ }

	protected override void OnUseAbility(Player player) {
		if (IsOnCooldown) {
			return;
		}

		int ring = -1;
		for (int i = 0; i < shieldComponents.Length; i++) {
			if (shieldComponents[i] == null) {
				ring = i;
				break;
			}
		}
		if (ring == -1) {
			return;
		}

		int realCount = projectileCount * (ring + 1);
		float spreadIterval = Mathf.Tau / realCount;
		Node2D shield = new Node2D {
			GlobalPosition = player.GlobalPosition
		};

		for (int i = 0; i < realCount; i++) {
			CreateProjectile(Vector2.Right.Rotated(spreadIterval * i), shield, ring);
		}

		shieldComponents[ring] = shield;
		GameManager.Instance.CurrentAir -= cost;

		GetTree().CurrentScene.AddChild(shield);
		StartCooldown();
	}

	private void CreateProjectile(Vector2 direction, Node2D parent, int ring) {
		direction = direction.Normalized();

		Bullet bubble = bulletPrefab.Instantiate<Bullet>();
		bubble.Position = direction * (32 * (ring + 1));
		bubble.RandomizeScale(RNG);

		parent.AddChild(bubble);
	}
}
