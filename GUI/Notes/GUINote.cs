using Godot;
using System;

public class GUINote : Control { 
    [Export]
    public int maxLines = 13;

    private Sprite noteTexture;
    private Label currentPage;
    private Button buttonNext;
    private Button buttonPrev;
    private RichTextLabel noteText;

    public override void _Ready() {

    }

	public override void _Process(float delta) {

	}
}