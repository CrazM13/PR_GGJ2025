using Godot;
using System;


public partial class Minimap : TextureRect {

	[Export] private LevelBuilder level;
	[Export] private int revealRange;
	[Export] private float updateInterval = 1;
	[Export] private int playerSize = 1;

	private Image minimapImage;

	private float timeUntilUpdate;

	private void UpdateMinimap(Vector2I position) {
		minimapImage ??= Image.CreateEmpty(level.Minimap.GetLength(0), level.Minimap.GetLength(1), false, Image.Format.Rgb8);

		int minX = Mathf.Max(0, position.X - revealRange);
		int maxX = Mathf.Min(minimapImage.GetWidth(), position.X + revealRange);

		int minY = Mathf.Max(0, position.Y - revealRange);
		int maxY = Mathf.Min(minimapImage.GetWidth(), position.Y + revealRange);

		for (int x = minX; x < maxX; x++) {
			for (int y = minY; y < maxY; y++) {

				if (Distance(new Vector2I(x, y), position) < revealRange) {
					if (position.X + playerSize >= x && position.X - playerSize < x && position.Y - playerSize < y && position.Y + playerSize >= y) {
						minimapImage.SetPixel(x, y, Colors.Yellow);
					} else {
						minimapImage.SetPixel(x, y, level.Minimap[x, y]);
					}
				}
			}
		}

		this.Texture = ImageTexture.CreateFromImage(minimapImage);
	}

	public override void _Ready() {
		base._Ready();

		timeUntilUpdate = updateInterval;

	}

	public override void _Process(double delta) {
		base._Process(delta);

		timeUntilUpdate -= (float) delta;
		if (timeUntilUpdate < 0) {
			Vector2 playerPosition = GameManager.Instance.Player.GlobalPosition;

			Vector2I tilePosition = new Vector2I(Mathf.RoundToInt(playerPosition.X / 32), Mathf.RoundToInt(playerPosition.Y / 32)) + new Vector2I(16, 16);

			UpdateMinimap(tilePosition);
			this.Position = -tilePosition + (((Control)this.GetParent()).Size / 2);

			timeUntilUpdate = updateInterval;
		}
	}

	public static float Distance(Vector2I from, Vector2I to) {
		int x = to.X - from.X;
		int y = to.Y - from.Y;

		return Mathf.Sqrt((x * x) + (y * y));
	}

}
