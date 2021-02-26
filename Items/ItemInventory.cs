using Godot;
using System;

[Tool]
public class ItemInventory : Node2D {
	public ItemPawn itemPawn;

	private AnimatedSprite sprite;

	public Vector2 gridPos = Vector2.Zero;
	public Vector2 sizeGrid = Vector2.Zero;
	public Vector2 sizeSprite = Vector2.Zero;

	public AudioStreamSample dropAudioSample;
	public AudioStreamSample pickupAudioSample;

	public override void _Ready() {
		sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		ChangeItem();
	}

	public override void _Process(float _delta) {
	}

	private void ChangeItem() {
		sprite.Frames = itemPawn.SpriteInventoryFrames;
		sprite.Frame = itemPawn.SpriteInventoryFrame;
		sizeSprite = itemPawn.SpriteInventorySize;
		//sprite.Position = - spriteSize / 2f;
		sizeGrid = itemPawn.SpriteInventoryGridSize;

        if (!Engine.EditorHint) {
		    itemPawn.ParseActions();
        }
	}
}
