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

	private float air;
	public float CurrentAir {
		get => air;
		set {
			float oldValue = air;
			air = value;
			if (air > MaxAir) {
				air = MaxAir;
			} else if (air < 0) {
				air = 0;

				// TODO Game Lose
				if (Player != null) {
					Player.Die();
					WasSuccess = false;
					Level = null;
					SceneManagement.SceneManager.Instance.LoadScene("res://Scenes/LobbyScene.tscn");
				}
			}
		}
	}

	public float GetAirPercentage() {
		return CurrentAir / MaxAir;
	}

	public float MaxAir { get; set; } = 1f;

	public LevelBuilder Level { get; set; }
	public Player Player { get; set; }

	public float LoadingPercentage { get; set; }
	public float MonsterStrength { get; set; } = 0;
	public bool HasSeenIntro { get; set; } = false;
	public bool? WasSuccess { get; set; } = null;
	public int CurrentLevel { get; set; } = -1;

	private int selectedAbility = 0;
	public int SelectedAbility {
		get => selectedAbility;
		set {
			selectedAbility = (9 + value) % 9;
		}
	}

	public int CoinCount { get; set; } = 0;
	public bool[] ActivatedAbilities { get; set; } = new bool[9] {
		true,
		true,
		false,
		false,
		false,
		false,
		true,
		false,
		false,
	};

}
