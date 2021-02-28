using Godot;
using System;

public class ItemInventory : Node2D {
	public ItemPawn itemPawn;

	private AnimatedSprite sprite;

	public Vector2 gridPos = Vector2.Zero;

	public override void _Ready() {
		sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		ChangeItem();
	}

	public override void _Process(float _delta) {
	}

	public void AdjustPosition() {
		if (itemPawn.IsRotated) {
			sprite.RotationDegrees = -90;
			Vector2 pos = sprite.Position;
			pos.y = itemPawn.SpriteInventorySize.y;
			sprite.Position = pos;
		} else {
			sprite.RotationDegrees = 0;
			sprite.Position = Vector2.Zero;
		}
	}

	private void ChangeItem() {
		sprite.Frames = itemPawn.SpriteInventoryFrames;
		sprite.Frame = itemPawn.SpriteInventoryFrame;

		if (!Engine.EditorHint) {
			itemPawn.ParseActions();
		}
	}
}
