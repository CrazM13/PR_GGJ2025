using Godot;
using System;

public partial class EnemySpawner : Node
{
	[Export] PackedScene enemy_scn;
	[Export] Node2D[] spawn_points;
	[Export] float eps = 1f;

	float spawn_rate;
	float time_until_spawn = 0;

    public override void _Ready()
    {
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
        Vector2 location = spawn_points[rng.Randi() % spawn_points.Length].GlobalPosition;

        Enemy enemy = (Enemy)enemy_scn.Instantiate();
        enemy.GlobalPosition = location;

        GetTree().Root.AddChild(enemy);

    }
}
