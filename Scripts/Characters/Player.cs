using Godot;
using System;

public partial class Player : CharacterBody2D {

	[Export] private CharacterMovement movement;
	[Export] private RandomAudio collectLine;

	[Export] private float airRequirement = 0.01f;

	public override void _Ready() {
		base._Ready();

		this.GlobalPosition = GameManager.Instance.Level.GetStartingLocation() * 32;

		GameManager.Instance.AirPercentage = 1;
		GameManager.Instance.Player = this;
	}

	public override void _ExitTree() {
		base._ExitTree();

		if (GameManager.Instance.Player == this) {
			GameManager.Instance.Player = null;
		}
	}

	public override void _Process(double delta) {
		base._Process(delta);

		GetMovementInput((float) delta);
	}

	private void GetMovementInput(float delta) {

		Vector2 joyInput = new Vector2(Input.GetAxis("move_left", "move_right"), Input.GetAxis("move_up", "move_down"));

		movement.Move(joyInput);
	}

	private void ConsumeAirTick() {
		GameManager.Instance.AirPercentage -= airRequirement;
	}

	public void OnCollect(Collectable collectable) {
		if (!collectLine.Playing) collectLine.PlayRandom();
	}


}
