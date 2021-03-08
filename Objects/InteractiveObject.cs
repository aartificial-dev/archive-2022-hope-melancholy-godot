using System;

public interface InteractiveObject {
    bool MouseIn {set; get;}
    void Use();
    void MouseEntered();
    void MouseExited();
    void MouseEvent(Godot.Node viewport, Godot.InputEvent inputEvent, int shapeidx);
    void ShowInteractHint();
}
