using Godot;
using System;

public partial class Collectable : Area2D {

	[Export] private Node2D sprite;
	[Export] private AudioStreamPlayer2D sfx;
	[Export] private float height = 4;

	private bool collected = false;
	private float time = 0;

	public override void _Ready() {
		base._Ready();

		this.BodyEntered += this.OnAttemptCollect;
	}

	public override void _Process(double delta) {
		base._Process(delta);
		time += (float) delta;

		if (!collected) {
			sprite.Position = new Vector2(0, Mathf.Sin(time + this.GlobalPosition.X + this.GlobalPosition.Y) * height);
		}

	}

	private void OnAttemptCollect(Node2D body) {
		if (!collected && body is Player player) {
			player.OnCollect(this);
			sprite.QueueFree();
			sfx.Play();
			collected = true;

			GameManager.Instance.CoinCount++;
		}
	}
}
