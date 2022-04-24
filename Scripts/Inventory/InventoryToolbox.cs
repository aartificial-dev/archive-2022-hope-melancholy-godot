using Godot;
using System;
using ItemArray = Godot.Collections.Array<ItemInventory>;

[Tool]
public class InventoryToolbox : Inventory {
    [Export]
    public AudioStreamSample itemCombineSound = default(AudioStreamSample);
    private AudioStreamPlayer combineSoundPlayer;
    
    private AudioStreamPlayer toolBoxOpenPlayer;
    private AudioStreamPlayer toolBoxClosePlayer;

    private TextureButton combineButton;

    public override void _Ready() {
        base._Ready();

        toolBoxClosePlayer = GetNode<AudioStreamPlayer>("ToolBoxClosePlayer");
        toolBoxOpenPlayer = GetNode<AudioStreamPlayer>("ToolBoxOpenPlayer");

        combineButton = GetNode<TextureButton>("TextureButton");
        combineButton.Connect("pressed", this, nameof(CombineItems));
    
        combineSoundPlayer = new AudioStreamPlayer();
        combineSoundPlayer.Stream = itemCombineSound;
        this.AddChild(combineSoundPlayer);
    }

    public override void _Process(float delta) {
        base._Process(delta);

        ////////////////// MAKE ITEM DROP WHEN TOOLBOX NOT IN INVENTORY ////////////////////
    }

    public void CombineItems() {
        combineSoundPlayer.Play();
    }

    public void PlayOpenSound() {
        toolBoxOpenPlayer.Play();
    }

    public void PlayCloseSound() {
        toolBoxClosePlayer.Play();
    }

    protected override void UpdateGrid() {
        name = "toolbox";
        gridSize = new Vector2(5, 3);
        cellSize = new Vector2(11, 11);
        cellOffset = new Vector2(11, 0);
        blacklistType = ItemPawn.ItemType.Chip;
        base.UpdateGrid();
    }
}