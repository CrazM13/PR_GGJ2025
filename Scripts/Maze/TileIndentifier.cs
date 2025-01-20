using Godot;
using System;

[GlobalClass]
public partial class TileIndentifier : Resource {

	[Export] public int Source { get; set; } = 0;
	[Export] public Vector2I AtlasCoordinates { get; set; } = Vector2I.Zero;
	[Export] public int Alternative { get; set; } = 0;

}
