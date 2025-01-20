using Godot;
using System;
using System.Collections.Generic;

public class PerfectMazeGenerator {

	public int WIDTH { get; set; }
	public int HEIGHT { get; set; }

	public MazeNode Root { get; set; }

	private static readonly Vector2I[] NEIGHBORS = new Vector2I[] {
		Vector2I.Up, Vector2I.Down, Vector2I.Right, Vector2I.Left
	};

	private List<Vector2I> used;
	private RandomNumberGenerator rng;

	private int exitCount = 0;
	public Vector2I Entrence { get; set; }
	public Vector2I Exit { get; set; }

	public PerfectMazeGenerator(int width, int height) {
		this.WIDTH = width;
		this.HEIGHT = height;

		rng = new RandomNumberGenerator();

		Root = new MazeNode(Vector2I.Zero);
		used = new List<Vector2I>() {
			Root.Position
		};

		exitCount = 0;

		RecursivePathMaker(Root);
	}

	public void RecursivePathMaker(MazeNode node) {
		int offset = rng.RandiRange(0, 4);
		for (int i = 0; i < NEIGHBORS.Length; i++) {
			Vector2I newPos = node.Position + NEIGHBORS[(i + offset) % NEIGHBORS.Length];
			if (!used.Contains(newPos)) {
				if (IsInMaze(newPos)) {
					MazeNode newNode = new MazeNode(newPos);
					node.Children.Add(newNode);
					newNode.Parent = node;
					used.Add(newPos);
					RecursivePathMaker(newNode);
				} else if (exitCount < 2) {
					if (exitCount == 0) Entrence = newPos;
					else Exit = newPos;
					MazeNode newNode = new MazeNode(newPos) {
						Buildable = exitCount == 0,
					};
					node.Children.Add(newNode);
					newNode.Parent = node;

					exitCount++;
				}
			}
		}
	}

	public bool IsInMaze(Vector2I position) {
		return position.X >= 0 && position.X < WIDTH && position.Y >= 0 && position.Y < HEIGHT;
	}

	public bool IsInMaze(int x, int y) {
		return x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT;
	}

}
