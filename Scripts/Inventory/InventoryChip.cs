using Godot;
using System;
using ItemArray = Godot.Collections.Array<ItemInventory>;

[Tool]
public class InventoryChip : Inventory {

    private AnimatedSprite[] chipSlot = new AnimatedSprite[4];
    private AnimatedSprite[] chipSlotHint = new AnimatedSprite[4];

    public override void _Ready() {
        base._Ready();

        chipSlot[0] = GetNode<AnimatedSprite>("ChipSlot1");
        chipSlotHint[0] = GetNode<AnimatedSprite>("ChipSlot1/ChipHint");
        chipSlot[1] = GetNode<AnimatedSprite>("ChipSlot2");
        chipSlotHint[1] = GetNode<AnimatedSprite>("ChipSlot2/ChipHint");
        chipSlot[2] = GetNode<AnimatedSprite>("ChipSlot3");
        chipSlotHint[2] = GetNode<AnimatedSprite>("ChipSlot3/ChipHint");
        chipSlot[3] = GetNode<AnimatedSprite>("ChipSlot4");
        chipSlotHint[3] = GetNode<AnimatedSprite>("ChipSlot4/ChipHint");
    }

    public override void _Process(float delta) {
        for (int i = 0; i < 4; i ++) {
            chipSlot[i].Visible = false;
            chipSlotHint[i].Visible = false;
        }
        foreach (ItemInventory item in itemList) {
            item.Visible = false;
            int pos = Mathf.FloorToInt(item.gridPos.x + (item.gridPos.y * 2f));
            chipSlot[pos].Visible = true;
            chipSlotHint[pos].Visible = true;
        }
        base._Process(delta);
    }

    protected override void UpdateGrid() {
        name = "chips";
        gridSize = new Vector2(2, 2);
        allowedType = ItemPawn.ItemType.Chip;
        cellSpace = new Vector2(2, 1);
        cellSize = new Vector2(32, 16);
        itemSound = ResourceLoader.Load<AudioStreamSample>("res://Sounds/Inventory/snd_inv_chip.wav");
        base.UpdateGrid();
    }

}