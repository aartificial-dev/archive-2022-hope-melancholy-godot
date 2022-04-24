using Godot;
using System;

public class WireSegment : RigidBody2D { 
    public PinJoint2D pinLeft = null;
    public PinJoint2D pinRight = null;

    private PackedScene partScene = ResourceLoader.Load<PackedScene>("res://Objects/Wire/PartElectricBurn.tscn");

    [Export]
    public int health = 2;

    public WireSpawner parent = null;

    public void HitByBullet(Vector2 pos, Vector2 direction, float speed, float damage, Type type) {
        if (this.IsQueuedForDeletion()) return;
        int connect = parent.CheckConnection(this);
        this.ApplyImpulse(this.GlobalPosition - pos, direction * speed * 5f);
        if ((connect & (int) WireSpawner.WireConnection.BothPin) == 0) {
            return;
        }
        health --;
        if (health > 0) return;
        if ((connect & (int) WireSpawner.WireConnection.BothPin) > 0) {
            PartElectricBurn part = (PartElectricBurn) partScene.Instance();
            this.GetParent().AddChild(part);
            part.GlobalPosition = this.GlobalPosition;
        }
        parent.RemoveSegment(this); 
        this.GetParent().RemoveChild(this);
        this.QueueFree();
        try {
            if (!(pinLeft is null) && (connect & (int) WireSpawner.WireConnection.Left) > 0) {
                pinLeft.GetParent().RemoveChild(pinLeft);
                pinLeft.QueueFree();
            }
        } catch (Exception e) {
            e.ToString();
        }

        try {
            if (!(pinRight is null) && (connect & (int) WireSpawner.WireConnection.Right) > 0) {
                pinRight.GetParent().RemoveChild(pinRight);
                pinRight.QueueFree();
            }
        } catch (Exception e) {
            e.ToString();
        }

    }
}