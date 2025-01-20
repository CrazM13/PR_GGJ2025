using Godot;
using System;
using System.Collections.Generic;

public class MazeNode {

	public Vector2I Position { get; set; }

	public List<MazeNode> Children { get; private set; }
	public MazeNode Parent { get; set; }

	public MazeNode(Vector2I position) {
		this.Position = position;
		this.Children = new List<MazeNode>();
	}

}
