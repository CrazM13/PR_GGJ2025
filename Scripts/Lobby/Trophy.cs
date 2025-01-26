using Godot;
using System;

public partial class Trophy : Node2D {

	[Export] private int abilityIndex;

	public override void _Ready() {
		base._Ready();

		this.Visible = GameManager.Instance.ActivatedAbilities[abilityIndex];

	}

}
