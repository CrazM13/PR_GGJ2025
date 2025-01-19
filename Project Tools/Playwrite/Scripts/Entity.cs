using Godot;
using Godot.Collections;
using System;

namespace Playwrite {
	public abstract partial class Entity : CharacterBody2D {
		private Dictionary<string, EntityComponent> components;
		
		public Entity() {
			components = new Dictionary<string, EntityComponent>();
		}

		public void AddComponent(EntityComponent component) {
			if (components.ContainsKey(component.Name)) {
				components[component.Name] = component;
			} else {
				components.Add(component.Name, component);
			}
		}

		public void RemoveComponent(EntityComponent component) {

			string key = component.GetType().Name;

			if (components.ContainsKey(key)) {
				components.Remove(key);
			}
		}

		public EntityComponent GetComponent(string name) {
			if (components.ContainsKey(name)) {
				return components[name];
			}

			return null;
		}

		public EntityComponent[] GetComponents() {
			EntityComponent[] entityComponents = new EntityComponent[components.Count];
			components.Values.CopyTo(entityComponents, 0);
			return entityComponents;
		}

		public T GetComponent<T>() where T : EntityComponent {
			string key = typeof(T).Name;

			if (components.ContainsKey(key)) {
				return (T) components[key];
			}

			return null;
		}

	}
}
