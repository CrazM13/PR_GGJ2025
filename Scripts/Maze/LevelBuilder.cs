using Godot;
using System;
using System.Collections.Generic;

public partial class LevelBuilder : Node {

	[Export] private MazeRuleSet ruleSet;
	[Export] private MazeRuleSet bossRuleSet;

	[ExportGroup("Generation Settings")]
	[Export] private int mazeSize = 64;
	[Export] private int cellsPerFrame = 1;
	[Export] private int cellSize = 16;

	public Vector2I MapSize => new Vector2I((mazeSize + 2) * cellSize, (mazeSize + 2) * cellSize);

	public Color[,] Minimap { get; set; }

	PerfectMazeGenerator perfectMaze;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	private Queue<MazeNode> toLoadCell = new Queue<MazeNode>();
	private int loadedCells;

	public override void _Ready() {
		base._Ready();
		GameManager.Instance.Level = this;
		GameManager.Instance.LoadingPercentage = 0;

		Minimap = new Color[MapSize.X, MapSize.Y];

		perfectMaze = new PerfectMazeGenerator(mazeSize, mazeSize);

		for (int x = 0; x < MapSize.X; x++) {
			for (int y = 0; y < MapSize.Y; y++) {
				Minimap[x, y] = Colors.Black;
			}
		}

		BuildNode(perfectMaze.Root);
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (toLoadCell.Count > 0) {
			for (int _ = 0; _ < cellsPerFrame; _++) {
				if (toLoadCell.Count > 0) {
					MazeNode node = toLoadCell.Dequeue();

					BuildNode(node);
				}
			}
		}

	}

	public override void _ExitTree() {
		base._ExitTree();

		GameManager.Instance.Level = null;
	}

	public void BuildNode(MazeNode node) {

		Vector2I startingPosition = node.Position * cellSize;

		CellConnections connections = ConvertNodeToConnections(node);

		PackedScene cell;

		if (node.NonSpecial) {
			cell = ruleSet.GetCellPrefab(connections, rng);
		} else {
			cell = bossRuleSet.GetCellPrefab(connections, rng);
		}

		if (cell == null) {
			GD.Print(connections);
		} else {
			Node2D cellRoot = cell.Instantiate<Node2D>();
			cellRoot.GlobalPosition = startingPosition * 32;
			AddChild(cellRoot);

			// Minimap
			startingPosition = (node.Position + Vector2I.One) * cellSize;

			TileMapLayer map = cellRoot.GetChild<TileMapLayer>(0);
			for (int x = 0; x < cellSize; x++) {
				for (int y = 0; y < cellSize; y++) {
					Minimap[startingPosition.X + x, startingPosition.Y + y] = map.GetCellTileData(new Vector2I(x, y))?.GetCustomDataByLayerId(0).As<Color>() ?? Colors.Black;
				}
			}
		}

		loadedCells++;
		GameManager.Instance.LoadingPercentage = (float)loadedCells / (mazeSize * mazeSize);

		foreach (MazeNode child in node.Children) {
			toLoadCell.Enqueue(child);
		}
	}

	public Vector2I GetStartingLocation() {
		return (perfectMaze.Entrence * cellSize) + new Vector2I(cellSize / 2, cellSize / 2);
	}

	public Vector2I GetExitLocation() {
		return (perfectMaze.Exit * cellSize) + new Vector2I(cellSize / 2, cellSize / 2);
	}

	private static CellConnections ConvertNodeToConnections(MazeNode node) {
		CellConnections connections = 0;

		if (node.Parent != null) {

			if (node.Parent.Position == node.Position + Vector2I.Up) {
				connections |= CellConnections.UP;
			} else if (node.Parent.Position == node.Position + Vector2I.Down) {
				connections |= CellConnections.DOWN;
			} else if (node.Parent.Position == node.Position + Vector2I.Left) {
				connections |= CellConnections.LEFT;
			} else if (node.Parent.Position == node.Position + Vector2I.Right) {
				connections |= CellConnections.RIGHT;
			}

		}
		
		foreach (MazeNode child in node.Children) {
			if (child.Position == node.Position + Vector2I.Up) {
				connections |= CellConnections.UP;
			} else if (child.Position == node.Position + Vector2I.Down) {
				connections |= CellConnections.DOWN;
			} else if (child.Position == node.Position + Vector2I.Left) {
				connections |= CellConnections.LEFT;
			} else if (child.Position == node.Position + Vector2I.Right) {
				connections |= CellConnections.RIGHT;
			}
		}

		return connections;
	}

}
