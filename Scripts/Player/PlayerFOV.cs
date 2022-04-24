using Godot;
using System;

public class PlayerFOV : Node2D {

    public Godot.Collections.Array occluderArr = null;
    public Node2D position = null;
    public PlayerCamera camera;

    public override void _Ready() {
        camera = GetParent<PlayerCamera>();
    }

	public override void _Process(float delta) {

	}

    public override void _Draw() {
        // DrawCircle(GameHelper.GetMousePosScene(this) - camera.GlobalPosition, 3, Color.ColorN("Blue"));
        if (occluderArr is null) return;
        foreach (Line2D occluder in occluderArr) {
            DrawFOV(occluder);
        }
    }

    private void DrawFOV(Line2D occluder) {
        if (occluder is null || position is null) return;
        Vector2[] points = occluder.Points;
        for (int i = 0; i < points.Length; i ++) {
            Vector2[] poly;
            int s = (i < points.Length - 1) ? i + 1 : 0;
            Vector2 p1 = points[i] - camera.GlobalPosition;
            Vector2 p2 = points[s] - camera.GlobalPosition;

            Vector2 pos = position.GlobalPosition - camera.GlobalPosition;
            float angToP1 = pos.AngleToPoint(p1);
            float angToP2 = pos.AngleToPoint(p2);

            Vector2 p4 = p1 - new Vector2( Mathf.Cos(angToP1) * 200f, Mathf.Sin(angToP1) * 200f );
            Vector2 p3 = p2 - new Vector2( Mathf.Cos(angToP2) * 200f, Mathf.Sin(angToP2) * 200f );

            poly = new Vector2[4]{p1, p2, p3, p4};
            DrawColoredPolygon(poly, Color.ColorN("Black"));
        }
    }
}