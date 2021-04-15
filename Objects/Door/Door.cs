using Godot;
using System;

[Tool]
public class Door : Area2D, InteractiveObject { 
	private bool isLocked = false;
	private bool isOpened = false;

	[Export]
	public NodePath connectDoor = "";
	public bool drawPath = false;
	[Export]
	public bool DrawPath {
		set { drawPath = value; this.Update(); UpdateDoorTarget();  }
		get { return drawPath; }
	}
	[Export]
	public Vector2 playerOffset = new Vector2(0f, 24f);

	private InteractiveHelper<Door> helper;

	private Door doorTarget = null;
	private Vector2 prevPos;

	private AudioStreamPlayer audioDoor;

	public override void _Ready() {
		if (!Engine.EditorHint) {
			helper = new InteractiveHelper<Door>(this, "Door", PlayerCamera.InteractHintIcon.gear, -playerOffset);
			this.Connect("input_event", helper, nameof(helper.MouseEvent));
			this.Connect("mouse_entered", helper, nameof(helper.MouseEntered));
			this.Connect("mouse_exited", helper, nameof(helper.MouseExited));

			audioDoor = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		}

		prevPos = this.GlobalPosition;
		UpdateDoorTarget();
	}
	
	public override void _Process(float delta) {
		if (Engine.EditorHint) {
			try {
			if (prevPos != this.GlobalPosition) {
				this.Update();
				if (!(doorTarget is null)) {
					doorTarget.Update();
				}
			}
			prevPos = this.GlobalPosition;
			} catch (Exception e) {
				e.ToString();
				GD.PrintErr(e.ToString());
			} finally {
				UpdateDoorTarget();
			}
			return;
		}
		helper.CheckHint();
	}

	public void Use() {
		// USE EVENT
		audioDoor.Play();
		Player player = FindPlayer();
		PlayerCamera camera = FindCamera();
		if (player is null || camera is null|| doorTarget is null) return;
		player.GlobalPosition = doorTarget.GlobalPosition - playerOffset;
		camera.ActualPosition = player.GlobalPosition;
		camera.GlobalPosition = camera.ActualPosition;
	}

	public override void _Draw() {
		if (Engine.EditorHint && drawPath && !(doorTarget is null)) {
			Vector2 ePos = doorTarget.GlobalPosition - this.GlobalPosition;
			float lr = -4f;
			if (ePos.y < 0) {
				lr = 4f;
			}
			Vector2 startPos = new Vector2(lr, 0) - playerOffset;
			Vector2 endPos = doorTarget.GlobalPosition - this.GlobalPosition + new Vector2(lr, 0) - playerOffset;
			float ang = startPos.AngleToPoint(endPos);
			DrawPolylineColors(new Vector2[]{startPos, endPos}, 
							   new Color[]{Color.ColorN("Blue"), Color.ColorN("Red")}, 1, false);
							   
			DrawArc(-playerOffset, 4, 0f, 2f * Mathf.Pi, 36, Color.ColorN("Blue"), 1, false);
			// DrawArc(endPos, 4, 0f, Mathf.Pi, 5, Color.ColorN("Red"), 1, false); // DRAW ARROW INSTEAD
			DrawLine(endPos, endPos + new Vector2((float) Mathf.Cos(ang - Mathf.Pi / 5), Mathf.Sin(ang - Mathf.Pi / 5)) * 6f, Color.ColorN("Red"), 1, false);
			DrawLine(endPos, endPos + new Vector2((float) Mathf.Cos(ang + Mathf.Pi / 5), Mathf.Sin(ang + Mathf.Pi / 5)) * 6f, Color.ColorN("Red"), 1, false);
		}
	}

	public void UpdateDoorTarget() {
		if (connectDoor is null || connectDoor == "") return;
		Door target = GetNodeOrNull<Door>(connectDoor);
		if (target is Door && !(target is null)) {
			doorTarget = target;
		} else {
			doorTarget = null;
		}
	}
    
    private Player FindPlayer() {
        Player player = GameHelper.GetNodeInGroup<Player>(this, "Player");
        return player;
    }

    private PlayerCamera FindCamera() {
        PlayerCamera camera = GameHelper.GetNodeInGroup<PlayerCamera>(this, "Player");
        return camera;
    }
}
