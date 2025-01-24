using Godot;
using System;

public partial class StartLevel : Area2D {

	[Export] private int levelNumber;
	[Export] private string levelPath;

	public override void _Ready() {
		base._Ready();

		this.BodyEntered += this.OnBodyEntered;

	}

	private void OnBodyEntered(Node2D body) {
		if (body is Player) {
			GameManager.Instance.CurrentLevel = levelNumber;
			SceneManagement.SceneManager.Instance.LoadScene(levelPath);
		}
	}
}
