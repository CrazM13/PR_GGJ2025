using Godot;
using System;

public partial class AbilityIconUI : TextureRect {

	[Export] private int abilityIndex;

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

}
