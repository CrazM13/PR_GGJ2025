using Godot;
using System;
using System.Collections.Generic;

namespace AudioManagement {
	public class NodePool<T> where T : Node {

		private Queue<T> unactive;
		private List<T> active;

		public NodePool() {
			unactive = new Queue<T>();
			active = new List<T>();
		}

		public T CreateNode() {

			T newNode = unactive.Dequeue();

			active.Add(newNode);

			return newNode;
		}

		public void DeleteNode(T node) {
			node.GetParent()?.RemoveChild(node);
			if (active.Contains(node)) active.Remove(node);
			unactive.Enqueue(node);
		}

		public List<T> GetActiveNodes() {
			return new List<T>(active);
		}

		public int RemainingNodesCount => unactive.Count;

	}
}
