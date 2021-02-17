using Godot;
using System;

public class PlayerCamera : Camera2D {
    
    [Export]
    public NodePath playerPath = "../Player";

    private Player player;

    public override void _Ready() {
        player = GetNodeOrNull<Player>(playerPath);
        if (!(player is null)) {
            player.camera = this;
        }
    }

    public override void _Process(float delta) {
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
    }
}