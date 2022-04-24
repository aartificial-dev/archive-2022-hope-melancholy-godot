using Godot;
using System;

public class GUI : Control { 
    private GUIStatus guiStatus;
    private GUIPreview guiPreview;
    private InventoryManager inventoryManager;
    private PlayerCamera camera;
    private Player player;
    private RichTextLabel controlsHint;

    private uint hintFlags = 0b0;

    public enum HintFlags {
        Use = 0b00001,
        Rotate = 0b00010,
        Drop = 0b00100,
        Description = 0b01000,
        Scroll = 0b10000
    }

    public override void _Ready() {
        guiStatus = GetNode<GUIStatus>("Status");
        guiPreview = GetNode<GUIPreview>("Preview");
        inventoryManager = GetNode<InventoryManager>("InventoryManager");
        camera = this.GetParent().GetParent<PlayerCamera>();
        controlsHint = GetNode<RichTextLabel>("ControlsHint");
    }

	public override void _Process(float delta) {
        //GetNode<ColorRect>("CanvasCRT/CRTEffect").Visible = this.Visible;

        MakeControlsHint();
	}

    public void InventoryDrop(String name) {
        inventoryManager.InventoryDrop(name);
    }

    public void SetHintFlags(uint flags) {
        hintFlags = flags;
    }

    public uint GetHintFlags(uint flag) {
        return hintFlags;
    }

    public void AddHintFlag(uint flag) {
        hintFlags = hintFlags | flag;
    }

    public void MakeControlsHint() {
        String str = "[center] ";
        String sUse = "[img]res://Assets/Sprites/GUI/Inventory/spr_inventory_hint3.png[/img] - use";
        String sRot = "[img]res://Assets/Sprites/GUI/Inventory/spr_inventory_hint3.png[/img] - rotate";
        String sDrop = "[img]res://Assets/Sprites/GUI/Inventory/spr_inventory_hint2.png[/img] - drop";
        String sDesc = "[img]res://Assets/Sprites/GUI/Inventory/spr_inventory_hint1.png[/img] - description";
        String sScr = "[img]res://Assets/Sprites/GUI/Inventory/spr_inventory_hint4.png[/img] - scroll";

        if ( (hintFlags & (uint) HintFlags.Use) != 0b0) {
            str += sUse;
            if ( (hintFlags & (uint) HintFlags.Drop) != 0b0 ) {
                str += ", ";
            }
            if ( (hintFlags & (uint) HintFlags.Rotate) != 0b0 ) {
                str += ", ";
            }
        }
        if ( (hintFlags & (uint) HintFlags.Rotate) != 0b0) {
            str += sRot;
            if ( (hintFlags & (uint) HintFlags.Drop) != 0b0 ) {
                str += ", ";
            }
        }
        if ( (hintFlags & (uint) HintFlags.Drop) != 0b0) {
            str += sDrop;
            if ( (hintFlags & (uint) HintFlags.Description) != 0b0 ) {
                str += ", ";
            }
        }
        if ( (hintFlags & (uint) HintFlags.Description) != 0b0) {
            str += sDesc;
            if ( (hintFlags & (uint) HintFlags.Scroll) != 0b0 ) {
                str += ", ";
            }
        }
        if ( (hintFlags & (uint) HintFlags.Scroll) != 0b0) {
            str += sScr;
        }

        str += " [/center]";

        controlsHint.BbcodeText = str;
        hintFlags = 0b0;
    }

    public Player GetPlayer() {
        return player;
    }

    public void SetPlayer(Player player) {
        this.player = player;
    }

    public bool PickFloorItem(ItemFloor item) {
        return inventoryManager.PickFloorItem(item);
    }

    public ItemPawn GetItemUnderCursor() {
        return inventoryManager.GetItemUnderCursor();
    }

    public ItemPawn GetItemInHand() {
        if (inventoryManager.itemInHand is null) return null;
        return inventoryManager.itemInHand.itemPawn;
    }

    public ItemInventory GetWeapon(int slot) {
        return inventoryManager.GetWeapon(slot);
    }

    public ItemInventory GetItemByID(String id) {
        return inventoryManager.GetItemByID(id);
    }

    public void RemoveItem(String id) {
        inventoryManager.RemoveItem(id);
    }
    
    public int GetPlayerHealth() {
        return GetPlayer().health;
    }
    
    public int GetPlayerHealthMax() {
        return GetPlayer().healthMax;
    }
    
    public int GetPlayerSanity() {
        return GetPlayer().sanity;
    }
    
    public int GetPlayerSanityMax() {
        return GetPlayer().sanityMax;
    }

    public ItemPawn GetUsableItem() {
        return inventoryManager.GetUsableItem();
    }

    public void UseUsableItem() {
        inventoryManager.UseUsableItem();
    }
}