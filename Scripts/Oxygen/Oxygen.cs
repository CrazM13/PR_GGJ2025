using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class Oxygen : TextureProgressBar {

	[Export] private float minLength = 32;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {

		this.Size = new Vector2(this.Size.X, minLength * GameManager.Instance.MaxAir);
		this.Value = GameManager.Instance.GetAirPercentage() * 0.6f;
	}
}
