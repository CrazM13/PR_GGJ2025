using Godot;
using System;

public class GameManager {

	#region Singleton
	private static GameManager instance;
	public static GameManager Instance {
		get {
			instance ??= new GameManager();
			return instance;
		}
	}

	private GameManager() { /*MT*/ }
	#endregion

	private float airPercentage;
	public float AirPercentage {
		get => airPercentage;
		set {
			airPercentage = value;
			if (airPercentage > 1) {
				airPercentage = 1;
			} else if (airPercentage < 0) {
				airPercentage = 0;

				// TODO Game Lose
				if (Player != null) {
					Player.Die();
					SceneManagement.SceneManager.Instance.LoadScene(SceneManagement.SceneManager.Instance.GetTree().CurrentScene.SceneFilePath);
				}
			}
		}
	}

	public LevelBuilder Level { get; set; }
	public Player Player { get; set; }

	public float LoadingPercentage { get; set; }
	public float MonsterStrength { get; set; } = 0;

	private int selectedAbility = 0;
	public int SelectedAbility {
		get => selectedAbility;
		set {
			selectedAbility = (7 + value) % 7;
		}
	}

	public int CoinCount { get; set; } = 0;
	public bool[] ActivatedAbilities { get; set; } = new bool[7] {
		true,
		true,
		false,
		false,
		false,
		false,
		false
	};

}
