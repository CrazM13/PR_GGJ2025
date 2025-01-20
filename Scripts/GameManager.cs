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
			}

			// TODO: Set UI
		}
	}

	public LevelBuilder Level { get; set; }

}
