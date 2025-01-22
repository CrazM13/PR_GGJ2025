using Godot;
using System;

[GlobalClass]
public partial class MazeRuleCell : Resource {

	[Export] public CellConnections Connections { get; set; }
	[Export] public PackedScene[] SceneSet { get; set; }

}
