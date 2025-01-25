using Godot;
using System;

public partial class AbilityIconUI : TextureRect {

	[Export] private int abilityIndex;
	[Export] private TextureProgressBar cooldownUI;

	public override void _Input(InputEvent @event) {
		base._Input(@event);

		if (@event is InputEventMouseButton mouseEvent) {

			if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed) {
				if (this.GetRect().HasPoint(GetLocalMousePosition())) {
					GameManager.Instance.SelectedAbility = abilityIndex;
				}
			}
		}
	}

	public override void _Process(double delta) {
		base._Process(delta);

		cooldownUI.Value = GameManager.Instance.Player.GetCooldownPercentage(abilityIndex);

	}

}
