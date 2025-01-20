using Godot;
using System;

public partial class RandomAudio : AudioStreamPlayer2D {

	int lastPlayed = 0;

	[Export] private AudioStream[] streams;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public void PlayRandom() {
		if (streams.Length == 0) return;
		if (streams.Length == 1) {
			this.Stream = streams[0];
			Play();
			return;
		}

		int offset = rng.RandiRange(1, streams.Length - 1);
		int index = (lastPlayed + offset) % streams.Length;

		this.Stream = streams[index];
		Play();
		lastPlayed = index;
	}

}
