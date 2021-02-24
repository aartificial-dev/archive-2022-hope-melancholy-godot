using Godot;
using System;

[Tool]
public class ItemInventory : Node2D {
    public ItemPawn itemPawn;

    private AnimatedSprite sprite;

    public Vector2 gridPos = Vector2.Zero;
    public Vector2 sizeGrid = Vector2.Zero;
    public Vector2 sizeSprite = Vector2.Zero;

    public override void _Ready() {
        sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        ChangeItem();

    }

    public override void _Process(float _delta) {
    }

    private void ChangeItem() {
        sprite.Frame = (int) itemPawn.spriteFrame;
        sizeSprite = itemPawn.sizeSprite;
        //sprite.Position = - spriteSize / 2f;
        sizeGrid = itemPawn.sizeGrid;

        itemPawn.ParseActions();
    }
}
