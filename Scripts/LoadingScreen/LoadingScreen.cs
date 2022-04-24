using Godot;
using System;

[Tool]
public class LoadingScreen : Control { 

    [Export(PropertyHint.Range,"0,100")]
    public int value = 0;

    private ProgressBar bar;
    private Label persent;

    public override void _Ready() {
        persent = GetNode<Label>("Percent");
        bar = GetNode<ProgressBar>("ProgressBar");
    }

	public override void _Process(float delta) {
        persent.Text = GD.Str(value, "%");
        bar.Value = value;
	}
}