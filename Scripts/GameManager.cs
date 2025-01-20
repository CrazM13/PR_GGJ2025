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

			// TODO: Set UI
		}
	}

}
