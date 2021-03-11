using System;
using Godot;

public static class GameHelper {
    public static Godot.Collections.Array GetNodesInGroup(Node caller, String group) {
        Godot.Collections.Array arr = caller.GetTree().GetNodesInGroup(group);
        if (arr.Count > 0) {
            return arr;
        }
        return null;
    }
    
    public static T GetNodeInGroup<T>(Node caller, String group) {
        Godot.Collections.Array arr = caller.GetTree().GetNodesInGroup(group);
        foreach (Node node in arr) {
            if (node is T result) {
                return result;
            }
        }
        return default(T);
    }

    /// <summary>Returns mouse position on screen</summary>
    public static Vector2 GetMousePos(CanvasItem caller) {
        return caller.GetViewport().GetMousePosition() / GetViewScale();
    }

    /// <summary>Returns global mouse position in scene</summary>
    public static Vector2 GetMousePosScene(CanvasItem caller) {
        Camera2D cam = GameHelper.GetNodeInGroup<Camera2D>(caller, "Player");
        return GetMousePos(caller) + cam.GlobalPosition - new Vector2(320, 180) / 2;
        // return caller.GetGlobalMousePosition() / GetViewScale();
    }

    public static Vector2 GetViewScale() {
        return OS.WindowSize / new Vector2(320, 180);
    }
}