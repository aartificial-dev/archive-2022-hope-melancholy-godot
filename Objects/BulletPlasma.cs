using Godot;
using System;
using NodeDictionary = System.Collections.Generic.Dictionary<System.String, bool>;

public class BulletPlasma : IBullet {

    public override void _Ready() {

        speed = 10f;
        damage = 2f;

        base._Ready();
    }

    public override void _PhysicsProcess(float delta) {
        base._PhysicsProcess(delta);
    }
}
