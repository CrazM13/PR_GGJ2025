using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class Oxygen : TextureProgressBar {

	private readonly Color NORMAL_TINT = new Color("00ffff");
	private readonly Color HURT_TINT = new Color("D1FFFF");
	private readonly Color HEAL_TINT = new Color("D1FFFF");

	[Export] private TextureProgressBar damageIndicator;
	[Export] private float minLength = 32;
	[Export] private float shakeDecay = 2;

	private float animTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {

		animTimer += (float) delta * 20;

		this.GetParent<Control>().Size = new Vector2(this.Size.X, minLength * GameManager.Instance.MaxAir);

		float newAir = GameManager.Instance.GetAirPercentage();
		bool isFilling = false;
		bool skipFilling = false;
		if (this.Value < newAir) {
			if (damageIndicator.Value < newAir) {
				damageIndicator.Value = newAir;
				if (Mathf.Abs(this.Value - damageIndicator.Value) < 0.2f) skipFilling = true;
				isFilling = true;
			} else if (damageIndicator.Value > newAir) {
				damageIndicator.Value = newAir;
				isFilling = true;
			}
		} else if (this.Value > newAir) {
			this.Value = newAir;
			if (Mathf.Abs(this.Value - damageIndicator.Value) < 0.2f) skipFilling = true;
			isFilling = true;
		}
		
		if (!skipFilling) {
			if (this.Value != newAir) {
				this.Value = Mathf.MoveToward(this.Value, newAir, (float) delta * 0.1f);
			}
			if (damageIndicator.Value != newAir) {
				damageIndicator.Value = Mathf.MoveToward(damageIndicator.Value, newAir, (float) delta * 0.1f);
			}
		}

		if (isFilling) {
			this.TintProgress = HURT_TINT;
			this.GetParent<Control>().Position = new Vector2(Mathf.Sin(animTimer * 13), Mathf.Cos(animTimer * 17));
		} else {
			this.GetParent<Control>().Position = Vector2.Zero;
			this.TintProgress = this.TintProgress.Lerp(NORMAL_TINT, (float) delta * 4);
		}
	}
}
