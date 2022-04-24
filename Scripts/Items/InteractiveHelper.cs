using System;
using Godot;

public class InteractiveHelper<T> : Godot.Object where T : Godot.Node2D, InteractiveObject {
    private bool MouseIn {set; get;}

    private Player player = null;
    private PlayerCamera camera = null;

    private T baseClass;
    private String hintName = "";
    private PlayerCamera.InteractHintIcon icon = PlayerCamera.InteractHintIcon.hand;
    private Vector2 positionOffset = Vector2.Zero;


    public InteractiveHelper(T baseClass, String hintName, PlayerCamera.InteractHintIcon icon, Vector2 positionOffset) {
        this.baseClass = baseClass;
        this.hintName = hintName;
        this.icon = icon;
        this.positionOffset = positionOffset;
    }

    public void CheckHint() {
        if (MouseIn) {
            FindCamera();
            FindPlayer();
            if (! (player is null || camera is null) ) {
                ShowInteractHint();
            }
        }
    }

    public void ShowInteractHint() {
        bool interactDistance = false;
        if (!(player is null)) {
            interactDistance = (baseClass.GlobalPosition + positionOffset).DistanceTo(player.GlobalPosition) <= player.interactDistance;
        }
        if (interactDistance) {
            camera.ShowInteractHint(hintName, icon, baseClass.GlobalPosition + positionOffset, baseClass);
        }
    }

    public void MouseEvent(Node viewport, InputEvent inputEvent, int shapeidx) {
        if (Engine.EditorHint) return;
        if (inputEvent.IsActionReleased("mb_right")) {
            FindCamera();
            FindPlayer();
            if (! (player is null || camera is null) ) {
                baseClass.Use();
            }
        }
    }

    public void MouseEntered() {
        MouseIn = true;
    }
    public void MouseExited() {
        MouseIn = false;
    }
    
    private Player FindPlayer() {
        if (!(player is null)) return player;
        player = GameHelper.GetNodeInGroup<Player>(baseClass, "Player");
        return player;
    }

    private PlayerCamera FindCamera() {
        if (!(camera is null)) return camera;
        camera = GameHelper.GetNodeInGroup<PlayerCamera>(baseClass, "Player");
        return camera;
    }

    public void ChangeHintName(String hint) {
        hintName = hint;
    }

    public void ChangeHintIcon(PlayerCamera.InteractHintIcon icon) {
        this.icon = icon;
    }
}