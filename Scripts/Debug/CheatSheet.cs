using Godot;
using System;

public class CheatSheet : Control { 

    private Label debugLabelDelta;
    private Label debugLabelFPS;
    private Label debugLabelMemory;

    public override void _Ready() {
        GetNode<Button>("HBoxContainer/LightButton").Connect("pressed", this, nameof(ToggleLightSystem));
        GetNode<Button>("HBoxContainer/Debug").Connect("pressed", this, nameof(ShowDebugPanel));

        debugLabelDelta = GetNode<Label>("HBoxContainer/Debug/Panel/VBoxContainer/delta");
        debugLabelFPS = GetNode<Label>("HBoxContainer/Debug/Panel/VBoxContainer/fps");
        debugLabelMemory = GetNode<Label>("HBoxContainer/Debug/Panel/VBoxContainer/memory");
    }

	public override void _Process(float delta) {
        if (Input.IsActionJustPressed("key_cheats") && OS.IsDebugBuild()) {
            this.Visible = !this.Visible;
        }
        if (!this.Visible) return;

        debugLabelDelta.Text = GD.Str("delta: ", delta.ToString().Substring(0, Mathf.Min(6, delta.ToString().Length)));
        debugLabelFPS.Text = GD.Str("fps: ", Engine.GetFramesPerSecond(), "/", Engine.TargetFps);
        debugLabelMemory.Text = GD.Str("mem: ",  BytesToString(OS.GetStaticMemoryUsage()));

	}

    public void ShowDebugPanel() {
        Panel panel = GetNode<Panel>("HBoxContainer/Debug/Panel");
        panel.Visible = !panel.Visible;
    }

    public void ToggleLightSystem() {
        Godot.Collections.Array arr = this.GetTree().GetNodesInGroup("LightHolder");
        foreach (Node node in arr) {
            if (node is Node2D holder) {
                holder.Visible = !holder.Visible;
                break;
            }
        }
    }

    private String BytesToString(ulong byteCount){
        String[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (byteCount == 0)
            return "0" + suf[0];
        int place = Convert.ToInt32(Math.Floor(Math.Log(byteCount, 1024)));
        double num = Math.Round(byteCount / Math.Pow(1024, place), 1);
        return num.ToString() + suf[place];
    }
}