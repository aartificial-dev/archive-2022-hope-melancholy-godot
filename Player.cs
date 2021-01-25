using Godot;
using System;

public class Player : KinematicBody2D {

    private float gravity = 9.8f;
    private float speed = 30f;

    private AnimatedSprite sprite;

    public override void _Ready() {
        sprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }

    public override void _Process(float delta) {
        
    }

    public override void _PhysicsProcess(float delta) {
        ProcessMove(delta);
    }

    private void ProcessMove(float delta) {
        Vector2 dir = Vector2.Zero;
        if (Input.IsActionPressed("key_right")) {
            dir.x += speed;
        }
        if (Input.IsActionPressed("key_left")) {
            dir.x -= speed;
        }
        // if (!this.IsOnFloor()) {
        //     dir.y += gravity * delta;
        // }

        if (dir.x != 0f && sprite.Animation != "walk") {
            sprite.Animation = "walk";
        } else if (dir.x == 0f && sprite.Animation != "idle") {
            sprite.Animation = "idle";
        }

        if (dir.x < 0f) {
            sprite.FlipH = true;
        } 
        if (dir.x > 0f) {
            sprite.FlipH = false;
        }

        this.MoveAndSlide(dir, Vector2.Up);
    }
}
