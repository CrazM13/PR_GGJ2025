using Godot;
using System;
using System.Collections.Generic;

public partial class LevelBuilder : Node {

	[Export] private TileMapLayer[] layers;

	[ExportGroup("Generation Settings")]
	[Export] private int mazeSize = 64;
	[Export] private int hallwayWidth = 2;
	[Export] private int wallWidth = 2;

	[Export] private TileIndentifier floorTile;
	[Export] private TileIndentifier wallTile;

	private List<Vector2I> wallTiles;
	private List<Vector2I> floorTiles;
	PerfectMazeGenerator perfectMaze;
	private RandomNumberGenerator rng = new RandomNumberGenerator();
	public override void _Ready() {
		base._Ready();

		perfectMaze = new PerfectMazeGenerator(mazeSize, mazeSize);
		wallTiles = new List<Vector2I>();
		floorTiles = new List<Vector2I>();

		BuildNode(perfectMaze.Root);

		foreach (Vector2I cell in wallTiles) {
			layers[0].SetCell(cell, wallTile.Source, wallTile.AtlasCoordinates, wallTile.Alternative);
		}

		foreach (Vector2I cell in floorTiles) {
			layers[0].SetCell(cell, floorTile.Source, floorTile.AtlasCoordinates, floorTile.Alternative);
		}

		PlacePattern(100);
	}

	public void BuildNode(MazeNode node) {
		int roomWidth = hallwayWidth + (wallWidth * 2);
		Vector2I startingPosition = node.Position * roomWidth;

		for (int x = 0; x < roomWidth; x++) {
			for (int y = 0; y < roomWidth; y++) {
				
				if ((x < wallWidth && y < wallWidth) || 
					(x >= roomWidth - wallWidth && y >= roomWidth - wallWidth) ||
					(x >= roomWidth - wallWidth && y < wallWidth) ||
					(x < wallWidth && y >= roomWidth - wallWidth)) {
					// Corners
					wallTiles.Add(startingPosition + new Vector2I(x, y));
				} else if (x < wallWidth) {
					AddWallOrFloor(node, startingPosition + new Vector2I(x, y), Vector2I.Left);
				} else if (y < wallWidth) {
					AddWallOrFloor(node, startingPosition + new Vector2I(x, y), Vector2I.Up);
				} else if (x >= roomWidth - wallWidth) {
					AddWallOrFloor(node, startingPosition + new Vector2I(x, y), Vector2I.Right);
				} else if (y >= roomWidth - wallWidth) {
					AddWallOrFloor(node, startingPosition + new Vector2I(x, y), Vector2I.Down);
				} else {
					floorTiles.Add(startingPosition + new Vector2I(y, x));
				}


			}
		}

		foreach (MazeNode child in node.Children) {
			BuildNode(child);
		}
	}

	private void AddWallOrFloor(MazeNode node, Vector2I position, Vector2I direction) {
		if (node.Parent != null && node.Position + direction == node.Parent.Position) {
			floorTiles.Add(position);
		}

		int index = node.Children.FindIndex((c) => c.Position == node.Position + direction);
		if (index == -1) {
			wallTiles.Add(position);
		} else {
			floorTiles.Add(position);
		}
	}

	private void PlacePattern(int count) {
		int roomCount = layers[0].TileSet.GetPatternsCount();
		int roomWidth = hallwayWidth + (wallWidth * 2);

		for (int i = 0; i < roomCount * count; i++) {
			TileMapPattern room = layers[0].TileSet.GetPattern(i % roomCount);

			Vector2I size = room.GetSize();
			Vector2I position = new Vector2I(rng.RandiRange(0, mazeSize), rng.RandiRange(0, mazeSize)) * roomWidth;

			if (layers[0].GetUsedRect().HasPoint(position + size + Vector2I.One) && layers[0].GetUsedRect().HasPoint(position - Vector2I.One)) {
				layers[0].SetPattern(position, room);
			}
		}
	}

}
