using Godot;
using System;
using ItemArray = Godot.Collections.Array<ItemInventory>;

[Tool]
public class InventoryChip : Inventory {

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