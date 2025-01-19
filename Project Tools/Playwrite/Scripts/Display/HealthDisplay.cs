using Godot;
using System;

namespace Playwrite {
	[GlobalClass]
	public partial class HealthDisplay : TextureProgressBar {

		[Export] HealthComponent healthComponent;

		public override void _Ready() {
			this.MinValue = 0;
			this.MaxValue = healthComponent.MaxHealth;
			this.Value = healthComponent.Health;

			healthComponent.OnHealthChange += this.OnHealthChange;
		}

		private void OnHealthChange(DamageEvent @event) {

			this.Value = healthComponent.Health;

		}
	}
}
