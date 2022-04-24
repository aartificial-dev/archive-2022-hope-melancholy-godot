using Godot;
using System;

public class InventoryManager : Control { 
    [Export]
    public Godot.Collections.Array<NodePath> inventoriesPath;
    [Export]
    public Godot.Collections.Array<NodePath> weaponsPath;
    [Export]
    public NodePath toolPath;
    [Export]
    public NodePath usablePath;
    [Export]
    public NodePath inventoryPath;

    private Godot.Collections.Array<Inventory> inventories = new Godot.Collections.Array<Inventory>();
    private Godot.Collections.Array<Inventory> weaponSlots = new Godot.Collections.Array<Inventory>();
    private Inventory slotTool;
    private Inventory slotUsable;
    private Inventory inventory;

    public ItemInventory itemInHand = null;
    public Control inventoryHand;

    private GUI gui;

    private PackedScene itemFloorScene = ResourceLoader.Load<PackedScene>("res://Scenes/Items/ItemFloor.tscn");
    private PackedScene itemInventoryScene = ResourceLoader.Load<PackedScene>("res://Scenes/Items/ItemInventory.tscn");

    private ItemScriptParser itemUseParser = null;
    private AudioStreamPlayer itemAudioPlayer;

    public override void _Ready() {
        gui = this.GetParent<GUI>();

        foreach (NodePath path in inventoriesPath) {
            Inventory inv = GetNode<Inventory>(path);
            inventories.Add(inv);
            inv.inventoryManager = this;
        }
        foreach (NodePath path in weaponsPath) {
            Inventory inv = GetNode<Inventory>(path);
            weaponSlots.Add(inv);
        }

        slotTool = GetNode<Inventory>(toolPath);
        slotUsable = GetNode<Inventory>(usablePath);
        inventory = GetNode<Inventory>(inventoryPath);
        inventoryHand = GetNode<Control>("InventoryHand");

        itemAudioPlayer = new AudioStreamPlayer();
        this.AddChild(itemAudioPlayer);
    }

	public override void _Process(float delta) {
        Player player = gui.GetPlayer();
        Inventory inv = GetInventoryUnderCursor();
        if (itemUseParser is null) {
            itemUseParser = new ItemScriptParser(player, player.camera, this);
        }
        if (!player.GetCanMove()) return;
        if (!gui.Visible) return;
        
        if (!(itemInHand is null)) {
            if (Input.IsActionJustPressed("mb_right")) {
                itemInHand.itemPawn.Rotate();
            }
            itemInHand.AdjustPosition();
            itemInHand.GlobalPosition = GameHelper.GetMousePos(this) - itemInHand.itemPawn.SpriteInventorySize / 2f;
            if (Input.IsActionJustPressed("mb_left")) {
                if (!(inv is null)) {
                    bool isPlaced = inv.PlaceItem(itemInHand);
                    if (isPlaced) gui.MakeControlsHint();
                }
            }
            if (Input.IsActionJustPressed("key_drop")) {
                DropItem(itemInHand, player.GlobalPosition); 
                itemInHand = null;
            }
        } else {
            if (!(inv is null)) {
                if (Input.IsActionJustPressed("mb_left")) {
                    inv.TakeItem();
                    gui.MakeControlsHint();
                }
                if (Input.IsActionJustPressed("mb_right")) {
                    inv.UseItem();
                }
                if (Input.IsActionJustPressed("key_drop")) {
                    ItemInventory item = inv.GetItemUnderCursor();
                    if (!(item is null)) {
                        inv.RemoveItem(item);
                        DropItem(item, player.GlobalPosition); 
                    }
                }
            }
        }
        ItemPawn curItem = GetItemUnderCursor();
        if (!(itemInHand is null)) {
            gui.AddHintFlag( (uint) GUI.HintFlags.Drop );
            gui.AddHintFlag( (uint) GUI.HintFlags.Rotate );
        } else
        if (!(curItem is null)) {
            gui.AddHintFlag( (uint) GUI.HintFlags.Drop );
            if (!(curItem.ParsedScript.Use is null)) {
                gui.AddHintFlag( (uint) GUI.HintFlags.Use );
            }
        }

        foreach (Inventory weap in weaponSlots) {
            if (weap.GetItemCount() > 0) {
                weap.GetNode<Sprite>("Overlay").Visible = false;
            } else {
                weap.GetNode<Sprite>("Overlay").Visible = true;
            }
        }
        if (slotTool.GetItemCount() > 0) {
            slotTool.GetNode<Sprite>("Overlay").Visible = false;
        } else {
            slotTool.GetNode<Sprite>("Overlay").Visible = true;
        }
        if (slotUsable.GetItemCount() > 0) {
            slotUsable.GetNode<Sprite>("Overlay").Visible = false;
        } else {
            slotUsable.GetNode<Sprite>("Overlay").Visible = true;
        }

	}

    public bool PickFloorItem(ItemFloor item) {
        bool isPicked = inventory.TryAddItem(item.itemPawn);
        if (isPicked) {
            itemAudioPlayer.Stream = item.itemPawn.AudioPickup;
            itemAudioPlayer.Play();
        }
        return isPicked;
    }

    /// <summary> Returns <c>true</c> if one of actions was purge (delete item), otherwise <c>false</c>. </summary>
    public bool UseItem(ItemPawn itemPawn) {
        // GD.Print("Use emmited");
        itemAudioPlayer.Stream = itemPawn.AudioUse;
        itemAudioPlayer.Play();
        return itemUseParser.PerformAction(ItemScriptParser.Actions.Use, itemPawn);
        // do use stuff
    }

    public void DropItem(ItemInventory item, Vector2 pos) {
        ItemFloor itemFloor = (ItemFloor) itemFloorScene.Instance();
        itemFloor.itemPawn = item.itemPawn;
        itemFloor.Position = pos;
        itemAudioPlayer.Stream = item.itemPawn.AudioDrop;
        itemAudioPlayer.Play();

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
        if (index < weaponSlots.Count) {
            ItemInventory item = weaponSlots[index].GetItem(0);
            if (!(item is null)) {
                return item;
            }
        }
        return null;
    }

    
    public void InventoryEnable(String name) {
        Inventory inv = GetInventoryByName(name);
        // if (inv.Visible != true && inv is InventoryToolbox invTool) {
        //     invTool.PlayOpenSound();
        // } 

        inv.Visible = true;
    }

    public void InventoryDisable(String name) {
        Inventory inv = GetInventoryByName(name);
        // if (inv.Visible != false && inv is InventoryToolbox invTool) {
        //     invTool.PlayCloseSound();
        // } 

        inv.Visible = false;
    }

    public void InventoryToggle(String name) {
        Inventory inv = GetInventoryByName(name);
        // if (inv is InventoryToolbox invTool) {
        //     if (inv.Visible == true) {
        //         invTool.PlayCloseSound();
        //     } else {
        //         invTool.PlayOpenSound();
        //     }
        // } 
        inv.Visible = !inv.Visible;
    }

    public void InventoryDrop(String name) {
        Inventory inv = GetInventoryByName(name);
        AudioStreamSample sound = null;

        while (inv.GetItemCount() > 0) {
            ItemInventory item = inv.GetItem(0);
            inv.RemoveItem(0);
            DropItem(item, gui.GetPlayer().Position);
            sound =  item.itemPawn.AudioDrop;
        }
        if (sound is null) return;
        itemAudioPlayer.Stream = sound;
        itemAudioPlayer.Play();
        
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

    public Inventory GetInventoryUnderCursor() {
        foreach (Inventory inv in inventories) {
            //GD.Print(inv.IsCursorOnInventory());
            if (inv.IsCursorOnInventory()) {return inv;}
        }
        return null;
    }

    public ItemPawn GetItemUnderCursor() {
        Inventory inv = GetInventoryUnderCursor();
        if (inv is null) return null;
        ItemInventory item = inv.GetItemUnderCursor();
        if (item is null) return null;
        return item.itemPawn;
    }

    public ItemInventory GetItemByID(String id) {
        return inventory.GetItemByID(id);
    }

    public void RemoveItem(String id) {
        inventory.RemoveItem(GetItemByID(id));
    }
    
    public ItemPawn GetUsableItem() {
        if (slotUsable.GetItemCount() > 0) {
            return slotUsable.GetItem(0).itemPawn;
        } else {
            return null;
        }
    }

    public void UseUsableItem() {
        if (slotUsable.GetItemCount() > 0) {
            ItemPawn item = slotUsable.GetItem(0).itemPawn;
            int ammo = item.Ammo;
            ammo --;
            if (ammo == 0) {
                slotUsable.RemoveItem(0);
            } else {
                item.Ammo = ammo;
            }
        }
    }
}