using Godot;
using System;
using System.Collections.Generic;

namespace AudioManagement {
	[GlobalClass, Icon("res://Project Tools/AudioManager/Resources/AudioManager.svg")]
	public partial class AudioManager : Node {

		public static AudioManager Instance { get; private set; }

		[Export] private int maxGlobalAudioPlayers = 10;
		[Export] private int max2DAudioPlayers = 10;
		[Export] private int max3DAudioPlayers = 10;

		NodePool<AudioStreamPlayer> globalAudioPlayers = new();
		NodePool<AudioStreamPlayer2D> local2DAudioPlayers = new();
		NodePool<AudioStreamPlayer3D> local3DAudioPlayers = new();

		public override void _EnterTree() {
			base._EnterTree();

			Instance = this;

			for (int _ = 0; _ < maxGlobalAudioPlayers; _++) {
				globalAudioPlayers.DeleteNode(new AudioStreamPlayer());
			}

			for (int _ = 0; _ < max2DAudioPlayers; _++) {
				local2DAudioPlayers.DeleteNode(new AudioStreamPlayer2D());
			}

			for (int _ = 0; _ < max3DAudioPlayers; _++) {
				local3DAudioPlayers.DeleteNode(new AudioStreamPlayer3D());
			}
		}

		public override void _ExitTree() {
			base._ExitTree();

			if (Instance == this) Instance = null;
		}

		public AudioStreamPlayer PlayGlobal(AudioStream audioStream, string audioBus) {

			AudioStreamPlayer globalAudioNode = CreateNode(globalAudioPlayers);

			if (globalAudioNode != null) {
				globalAudioNode.Stream = audioStream;
				globalAudioNode.Bus = audioBus;

				globalAudioNode.Play();
			}

			return globalAudioNode;
		}

		public AudioStreamPlayer3D PlayLocal(Vector3 position3D, AudioStream audioStream, string audioBus) {

			AudioStreamPlayer3D local3DAudioNode = CreateNode(local3DAudioPlayers);

			if (local3DAudioNode != null) {
				local3DAudioNode.Stream = audioStream;
				local3DAudioNode.Bus = audioBus;

				local3DAudioNode.GlobalPosition = position3D;

				local3DAudioNode.Play();
			}

			return local3DAudioNode;
		}

		public AudioStreamPlayer2D PlayLocal(Vector2 position2D, AudioStream audioStream, string audioBus) {

			AudioStreamPlayer2D local2DAudioNode = CreateNode(local2DAudioPlayers);

			if (local2DAudioNode != null) {
				local2DAudioNode.Stream = audioStream;
				local2DAudioNode.Bus = audioBus;

				local2DAudioNode.GlobalPosition = position2D;

				local2DAudioNode.Play();
			}

			return local2DAudioNode;
		}

		private T CreateNode<T>(NodePool<T> pool) where T : Node {
			if (pool.RemainingNodesCount <= 0) return null;

			T newNode = pool.CreateNode();

			AddChild(newNode);

			return newNode;
		}

		public override void _Process(double delta) {
			base._Process(delta);

			// Check for finished sounds
			List<AudioStreamPlayer> activeGlobalNodes = globalAudioPlayers.GetActiveNodes();
			foreach (AudioStreamPlayer node in activeGlobalNodes) {
				if (!node.Playing) {
					globalAudioPlayers.DeleteNode(node);
				}
			}

			List<AudioStreamPlayer2D> active2DNodes = local2DAudioPlayers.GetActiveNodes();
			foreach (AudioStreamPlayer2D node in active2DNodes) {
				if (!node.Playing) {
					local2DAudioPlayers.DeleteNode(node);
				}
			}

			List<AudioStreamPlayer3D> active3DNodes = local3DAudioPlayers.GetActiveNodes();
			foreach (AudioStreamPlayer3D node in active3DNodes) {
				if (!node.Playing) {
					local3DAudioPlayers.DeleteNode(node);
				}
			}

		}

	}
}
