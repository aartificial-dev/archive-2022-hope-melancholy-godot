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
        ProcessWeapons(delta);
    }

    public override void _Input(InputEvent @event) {
        if (!player.GetCanMove()) return;
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

    public void ProcessWeapons(float delta) {
        inventoryGUI.SetWeaponSlot(selectedWeaponSlot);
        selectedWeapon = inventoryGUI.GetWeapon(selectedWeaponSlot);
        if (selectedWeapon is null) {
            weaponSprite.Frame = 0;
            SetWeaponLableVisible(false);
        } else if (selectedWeapon.itemPawn.AmmoMax != 0) {
            weaponSprite.Frame = 0;//selectedWeapon.itemPawn.guiFrame;

            String name = selectedWeapon.itemPawn.Name;
            int ammoCount = (int) selectedWeapon.itemPawn.Ammo;
            int actAmmo, maxAmmo;
            switch (name) {
                case "Handgun":
                    SetWeaponLabelText(ammoCount, player.ammoHandgun);
                break;
                case "Flashlight":
                    actAmmo = Mathf.CeilToInt(ammoCount / 1000);
                    maxAmmo = Mathf.CeilToInt(player.ammoBattery / 1000);
                    if (actAmmo == 0 && ammoCount > 0) actAmmo = 1;
                    if (maxAmmo == 0 && player.ammoBattery > 0) maxAmmo = 1;
                    SetWeaponLabelText(actAmmo, maxAmmo);
                break;
                case "Lamp":
                    actAmmo = Mathf.CeilToInt(ammoCount / 1000);
                    maxAmmo = Mathf.CeilToInt(player.ammoBattery / 1000);
                    if (actAmmo == 0 && ammoCount > 0) actAmmo = 1;
                    if (maxAmmo == 0 && player.ammoBattery > 0) maxAmmo = 1;
                    SetWeaponLabelText(actAmmo, maxAmmo);
                break;
                default:
                    SetWeaponLableVisible(false);
                break;
            }
        }
    }

    public bool IsCursorOnInventory() {
        return inventoryGUI.IsCursorOnInventory();
    }

    private void SetWeaponLabelText(int actual, int max) {
        SetWeaponLableVisible(true);
        weaponAmmoLabel.Text = Mathf.Clamp(Mathf.Ceil(actual), 0, 99).ToString();
        weaponAmmoMaxLabel.Text = Mathf.Clamp(Mathf.Ceil(max), 0, 99).ToString();
        if (weaponAmmoLabel.Text.Length == 1) {
            weaponAmmoLabel.Text = "0" + weaponAmmoLabel.Text;
        }
        if (weaponAmmoMaxLabel.Text.Length == 1) {
            weaponAmmoMaxLabel.Text = "0" + weaponAmmoMaxLabel.Text;
        }
    }

    private void SetWeaponLableVisible(bool visible) {
        weaponAmmoLabel.Visible = visible;
        weaponDivLabel.Visible = visible;
        weaponAmmoMaxLabel.Visible = visible;
    }

    public bool GetCanMove() {
        return player.GetCanMove();
    }
}