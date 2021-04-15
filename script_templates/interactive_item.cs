using Godot;
using System;

public class %CLASS% : %BASE%, InteractiveObject { 

    private InteractiveHelper<%CLASS%> helper;

    public override void _Ready() {
        helper = new InteractiveHelper<%CLASS%>(this, "TEMPLATE_NAME", PlayerCamera.InteractHintIcon.gear);
        AREANODE.Connect("input_event", helper, nameof(helper.MouseEvent));
        AREANODE.Connect("mouse_entered", helper, nameof(helper.MouseEntered));
        AREANODE.Connect("mouse_exited", helper, nameof(helper.MouseExited));
    }
    
    public override void _Process(float delta) {
        if (Engine.EditorHint) return;
        helper.CheckHint();
    }

    public void Use() {
        // USE EVENT
    }
}