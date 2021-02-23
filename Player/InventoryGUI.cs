using Godot;
using System;

public class InventoryGUI : Node2D {

    private bool isOpened = false;
    private float startPosY = 0;
    private float startPosX = 0;

    public Player player;
    
    [Export]
    public float closedOffset = 50f;
    [Export]
    public float closeSpeed = 5f;
    [Export]
    public Godot.Collections.Array<NodePath> inventoriesPath;
    [Export]
    public Godot.Collections.Array<NodePath> weaponsPath;

    private Godot.Collections.Array<Inventory> inventories = new Godot.Collections.Array<Inventory>();
    
    public Godot.Collections.Array<Inventory> weapons = new Godot.Collections.Array<Inventory>();

    public ItemInventory itemInHand = null;
    public Node2D inventoryHand;

    PackedScene itemFloorScene = ResourceLoader.Load<PackedScene>("res://Items/ItemFloor.tscn");
    PackedScene itemInventoryScene = ResourceLoader.Load<PackedScene>("res://Items/ItemInventory.tscn");

    public override void _Ready() {
        startPosY = this.Position.y;
        startPosX = this.Position.x;

        inventoryHand = GetNode<Node2D>("InventoryHand");

        foreach (NodePath path in inventoriesPath) {
            Inventory inv = GetNode<Inventory>(path);
            inventories.Add(inv);
            inv.inventoryGUI = this;
        }
        foreach (NodePath path in weaponsPath) {
            Inventory inv = GetNode<Inventory>(path);
            weapons.Add(inv);
        }
    }

    public override void _Process(float delta) {
        Vector2 pos = this.Position;
        if (Input.IsActionJustPressed("key_inventory")) {
            isOpened = !isOpened;
        }
        if (isOpened) {
            if (this.Position.y < startPosY) {
                pos = pos.LinearInterpolate(new Vector2(startPosX, startPosY), closeSpeed * delta);
            } else {
                pos.y = startPosY;
            }
        } else {
            if (this.Position.y >= startPosY - closedOffset) {
                pos = pos.LinearInterpolate(new Vector2(startPosX, startPosY - closedOffset), closeSpeed * delta);
            }
        }
        this.Position = pos;

        if (isOpened && this.Position.y >= startPosY - 1f) {
            // ProcessInventory();
        }

        if (!(itemInHand is null)) {
            itemInHand.GlobalPosition = this.GetGlobalMousePosition() - itemInHand.spriteSize / 2f;
            if (Input.IsActionJustPressed("mb_left")) {
                bool doDropItem = true;
                foreach (Inventory inv in inventories) {
                    bool isItemPlaced = inv.PlaceItem(itemInHand); // returns true if cursor is on inventory
                    if (isItemPlaced) {
                        doDropItem = false;
                        return;
                    }
                }
                if (doDropItem) {
                    Vector2 dropPos = this.GetGlobalMousePosition();

                    float distance = player.GlobalPosition.DistanceTo(dropPos);
                    if (distance > player.interactDistance) {
                        float ang = player.GlobalPosition.AngleToPoint(dropPos);
                        dropPos = player.GlobalPosition - new Vector2(Mathf.Cos(ang) * player.interactDistance, Mathf.Sin(ang) * player.interactDistance);
                    }

                    player.itemDropRayCast.CastTo = dropPos - player.GlobalPosition;
                    player.itemDropRayCast.ForceRaycastUpdate();

                    if (player.itemDropRayCast.IsColliding()) {
                        DropHandItem(player.itemDropRayCast.GetCollisionPoint());
                    } else {
                        DropHandItem(dropPos);
                    }
                    
                }
                
            }
        } else {
            if (Input.IsActionJustPressed("mb_left")) {
                foreach (Inventory inv in inventories) {
                    inv.TakeItem();
                }
            }
        }
    }

    public bool PickFloorItem(ItemFloor item) {
        if (!(itemInHand is null)) return false;
        ItemInventory itemInventory = (ItemInventory) itemInventoryScene.Instance();
        itemInventory.itemType = item.itemType;
        itemInventory.itemPawn = item.itemPawn;

        inventoryHand.AddChild(itemInventory);
        itemInHand = itemInventory;
        return true;
    }

    public void DropHandItem(Vector2 pos) {
        inventoryHand.RemoveChild(itemInHand);
        DropItem(itemInHand, pos);
        itemInHand = null;
    }

    public void DropItem(ItemInventory item, Vector2 pos) {
        ItemFloor itemFloor = (ItemFloor) itemFloorScene.Instance();
        itemFloor.itemType = item.itemType;
        itemFloor.itemPawn = item.itemPawn;
        itemFloor.Position = pos;

        Godot.Collections.Array arr = this.GetTree().GetNodesInGroup("ItemHolder");
        foreach (Node node in arr) {
            if (node is Node2D) {
                node.AddChild(itemFloor);
                break;
            }
        }
        item.QueueFree();
    } 

    public ItemInventory GetWeapon(int index) {
        if (index < weapons.Count) {
            ItemInventory item = weapons[index].GetItem(0);
            if (!(item is null)) {
                return item;
            }
        }
        return null;
    }

    public void SetWeaponSlot(int index) {
        for (int i = 0; i < weapons.Count; i ++) {
            Inventory weap = weapons[i];
            if (i == index) {
                weap.GetNode<Sprite>("SelectOverlay").Visible = true;
            } else {
                weap.GetNode<Sprite>("SelectOverlay").Visible = false;
            }
            
        }
    }

}
