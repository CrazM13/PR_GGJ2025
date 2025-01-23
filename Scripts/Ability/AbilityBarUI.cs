using Godot;
using System;

public partial class AbilityBarUI : Control {

	[Export] private Control[] abilityIcons;

	int previousActive = -1;

	public override void _Process(double delta) {
		base._Process(delta);

		int newActive = GameManager.Instance.SelectedAbility;
		if (newActive != previousActive) {
			previousActive = newActive;

			SetActive(newActive);
		}

	}

	public void SetActive(int index) {
		if (index < 0 || index >= abilityIcons.Length) return;

		for (int i = 0; i < abilityIcons.Length; i++) {
			if (i == index) {
				abilityIcons[i].Position = new Vector2(-4, -4);
				abilityIcons[i].Scale = new Vector2(1.25f, 1.25f);
				abilityIcons[i].ZIndex = 1;
			} else {
				abilityIcons[i].Position = Vector2.Zero;
				abilityIcons[i].Scale = Vector2.One;
				abilityIcons[i].ZIndex = 0;
			}
		}
	}

}
