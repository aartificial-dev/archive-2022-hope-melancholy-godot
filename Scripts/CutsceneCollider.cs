using Godot;
using System;

public class CutsceneCollider : Area2D { 
    [Export]
    public NodePath cutscenePlayerPath;
    [Export]
    public String cutsceneName = "";

    private CutscenePlayer cutscenePlayer;

    public override void _Ready() {
        cutscenePlayer = GetNodeOrNull<CutscenePlayer>(cutscenePlayerPath);
        if (cutscenePlayer is null) {
            this.GetParent().RemoveChild(this);
            this.QueueFree();
            return;
        }
        if (!cutscenePlayer.HasAnimation(cutsceneName)) {
            this.GetParent().RemoveChild(this);
            this.QueueFree();
            return;
        }
        this.Connect("body_entered", this, nameof(BodyEntered));
    }

    public void BodyEntered(Node body) {
        // GD.Print(body);
        if (body is Player) {
            cutscenePlayer.Play(cutsceneName);
            this.GetParent().RemoveChild(this);
            this.QueueFree();
        }
    }

}