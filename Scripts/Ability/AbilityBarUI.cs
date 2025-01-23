using Godot;
using System;

public partial class AbilityBarUI : Control {

	[Export] private Control[] abilityIcons;
	[Export] private Control[] lockIcons;

	private float[] lockShakeTimer;
	private float time;

	int previousActive = -1;

	public override void _Ready() {
		base._Ready();

		lockShakeTimer = new float[lockIcons.Length];
		for (int i = 0; i < lockIcons.Length; i++) {
			lockIcons[i].PivotOffset = lockIcons[i].Size * 0.5f;
		}

	}

	public override void _Process(double delta) {
		base._Process(delta);

		int newActive = GameManager.Instance.SelectedAbility;
		if (newActive != previousActive) {
			previousActive = newActive;

			SetActive(newActive);
		}

		UpdateLocks((float) delta);

	}

	private void UpdateLocks(float delta) {
		time += delta * 25f;

		if (Input.IsActionPressed("use_action") && !GameManager.Instance.ActivatedAbilities[previousActive]) {
			ShakeLock(previousActive);
		}

		for (int i = 0; i < lockIcons.Length; i++) {
			lockIcons[i].Visible = !GameManager.Instance.ActivatedAbilities[i];
			if (lockShakeTimer[i] > 0) {
				lockShakeTimer[i] -= delta;

				lockIcons[i].Rotation = Mathf.Sin(time + (i * delta)) * lockShakeTimer[i];
			}
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

	public void ShakeLock(int index) {
		lockShakeTimer[index] = 1f;
	}

}
