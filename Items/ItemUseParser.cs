using Godot;
using System;

public class ItemUseParser {
    private Player player;
    private PlayerCamera camera;
    private InventoryGUI inventoryGUI;

    public ItemUseParser(Player player, PlayerCamera camera, InventoryGUI inventoryGUI) {
        this.player = player;
        this.camera = camera;
        this.inventoryGUI = inventoryGUI;
    }

    public static void ParseActions(String actions) {
        String[] arr = actions.Split("\n", false);
        System.Collections.Generic.List<String> commands = new System.Collections.Generic.List<String>();
        int i = 0;
        foreach (String str in arr) {
            if (!str.StartsWith("~")) {
                commands.Add(str);
            }
            i ++;
        }
    }
}
