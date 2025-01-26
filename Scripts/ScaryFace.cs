using Godot;
using System;

public partial class ScaryFace : Node2D {

	[Export] private float maxLifetime;
	[Export] private Curve fade;

	private float lifetime;

	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public override void _Ready() {
		base._Ready();

		lifetime = rng.RandfRange(-10f, -2f);

	}

	public override void _Process(double delta) {
		base._Process(delta);

		lifetime += (float) delta;

		this.SelfModulate = new Color(this.SelfModulate, fade.Sample(lifetime / maxLifetime));

		if (lifetime > maxLifetime) {
			lifetime = 0;
			this.Position = new Vector2(rng.RandfRange(-256, 256), rng.RandfRange(-256, 256));
		}

	}

}
