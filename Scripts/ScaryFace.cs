using Godot;
using System;

public partial class ScaryFace : Node2D {

	[Export] private float maxLifetime;
	[Export] private Curve fade;

	private float lifetime;

	

	public override void _Process(double delta) {
		base._Process(delta);

		lifetime += (float) delta;

		this.SelfModulate = new Color(this.SelfModulate, fade.Sample(lifetime / maxLifetime));

		if (lifetime > maxLifetime) {
			QueueFree();
		}

	}

}
