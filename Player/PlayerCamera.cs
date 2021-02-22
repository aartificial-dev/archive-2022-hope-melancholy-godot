using Godot;
using System;

public class PlayerCamera : Camera2D {
    
    [Export]
    public NodePath playerPath = "../Player";

    private Player player;
    private InventoryGUI inventoryGUI;

    private Node2D interactHint;
    private Vector2 interactPos;
    private AnimatedSprite interactAnimation;
    private Label interactLabel;
    private Timer interactTimer;

    private AnimatedSprite weaponSprite;

    public ItemInventory selectedWeapon = null;
    
    private int selectedWeaponSlot = 0;
    private int weaponSlotAmount = 2;
    
    private Label weaponAmmoLabel;
    private Label weaponDivLabel;
    private Label weaponAmmoMaxLabel;

    public enum InteractHintIcon {
        eye, gear, hand, mouth
    }

    public override void _Ready() {
        this.Visible = true;
        player = GetNodeOrNull<Player>(playerPath);
        if (!(player is null)) {
            player.camera = this;
        }

        inventoryGUI = GetNode<InventoryGUI>("Inventory");
        interactHint = GetNode<Node2D>("InteractHint");
        interactTimer = GetNode<Timer>("InteractHint/Timer");
        interactAnimation = GetNode<AnimatedSprite>("InteractHint/AnimatedSprite");
        interactLabel = GetNode<Label>("InteractHint/Label");

        interactTimer.Connect("timeout", this, nameof(HideIneractHint));
        
        inventoryGUI.player = player;

        weaponSprite = GetNode<AnimatedSprite>("Weapons/SpriteWeapon");
        weaponAmmoLabel = GetNode<Label>("Weapons/LabelAmmo");
        weaponDivLabel = GetNode<Label>("Weapons/LabelDivider");
        weaponAmmoMaxLabel = GetNode<Label>("Weapons/LabelAmmoMax");

        Color col = interactHint.Modulate;
        col.a = 0f;
        interactHint.Modulate = col;
    }

    public override void _Process(float delta) {
        GetNode<Label>("Label").Text = Engine.GetFramesPerSecond().ToString();
        ProcessWeapons();
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed) {
            if ((ButtonList) mouseEvent.ButtonIndex == ButtonList.WheelUp) {
                selectedWeaponSlot ++;
            }
            if ((ButtonList) mouseEvent.ButtonIndex == ButtonList.WheelDown) {
                selectedWeaponSlot --;
            }
            selectedWeaponSlot = Mathf.Wrap(selectedWeaponSlot, 0, weaponSlotAmount);
        }        
    }

    public void UpdateCamera(float delta) {
        if (player is null) return;
        
        Transform2D _tr = this.Transform;
        Vector2 pos = _tr.origin;

        // let _h = ( view_h / 2) * 0.75;
        // x = lerp(x, target.xprevious, 0.05);
        // x = lerp(x, mouse_x, 0.005);
        // //x = clamp(x, 0 +  (view_w / 2), room_width -  (view_w / 2));

        // y = lerp(y, target.y - _h, 0.05);

        float h = ((this.GetViewport().Size.y * 0.25f) / 2f) * 0.5f;
        Vector2 mousePos = this.GetGlobalMousePosition();

        pos.x = Mathf.Lerp(pos.x, player.Transform.origin.x, delta * 4f);
        pos.x = Mathf.Lerp(pos.x, mousePos.x, delta * 0.4f);
        pos.y = Mathf.Lerp(pos.y, player.Transform.origin.y - h, delta * 4f);

        _tr.origin = pos;
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
    }

    public bool PickFloorItem(ItemFloor item) {
        return inventoryGUI.PickFloorItem(item);
    }

    public void ShowInteractHint(String name, PlayerCamera.InteractHintIcon icon, Vector2 pos) {
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
    }

    public void HideIneractHint() {
        interactHint.Visible = false;
    }

    public void ProcessWeapons() {
        inventoryGUI.SetWeaponSlot(selectedWeaponSlot);
        selectedWeapon = inventoryGUI.GetWeapon(selectedWeaponSlot);
        if (selectedWeapon is null) {
            weaponSprite.Frame = 0;
            weaponAmmoLabel.Visible = false;
            weaponDivLabel.Visible = false;
            weaponAmmoMaxLabel.Visible = false;
        } else {
            weaponSprite.Frame = ItemList.GetWeaponFrame(selectedWeapon);

            if (selectedWeapon.itemType == ItemList.Items.handgun) {
                weaponAmmoLabel.Visible = true;
                weaponDivLabel.Visible = true;
                weaponAmmoMaxLabel.Visible = true;
            } else {
                weaponAmmoLabel.Visible = false;
                weaponDivLabel.Visible = false;
                weaponAmmoMaxLabel.Visible = false;
            }
        }
    }
}