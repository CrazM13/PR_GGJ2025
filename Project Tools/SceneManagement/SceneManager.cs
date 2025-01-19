using Godot;
using System;

namespace SceneManagement {
	[GlobalClass]
	public partial class SceneManager : Node {

		private static readonly Color BLACK = Colors.Black;
		private static readonly Color TRANSPARENT = new Color(0, 0, 0, 0);

		[Export] private float fadeDuration = 0.25f;
		[Export, ExportGroup("Async Loading")] private bool allowAsyncLoading = false;
		[Export] private float minLoadingTime = 0.25f;

		public static SceneManager Instance { get; private set; }

		private static string pathToLoad;
		private static bool isLoadingAsync = false;

		private CanvasLayer internalCanvas;
		private ColorRect internalColourRect;

		private float fadeTime = 0;

		private enum SceneLoadingMode {
			LOADING_IN,
			LOADING_OUT,
			IDLE
		}
		private SceneLoadingMode sceneLoadingMode;

		#region Events
		public SceneManager() {
			internalCanvas = new CanvasLayer();
			internalColourRect = new ColorRect();
			internalCanvas.AddChild(internalColourRect);
			internalColourRect.Color = Colors.Black;
			internalColourRect.Size = new Vector2(10000, 10000);

			internalCanvas.Layer = 100;

			AddChild(internalCanvas);
			sceneLoadingMode = SceneLoadingMode.LOADING_IN;
		}

		public override void _EnterTree() {
			base._EnterTree();

			Instance = this;

			internalColourRect.Size = GetViewport().GetVisibleRect().Size;
		}

		public override void _ExitTree() {
			base._ExitTree();

			if (Instance == this) Instance = null;
		}

		public override void _Process(double delta) {
			base._Process(delta);

			UpdateLoadingAnimation((float) delta);

			if (allowAsyncLoading && sceneLoadingMode == SceneLoadingMode.IDLE && isLoadingAsync) {
				UpdateAsyncLoading();
			}

		}
		#endregion

		#region Interface
		public void LoadScene(string scenePath, string loadingScenePath) {
			if (sceneLoadingMode != SceneLoadingMode.IDLE || isLoadingAsync) {
				GD.Print("[!!!] New request to load scene while scene is still loading!");
				return;
			}

			pathToLoad = scenePath;
			isLoadingAsync = true;
			ResourceLoader.LoadThreadedRequest(scenePath);
			ForceLoadScene(loadingScenePath);
		}

		public void LoadScene(string scenePath) {
			if (sceneLoadingMode != SceneLoadingMode.IDLE || isLoadingAsync) {
				GD.Print("[!!!] New request to load scene while scene is still loading!");
				return;
			}

			ForceLoadScene(scenePath);
		}

		public void LoadScene(PackedScene scene) {
			if (sceneLoadingMode != SceneLoadingMode.IDLE || isLoadingAsync) {
				GD.Print("[!!!] New request to load scene while scene is still loading!");
				return;
			}

			ForceLoadScene(scene);
		}

		public static float GetLoadingProgress() {
			if (string.IsNullOrEmpty(pathToLoad) || !isLoadingAsync) {
				return 0;
			} else {
				Godot.Collections.Array progress = new();
				ResourceLoader.LoadThreadedGetStatus(pathToLoad, progress);

				return progress[0].As<float>();
			}
		}

		public void Quit() {
			GetTree().Quit();
		}
		#endregion

		private void ForceLoadScene(string scenePath) {
			PackedScene newScene = ResourceLoader.Load<PackedScene>(scenePath);
			ForceLoadScene(newScene);
		}

		private void ForceLoadScene(PackedScene scene) {
			this.AddChild(internalCanvas);

			fadeTime = 0;
			sceneLoadingMode = SceneLoadingMode.LOADING_OUT;

			Node newRoot = scene.Instantiate();
			SceneTree sceneTree = GetTree();
			sceneTree.CreateTimer(fadeDuration).Timeout += () => {
				sceneTree.Root.AddChild(newRoot);
				sceneTree.UnloadCurrentScene();
				sceneTree.CurrentScene = newRoot;
			};
		}

		private void UpdateLoadingAnimation(float delta) {
			fadeTime += delta;

			switch (sceneLoadingMode) {
				case SceneLoadingMode.LOADING_OUT:
					internalColourRect.Color = TRANSPARENT.Lerp(BLACK, fadeTime / fadeDuration);
					break;
				case SceneLoadingMode.LOADING_IN:
					internalColourRect.Color = BLACK.Lerp(TRANSPARENT, fadeTime / fadeDuration);

					if (fadeTime >= fadeDuration) {
						sceneLoadingMode = SceneLoadingMode.IDLE;
						this.RemoveChild(internalCanvas);
					}
					break;
			}
		}

		private void UpdateAsyncLoading() {
			if (!string.IsNullOrEmpty(pathToLoad)) {
				ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(pathToLoad);
				if (status == ResourceLoader.ThreadLoadStatus.Loaded) {

					PackedScene newScene = (PackedScene) ResourceLoader.LoadThreadedGet(pathToLoad);

					pathToLoad = "";
					isLoadingAsync = false;

					if (minLoadingTime > 0) {
						GetTree().CreateTimer(minLoadingTime).Timeout += () => ForceLoadScene(newScene);
					} else {
						ForceLoadScene(newScene);
					}
				}
			}
		}

	}
}
