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
    public int guiFrame;
    public String textField;
    public String useCommands;

    public enum ItemType {
        Any, Weapon, Quest, Grabage, Chip, Ammo, Medicine, Keycard, Notes, Tool
    }

    public ItemPawn(String name = "item", int spriteFrame = 0, 
                    Vector2 sizeFloor = default(Vector2), Vector2 sizeGrid = default(Vector2), Vector2 sizeSprite = default(Vector2), 
                    ItemPawn.ItemType type = ItemPawn.ItemType.Any, IntArray intArray = default(IntArray), int guiFrame = 0, String textField = "", String useCommands = "") {
        this.name = name;
        this.spriteFrame = spriteFrame;
        this.sizeFloor = sizeFloor;
        this.sizeGrid = sizeGrid;
        this.sizeSprite = sizeSprite;
        this.type = type;
        this.intArray = intArray;
        this.guiFrame = guiFrame;
        this.textField = textField;
        this.useCommands = useCommands;
    }

    public static ItemPawn MakePawnFromGD(Resource res) { 
        String name = (String) res.Get("name");
        int spriteFrame = (int) res.Get("spriteFrame");
        Vector2 sizeFloor = (Vector2) res.Get("sizeFloor");
        Vector2 sizeGrid = (Vector2) res.Get("sizeGrid");
        Vector2 sizeSprite = (Vector2) res.Get("sizeSprite");
        ItemPawn.ItemType type = (ItemPawn.ItemType) Enum.ToObject(typeof(ItemPawn.ItemType) , (int) res.Get("type"));
        IntArray intArray = (IntArray) res.Get("intArray");
        int guiFrame = (int) res.Get("guiFrame");
        String textField = (String) res.Get("textField");
        String useCommands = (String) res.Get("useCommands");
        // GD.Print(name, "  ", spriteFrame, "  ", sizeFloor, "  ", sizeGrid, "  ", sizeSprite, "  ", type, "  ", intArray, "  ", textField);
        return new ItemPawn(name, spriteFrame, sizeFloor, sizeGrid, sizeSprite, type, intArray, guiFrame, textField, useCommands);
    }
}
