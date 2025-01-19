using Godot;
using System;

namespace Playwrite {
	[GlobalClass]
	public partial class TileBasedMovementComponent : EntityComponent {

		private enum Axis {
			HORIZONTAL,
			VERTICAL
		}

		[Export] protected Vector2I tileSize = new Vector2I(16, 16);
		[Export] protected bool diagonalMovement = false;
		[Export] private Axis favouredAxis = Axis.HORIZONTAL;
		[Export] private float baseMovementSpeed = 1f;

		private Vector2I? targetTile;
		private Vector2I currentTile;

		public float SpeedModifier { get; set; } = 1f;

		public bool IsMoving() => targetTile.HasValue;

		public override void _Process(double delta) {
			base._Process(delta);

			if (targetTile.HasValue) {
				UpdateMovement((float) delta);
			}
		}

		private void UpdateMovement(float delta) {
			Vector2 targetPos = ConvertTileToGlobal(targetTile.Value, true);
			Vector2 frameTargetPos = SourceEntity.GlobalPosition.MoveToward(targetPos, baseMovementSpeed * SpeedModifier * delta);

			if (SourceEntity.TestMove(SourceEntity.Transform, frameTargetPos - SourceEntity.GlobalPosition)) {
				targetTile = currentTile;
			}

			SourceEntity.GlobalPosition = frameTargetPos;

			if (targetPos == SourceEntity.GlobalPosition) {
				currentTile = targetTile.Value;
				targetTile = null;
			} else {
				Vector2I newTile = ConvertGlobalToTile(SourceEntity.GlobalPosition);
				if (newTile != currentTile) {
					currentTile = newTile;
				}
			}
		}

		public void Move(Vector2I direction) {

			if (!diagonalMovement && direction.X != 0 && direction.Y != 0) {
				if (favouredAxis == Axis.HORIZONTAL) {
					direction.Y = 0;
				} else {
					direction.X = 0;
				}
			}

			targetTile = currentTile + direction;
		}

		protected Vector2 ConvertTileToGlobal(Vector2I tile, bool center) {
			Vector2 globalPosition = new Vector2(tile.X * this.tileSize.X, tile.Y * this.tileSize.Y);

			if (center) {
				globalPosition += new Vector2(tileSize.X * 0.5f, tileSize.Y * 0.5f);
			}

			return globalPosition;
		}

		protected Vector2I ConvertGlobalToTile(Vector2 position) {
			Vector2I tile = new Vector2I((int) (position.X / tileSize.X), (int) (position.Y / tileSize.Y));

			return tile;
		}

	}
}
