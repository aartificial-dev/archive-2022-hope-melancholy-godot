using Godot;
using System;
using NodeDictionary = System.Collections.Generic.Dictionary<System.String, bool>;

public class Bullet : IBullet {

    private PackedScene trailScene = ResourceLoader.Load<PackedScene>("res://Objects/BulletTrail.tscn");
    public BulletTrail bulletTrail;

    public override void _Ready() {
        bulletTrail = (BulletTrail) trailScene.Instance();
        this.GetParent().AddChild(bulletTrail);
        bulletTrail.bullet = this;

        speed = 20f;
        damage = 1f;

        base._Ready();
    }

    public override void _PhysicsProcess(float delta) {
        if (!(bulletTrail is null)) {
            bulletTrail.AddPoint(this.Position);
        }
        base._PhysicsProcess(delta);
    }
}
