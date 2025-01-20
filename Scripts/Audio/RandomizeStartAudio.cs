using Godot;
using System;

public partial class RandomizeStartAudio : AudioStreamPlayer2D {

	public override void _Ready() {
		base._Ready();

		this.Play(((this.GlobalPosition.X * 17) + (this.GlobalPosition.Y * 13) % 20) / 20);

	}

}
