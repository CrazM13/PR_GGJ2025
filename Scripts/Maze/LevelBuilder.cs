using Godot;
using System;
using System.Collections.Generic;

public partial class LevelBuilder : Node {

	[Export] private TileMapLayer[] layers;
	[Export] private MazeRoomBuilder cellBuilder;

	[ExportGroup("Generation Settings")]
	[Export] private int mazeSize = 64;
	[Export] private int roomMultiplier = 50;
	[Export] private int cellsPerFrame = 1;

	PerfectMazeGenerator perfectMaze;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	private Queue<MazeNode> toLoadCell = new Queue<MazeNode>();
	private Queue<(TileMapPattern, Vector2I)> toLoadRoom = new Queue<(TileMapPattern, Vector2I)>();
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
					PlacePattern(roomMultiplier);
					CreateExit();
				}
			}
		}

		if (toLoadRoom.Count > 0) {
			for (int _ = 0; _ < cellsPerFrame; _++) {
				if (toLoadRoom.Count > 0) {
					(TileMapPattern room, Vector2I position) node = toLoadRoom.Dequeue();

					layers[0].SetPattern(node.position, node.room);
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

		Vector2I startingPosition = node.Position * cellBuilder.RoomSize;

		CellConnections connections = ConvertNodeToConnections(node);
		TileMapPattern cell = cellBuilder.BuildCellWithConntections(connections);
		layers[0].SetPattern(startingPosition, cell);

		loadedCells++;
		GameManager.Instance.LoadingPercentage = (float)loadedCells / (mazeSize * mazeSize);

		foreach (MazeNode child in node.Children) {
			toLoadCell.Enqueue(child);
		}
	}

	private void PlacePattern(int count) {
		int roomCount = layers[0].TileSet.GetPatternsCount();

		for (int i = 0; i < roomCount * count; i++) {
			TileMapPattern room = layers[0].TileSet.GetPattern(i % roomCount);

			Vector2I size = room.GetSize();
			Vector2I position = new Vector2I(rng.RandiRange(0, mazeSize), rng.RandiRange(0, mazeSize)) * cellBuilder.RoomSize;

			if (layers[0].GetUsedRect().HasPoint(position + size + Vector2I.One) && layers[0].GetUsedRect().HasPoint(position - Vector2I.One)) {
				toLoadRoom.Enqueue((room, position));
			}
		}
	}

	public Vector2I GetStartingLocation() {
		return (perfectMaze.Entrence * cellBuilder.RoomSize) + new Vector2I(cellBuilder.RoomSize / 2, cellBuilder.RoomSize / 2);
	}

	public Vector2I GetExitLocation() {
		return (perfectMaze.Exit * cellBuilder.RoomSize) + new Vector2I(cellBuilder.RoomSize / 2, cellBuilder.RoomSize / 2);
	}

	private void CreateExit() {
		Vector2 realExitPos = GetExitLocation() * layers[0].TileSet.TileSize;
		Area2D exitTrigger = new Area2D() {
			GlobalPosition = realExitPos
		};
		exitTrigger.AddChild(new CollisionShape2D() {
			Shape = new RectangleShape2D() {
				Size = new Vector2(cellBuilder.RoomSize * 32, cellBuilder.RoomSize * 32)
			}
		});
		exitTrigger.BodyEntered += (body) => {
			if (body is Player) {
				// TODO WIN
				SceneManagement.SceneManager.Instance.LoadScene(GetTree().CurrentScene.SceneFilePath, "res://Scenes/loading_scene.tscn");
			}
		};
		AddChild(exitTrigger);
	}

	private static CellConnections ConvertNodeToConnections(MazeNode node) {
		CellConnections connections = CellConnections.NONE;

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
