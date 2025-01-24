using Godot;
using System;
public partial class Lobby : Node {

	[Export] private Textbox text;

	[Export] private AudioStream[] tts;

	public override void _Ready() {
		base._Ready();

		if (!GameManager.Instance.HasSeenIntro) {
			GameManager.Instance.HasSeenIntro = true;

			text.QueueText("Welcome aboard diver! Glad you chose to comply! I mean you didn't get much of a choice, we did kinda stuff you in that suit while you were sleeping.", tts[0]);
			text.QueueText("Yeah, thanks for that.", tts[1]);
			text.QueueText("Oh, you're welcome!", tts[2]);
			text.QueueText("That was sarcasm, moron. Fine since I'm here what am I doing?", tts[3]);
			text.QueueText("Simple, in front of you are seven rooms, within each is a seemingly impossible space. None of it is real, it's like... well it's like a game! Just not really... You are in the space of all conscious minds, not sure why it is a hotel, but it is both infinite and finite in different ways just like human thought.", tts[4]);
			text.QueueText("Not sure what about that is \"simple\" bud. Give me it in dumb person speak.", tts[5]);
			text.QueueText("I had a whole speech planned about this, had like thirty sci-fi references, but fine. You are going to go into each room, search for the concept inside, and destroy it!", tts[6]);
			text.QueueText("Destroying a concept. Sure thing, exactly what I planned on spending my Sunday doing.", tts[7]);
			text.QueueText("Really? That is an odd thing to plan for...", tts[8]);
			text.QueueText("Sarcasm. Why are we underwater anyway?", tts[9]);
			text.QueueText("\"We\" are not, and in a less pedantic sense, you are not either. Ever heard of a noosphere? You are where concepts live. To put it plainly: where you are doesn't exist. It is all in your mind, or rather everyone's mind.", tts[10]);
			text.QueueText("Your mind is the LAST place I want to be Stephen. And how does that explain the bloody water?", tts[11]);
			text.QueueText("You are afraid of the ocean, hence being underwater. Concepts are a real thing where you are, just try not to think about it too hard or you might create a new concept to fight.", tts[12]);
			text.QueueText("Stephen, if HR doesn't kill you I will.", tts[13]);
			text.QueueText("Kill the concepts first. You can kill me later.", tts[14]);
			text.QueueText("I'm taking that as permission. Alright, which is first?", tts[15]);
			text.QueueText("Pick a room, any room. When you pick one I'll be in touch. Dr. Wakeem out", tts[16]);
			text.QueueText("You are not a doctor Stephen. Out.", tts[17]);

			text.NextMessage();
		}
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (text.IsDisplayingText) {
			GetTree().Paused = true;
		} else {
			GetTree().Paused = false;
		}

		GameManager.Instance.AirPercentage += 0.1f;

	}

}
