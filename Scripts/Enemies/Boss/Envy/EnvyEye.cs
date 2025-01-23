using Godot;
using System;

public partial class EnvyEye : Node2D {

	public override void _Process(double delta) {
		base._Process(delta);

		LookAt(GameManager.Instance.Player.GlobalPosition);

		GetChild<Node2D>(0).Rotation = -this.Rotation;

	}

}
