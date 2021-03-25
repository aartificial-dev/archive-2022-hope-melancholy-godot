using Godot;
using System;

public class PartElectricBurn : Node2D { 

    Particles2D part;

    public override void _Ready() {
        part = GetNode<Particles2D>("Particles2D");
        part.Emitting = true;
    }

	public override void _Process(float delta) {
        if (!part.Emitting) {
            this.GetParent().RemoveChild(this);
            this.QueueFree();
        }
	}
}