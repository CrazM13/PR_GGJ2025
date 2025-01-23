using Godot;
using System;

public partial class CameraController : Node2D {

	[Export] Camera2D camera;
	[Export] private float speed = 200;

	private Vector2? lookAtLocal;

	public void CameraLookAt(Vector2 position) {
		lookAtLocal = ToLocal(position);

		this.GlobalPosition = camera.GetScreenCenterPosition();

		camera.DragVerticalEnabled = false;
		camera.DragHorizontalEnabled = false;
	}

	public void CameraLookAtLocal(Vector2 position) {
		lookAtLocal = position;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (lookAtLocal.HasValue) {
			this.Position = this.Position.MoveToward(lookAtLocal.Value, (float) delta * speed);

			if (this.Position == lookAtLocal) {
				lookAtLocal = null;
				camera.DragVerticalEnabled = true;
				camera.DragHorizontalEnabled = true;
			}
		}

	}


}
