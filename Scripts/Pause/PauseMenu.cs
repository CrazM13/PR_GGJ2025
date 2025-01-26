using Godot;
using System;

public partial class PauseMenu : CanvasLayer {

	private void GoToScene(string scene) {

		GameManager.Instance.Level = null;

		SceneManagement.SceneManager.Instance.LoadScene(scene);
	}

	public void Pause(bool pauseState) {
		GetTree().Paused = pauseState;
		this.Visible = pauseState;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (Input.IsActionJustPressed("pause")) {
			Pause(!this.Visible);
		}
	}

}
