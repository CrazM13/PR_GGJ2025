using Godot;
using System;

public partial class CameraController : Node2D {

	[Export] Camera2D camera;
	[Export] private float speed = 200;

	private Vector2? lookAtLocal;

	private float shakeTimer;
	private float shakeStrength;

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

		if (shakeStrength > 0) {
			shakeTimer += (float) delta;

			camera.Offset = new Vector2(Mathf.Sin(shakeTimer * 13), Mathf.Cos(shakeTimer * 17)) * shakeStrength;

			shakeStrength *= 0.75f;
			if (shakeStrength < 1) {
				shakeStrength = 0;
				camera.Offset = Vector2.Zero;
			}
		}

	}

	public void Shake(float strength) {
		shakeStrength = Mathf.Max(shakeStrength, strength);
	}


}
