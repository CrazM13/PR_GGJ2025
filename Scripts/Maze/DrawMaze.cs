using Godot;
using System;

public partial class DrawMaze : Node2D {

	PerfectMazeGenerator generator;

	private Color[] colors = new Color[] {
		Colors.Green,
		Colors.Red,
		Colors.Blue,
		Colors.Yellow,
		Colors.GreenYellow,
		Colors.Pink,
		Colors.Purple,
		Colors.Orange,
		Colors.Orchid,
		Colors.Chocolate,
		Colors.ForestGreen,
		Colors.Fuchsia,
		Colors.SkyBlue,
		Colors.Magenta
	};

	public override void _Draw() {
		base._Draw();

		generator ??= new PerfectMazeGenerator(32, 32);

		MazeNode targetNode = generator.Root;
		DrawNode(targetNode, 0);
	}

	private void DrawNode(MazeNode node, int colour) {
		DrawCircle(GetPosition(node.Position), 4, colors[colour]);

		foreach (MazeNode child in node.Children) {
			DrawLine(GetPosition(node.Position), GetPosition(child.Position), colors[colour]);
			DrawNode(child, colour);

			colour = (colour + 1) % colors.Length;
		}
	}

	private Vector2 GetPosition(Vector2I position) {
		return new Vector2((position.X + 0.5f) * 32 * 6, (position.Y + 0.5f) * 32 * 6);
	}

}
