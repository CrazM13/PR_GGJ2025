using Godot;
using System;

[Flags]
public enum CellConnections {

	UP = 1 << 0,
	DOWN = 1 << 1,
	LEFT = 1 << 2,
	RIGHT = 1 << 3

}
