using Godot;
using System;

public class Inventory : Node2D {

    private bool isOpened = false;
    private float startPosY = 0;
    private float startPosX = 0;
    
    [Export]
    public float closedOffset = 50f;
    [Export]
    public float closeSpeed = 5f;

    
    public override void _Ready() {
        startPosY = this.Position.y;
        startPosX = this.Position.x;
    }

    public override void _Process(float delta) {
        Vector2 pos = this.Position;
        if (Input.IsActionJustPressed("key_inventory")) {
            isOpened = !isOpened;
        }
        if (isOpened) {
            if (this.Position.y < startPosY) {
                pos = pos.LinearInterpolate(new Vector2(startPosX, startPosY), closeSpeed * delta);
            } else {
                pos.y = startPosY;
            }
        } else {
            if (this.Position.y >= startPosY - closedOffset) {
                pos = pos.LinearInterpolate(new Vector2(startPosX, startPosY - closedOffset), closeSpeed * delta);
            }
        }
        this.Position = pos;

        if (isOpened && this.Position.y >= startPosY - 1f) {
            // ProcessInventory();
        }
    }
}
