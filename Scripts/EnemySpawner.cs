using Godot;
using System;

public partial class EnemySpawner : Node
{
	[Export] PackedScene enemy_scn;
	[Export] float eps = 1f;

	[Export] private int mapSize = 64;
	private Vector2[] spawnPoints;

	float spawn_rate;
	float time_until_spawn = 0;

    public override void _Ready()
    {
		spawnPoints = new Vector2[mapSize * mapSize];
		for (int i = 0, x = 0; x < mapSize; x++) {
			for (int y = 0; y < mapSize; y++, i++) {
				spawnPoints[i] = new Vector2((x + 0.5f) * 512, (y + 0.5f) * 512);
			}
		}

		spawn_rate = 1 / eps;
    }

    public override void _Process(double delta)
    {
        if (time_until_spawn > spawn_rate)
        {
            Spawn();
            time_until_spawn = 0;
        }
        else
        {
            time_until_spawn += (float)delta;
        }
    }
    private void Spawn()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        Vector2 location = spawnPoints[rng.Randi() % spawnPoints.Length];

        Node2D enemy = (Node2D) enemy_scn.Instantiate();
        enemy.GlobalPosition = location;

        GetTree().Root.AddChild(enemy);

    }
}
