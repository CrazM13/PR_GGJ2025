using Godot;
using Godot.Collections;
using System;

public partial class MazeRoomBuilder : Node {

	[Export] private TileIndentifier wallTile;
	[Export] private RuleTileIdentifier wallRuleTile;
	[Export] private TileIndentifier floorTile;

	[Export] private int hallwayWidth = 2;
	[Export] private int wallWidth = 2;

	public int RoomSize {
		get => (wallWidth * 2) + hallwayWidth;
	}

	private Dictionary<CellConnections, TileMapPattern> cache = new Dictionary<CellConnections, TileMapPattern>();

	public TileMapPattern BuildCellWithConntections(CellConnections connections) {
		if (cache.ContainsKey(connections)) return cache[connections];

		TileMapPattern hallwayCell = new TileMapPattern();
		bool[,] walls = new bool[RoomSize, RoomSize];


		for (int x = 0; x < RoomSize; x++) {
			for (int y = 0; y < RoomSize; y++) {

				if (x < wallWidth && y < wallWidth) {
					walls[x, y] = true;
				} else if (x >= RoomSize - wallWidth && y >= RoomSize - wallWidth) {
					walls[x, y] = true;
				} else if (x >= RoomSize - wallWidth && y < wallWidth) {
					walls[x, y] = true;
				} else if (x < wallWidth && y >= RoomSize - wallWidth) {
					walls[x, y] = true;
				} else if (x < wallWidth) {
					if (!connections.HasFlag(CellConnections.LEFT)) {
						walls[x, y] = true;
					}
				} else if (y < wallWidth) {
					if (!connections.HasFlag(CellConnections.UP)) {
						walls[x, y] = true;
					}
				} else if (x >= RoomSize - wallWidth) {
					if (!connections.HasFlag(CellConnections.RIGHT)) {
						walls[x, y] = true;
					}
				} else if (y >= RoomSize - wallWidth) {
					if (!connections.HasFlag(CellConnections.DOWN)) {
						walls[x, y] = true;
					}
				}
			}
		}

		for (int x = 0; x < RoomSize; x++) {
			for (int y = 0; y < RoomSize; y++) {
				Vector2I position = new Vector2I(x, y);

				if (walls[x,y]) {

					if (GetWallAt(walls, position + Vector2I.Up)) {
						if (GetWallAt(walls, position + Vector2I.Down)) {
							if (GetWallAt(walls, position + Vector2I.Left)) {
								if (GetWallAt(walls, position + Vector2I.Right)) {
									SetCell(hallwayCell, position, wallRuleTile.Center);
								} else {
									SetCell(hallwayCell, position, wallRuleTile.Right);
								}
							} else if (GetWallAt(walls, position + Vector2I.Right)) {
								SetCell(hallwayCell, position, wallRuleTile.Left);
							} else {
								SetCell(hallwayCell, position, wallRuleTile.Center);
							}
						} else {
							if (GetWallAt(walls, position + Vector2I.Left)) {
								if (GetWallAt(walls, position + Vector2I.Right)) {
									SetCell(hallwayCell, position, wallRuleTile.BottomCenter);
								} else {
									SetCell(hallwayCell, position, wallRuleTile.BottomRight);
								}
							} else if (GetWallAt(walls, position + Vector2I.Right)) {
								SetCell(hallwayCell, position, wallRuleTile.BottomLeft);
							} else {
								SetCell(hallwayCell, position, wallRuleTile.BottomCenter);
							}
						}
					} else {
						if (GetWallAt(walls, position + Vector2I.Left)) {
							if (GetWallAt(walls, position + Vector2I.Right)) {
								SetCell(hallwayCell, position, wallRuleTile.TopCenter);
							} else {
								SetCell(hallwayCell, position, wallRuleTile.TopRight);
							}
						} else if (GetWallAt(walls, position + Vector2I.Right)) {
							SetCell(hallwayCell, position, wallRuleTile.TopLeft);
						} else {
							SetCell(hallwayCell, position, wallRuleTile.TopCenter);
						}
					}

				} else {
					SetCell(hallwayCell, position, floorTile);
				}

				
			}
		}

		cache.Add(connections, hallwayCell);
		return hallwayCell;
	}

	private void SetCell(TileMapPattern cell, Vector2I position, TileIndentifier indentifier) {
		cell.SetCell(position, indentifier.Source, indentifier.AtlasCoordinates, indentifier.Alternative);
	}

	private static bool GetWallAt(bool[,] data, Vector2I at) {
		if (at.X < 0 || at.Y < 0) { return true; }
		if (at.X >= data.GetLength(0) || at.Y >= data.GetLength(0)) { return true; }

		return data[at.X, at.Y];
	}

}
