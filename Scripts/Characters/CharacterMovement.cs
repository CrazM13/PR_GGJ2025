using Godot;
using System;

public partial class CharacterMovement : Node {

	[Export] private CharacterBody2D sourceEntity;
	[Export] private float baseMovementSpeed = 1f;
	[Export] private float drag = 0.5f;

	public float SpeedModifier { get; set; } = 1f;
	public float DragModifier { get; set; } = 1f;

	public bool IsMoving() {
		return sourceEntity.Velocity.LengthSquared() != 0;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		sourceEntity.MoveAndSlide();

		sourceEntity.Velocity *= 1f - (drag * DragModifier);
	}

	public void Move(Vector2 direction) {
		sourceEntity.Velocity += direction * baseMovementSpeed * SpeedModifier;
	}

}
