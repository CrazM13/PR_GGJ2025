using Godot;
using System;

public partial class FollowTrack : PathFollow2D {

	[Export] private float speed;

	public override void _Process(double delta) {
		base._Process(delta);

		this.Progress += (float) delta * speed;

	}

}
