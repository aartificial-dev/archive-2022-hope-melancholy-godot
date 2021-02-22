using Godot;
using System;
using IntArray = Godot.Collections.Array;

public readonly struct ItemList {
    public enum Items {
        none, 
        tube, handgun, 
        syringe, toolbox, keycard, 
        lamp, flashlight, 
        chip, 
        tapePlayer, tapePlayerBroken, battery, 
        shotgunShells, handgunMag, rifleMag, plasmaMag
    }

    public ItemPawn GenerateItem(int id) {
        switch (id) {
            case (int) ItemList.Items.tube:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");

            case (int) ItemList.Items.handgun:
                return new ItemPawn("Handgun", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Weapon, default(IntArray), "");

            case (int) ItemList.Items.syringe:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");

            case (int) ItemList.Items.toolbox:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");

            case (int) ItemList.Items.keycard:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");

            case (int) ItemList.Items.lamp:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");

            case (int) ItemList.Items.flashlight:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");

            case (int) ItemList.Items.chip:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");

            case (int) ItemList.Items.tapePlayer:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");

            case (int) ItemList.Items.tapePlayerBroken:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");

            case (int) ItemList.Items.battery:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");

            case (int) ItemList.Items.shotgunShells:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");

            case (int) ItemList.Items.handgunMag:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");

            case (int) ItemList.Items.rifleMag:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");

            case (int) ItemList.Items.plasmaMag:
                return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");
        }
        return new ItemPawn("Tube", 1, new Vector2(28, 3), new Vector2(5, 1), new Vector2(55, 11), ItemPawn.ItemType.Any, default(IntArray), "");
    }

    public static Vector2 GetSizeFloor(int id) {
        switch (id) {
            case (int) ItemList.Items.tube:
                return new Vector2(28, 3);

            case (int) ItemList.Items.handgun:
                return new Vector2(18, 9);

            case (int) ItemList.Items.syringe:
                return new Vector2(16, 4);

            case (int) ItemList.Items.toolbox:
                return new Vector2(20, 8);

            case (int) ItemList.Items.keycard:
                return new Vector2(9, 4);

            case (int) ItemList.Items.lamp:
                return new Vector2(9, 11);

            case (int) ItemList.Items.flashlight:
                return new Vector2(16, 4);

            case (int) ItemList.Items.chip:
                return new Vector2(14, 6);

            case (int) ItemList.Items.tapePlayer:
                return new Vector2(16, 6);

            case (int) ItemList.Items.tapePlayerBroken:
                return new Vector2(21, 6);

            case (int) ItemList.Items.battery:
                return new Vector2(9, 5);

            case (int) ItemList.Items.shotgunShells:
                return new Vector2(15, 6);

            case (int) ItemList.Items.handgunMag:
                return new Vector2(16, 5);

            case (int) ItemList.Items.rifleMag:
                return new Vector2(16, 8);

            case (int) ItemList.Items.plasmaMag:
                return new Vector2(3, 5);
        }

        return new Vector2(8 * 2, 4 * 2);
    }

    public static Vector2 GetSizeFloor(ItemList.Items item) {
        return GetSizeFloor((int) item);
    }

    public static Vector2 GetSizeInv(int id) {
        switch (id) {
            case (int) ItemList.Items.tube:
                return new Vector2(5, 1);

            case (int) ItemList.Items.handgun:
                return new Vector2(3, 2);

            case (int) ItemList.Items.syringe:
                return new Vector2(2, 1);

            case (int) ItemList.Items.toolbox:
                return new Vector2(3, 2);

            case (int) ItemList.Items.keycard:
                return new Vector2(1, 1);

            case (int) ItemList.Items.lamp:
                return new Vector2(1, 2);

            case (int) ItemList.Items.flashlight:
                return new Vector2(2, 1);

            case (int) ItemList.Items.chip:
                return new Vector2(1, 1);

            case (int) ItemList.Items.tapePlayer:
                return new Vector2(2, 1);

            case (int) ItemList.Items.tapePlayerBroken:
                return new Vector2(2, 1);

            case (int) ItemList.Items.battery:
                return new Vector2(1, 1);

            case (int) ItemList.Items.shotgunShells:
                return new Vector2(3, 2);

            case (int) ItemList.Items.handgunMag:
                return new Vector2(3, 2);

            case (int) ItemList.Items.rifleMag:
                return new Vector2(3, 2);

            case (int) ItemList.Items.plasmaMag:
                return new Vector2(3, 2);
        }

        return Vector2.Zero;
    }

    public static Vector2 GetSizeInv(ItemList.Items item) {
        return GetSizeInv((int) item);
    }

    public static Vector2 GetSizeInvSprite(int id) {
        if (id == (int) ItemList.Items.chip) {
            return new Vector2(33f, 22f);
        }
        return GetSizeInv(id) * 11f;
    }

    public static Vector2 GetSizeInvSprite(ItemList.Items item) {
        return GetSizeInv((int) item);
    }

    public static String GetItemName(int id) {
        switch (id) {
            case (int) ItemList.Items.tube:
                return "Tube";

            case (int) ItemList.Items.handgun:
                return "Handgun";

            case (int) ItemList.Items.syringe:
                return "Syringe";

            case (int) ItemList.Items.toolbox:
                return "Toolbox";

            case (int) ItemList.Items.keycard:
                return "Keycard";

            case (int) ItemList.Items.lamp:
                return "Lamp";

            case (int) ItemList.Items.flashlight:
                return "Flashlight";

            case (int) ItemList.Items.chip:
                return "Chip";

            case (int) ItemList.Items.tapePlayer:
                return "Tape Player";

            case (int) ItemList.Items.tapePlayerBroken:
                return "Broken Tape Player";

            case (int) ItemList.Items.battery:
                return "Battery";

            case (int) ItemList.Items.shotgunShells:
                return "Shotgun Shells";

            case (int) ItemList.Items.handgunMag:
                return "Handgun Mag";

            case (int) ItemList.Items.rifleMag:
                return "Rigle Mag";

            case (int) ItemList.Items.plasmaMag:
                return "Plasma Mag";
        }

        return "default";
    }

    public static String GetItemName(ItemList.Items item) {
        return GetItemName((int) item);
    }

    public static ItemPawn.ItemType GetItemType(int id) {
        switch (id) {
            case (int) ItemList.Items.tube:
                return ItemPawn.ItemType.Weapon;

            case (int) ItemList.Items.handgun:
                return ItemPawn.ItemType.Weapon;

            case (int) ItemList.Items.syringe:
                return ItemPawn.ItemType.Medicine;

            case (int) ItemList.Items.toolbox:
                return ItemPawn.ItemType.Tool;

            case (int) ItemList.Items.keycard:
                return ItemPawn.ItemType.Quest;

            case (int) ItemList.Items.lamp:
                return ItemPawn.ItemType.Weapon;

            case (int) ItemList.Items.flashlight:
                return ItemPawn.ItemType.Weapon;

            case (int) ItemList.Items.chip:
                return ItemPawn.ItemType.Chip;

            case (int) ItemList.Items.tapePlayer:
                return ItemPawn.ItemType.Tool;

            case (int) ItemList.Items.tapePlayerBroken:
                return ItemPawn.ItemType.Grabage;

            case (int) ItemList.Items.battery:
                return ItemPawn.ItemType.Grabage;

            case (int) ItemList.Items.shotgunShells:
                return ItemPawn.ItemType.Any;

            case (int) ItemList.Items.handgunMag:
                return ItemPawn.ItemType.Any;

            case (int) ItemList.Items.rifleMag:
                return ItemPawn.ItemType.Any;

            case (int) ItemList.Items.plasmaMag:
                return ItemPawn.ItemType.Any;
        }

        return ItemPawn.ItemType.Any;
    }

    public static ItemPawn.ItemType GetItemType(ItemList.Items item) {
        return GetItemType((int) item);
    }

    public static int GetWeaponFrame(ItemInventory item) {
        switch (item.itemType) {
            case ItemList.Items.tube:
                return 1;

            case ItemList.Items.handgun:
                return 2;

            case ItemList.Items.lamp:
                return 3;

            case ItemList.Items.flashlight:
                return 4;
        }
        return 0;

    }
}
