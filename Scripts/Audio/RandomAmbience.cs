using Godot;
using System;

public partial class RandomAmbience : AudioStreamPlayer2D {

	int lastPlayed = 0;
	[Export] private float range;
	[Export] private AudioStream[] streams;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public void PlayAmbience() {
		this.GlobalPosition = GameManager.Instance.Player.Position;

		float x = rng.Randf();
		float y = rng.Randf();

		this.GlobalPosition += new Vector2(x, y) * range;

		this.Stream = GetRandom();
		this.Play();
	}

	private AudioStream GetRandom() {
		if (streams.Length == 0) return null;
		if (streams.Length == 1) {
			return streams[0];
		}

		int offset = rng.RandiRange(1, streams.Length - 1);
		int index = (lastPlayed + offset) % streams.Length;

		lastPlayed = index;
		return streams[index];
	}

}
