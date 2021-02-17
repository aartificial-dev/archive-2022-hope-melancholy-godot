using Godot;
using System;

public class Player : KinematicBody2D {

    private float gravity = 9.8f;
    private float gravityMultiplier = 19f;
    private float speed = 50f;

    private AnimatedSprite spriteMovement;
    private AnimatedSprite spriteWeapons;

    public PlayerCamera camera;

    public override void _Ready() {
        spriteMovement = GetNode<AnimatedSprite>("MoveSprites/SpriteMovement");
        spriteWeapons = GetNode<AnimatedSprite>("WeaponSprites/SpriteWeapons");
    }

    public override void _Process(float delta) {
        
    }

    public override void _PhysicsProcess(float delta) {
        ProcessMove(delta);

        camera.UpdateCamera(delta);
    }

    private void ProcessMove(float delta) {
        Vector2 dir = Vector2.Zero;
        if (Input.IsActionPressed("key_right")) {
            dir.x += speed;
        }
        if (Input.IsActionPressed("key_left")) {
            dir.x -= speed;
        }
        if (!this.IsOnFloor()) {
            dir.y += gravity * gravityMultiplier;
        }

        //GD.Print(this.IsOnFloor());

        if (dir.x != 0f) {
            if (this.TestMove(this.Transform, new Vector2(dir.x / 2f * delta, 0f), false)) {
                SetMoveAnimation("wall");
                dir.x = 0;
            } else {
                SetMoveAnimation("walk");
            }
        } else if (dir.x == 0f) {
            SetMoveAnimation("idle");
            Transform2D _tr = this.Transform;
            _tr.origin = _tr.origin.Round();
            this.Transform = _tr;
        }

        if (dir.x < 0f) {
            spriteMovement.FlipH = true;
        } 
        if (dir.x > 0f) {
            spriteMovement.FlipH = false;
        }

        this.MoveAndSlide(dir, Vector2.Up);

    }

    private void SetMoveAnimation(String anim) {
        if (spriteMovement.Animation != anim) {
            spriteMovement.Animation = anim;
        }
    }
}
