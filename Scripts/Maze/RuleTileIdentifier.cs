using Godot;
using System;

[GlobalClass]
public partial class RuleTileIdentifier : Resource {

	[Export] public TileIndentifier TopLeft { get; set; }
	[Export] public TileIndentifier TopCenter { get; set; }
	[Export] public TileIndentifier TopRight { get; set; }
	[Export] public TileIndentifier Left { get; set; }
	[Export] public TileIndentifier Center { get; set; }
	[Export] public TileIndentifier Right { get; set; }
	[Export] public TileIndentifier BottomLeft { get; set; }
	[Export] public TileIndentifier BottomCenter { get; set; }
	[Export] public TileIndentifier BottomRight { get; set; }

}
