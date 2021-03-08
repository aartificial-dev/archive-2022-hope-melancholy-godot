using Godot;
using System;
using NodeDictionary = System.Collections.Generic.Dictionary<System.String, bool>;

public class IBullet : KinematicBody2D {
    public Vector2 direction = Vector2.Right;
    protected float speed = 20f;
    protected float damage = 1f;

    protected RayCast2D ray;

    protected NodeDictionary nodeDict = new NodeDictionary(){
        {typeof(Flare).Name, false},
    };

    public override void _Ready() {
        this.Rotation = Mathf.Atan2(direction.y, direction.x);

        ray = GetNode<RayCast2D>("RayCast2D");
    }

    public override void _PhysicsProcess(float delta) {
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


    protected void Destroy() {
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
                method.Invoke(body, new System.Object[]{ray.GetCollisionPoint(), direction, speed, damage, this.GetType()});
            }
        }
        if (doDestroy) {
            Destroy();
        }
    }
}
