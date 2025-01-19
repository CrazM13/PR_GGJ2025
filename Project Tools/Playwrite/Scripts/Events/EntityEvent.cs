using Godot;
using System;

namespace Playwrite {
	public partial class EntityEvent : GodotObject {

		public Entity Source { get; set; }

		public EntityEvent(Entity source) {
			Source = source;
		}

	}
}