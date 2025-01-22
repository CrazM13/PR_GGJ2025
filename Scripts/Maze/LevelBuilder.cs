using Godot;
using System;
using System.Collections.Generic;

public partial class LevelBuilder : Node {

	[Export] private TileMapLayer[] layers;
	[Export] private MazeRuleSet ruleSet;

	[ExportGroup("Generation Settings")]
	[Export] private int mazeSize = 64;
	[Export] private int cellsPerFrame = 1;
	[Export] private int cellSize = 16;



	PerfectMazeGenerator perfectMaze;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	private Queue<MazeNode> toLoadCell = new Queue<MazeNode>();
	private int loadedCells;

	public override void _Ready() {
		base._Ready();
		GameManager.Instance.Level = this;
		GameManager.Instance.LoadingPercentage = 0;

		perfectMaze = new PerfectMazeGenerator(mazeSize, mazeSize);

		BuildNode(perfectMaze.Root);
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (toLoadCell.Count > 0) {
			for (int _ = 0; _ < cellsPerFrame; _++) {
				if (toLoadCell.Count > 0) {
					MazeNode node = toLoadCell.Dequeue();

					BuildNode(node);
				} else {
					CreateExit();
				}
			}
		}

	}

	public override void _ExitTree() {
		base._ExitTree();

		if (GameManager.Instance.Level == this) GameManager.Instance.Level = null;
	}

	public void BuildNode(MazeNode node) {
		if (!node.Buildable) return;

		Vector2I startingPosition = node.Position * cellSize;

		CellConnections connections = ConvertNodeToConnections(node);

		PackedScene cell = ruleSet.GetCellPrefab(connections, rng);

		if (cell == null) {
			GD.Print(connections);
		} else {
			Node2D cellRoot = cell.Instantiate<Node2D>();
			cellRoot.GlobalPosition = startingPosition * 32;
			AddChild(cellRoot);
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

	private void CreateExit() {
		Vector2 realExitPos = GetExitLocation() * layers[0].TileSet.TileSize;
		Area2D exitTrigger = new Area2D() {
			GlobalPosition = realExitPos,
			Name = "EXIT AREA"
		};
		exitTrigger.AddChild(new CollisionShape2D() {
			Shape = new RectangleShape2D() {
				Size = new Vector2(cellSize * 32, cellSize * 32)
			}
		});
		exitTrigger.BodyEntered += (body) => {
			if (body is Player) {
				// TODO WIN
				SceneManagement.SceneManager.Instance.LoadScene(GetTree().CurrentScene.SceneFilePath);
			}
		};
		AddChild(exitTrigger);
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
