using Godot;
using System;
using IntArray = Godot.Collections.Array;

public class ItemPawn {
    public String name;
    public int spriteFrame;
    public Vector2 sizeFloor;
    public Vector2 sizeGrid;
    public Vector2 sizeSprite;
    public ItemPawn.ItemType type;
    public IntArray intArray;
    public String textField;
    public int guiFrame;

    public enum ItemType {
        Any, Weapon, Quest, Grabage, Chip, Ammo, Medicine, Keycard, Notes, Tool
    }

    public ItemPawn(String name = "item", int spriteFrame = 0, 
                    Vector2 sizeFloor = default(Vector2), Vector2 sizeGrid = default(Vector2), Vector2 sizeSprite = default(Vector2), 
                    ItemPawn.ItemType type = ItemPawn.ItemType.Any, IntArray intArray = default(IntArray), String textField = "", int guiFrame = 0) {
        this.name = name;
        this.spriteFrame = spriteFrame;
        this.sizeFloor = sizeFloor;
        this.sizeGrid = sizeGrid;
        this.sizeSprite = sizeSprite;
        this.type = type;
        this.intArray = intArray;
        this.textField = textField;
        this.guiFrame = guiFrame;
    }

    public static ItemPawn MakePawnFromGD(Resource res) { 
        String name = (String) res.Get("name");
        int spriteFrame = (int) res.Get("spriteFrame");
        Vector2 sizeFloor = (Vector2) res.Get("sizeFloor");
        Vector2 sizeGrid = (Vector2) res.Get("sizeGrid");
        Vector2 sizeSprite = (Vector2) res.Get("sizeSprite");
        ItemPawn.ItemType type = (ItemPawn.ItemType) Enum.ToObject(typeof(ItemPawn.ItemType) , (int) res.Get("type"));
        IntArray intArray = (IntArray) res.Get("intArray");
        String textField = (String) res.Get("textField");
        int guiFrame = (int) res.Get("guiFrame");
        // GD.Print(name, "  ", spriteFrame, "  ", sizeFloor, "  ", sizeGrid, "  ", sizeSprite, "  ", type, "  ", intArray, "  ", textField);
        return new ItemPawn(name, spriteFrame, sizeFloor, sizeGrid, sizeSprite, type, intArray, textField, guiFrame);
    }
}
