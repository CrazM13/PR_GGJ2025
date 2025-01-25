using Godot;
using System;

public partial class O2Frog : Area2D {

	[ExportGroup("References")]
	[Export] private Textbox textBox;
	[Export] private AudioStream tts;
	[Export] private AudioStreamPlayer2D transactSFX;

	[ExportGroup("Settings")]
	[Export] private float airPerCoin = 0.01f;
	[Export] private float transactionCooldown = 0.1f;

	private bool hasInteracted = false;

	private bool isTransacting = false;
	private float currentCD = 0;

	public override void _Ready() {
		base._Ready();

		this.BodyEntered += this.OnBodyEnter;
		this.BodyExited += this.OnBodyExit;

		GameManager.Instance.CoinCount = 100;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (isTransacting && GameManager.Instance.CoinCount > 0) {
			currentCD -= (float) delta;
			if (currentCD <= 0) {
				GameManager.Instance.CoinCount--;
				GameManager.Instance.MaxAir += airPerCoin;
				transactSFX.Play();

				currentCD = transactionCooldown;
			}
		}

	}

	private void OnBodyExit(Node2D body) {
		if (body is Player) isTransacting = false;
	}

	private void OnBodyEnter(Node2D body) {
		if (body is Player) {
			isTransacting = true;
			if (!hasInteracted) {
				textBox.QueueText("Ribbit [TRANSLATION: Hello there! I'm Anura!]", tts);
				textBox.QueueText("Ribbit [TRANSLATION: Bring me money and I bring you precious air!]", tts);
				textBox.QueueText("Ribbit [TRANSLATION: I have a lot of air to spare!]", tts);

				textBox.NextMessage();
				hasInteracted = true;
			}
		}
	}
}
