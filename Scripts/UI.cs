using Godot;
using Godot.Collections;
using System;

public partial class UI : CanvasLayer
{
	[Export] public Label score_label;

	public int score = 0;

    public override void _Process(double delta)
    {
		score_label.Text = GameManager.Instance.CoinCount.ToString();

    }

    public void AddScore()
	{
		score = score + 1;
		score_label.Text = "x " + score.ToString();
	}
}