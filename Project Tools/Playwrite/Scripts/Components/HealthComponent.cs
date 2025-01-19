using Godot;
using System;

namespace Playwrite {
	[GlobalClass]
	public partial class HealthComponent : EntityComponent {

		[Signal] public delegate void OnHealthChangeEventHandler(DamageEvent @event);

		[Export] public int MaxHealth { get; set; }
		public int Health { get; private set; }

		public override void _EnterTree() {
			base._EnterTree();
			ResetHealth(SourceEntity);
		}

		public void Damage(DamageEvent @event) {
			Health -= @event.Damage;
			if (Health > MaxHealth) {
				Health = MaxHealth;
			}
			EmitHealthChangeEvent(@event);
		}

		public void SetHealth(int health, Entity source = null) {
			Health = health;
			if (Health > MaxHealth) {
				Health = MaxHealth;
			}
			EmitHealthChangeEvent(new DamageEvent(source, 0, false));
		}

		public void ResetHealth(Entity source = null) {
			Health = MaxHealth;
			EmitHealthChangeEvent(new DamageEvent(source, 0, false));
		}

		private void EmitHealthChangeEvent(DamageEvent @event) {
			EmitSignal(SignalName.OnHealthChange, @event);
		}

	}
}