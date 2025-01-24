using Godot;
using System;

public partial class StartLevel : Area2D {

	[Export] private int levelNumber;
	[Export] private string levelPath;

	[Export] private AudioStream sfx;
	[Export] private Textbox textBox;
	[Export] private string[] introText;

	public override void _Ready() {
		base._Ready();

		this.BodyEntered += this.OnBodyEntered;

	}

	private void OnBodyEntered(Node2D body) {
		if (body is Player) {

			textBox.OnTexboxComplete += () => {
				GameManager.Instance.CurrentLevel = levelNumber;
				SceneManagement.SceneManager.Instance.LoadScene(levelPath);
			};

			if (introText != null) {
				foreach (string line in introText) {
					textBox.QueueText(line, sfx);
				}
			}

			textBox.NextMessage();

		}
	}
}
