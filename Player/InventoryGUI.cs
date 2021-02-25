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

    private AudioStreamPlayer itemAudioPlayer;

    PackedScene itemFloorScene = ResourceLoader.Load<PackedScene>("res://Items/ItemFloor.tscn");
    PackedScene itemInventoryScene = ResourceLoader.Load<PackedScene>("res://Items/ItemInventory.tscn");

    ItemScriptParser itemUseParser = null;

    public override void _Ready() {
        startPosY = this.Position.y;
        startPosX = this.Position.x;

        inventoryHand = GetNode<Node2D>("InventoryHand");

        itemAudioPlayer = GetNode<AudioStreamPlayer>("ItemAudioPlayer");

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
        if (itemUseParser is null) {
            itemUseParser = new ItemScriptParser(player, player.camera, this);
        }
        if (!player.GetCanMove()) return;

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
            itemInHand.GlobalPosition = this.GetGlobalMousePosition() - itemInHand.sizeSprite / 2f;
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
            if (Input.IsActionJustPressed("mb_right")) {
                foreach (Inventory inv in inventories) {
                    inv.UseItem();
                }
            }
        }
    }

    /// <summary> Returns <c>true</c> if item succesfully can be picked up or should be deleted, otherwise <c>false</c>. </summary>
    public bool PickFloorItem(ItemFloor item) {
        if (!(itemInHand is null)) return false;
        itemAudioPlayer.Stream = item.pickupAudioSample;
        itemAudioPlayer.Play();
        // GD.Print("Pickup emmited");
        bool doDelete = itemUseParser.PerformAction(ItemScriptParser.Actions.Pickup, item.itemPawn);
        // do pickup action
        // return true;
        if (doDelete) {
            return true;
        }

        ItemInventory itemInventory = (ItemInventory) itemInventoryScene.Instance();
        itemInventory.itemPawn = item.itemPawn;
        
        itemInventory.dropAudioSample = item.dropAudioSample;
        itemInventory.pickupAudioSample = item.pickupAudioSample;

        inventoryHand.AddChild(itemInventory);
        itemInHand = itemInventory;
        return true;
    }

    public void DropHandItem(Vector2 pos) {
        inventoryHand.RemoveChild(itemInHand);
        itemAudioPlayer.Stream = itemInHand.pickupAudioSample;
        itemAudioPlayer.Play();

        // GD.Print("Drop emmited");
        bool doDelete = itemUseParser.PerformAction(ItemScriptParser.Actions.Drop, itemInHand.itemPawn);
        // do drop action
        if (doDelete) {
            itemInHand = null;
        }
        
        DropItem(itemInHand, pos);
        itemInHand = null;
    }

    /// <summary> Returns <c>true</c> if one of actions was purge (delete item), otherwise <c>false</c>. </summary>
    public bool UseItem(ItemPawn itemPawn) {
        // GD.Print("Use emmited");
        itemAudioPlayer.Stream = itemPawn.useAudio;
        itemAudioPlayer.Play();
        return itemUseParser.PerformAction(ItemScriptParser.Actions.Use, itemPawn);
        // do use stuff
    }

    public void DropItem(ItemInventory item, Vector2 pos) {
        ItemFloor itemFloor = (ItemFloor) itemFloorScene.Instance();
        itemFloor.itemPawn = item.itemPawn;
        itemFloor.Position = pos;
        itemFloor.dropAudioSample = item.dropAudioSample;
        itemFloor.pickupAudioSample = item.pickupAudioSample;

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

    public void InventoryEnable(String name) {
        Inventory inv = GetInventoryByName(name);
        if (inv.Visible != true && inv is InventoryToolbox invTool) {
            invTool.PlayOpenSound();
        } 

        inv.Visible = true;
    }

    public void InventoryDisable(String name) {
        Inventory inv = GetInventoryByName(name);
        if (inv.Visible != false && inv is InventoryToolbox invTool) {
            invTool.PlayCloseSound();
        } 

        inv.Visible = false;
    }

    public void InventoryToggle(String name) {
        Inventory inv = GetInventoryByName(name);
        if (inv is InventoryToolbox invTool) {
            if (inv.Visible == true) {
                invTool.PlayCloseSound();
            } else {
                invTool.PlayOpenSound();
            }
        } 
        inv.Visible = !inv.Visible;
    }

    public void InventoryDrop(String name) {
        Inventory inv = GetInventoryByName(name);
        AudioStreamSample sound = null;

        while (inv.GetItemCount() > 0) {
            ItemInventory item = inv.GetItem(0);
            inv.RemoveItem(0);
            sound = item.dropAudioSample;
            DropItem(item, player.Position);
        }

        if (!(sound is null)) {
            itemAudioPlayer.Stream = sound;
            itemAudioPlayer.Play();
        }
        
    }

    public Inventory GetInventoryByName(String name) {
        foreach (Inventory inv in inventories) {
            if (inv.name == name) {
                return inv;
            }
        }
        return null;
    }

    public bool IsCursorOnInventory() {
        foreach (Inventory inv in inventories) {
            //GD.Print(inv.IsCursorOnInventory());
            if (inv.IsCursorOnInventory()) {return true;}
        }
        return false;
    }

}
