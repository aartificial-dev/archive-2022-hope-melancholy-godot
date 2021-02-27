using Godot;
using System;

public class InventoryMockup : Control{ 

    public override void _Ready() {

    }

	public override void _Process(float delta) {
        if (Input.IsActionJustPressed("key_space")) {
            GetNode<TextureRect>("Preview/TextureRect").Visible = !GetNode<TextureRect>("Preview/TextureRect").Visible;
        }
        if (Input.IsActionPressed("key_description") && !GetNode<TextureRect>("Preview/TextureRect").Visible) {
            GetNode<ColorRect>("Preview/ColorRect").Visible = true;
        } else {
            GetNode<ColorRect>("Preview/ColorRect").Visible = false;
        }
	}
}