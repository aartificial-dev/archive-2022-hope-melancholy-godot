using Godot;
using System;

[Tool]
public class ItemFloor : RigidBody2D {
    [Export]
    ItemList.Items itemType = ItemList.Items.none;

    private AnimatedSprite sprite;
    private CollisionShape2D collision;
    private RectangleShape2D rectangleShape2D;

    public override void _Ready() {
        sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        collision = GetNode<CollisionShape2D>("CollisionShape2D");
        rectangleShape2D = new RectangleShape2D();
        collision.Shape = rectangleShape2D;
        ChangeItem();
        
    }

    public override void _Process(float delta) {
        if (Engine.EditorHint && sprite.Frame != (int) itemType) {
            ChangeItem();
        }
    }

    private void ChangeItem() {
        sprite.Frame = (int) itemType;
        rectangleShape2D.Extents = ItemList.GetSizeFloor(sprite.Frame) / 2f;
        collision.Position = new Vector2(0f, -rectangleShape2D.Extents.y);
    }
}
