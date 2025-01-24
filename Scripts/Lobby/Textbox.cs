using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Textbox : CanvasLayer {

	[Export] private Label text;
	[Export] private AudioStreamPlayer ttsAudio;

	public class TextItem {
		public string text { get; set; }
		public AudioStream tts { get; set; }
	}

	private Queue<TextItem> queuedLines = new Queue<TextItem>();
	private bool wasPressed = true;

	public bool IsDisplayingText => this.Visible;

	public void DisplayText(string text) {
		this.text.Text = text;
		ttsAudio.Stop();
	}

	public void DisplayText(TextItem text) {
		this.text.Text = text.text;
		ttsAudio.Stream = text.tts;
		ttsAudio.Play(0);
	}

	public void QueueText(TextItem text) {
		queuedLines.Enqueue(text);
	}

	public void QueueText(string text, AudioStream tts = null) {
		queuedLines.Enqueue(new TextItem() {
			text = text,
			tts = tts
		});
	}

	public override void _Process(double delta) {
		base._Process(delta);

		bool isPressed = Input.IsAnythingPressed();
		if (!wasPressed && isPressed) {
			NextMessage();
		}
		wasPressed = isPressed;

	}

	public void NextMessage() {
		this.text.Text = "";
		ttsAudio.Stop();

		if (queuedLines.Count > 0) {
			DisplayText(queuedLines.Dequeue());
		} else {
			Visible = false;
		}
	}

}
