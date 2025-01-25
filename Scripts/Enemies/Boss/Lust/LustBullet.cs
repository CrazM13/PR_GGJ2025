using Godot;
using System;
public partial class LustBullet : Bullet {

	[Export] private PackedScene decayProduct;

	protected override void OnCollision(KinematicCollision2D collision) {
		//base.OnCollision(collision);

		if (collision.GetCollider() is Player) {
			GameManager.Instance.CurrentAir -= damage;
			GameManager.Instance.Player.OnDamage();

			IsActive = false;

			sprite.Visible = false;

			QueueFree();
		} else {
			this.Velocity = this.Velocity.Bounce(collision.GetNormal());
		}

	}

	protected override void OnDecay() {
		base.OnDecay();

		Node2D decay = decayProduct.Instantiate<Node2D>();
		decay.GlobalPosition = this.GlobalPosition;

		GetTree().CurrentScene.AddChild(decay);
	}

}
