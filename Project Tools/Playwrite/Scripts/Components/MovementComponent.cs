using Godot;
using System;

namespace Playwrite {
	[GlobalClass]
	public partial class MovementComponent : EntityComponent {

		[Export] private float baseMovementSpeed = 1f;
		[Export] private float drag = 0.5f;

		public float SpeedModifier { get; set; } = 1f;
		public float DragModifier { get; set; } = 1f;

		public bool IsMoving() {
			return SourceEntity.Velocity.LengthSquared() != 0;
		}

		public override void _Process(double delta) {
			base._Process(delta);

			SourceEntity.MoveAndSlide();

			SourceEntity.Velocity *= 1f - (drag * DragModifier);
		}

		public void Move(Vector2 direction) {
			SourceEntity.Velocity += direction * baseMovementSpeed * SpeedModifier;
		}

	}
}
