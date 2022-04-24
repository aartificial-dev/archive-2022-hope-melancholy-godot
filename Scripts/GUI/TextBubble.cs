using Godot;
using System;

[Tool]
public class TextBubble : Node2D { 
    [Export(PropertyHint.MultilineText)]
    public String text = "";
    [Export(PropertyHint.Range, "20,300,1")]
    public float maxWidth = 173;
    [Export(PropertyHint.Range, "14,70,1")]
    public float maxHeight = 35;
    [Export]
    public float textHeight = 11;
    [Export]
    public float textWidth = 4.5f;
    [Export]
    public bool followCamera = false;
    [Export(PropertyHint.Range, "-300,300,1")]
    public float bubbleOffsetX = 0;
    [Export(PropertyHint.Range, "-300,300,1")]
    public float bubbleOffsetY = 0;
    [Export]//(PropertyHint.ResourceType, "AudioStreamSample")]
    public AudioStreamSample sound = ResourceLoader.Load<AudioStreamSample>("res://Sounds/snd_gui_preview_text.wav");
    [Export]
    public bool followObject = false;
    [Export]
    public NodePath followObjectPath = "";
    [Export]
    public bool showPromptInEnd = false;
    [Export]
    public float textAppearSpeed = 0.05f;
    [Export]
    public bool getAppearTime = false;
    
    private RandomNumberGenerator rnd = new RandomNumberGenerator();
    private AudioStreamPlayer2D audioStreamPlayer;

    private float minWidth = 20;
    private float minHeight = 14;

    private float sideMargin = 7f;

    private TextureRect bubbleArm;
    private NinePatchRect bubbleBack;
    private Label bubbleText;
    private Label continuePrompt;

    private Timer timer;

    private PlayerCamera camera = null;

    private Node2D objFollow;

    public override void _Ready() {
        bubbleArm = GetNode<TextureRect>("BubbleArm");
        bubbleBack = GetNode<NinePatchRect>("BubbleBack");
        bubbleText = GetNode<Label>("BubbleBack/BubbleText");
        audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        continuePrompt = GetNode<Label>("ContinuePrompt");

        timer = GetNode<Timer>("Timer");
        timer.Connect("timeout", this, nameof(AddChar));

        if (!Engine.EditorHint) {
            bubbleText.VisibleCharacters = 0;
            continuePrompt.Visible = false;
            audioStreamPlayer.Stream = sound;
            objFollow = GetNodeOrNull<Node2D>(followObjectPath);
        }
    }

	public override void _Process(float delta) {
        bubbleText.Text = text;
        String[] textArr = text.Split("\n");
        String longText = "";
        foreach (String t in textArr) {
            if (t.Length > longText.Length) {
                longText = t;
            }
        }

        float width = Mathf.Clamp((float) longText.Length * textWidth , minWidth, maxWidth);
        float height = Mathf.Clamp( (Mathf.CeilToInt(((float) longText.Length * textWidth ) / maxWidth) + text.Count("\n") ) * textHeight, minHeight, maxHeight);

        bubbleBack.RectSize = new Vector2(Mathf.FloorToInt(width), Mathf.FloorToInt(height));

        float posAdd = -7;
        if (!Engine.EditorHint) {
            if (!(FindCamera() is null) && followCamera) {
                posAdd = camera.GlobalPosition.x - (width / 2f) - this.GlobalPosition.x;
            }
        }
        posAdd += bubbleOffsetX;
        posAdd = Mathf.Clamp(posAdd, - width + sideMargin * 2f, -sideMargin);
        posAdd = Mathf.RoundToInt(posAdd);
        bubbleBack.RectPosition = new Vector2(posAdd, -Mathf.FloorToInt(height) - 6f);
        continuePrompt.RectPosition = new Vector2(bubbleBack.RectPosition.x + width - 7 - 32, bubbleBack.RectPosition.y - 6);

        if (!Engine.EditorHint) {
            if (!(objFollow is null) && followObject) {
                this.Position = new Vector2(objFollow.GlobalPosition.x, objFollow.GlobalPosition.y + bubbleOffsetY);
            }
        }

        if (!this.Visible) {
            bubbleText.VisibleCharacters = 0;
            continuePrompt.Visible = false;
        } else if (bubbleText.VisibleCharacters < text.Length && timer.TimeLeft == 0){
            timer.Start(textAppearSpeed);
        } else if (showPromptInEnd && bubbleText.VisibleCharacters >= text.Length) {
            continuePrompt.Visible = true;
        }

        if (getAppearTime) {
             getAppearTime = false;
             GD.Print("Appear time: ", CalculateApeearTime(), " sec.");
        }
	}

    private float CalculateApeearTime() {
        return textAppearSpeed * (float) text.Length;
    }

    public void AddChar() {
        bubbleText.VisibleCharacters += 1;
        if (!Engine.EditorHint && bubbleText.VisibleCharacters % 2 == 0) audioStreamPlayer.Play();
    }

    private PlayerCamera FindCamera() {
        if (!(camera is null)) return camera;
        Godot.Collections.Array arr = this.GetTree().GetNodesInGroup("Player");
        foreach (Node node in arr) {
            if (node is PlayerCamera) {
                camera = (PlayerCamera) node;
            }
        }
        return camera;
    }
}