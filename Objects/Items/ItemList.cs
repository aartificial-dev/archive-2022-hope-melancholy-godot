using Godot;
using System;

public readonly struct ItemList {
    public enum Items {
        none, tube, handgun, syringe, toolbox, keycard, lamp, flashlight, chip, tapePlayer, tapePlayerBroken, battery, shotgunShells, handgunMag, rifleMag, plasmaMag
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
                return new Vector2(2, 3);

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
}
