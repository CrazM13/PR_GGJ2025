using Godot;
using System;

namespace Playwrite {
	[GlobalClass]
	public abstract partial class EntityComponent : Node {

		[Export] public Entity SourceEntity { get; set; }

		public override void _EnterTree() {
			base._EnterTree();

			SourceEntity.AddComponent(this);

		}

	}

}
