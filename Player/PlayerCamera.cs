using Godot;
using System;

public class PlayerCamera : Camera2D {
    private Vector2 actualPos;

    private Player player;

    private Node2D interactHint;
    private Vector2 interactPos;
    private AnimatedSprite interactAnimation;
    private Label interactLabel;
    private Timer interactTimer;

    private PlayerFOV playerFOV;
  
    public ItemInventory selectedWeapon = null;
    
    private int selectedWeaponSlot = 0;

    private Vector2 resolution = new Vector2(320f, 180f);

    private GUI gui;
    private AudioStreamPlayer guiSoundPlayer = new AudioStreamPlayer();
    private AudioStreamSample guiSound = ResourceLoader.Load<AudioStreamSample>("res://Sounds/snd_gui_open.wav");

    private Vector2 positionPrevious = Vector2.Zero;

    public Node2D interactNode = null;

    public enum InteractHintIcon {
        eye, gear, hand, mouth
    }

    public override void _Ready() {
        this.Visible = true;
        actualPos = this.GlobalPosition;
        player = GameHelper.GetNodeInGroup<Player>(this, "Player");
        if (!(player is null)) {
            player.camera = this;
        }

        interactHint = GetNode<Node2D>("InteractHint");
        interactTimer = GetNode<Timer>("InteractHint/Timer");
        interactAnimation = GetNode<AnimatedSprite>("InteractHint/AnimatedSprite");
        interactLabel = GetNode<Label>("InteractHint/Label");

        interactTimer.Connect("timeout", this, nameof(HideIneractHint));
        
        Color col = interactHint.Modulate;
        col.a = 0f;
        interactHint.Modulate = col;

        gui = GetNode<GUI>("CanvasLayer/GUI");
        gui.SetPlayer(player);

        this.AddChild(guiSoundPlayer);
        guiSoundPlayer.Stream = guiSound;

        playerFOV = GetNode<PlayerFOV>("FOV");
        playerFOV.occluderArr = GameHelper.GetNodesInGroup(this, "OccluderFOV");
        playerFOV.position = player;
        
    }

    public override void _Process(float delta) {
        // ProcessWeapons(delta);
        if (Input.IsActionJustPressed("key_fullscreen")) {
            OS.WindowFullscreen = !OS.WindowFullscreen;
        }

        if (!player.GetCanMove()) return;
        selectedWeapon = gui.GetWeapon(selectedWeaponSlot);
    }

    public override void _Input(InputEvent @event) {
        if (!player.GetCanMove()) return;
        if (IsInventoryOpen()) return;
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed) {
            if ((ButtonList) mouseEvent.ButtonIndex == ButtonList.WheelUp) {
                selectedWeaponSlot ++;
            }
            if ((ButtonList) mouseEvent.ButtonIndex == ButtonList.WheelDown) {
                selectedWeaponSlot --;
            }
            selectedWeaponSlot = Mathf.Wrap(selectedWeaponSlot, 0, 2);
        }         
    }

    public void UpdateCamera(float delta) {
        if (player is null) return;
        if (player.GetIsInCutscene()) return;
        
        float h = resolution.y / 4f;
        Vector2 mousePos = GameHelper.GetMousePosScene(this);

        actualPos.x = Mathf.Lerp(actualPos.x, player.Transform.origin.x, delta * 4f);
        actualPos.x = Mathf.Lerp(actualPos.x, mousePos.x, delta * 0.4f);
        actualPos.y = Mathf.Lerp(actualPos.y, player.Transform.origin.y - h, delta * 4f);

        Transform2D _tr = this.Transform;
        _tr.origin = actualPos;//.Round();
        this.Transform = _tr;
        this.ForceUpdateScroll();

        if (interactTimer.TimeLeft > 0f) {
            Color col = interactHint.Modulate;
            if (interactTimer.TimeLeft > 0.45f) {
                col.a = Mathf.Lerp(col.a, interactTimer.TimeLeft / 0.5f, delta * 2f);
            } else {
                col.a = interactTimer.TimeLeft / 0.5f;
            }
            interactHint.Modulate = col;
            interactHint.GlobalPosition = interactPos;
        }

        //if (positionPrevious != GlobalPosition && !(playerFOV.occluderArr is null)) {
            playerFOV.Update();
        //}
        positionPrevious = GlobalPosition;
    }

    public bool PickFloorItem(ItemFloor item) {
        if (!GetCanMove()) return false;
        bool pick = gui.PickFloorItem(item);
        if (pick) interactNode = null;
        return pick;
    }

    public void ShowInteractHint(String name, PlayerCamera.InteractHintIcon icon, Vector2 pos, Node2D node) {
        interactTimer.Start(0.5f);
        interactHint.Visible = true;
        interactLabel.Text = name;
        interactLabel.Hide();
        interactLabel.Show();
        if (interactAnimation.Animation != icon.ToString()) {
            interactAnimation.Animation = icon.ToString();
        }
        interactPos = pos;
        interactHint.GlobalPosition = interactPos; 
        interactNode = node;
    }

    public void HideIneractHint() {
        interactHint.Visible = false;
    }

    public bool IsCursorOnInventory() {
        return false;//inventoryGUI.IsCursorOnInventory();
    }

    public bool GetCanMove() {
        return player.GetCanMove();
    }

    public ItemInventory GetItemByID(String id) {
        return gui.GetItemByID(id);
    }

    public void RemoveItem(String id) {
        gui.RemoveItem(id);
    }

    public bool IsInventoryOpen() {
        return gui.Visible;
    }

    public void SetGUIVisible(bool value) {
        gui.Visible = value;
        //gui.GetNode<ColorRect>("CRTEffect").Visible = value;
        gui.InventoryDrop("toolbox");
        guiSoundPlayer.Play();
    }

    public bool GetGUIVisible() {
        return gui.Visible;
    }

    public ItemPawn GetUsableItem() {
        return gui.GetUsableItem();
    }

    public void UseUsableItem() {
        gui.UseUsableItem();
    }

    public Player GetPlayer() {
        return player;
    }
}