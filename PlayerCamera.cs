using Godot;
using System;

public class PlayerCamera : Camera2D {
    
    [Export]
    public NodePath playerPath = "../Player";

    private Player player;

    public override void _Ready() {
        player = GetNode<Player>(playerPath);
    }

    public override void _Process(float delta) {
        Transform2D _tr = this.Transform;
        Vector2 pos = _tr.origin;
        pos = pos.LinearInterpolate(player.Transform.origin, 0.05f);
        _tr.origin = pos;
        this.Transform = _tr;
    }
}