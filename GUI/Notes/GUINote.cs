using Godot;
using System;

public class GUINote : Control { 
    [Export]
    public int maxLines = 13;
    [Export(PropertyHint.MultilineText)]
    public String text = "";
    [Export]
    public NoteType type = NoteType.Note;

    private int currentPage = 0;
    private int currentLine = 0;

    public enum NoteType {
        Note, News, Book
    }

    private Sprite noteTexture;
    private Label currentPageLabel;
    private Button buttonNext;
    private Button buttonPrev;
    private RichTextLabel noteText;

    public override void _Ready() {
        noteTexture = GetNode<Sprite>("CenterContainer/NoteTexture");
        currentPageLabel = GetNode<Label>("CenterContainer/NoteTexture/Control/Label");
        buttonNext = GetNode<Button>("CenterContainer/NoteTexture/Control/ButtonNext");
        buttonPrev = GetNode<Button>("CenterContainer/NoteTexture/Control/ButtonPrev");
        noteText = GetNode<RichTextLabel>("CenterContainer/NoteTexture/Control/MarginContainer/NoteText");

        buttonNext.Connect("pressed", this, nameof(NextPage));
        buttonPrev.Connect("pressed", this, nameof(PrevPage));

        // noteTexture.Frame = (int) type;
        noteText.BbcodeText = text;
    }

	public override void _Process(float _delta) {
        currentPageLabel.Text = (currentPage + 1).ToString();
	}

    public void NextPage() {
        int lines = noteText.GetVisibleLineCount();
        currentLine += lines;
        currentPage ++;
        noteText.ScrollToLine(currentLine);
        GD.Print(currentLine);
    }

    public void PrevPage() {
        int lines = noteText.GetVisibleLineCount();
        currentPage --;
        currentLine -= lines;
        currentPage = Mathf.Max(0, currentPage);
        currentLine = Mathf.Max(0, currentLine);
        noteText.ScrollToLine(currentLine);
    }
}