using Godot;
using System;
using Godot.Collections;
using ActionDictionary = System.Collections.Generic.Dictionary<System.String, Godot.Collections.Array<System.String>>;
using StringList = System.Collections.Generic.List<System.String>;
using CommandList = System.Collections.Generic.List<ItemScriptParser.ItemScriptCommand>;

public class ItemScriptParser {
    private Player player;
    private PlayerCamera camera;
    private InventoryManager inventoryManager;
    
    public struct ItemScriptActions {
        private CommandList pickup;
        public CommandList Pickup { get => pickup; }

        private CommandList use;
        public CommandList Use { get => use; }

        private CommandList drop;
        public CommandList Drop { get => drop; }

        public ItemScriptActions(CommandList pickup, CommandList use, CommandList drop) {
            this.pickup = pickup;
            this.use = use;
            this.drop = drop;
        }

        public bool CheckAction(Actions action) {
            switch (action) {
                case Actions.Pickup:
                    return !(pickup is null);
                case Actions.Use:
                    return !(use is null);
                case Actions.Drop:
                    return !(drop is null);
                default:
                    return false;
            }
        }

        public override string ToString() {
            String toString = "";
            toString += "Pickup : \n";
            toString += this.ToString(Actions.Pickup);
            toString += "\n";
            toString += "Use : \n";
            toString += this.ToString(Actions.Use);
            toString += "\n";
            toString += "Drop : \n";
            toString += this.ToString(Actions.Drop);
            toString += "\n";
            return toString;
        }

        public string ToString(Actions action) {
            String toString = "";
            CommandList list = null;
            switch (action) {
                case Actions.Pickup:
                    list = pickup;
                break;
                case Actions.Use:
                    list = use;
                break;
                case Actions.Drop:
                    list = drop;
                break;
                default:
                    return "null";
            }
            if (list is null) {
                return "null";
            }
            foreach (ItemScriptCommand command in list) {
                toString += command.ToString();
                toString += "\n";
            }
            return toString;
        }
    }

    public struct ItemScriptCommand {
        private String command;
        public String Command { get => command; }

        private Array<String> args;
        public Array<String> Args { get => args; }

        public ItemScriptCommand(String command, Array<String> args) {
            this.command = command;
            this.args = args;
        }

        public override string ToString() {
            String toString = "";
            toString += command;
            foreach (String str in args) {
                toString += " ";
                toString += str;
            }
            return toString;
        }
    }

    public enum Actions {
        Pickup, Use, Drop, None
    }

    private static ActionDictionary actionDictionary = new ActionDictionary(){
        {"define",      new Array<String>(){"pickup", "use", "drop"}},                                          // define -action
        {"inventory",   new Array<String>(){"enable", "disable", "toggle", "drop"}},                                    // inventory -action -name
        {"message",     null},                                                                                  // message -text
        {"heal",        new Array<String>(){"health", "sanity"}},                                               // heal -attribute -value
        {"damage",      new Array<String>(){"health", "sanity"}},                                               // damage -attribute -value
        {"note",        new Array<String>(){"sticker", "lined-paper", "blank-paper", "newspaper", "book"}},     // note -type
        {"ammo",        new Array<String>(){"handgun", "battery"}},                                             // ammo -type -amount
        {"purge",       null}                                                                                   // purge
    };

    public ItemScriptParser(Player player, PlayerCamera camera, InventoryManager inventoryManager) {
        this.player = player;
        this.camera = camera;
        this.inventoryManager = inventoryManager;
    }

    public static ItemScriptActions ParseActions(String actions) {
        String[] arr = actions.Split("\n", false);
        StringList commands = new StringList();
        // Loop for ignoring comments
        foreach (String line in arr) {
            if (!line.StartsWith("~")) {
                commands.Add(line);
            }
        }
        CommandList pickup = null;
        CommandList use = null;
        CommandList drop = null;

        Actions currentAction = Actions.None;

        foreach (String line in commands) {
            String[] command = line.Split(" ", false);
            if (command.Length == 0) continue;
            bool isSyntaxRight = CheckCommandSyntax(command);
            if (!isSyntaxRight) {
                GD.PrintErr("Error in item use parser command syntax: ", line);
                continue;
            }
            if (command[0] == "define") {
                switch (command[1]) {
                    case "pickup": 
                        currentAction = Actions.Pickup; 
                        pickup = new CommandList();
                    continue;
                    case "use": 
                        currentAction = Actions.Use; 
                        use = new CommandList();
                    continue;
                    case "drop": 
                        currentAction = Actions.Drop; 
                        drop = new CommandList();
                    continue;
                }
            }
            if (currentAction == Actions.None) continue;

            Array<String> args = new Array<String>();
            for (int i = 1; i < command.Length; i ++) args.Add(command[i].ToString());

            ItemScriptCommand newCommand = new ItemScriptCommand(command[0], args);
            
            switch (currentAction) {
                case Actions.Pickup: pickup.Add(newCommand); break;
                case Actions.Use: use.Add(newCommand); break;
                case Actions.Drop: drop.Add(newCommand); break;
            }
            CommandList it = new CommandList();
        }
        
        return new ItemScriptActions(pickup, use, drop);
    }

    private static bool CheckCommandSyntax(String[] command) {
        bool isCommandOk = actionDictionary.ContainsKey(command[0]);

        if (isCommandOk) {
            Array<String> args = actionDictionary[command[0]];
            if (args is null) return true;
            if (args.Contains(command[1])) return true;
        }
        return false;
    }
    
    /// <summary> Returns <c>true</c> if one of actions was purge (delete item), otherwise <c>false</c>. </summary>
    public bool PerformAction(ItemScriptParser.Actions action, ItemPawn itemPawn) {
        if (!itemPawn.ParsedScript.CheckAction(action)) return false;
        ItemScriptActions actions = itemPawn.ParsedScript;
        CommandList list = null;
        switch (action) {
            case Actions.Pickup:
                list = actions.Pickup;
            break;
            case Actions.Use:
                list = actions.Use;
            break;
            case Actions.Drop:
                list = actions.Drop;
            break;
        }
        
        foreach (ItemScriptCommand command in list) {
            switch (command.Command) {
                case "inventory":
                    PerformInventory(command.Args);
                break;
                case "message":
                    PerformMessage(command.Args);
                break;
                case "heal":
                    PerformHeal(command.Args);
                break;
                case "damage":
                    PerformDamage(command.Args);
                break;
                case "note":
                    PerformNote(command.Args, itemPawn);
                break;
                case "ammo":
                    PerformAmmo(command.Args);
                break;
                case "purge":
                    return true;

            }
        }

        return false;
    }

    // inventory -action -name
    private void PerformInventory(Array<String> args) {
        if (args.Count < 2) return;
        String state = args[0];
        String name = args[1];
        switch (state) {
            case "enable":
                inventoryManager.InventoryEnable(name);
                return;
            case "disable":
                inventoryManager.InventoryDisable(name);
                return;
            case "toggle":
                inventoryManager.InventoryToggle(name);
                return;
            case "drop":
                inventoryManager.InventoryDrop(name);
                return;
        }
    }

    // message -text
    private void PerformMessage(Array<String> args) {
        if (args.Count == 0) return;
        String output = "";
        for (int i = 0; i < args.Count; i ++) {
            output += args[i];
            if (i != args.Count - 1) output += " ";
        }
        ////////////////// ADD MESSAGE //////////////////
    }
    
    // heal -attribute -value
    private void PerformHeal(Array<String> args) {
        if (args.Count < 2) return;
        int i = 0;
        int.TryParse(args[1], out i);
        player.StatsAddValue(args[0], i);
    }
    
    // damage -attribute -value
    private void PerformDamage(Array<String> args) {
        if (args.Count < 2) return;
        int i = 0;
        int.TryParse(args[1], out i);
        player.StatsAddValue(args[0], -i);
    }
    
    // note -type
    private void PerformNote(Array<String> args, ItemPawn itemPawn) {
        //////////////// ADD NOTE ///////////////////
    }
    
    // ammo -type -amount
    private void PerformAmmo(Array<String> args) {
        if (args.Count < 2) return;
        int i = 0;
        int.TryParse(args[1], out i);
        player.AmmoAddValue(args[0], i);
    }
}
