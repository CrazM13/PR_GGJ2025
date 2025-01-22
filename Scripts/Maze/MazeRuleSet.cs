using Godot;
using System;

[GlobalClass]
public partial class MazeRuleSet : Resource {

	[Export] public MazeRuleCell[] rules;

	public PackedScene GetCellPrefab(CellConnections connection, RandomNumberGenerator rng) {
		foreach (MazeRuleCell rule in rules) {
			if (rule.Connections == connection) {
				int options = rule.SceneSet.Length;
				if (options == 0) return null;
				else if (options == 1) return rule.SceneSet[0];
				else {
					return rule.SceneSet[rng.RandiRange(0, options - 1)];
				}
			}
		}

		return null;
	}

}
