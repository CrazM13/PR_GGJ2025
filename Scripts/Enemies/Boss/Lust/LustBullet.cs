using Godot;
using System;
public partial class LustBullet : Bullet {

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

}
