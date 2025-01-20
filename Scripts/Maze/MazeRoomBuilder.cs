using Godot;
using Godot.Collections;
using System;

public partial class MazeRoomBuilder : Node {

	[Export] private TileIndentifier wallTile;
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

		for (int x = 0; x < RoomSize; x++) {
			for (int y = 0; y < RoomSize; y++) {

				bool isWall = false;

				if ((x < wallWidth && y < wallWidth) ||
					(x >= RoomSize - wallWidth && y >= RoomSize - wallWidth) ||
					(x >= RoomSize - wallWidth && y < wallWidth) ||
					(x < wallWidth && y >= RoomSize - wallWidth)) {
					// Corners
					isWall = true;
				} else if (x < wallWidth) {
					if (!connections.HasFlag(CellConnections.LEFT)) {
						isWall = true;
					}
				} else if (y < wallWidth) {
					if (!connections.HasFlag(CellConnections.UP)) {
						isWall = true;
					}
				} else if (x >= RoomSize - wallWidth) {
					if (!connections.HasFlag(CellConnections.RIGHT)) {
						isWall = true;
					}
				} else if (y >= RoomSize - wallWidth) {
					if (!connections.HasFlag(CellConnections.DOWN)) {
						isWall = true;
					}
				}

				if (isWall) {
					hallwayCell.SetCell(new Vector2I(x, y), wallTile.Source, wallTile.AtlasCoordinates, wallTile.Alternative);
				} else {
					hallwayCell.SetCell(new Vector2I(x, y), floorTile.Source, floorTile.AtlasCoordinates, floorTile.Alternative);
				}
			}
		}

		cache.Add(connections, hallwayCell);
		return hallwayCell;
	}

}
