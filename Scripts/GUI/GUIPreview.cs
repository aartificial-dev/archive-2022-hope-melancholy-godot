using Godot;
using System;

public class GUIPreview : TextureRect { 

    [Export]
    public int visibleLinesCount = 6;

    private GUI gui;

    private TextureRect noItemScreen;
    private ColorRect description;
    private Label descriptionText;
    private CSGMesh itemModel;
    private SpatialMaterial itemDefaultMaterial = ResourceLoader.Load<SpatialMaterial>("res://Models/defaultModelMaterial.tres");

    private AudioStreamPlayer textAudioPlayer = new AudioStreamPlayer();
    private AudioStreamSample textAudioSample = ResourceLoader.Load<AudioStreamSample>("res://Sounds/snd_gui_preview_text.wav");

    public override void _Ready() {
        gui = this.GetParent<GUI>();

        noItemScreen = GetNode<TextureRect>("NoItemScreen");
        description = GetNode<ColorRect>("Description");
        descriptionText = GetNode<Label>("Description/Text");
        itemModel = GetNode<CSGMesh>("Viewport/Spatial/ItemModel");

        this.AddChild(textAudioPlayer);
        textAudioPlayer.Stream = textAudioSample;
    }

	public override void _Process(float delta) {
        ItemPawn item = gui.GetItemUnderCursor();
        ItemPawn hand = gui.GetItemInHand();
        if (item is null || !(hand is null)) {
            description.Visible = false;
            noItemScreen.Visible = true;
            descriptionText.VisibleCharacters = 0;
            descriptionText.LinesSkipped = 0;
        } else {
            ShowModel(item);
            ShowDescription(item);
        }
	}

    private void ShowModel(ItemPawn itemPawn) {
        Mesh model = itemPawn.Model;
        if (model is null) {
            noItemScreen.Visible = true;
            return;
        }
        noItemScreen.Visible = false;
        itemModel.Mesh = model;
        
        SpatialMaterial mat = itemPawn.Material;
        if (mat is null) {
            itemModel.Material = itemDefaultMaterial;
        } else {
            itemModel.Material = mat;
        }
    }

    private void ShowDescription(ItemPawn itemPawn) {
        String descr = itemPawn.Description;
        bool doShowDescription = (descr.Length > 0 || itemPawn.Type == ItemPawn.ItemType.Weapon || itemPawn.Type == ItemPawn.ItemType.Usable);
        if (doShowDescription) {
            gui.AddHintFlag((uint) GUI.HintFlags.Description);
        }

        if (Input.IsActionPressed("key_description") && gui.Visible && doShowDescription) {
            description.Visible = true;
            String des = descr + GenerateBottomLine(itemPawn, descr);
            if (!descriptionText.Text.StartsWith(descr)) {
                descriptionText.VisibleCharacters = 0;
            }
            descriptionText.Text = des;
            if (descriptionText.VisibleCharacters < des.Length && des.Length != 0) {
                descriptionText.VisibleCharacters = descriptionText.VisibleCharacters + 2;
                textAudioPlayer.Play();
            }
            if (descriptionText.GetLineCount() > visibleLinesCount) {
                gui.AddHintFlag((uint) GUI.HintFlags.Scroll);
            }
        } else {
            description.Visible = false;
            descriptionText.VisibleCharacters = 0;
            descriptionText.LinesSkipped = 0;
        }
    }

    public override void _Input(InputEvent @event) {
        if (!description.Visible) { return; }
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed) {
            int lines = descriptionText.LinesSkipped;
            if ((ButtonList) mouseEvent.ButtonIndex == ButtonList.WheelUp) {
                lines --;
            }
            if ((ButtonList) mouseEvent.ButtonIndex == ButtonList.WheelDown) {
                lines ++;
            }

            lines = Mathf.Clamp(lines, 0, Mathf.Max(0, descriptionText.GetLineCount() - visibleLinesCount));
            descriptionText.LinesSkipped = lines;
        }         
    }

    private String GenerateBottomLine(ItemPawn itemPawn, String referenceText) {
        String newline = (referenceText.Length > 0) ? "\n" : "";
        switch (itemPawn.Id) {
            case "w_handgun":
                return GD.Str(newline, "Ammo: ", itemPawn.Ammo, "/", itemPawn.AmmoMax);
            case "w_flashlight":
                return GD.Str(newline, "Charge: ", Mathf.CeilToInt(( (float)itemPawn.Ammo / (float)itemPawn.AmmoMax ) * 100), "%");
            case "w_lamp":
                return GD.Str(newline, "Charge: ", Mathf.CeilToInt(( (float)itemPawn.Ammo / (float)itemPawn.AmmoMax ) * 100), "%");
            case "u_flare_pack":
                return GD.Str(newline, "Flares: ", itemPawn.Ammo, "/", itemPawn.AmmoMax);
        }
        return "";
    }
}