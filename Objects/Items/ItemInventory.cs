using Godot;
using System;

[Tool]
public class ItemInventory : Node2D {
    [Export]
    public ItemList.Items itemType = ItemList.Items.none;

    private AnimatedSprite sprite;

    public Vector2 gridPos = Vector2.Zero;
    public Vector2 gridSize = Vector2.Zero;
    public Vector2 spriteSize = Vector2.Zero;

    public override void _Ready() {
        sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        ChangeItem();

    }

    public override void _Process(float delta) {
        if (sprite.Frame != (int) itemType) {
            ChangeItem();
        }
    }

    private void ChangeItem() {
        sprite.Frame = (int) itemType;
        spriteSize = ItemList.GetSizeInvSprite(sprite.Frame);
        //sprite.Position = - spriteSize / 2f;
        gridSize = ItemList.GetSizeInv(sprite.Frame);
    }
}
