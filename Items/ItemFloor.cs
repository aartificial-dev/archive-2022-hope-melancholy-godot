using Godot;
using System;

[Tool]
public class ItemFloor : RigidBody2D, InteractiveObject {
    [Export(PropertyHint.ResourceType, "ItemPawnGD")]
    public Resource itemPawnResource;

    public ItemPawn itemPawn;

    private AnimatedSprite sprite;
    private CollisionShape2D collision;
    private RectangleShape2D rectangleShape2D;

    public bool MouseIn {set; get;}

    private Player player = null;
    private PlayerCamera camera = null;

    public override void _Ready() {
        sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        collision = GetNode<CollisionShape2D>("CollisionShape2D");
        rectangleShape2D = new RectangleShape2D();
        collision.Shape = rectangleShape2D;
        ChangeItem();
        
        this.Connect("input_event", this, nameof(MouseEvent));
        this.Connect("mouse_entered", this, nameof(MouseEntered));
        this.Connect("mouse_exited", this, nameof(MouseExited));
    }

    public override void _Process(float delta) {
        if (Engine.EditorHint) {
            TryConnectItemChangeSignal();
            return;
        }

        if (MouseIn) {
            FindCamera();
            FindPlayer();
            if (! (player is null || camera is null) ) {
                ShowInteractHint();
            }
        }
        
    }

    public void ShowInteractHint() {
        bool canPickupDistance = false;
        if (!(player is null)) {
            canPickupDistance = this.GlobalPosition.DistanceTo(player.GlobalPosition) <= player.interactDistance;
        }
        if (canPickupDistance) {
            camera.ShowInteractHint(itemPawn.Name, PlayerCamera.InteractHintIcon.hand, this.GlobalPosition, this);
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
            }
        } else {
            if (!Engine.EditorHint) {
                GD.PrintErr("Item Resource not set");
                this.GetParent().RemoveChild(this);
                this.QueueFree();
            }
        }
    }

    public void MouseEvent(Node viewport, InputEvent inputEvent, int shapeidx) {
        if (Engine.EditorHint) return;
        if (inputEvent.IsActionReleased("mb_right")) {
            FindCamera();
            FindPlayer();
            if (! (player is null || camera is null) ) {
                Use();
            }
        }
    }

    public void Use() {
        // pick up event
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

    public void MouseEntered() {
        MouseIn = true;
    }
    public void MouseExited() {
        MouseIn = false;
    }

    private void Pickup() {
        Godot.Collections.Array bodies = this.GetCollidingBodies();
        foreach (Node2D nodeBody in bodies) {
            if (nodeBody is RigidBody2D body) {
                body.Sleeping = false;
                body.ApplyCentralImpulse(Vector2.Up * 10f);
            }
        }
        MouseExited();
        this.GetParent().RemoveChild(this);
        this.QueueFree();
    }
    
    private Player FindPlayer() {
        if (!(player is null)) return player;
        Godot.Collections.Array arr = this.GetTree().GetNodesInGroup("Player");
        foreach (Node node in arr) {
            if (node is Player) {
                player = (Player) node;
            }
        }
        return player;
    }

    private PlayerCamera FindCamera() {
        if (!(camera is null)) return camera;
        Godot.Collections.Array arr = this.GetTree().GetNodesInGroup("Player");
        foreach (Node node in arr) {
            if (node is PlayerCamera) {
                camera = (PlayerCamera) node;
            }
        }
        return camera;
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
}
