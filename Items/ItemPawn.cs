using Godot;
using System;
using IntArray = Godot.Collections.Array;

public class ItemPawn {
    private String name;
    private ItemPawn.ItemType type;
    private String id;

    private SpriteFrames spriteFloorFrames;
    private int spriteFloorFrame;
    private Vector2 spriteFloorSize;

    private SpriteFrames spriteInventoryFrames;
    private int spriteInventoryFrame;
    private Vector2 spriteInventorySize;
    private Vector2 spriteInventoryGridSize;

    private int ammo;
    private int ammoMax;
    private bool isActive;

    private Mesh model;
    private SpatialMaterial material;
    private String description;

    private String itemScript;
    private AudioStreamSample audioPickup;
    private AudioStreamSample audioUse;
    private AudioStreamSample audioDrop;
    private AudioStreamSample audioActivate;

    private ItemScriptParser.ItemScriptActions parsedScript;

    private bool isRotated = false;

    public string Name { get => name; }
    public ItemType Type { get => type; }
    public string Id { get => id; }

    public SpriteFrames SpriteFloorFrames { get => spriteFloorFrames; }
    public int SpriteFloorFrame { get => spriteFloorFrame; }
    public Vector2 SpriteFloorSize { get => spriteFloorSize; }

    public SpriteFrames SpriteInventoryFrames { get => spriteInventoryFrames; }
    public int SpriteInventoryFrame { get => spriteInventoryFrame; }
    public Vector2 SpriteInventorySize { get => GetSpriteInventorySize(); }
    // public Vector2 SpriteInventorySizeAA { get => spriteInventorySize; } //////////////////////////////////////////////////////////////
    public Vector2 SpriteInventoryGridSize { get => GetSpriteInventoryGridSize(); }

    public int Ammo { get => ammo; set => ammo = value; }
    public int AmmoMax { get => ammoMax; }
    public bool IsActive {get => isActive; set => isActive = value; }

    public Mesh Model { get => model; }
    public SpatialMaterial Material { get => material; }
    public string Description { get => description; }

    public string ItemScript { get => itemScript; }
    public AudioStreamSample AudioPickup { get => audioPickup; }
    public AudioStreamSample AudioUse { get => audioUse; }
    public AudioStreamSample AudioDrop { get => audioDrop; }
    public AudioStreamSample AudioActivate { get => audioActivate; }

    public ItemScriptParser.ItemScriptActions ParsedScript { get => parsedScript; }
    public bool IsRotated { get => isRotated; }

    public enum ItemType {
        Any, None, Weapon, Quest, Grabage, Chip, Ammo, Medicine, Notes, Tool, Usable
    }
    
    public ItemPawn(string name, ItemType type, string id, 
                    SpriteFrames spriteFloorFrames, int spriteFloorFrame, Vector2 spriteFloorSize, 
                    SpriteFrames spriteInventoryFrames, int spriteInventoryFrame, Vector2 spriteInventorySize, Vector2 spriteInventoryGridSize, 
                    int ammo, int ammoMax, bool isActive,
                    Mesh model, SpatialMaterial material, string description, 
                    string itemScript, AudioStreamSample audioPickup, AudioStreamSample audioUse, AudioStreamSample audioDrop, AudioStreamSample audioActivate) {
        this.name = name;
        this.type = type;
        this.id = id;
        this.spriteFloorFrames = spriteFloorFrames;
        this.spriteFloorFrame = spriteFloorFrame;
        this.spriteFloorSize = spriteFloorSize;
        this.spriteInventoryFrames = spriteInventoryFrames;
        this.spriteInventoryFrame = spriteInventoryFrame;
        this.spriteInventorySize = spriteInventorySize;
        this.spriteInventoryGridSize = spriteInventoryGridSize;
        this.ammo = ammo;
        this.ammoMax = ammoMax;
        this.isActive = isActive;
        this.model = model;
        this.material = material;
        this.description = description;
        this.itemScript = itemScript;
        this.audioPickup = audioPickup;
        this.audioUse = audioUse;
        this.audioDrop = audioDrop;
        this.audioActivate = audioActivate;
    }

    public static ItemPawn MakePawnFromGD(Resource res) { 

        String name = (String) res.Get("name");
        ItemPawn.ItemType type = (ItemPawn.ItemType) Enum.ToObject(typeof(ItemPawn.ItemType) , (int) res.Get("itemType"));
        String id= (String) res.Get("itemID");

        SpriteFrames spriteFloorFrames = (SpriteFrames) res.Get("spriteFloorFrames");
        int spriteFloorFrame = (int) res.Get("spriteFloorFrame");
        Vector2 spriteFloorSize = (Vector2) res.Get("spriteFloorSize");

        SpriteFrames spriteInventoryFrames = (SpriteFrames) res.Get("spriteInventoryFrames");
        int spriteInventoryFrame = (int) res.Get("spriteInventoryFrame");
        Vector2 spriteInventorySize = (Vector2) res.Get("spriteInventorySize");
        Vector2 spriteInventoryGridSize = (Vector2) res.Get("spriteInventoryGridSize");

        int ammo = (int) res.Get("ammo");
        int ammoMax = (int) res.Get("ammoMax");
        bool isActive = (bool) res.Get("isActive");

        Mesh model = (Mesh) res.Get("model");
        SpatialMaterial material = (SpatialMaterial) res.Get("material");
        String description = (String) res.Get("description");

        String itemScript = (String) res.Get("itemScript");
        AudioStreamSample audioPickup = (AudioStreamSample) res.Get("audioPickup");
        AudioStreamSample audioUse = (AudioStreamSample) res.Get("audioUse");
        AudioStreamSample audioDrop = (AudioStreamSample) res.Get("audioDrop");
        AudioStreamSample audioActivate = (AudioStreamSample) res.Get("audioActivate");
        // GD.Print(name, "  ", spriteFrame, "  ", sizeFloor, "  ", sizeGrid, "  ", sizeSprite, "  ", type, "  ", intArray, "  ", textField);
        return new ItemPawn(name, type, id, 
                            spriteFloorFrames, spriteFloorFrame, spriteFloorSize, 
                            spriteInventoryFrames, spriteInventoryFrame, spriteInventorySize, spriteInventoryGridSize, 
                            ammo, ammoMax, isActive,
                            model, material, description,
                            itemScript, audioPickup, audioUse, audioDrop, audioActivate);
    }

    private Vector2 GetSpriteInventorySize() {
        if (isRotated) {
            return new Vector2(spriteInventorySize.y, spriteInventorySize.x);
        }
        return spriteInventorySize;
    }

    private Vector2 GetSpriteInventoryGridSize() {
        if (isRotated) {
            return new Vector2(spriteInventoryGridSize.y, spriteInventoryGridSize.x);
        }
        return spriteInventoryGridSize;
    }

    public void Rotate() {
        isRotated = !isRotated;
    }

    public void ParseActions() {
        parsedScript = ItemScriptParser.ParseActions(itemScript);
    }
}
