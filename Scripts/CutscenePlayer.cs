using Godot;
using System;

public class CutscenePlayer : AnimationPlayer { 

    private bool waitingForInput = false;
    private String waitInput = "";

    public override void _Ready() {

    }

	public override void _Process(float delta) {
        if (waitingForInput && Input.IsActionJustPressed(waitInput)) {
            this.Play();
        }
	}

    public void WaitForInput(String input) {
        this.Stop(false);
        waitingForInput = true;
        waitInput = input;
    }
}