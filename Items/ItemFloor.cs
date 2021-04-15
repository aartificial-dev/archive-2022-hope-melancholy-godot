using Godot;
using System;

[Tool]
public class ItemFloor : RigidBody2D, InteractiveObject {
    [Export(PropertyHint.ResourceType, "ItemPawnGD")]
    public Resource itemPawnResource = null;

    public ItemPawn itemPawn;

    private AnimatedSprite sprite;
    private CollisionShape2D collision;
    private RectangleShape2D rectangleShape2D;

    public bool MouseIn {set; get;}

    private Player player = null;
    private PlayerCamera camera = null;

    private InteractiveHelper<ItemFloor> helper;

    public override void _Ready() {

        if (!Engine.EditorHint) {
            helper = new InteractiveHelper<ItemFloor>(this, "", PlayerCamera.InteractHintIcon.hand, Vector2.Zero);
            this.Connect("input_event", helper, nameof(helper.MouseEvent));
            this.Connect("mouse_entered", helper, nameof(helper.MouseEntered));
            this.Connect("mouse_exited", helper, nameof(helper.MouseExited));
        }
        
        sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        collision = GetNode<CollisionShape2D>("CollisionShape2D");
        rectangleShape2D = new RectangleShape2D();
        collision.Shape = rectangleShape2D;
        ChangeItem();
    }

    public override void _Process(float delta) {
        if (Engine.EditorHint) {
            TryConnectItemChangeSignal();
        } else {
            helper.CheckHint();
        }
    }

    public void ChangeItem() {
        if (!(itemPawnResource is null)) {
            itemPawn = new ItemPawn(itemPawnResource);
        }
        if (!(itemPawn is null)) {
            sprite.Frames = itemPawn.SpriteFloorFrames;
            sprite.Frame = itemPawn.SpriteFloorFrame;
            rectangleShape2D.Extents = itemPawn.SpriteFloorSize / 2f;
            collision.Position = new Vector2(0f, -rectangleShape2D.Extents.y);
            if (!Engine.EditorHint) {
                itemPawn.ParseActions();
                helper.ChangeHintName(itemPawn.Name);
            }
        } else {
            if (!Engine.EditorHint) {
                GD.PrintErr("Item Resource not set");
                this.GetParent().RemoveChild(this);
                this.QueueFree();
            }
        }
    }

    private void TryConnectItemChangeSignal() {
        if (!(itemPawnResource is null)) {
            if (!itemPawnResource.IsConnected("value_changed", this, nameof(ChangeItem))) {
                itemPawnResource.Connect("value_changed", this, nameof(ChangeItem), default(Godot.Collections.Array), (uint) Godot.Object.ConnectFlags.Persist);
                ChangeItem();
            }
        } else {
            itemPawn = null;
        }
    }

    public void Use() {
        // pick up event
        FindCamera();
        FindPlayer();
        if (camera.interactNode is null) return;
        bool canPickupDistance = this.GlobalPosition.DistanceTo(player.GlobalPosition) <= player.interactDistance;
        bool isCurrentInstance = camera.interactNode.GetInstanceId() == this.GetInstanceId();
        if (canPickupDistance && isCurrentInstance) {
            bool canPickup = camera.PickFloorItem(this);
            if (canPickup) {
                Pickup();
            }
        }
    }

    private void Pickup() {
        Godot.Collections.Array bodies = this.GetCollidingBodies();
        foreach (Node2D nodeBody in bodies) {
            if (nodeBody is RigidBody2D body) {
                body.Sleeping = false;
                body.ApplyCentralImpulse(Vector2.Up * 10f);
            }
        }
        helper.MouseExited();
        this.GetParent().RemoveChild(this);
        this.QueueFree();
    }

    private PlayerCamera FindCamera() {
        if (!(camera is null)) return camera;
        camera = GameHelper.GetNodeInGroup<PlayerCamera>(this, "Player");
        return camera;
    }
    
    private Player FindPlayer() {
        if (!(player is null)) return player;
        player = GameHelper.GetNodeInGroup<Player>(this, "Player");
        return player;
    }
}
