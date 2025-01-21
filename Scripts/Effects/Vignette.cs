using Godot;
using System;

public partial class Vignette : Control {

	[Export] private float speed;
	[Export] private float maxScale;
	[Export] private float minScale;

	private float targetScale = 1;

	private float timer = 0;

	public override void _Ready() {
		base._Ready();

		this.PivotOffset = this.Size / 2;

	}

	public override void _Process(double delta) {
		base._Process(delta);

		GameManager.Instance.MonsterStrength += (float) delta * 0.01f;
		if (GameManager.Instance.MonsterStrength > 1) {
			GameManager.Instance.MonsterStrength = 1;
		}

		this.targetScale = ((1 - GameManager.Instance.MonsterStrength) * 2);
		timer += (float) delta * speed;

		float scale = (Mathf.Abs(Mathf.Sin(timer)) * (maxScale - minScale)) + minScale;
		this.Scale = new Vector2(targetScale + scale, targetScale + scale);
	}

}
