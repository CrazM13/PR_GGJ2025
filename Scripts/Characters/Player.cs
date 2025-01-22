using Godot;
using System;

public partial class Player : CharacterBody2D {

	[Export] private CharacterMovement movement;
	[Export] private RandomAudio collectLine;
	[Export] private AnimatedSprite2D sprite;
	[Export] private BaseAbility ability;

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

		if (Input.IsActionPressed("use_action")) {

			ability.UseAbility(this);
		}
	}

	private void GetMovementInput(float delta) {

		Vector2 joyInput = new Vector2(Input.GetAxis("move_left", "move_right"), Input.GetAxis("move_up", "move_down"));

		if (joyInput.LengthSquared() != 0) {

			if (sprite.Animation != "dead") {
				if (movement.IsMoving()) {
					if (sprite.Animation != "walking") sprite.Play("walking");
				} else {
					if (sprite.Animation != "idle") sprite.Play("idle");
				}
			}

			movement.SpeedModifier = Input.IsActionPressed("move_sprint") ? 3f : 1;
			if (movement.SpeedModifier != 1) {
				GameManager.Instance.AirPercentage -= airRequirement * delta;
			}

		} else {
			if (sprite.Animation == "walking") sprite.Play("idle");
		}

		

		movement.Move(joyInput);
	}

	private void ConsumeAirTick() {
		GameManager.Instance.AirPercentage -= airRequirement;
	}

	public void OnCollect(Collectable collectable) {
		if (!collectLine.Playing) collectLine.PlayRandom();
	}

	public void Die() {
		if (sprite.Animation != "dead") sprite.Play("dead");
	}

	public void MoveInDirection(Vector2 direction) {
		movement.Move(direction);
	}

}
