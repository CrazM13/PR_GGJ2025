using Godot;
using System;

public partial class WrathBoss : Enemy {

	private const float IDLE_ANIM_SPEED = 0.5f;
	private const float RESTING_ANIM_SPEED = 0.1f;
	private const float ATTACKING_ANIM_SPEED = 2f;

	private enum BossState {
		IDLE,
		ATTACKING,
		RESTING,
		DEAD
	}

	[Export] private Node2D head;
	[Export] private Node2D tendrils;
	[Export] private CanvasLayer bossHud;

	[ExportGroup("Attacks")]
	[Export] private PackedScene attackPrefab;
	[Export] private float attackCooldown = 0.2f;
	[Export] private float waveCooldown = 1f;
	[Export] private int attacksPerWave = 4;

	private float timeUntilWave = 0;
	private float timeUntilAttack = 0;
	private int attacksThisWave = 0;

	private float animTime = 0;
	private float animSpeed = 1;

	private RandomNumberGenerator rng;

	private BossState currentState;

	public override void _Ready() {
		base._Ready();

		currentState = BossState.IDLE;
		animSpeed = IDLE_ANIM_SPEED;

		rng = new RandomNumberGenerator();

		timeUntilWave = waveCooldown;
		timeUntilAttack = attackCooldown;

	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (currentState == BossState.RESTING || currentState == BossState.ATTACKING) {
			UpdateAttacks((float) delta);
		}

		UpdateAnimation((float) delta);
	}

	private void UpdateAttacks(float delta) {
		if (timeUntilWave > 0) {
			timeUntilWave -= delta;
			if (timeUntilWave <= 0) {
				attacksThisWave = 0;
				currentState = BossState.ATTACKING;
				animSpeed = ATTACKING_ANIM_SPEED;
			}
		} else if (timeUntilAttack > 0) {
			timeUntilAttack -= delta;

			if (timeUntilAttack <= 0) {
				attacksThisWave++;
				timeUntilAttack = attackCooldown;

				
				Node2D attack = attackPrefab.Instantiate<Node2D>();

				if (attacksThisWave % 4 == 0) {
					attack.GlobalPosition = GameManager.Instance.Player.GlobalPosition;
				} else {
					Vector2 offset = new Vector2(rng.RandiRange(-8, 8) * 32, rng.RandiRange(-8, 8) * 32);
					attack.GlobalPosition = this.GlobalPosition + offset;
				}
				
				GetTree().Root.AddChild(attack);

				if (attacksThisWave >= attacksPerWave) {
					currentState = BossState.RESTING;
					animSpeed = RESTING_ANIM_SPEED;

					timeUntilWave = waveCooldown;
				}
			}
		}
	}

	private void UpdateAnimation(float delta) {

		animTime += delta * animSpeed;

		if (head.Visible) {
			head.SelfModulate = head.SelfModulate.Lerp(Colors.White, delta * 10);
		}

		if (currentState != BossState.DEAD) {
			head.Scale = new Vector2((Mathf.Abs(Mathf.Cos(animTime)) * 0.25f) + 0.75f, (Mathf.Abs(Mathf.Sin(animTime)) * 0.5f) + 0.5f);
			tendrils.Scale = new Vector2((Mathf.Abs(Mathf.Sin(animTime)) * 0.1f) + 0.9f, (Mathf.Abs(Mathf.Cos(animTime)) * 0.1f) + 0.9f);
		} else {
			if (head.Visible) head.Scale = new Vector2(1, 1 - (animTime / 1));

			if (animTime >= 1) {
				head.Visible = false;
			}
		}
		
	}

	public override void ActivateEnemy() {
		if (IsActive) return;

		GetTree().Paused = true;
		GameManager.Instance.Player.CameraLookAt(this.GlobalPosition);
		GetTree().CreateTimer(4).Timeout += () => {
			bossHud.Visible = true;
			GameManager.Instance.Player.CameraReset();
			currentState = BossState.ATTACKING;
			animSpeed = ATTACKING_ANIM_SPEED;
			GetTree().Paused = false;
		};

		IsActive = true;
	}

	public override void OnDeath() {
		animSpeed = 1;
		currentState = BossState.DEAD;
		animTime = 0;

		GetTree().Paused = true;
		GameManager.Instance.Player.CameraLookAt(this.GlobalPosition);
		GameManager.Instance.ActivatedAbilities[2] = true;
		GetTree().CreateTimer(4).Timeout += () => {
			bossHud.Visible = false;
			GameManager.Instance.Player.CameraReset();
			GetTree().Paused = false;

			GameManager.Instance.WasSuccess = true;
			GameManager.Instance.Level = null;
			SceneManagement.SceneManager.Instance.LoadScene("res://Scenes/LobbyScene.tscn");
		};
	}

	public override void OnDamage() {
		head.SelfModulate = Colors.Red;
		head.Scale = Vector2.One;
	}

}
