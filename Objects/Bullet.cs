using Godot;
using System;
using NodeDictionary = System.Collections.Generic.Dictionary<System.String, bool>;

public class Bullet : KinematicBody2D {

    private PackedScene trailScene = ResourceLoader.Load<PackedScene>("res://Objects/BulletTrail.tscn");
    public BulletTrail bulletTrail;

    public Vector2 direction = Vector2.Right;
    private float speed = 20f;

    private RayCast2D ray;

    private NodeDictionary nodeDict = new NodeDictionary(){
        {typeof(Flare).Name, false},
    };

    public override void _Ready() {
        bulletTrail = (BulletTrail) trailScene.Instance();
        this.GetParent().AddChild(bulletTrail);
        bulletTrail.bullet = this;
        this.Rotation = Mathf.Atan2(direction.y, direction.x);

        ray = GetNode<RayCast2D>("RayCast2D");
    }

    public override void _Process(float _delta) {
    }

    public override void _PhysicsProcess(float delta) {
        if (!(bulletTrail is null)) {
            bulletTrail.AddPoint(this.Position);
        }
        this.Rotation = Mathf.Atan2(direction.y, direction.x);


        KinematicCollision2D collide = this.MoveAndCollide(direction * speed, false);
        
        ray.CastTo = new Vector2(speed, 0f);
        ray.ForceRaycastUpdate();
        Godot.Object collision = ray.GetCollider();
        if (!(collision is null)) {
            OnRayCollision(collision);
        }

        if (!(collide is null)) { 
            Destroy();
        }
    }


    private void Destroy() {
        this.Visible = false;
        this.GetParent().RemoveChild(this);
        this.QueueFree();
    }

    public void OnRayCollision(Godot.Object body) {
        //GD.Print(body);
        String name = body.GetType().Name;
        bool doDestroy = false;
        if (nodeDict.ContainsKey(name)) {
            doDestroy = nodeDict[name];
            var method = body.GetType().GetMethod("HitByBullet");
            if (!(method is null)) {
                method.Invoke(body, new System.Object[]{ray.GetCollisionPoint(), direction, speed});
            }
        }
        if (doDestroy) {
            Destroy();
        }
    }
}
