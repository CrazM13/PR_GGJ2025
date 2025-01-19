using Godot;
using System;

namespace Playwrite {
	public partial class DamageEvent : EntityEvent {

		public int Damage { get; set; }
		public bool IsFriendly { get; set; }

		public DamageEvent(Entity source, int amount, bool friendly) : base(source) {
			Damage = amount;
			IsFriendly = friendly;
		}
	}
}
