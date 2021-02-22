using Godot;
using System;

public class Bullet : KinematicBody2D {

    private PackedScene trailScene = ResourceLoader.Load<PackedScene>("res://Objects/BulletTrail.tscn");
    public BulletTrail bulletTrail;

    public Vector2 direction = Vector2.Right;
    private float speed = 20f;

    public override void _Ready() {
        bulletTrail = (BulletTrail) trailScene.Instance();
        this.GetParent().AddChild(bulletTrail);
        bulletTrail.bullet = this;
        this.Rotation = Mathf.Atan2(direction.y, direction.x);
    }

    public override void _Process(float _delta) {
    }

    public override void _PhysicsProcess(float delta) {
        if (!(bulletTrail is null)) {
            bulletTrail.AddPoint(this.Position);
        }
        this.Rotation = Mathf.Atan2(direction.y, direction.x);


        KinematicCollision2D collide = this.MoveAndCollide(direction * speed, false);
        
        if (!(collide is null)) {
            Destroy();
        }
    }


    private void Destroy() {
        this.Visible = false;
        this.GetParent().RemoveChild(this);
        this.QueueFree();
    }
}
