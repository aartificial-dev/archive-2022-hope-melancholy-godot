using Godot;
using System;

public class WireSpawner : Line2D { 

    [Export]
    public float segmentWidth = 4f;

    private PackedScene wirePinScene = ResourceLoader.Load<PackedScene>("res://Objects/Wire/WirePin.tscn");
    private PackedScene wireSegmentScene = ResourceLoader.Load<PackedScene>("res://Objects/Wire/WireSegment.tscn");

    private Godot.Collections.Array<PhysicsBody2D> segments = new Godot.Collections.Array<PhysicsBody2D>();

    public enum WireConnection {
        None = 0b0000,
        Left = 0b0001,
        Right = 0b0010,
        Both = 0b0011,
        LeftPin = 0b0100,
        RightPin = 0b1000,
        BothPin = 0b1100
    }

    public override void _Ready() {
        if (this.Points.Length < 2) {
            this.QueueFree();
            return;
        }
        Vector2 startPos = this.ToGlobal(this.Points[0]); 
        Vector2 endPos = this.ToGlobal(this.Points[this.Points.Length - 1]);
        int segmentAmount = Mathf.RoundToInt(startPos.DistanceTo(endPos) / segmentWidth);
        float rot = startPos.AngleToPoint(endPos);

        NodePath left = "";
        NodePath right = "";

        StaticBody2D pinLeft = SpawnWirePin(startPos);
        segments.Add(pinLeft);
        left = pinLeft.GetPath();
        WireSegment prevSegm = null;

        for (int i = 1; i < segmentAmount; i ++) {
            Vector2 pos = startPos.LinearInterpolate(endPos, (float) i / (float) segmentAmount);
            WireSegment segm = SpawnWireSegment(pos, rot);
            right = segm.GetPath();
            Vector2 pinPos = startPos.LinearInterpolate(endPos, ((float) i - 0.5f) / (float) segmentAmount);
            PinJoint2D pin = ConnectSegments(pinPos, left, right);
            left = right;
            if (!(prevSegm is null)) prevSegm.pinRight = pin;
            segm.pinLeft = pin;
            prevSegm = segm;
            segm.parent = this;
            segments.Add(segm);
        }

        StaticBody2D pinRight = SpawnWirePin(endPos);
        segments.Add(pinRight);
        right = pinRight.GetPath();
        Vector2 pinEndPos = startPos.LinearInterpolate(endPos, ((float) segmentAmount - 0.5f) / (float) segmentAmount);
        ConnectSegments(pinEndPos, left, right);


    }

    public StaticBody2D SpawnWirePin(Vector2 pos) {
        StaticBody2D body = (StaticBody2D) wirePinScene.Instance();
        body.GlobalPosition = pos;
        this.AddChild(body);
        return body;
    }

    public WireSegment SpawnWireSegment(Vector2 pos, float rot) {
        WireSegment body = (WireSegment) wireSegmentScene.Instance();
        body.GlobalPosition = pos;
        body.GlobalRotation = rot;
        this.AddChild(body);
        return body;
    }

    public PinJoint2D ConnectSegments(Vector2 pos, NodePath left, NodePath right) {
        PinJoint2D pin = new PinJoint2D();
        this.AddChild(pin);
        pin.GlobalPosition = pos;
        pin.Softness = 0.05f;
        pin.Bias = 0f;
        pin.NodeA = left;
        pin.NodeB = right;
        return pin;
    }

    public int CheckConnection(WireSegment segm) {
        int ind = -1;
        int result = (int) (WireConnection.Both | WireConnection.BothPin);
        for (int i = 0; i < segments.Count; i ++) {
            if (segments[i] == segm) {
                ind = i;
                break;
            }
        }
        if (ind == -1) return (int) WireConnection.None;

        for (int i = ind - 1; i > 0; i --) {
            if (segments[i] is null) {
                result = result - (int) (WireConnection.LeftPin);
                if (i == ind - 1) {
                    result = result - (int) (WireConnection.Left);
                }
                break;
            }
        }
        for (int i = ind + 1; i < segments.Count - 1; i ++) {
            if (segments[i] is null) {
                result = result - (int) (WireConnection.RightPin);
                if (i == ind + 1) {
                    result = result - (int) (WireConnection.Right);
                }
                break;
            }
        }
        
        return result;
    }

    public void RemoveSegment(WireSegment segm) {
        int ind = -1;
        for (int i = 0; i < segments.Count; i ++) {
            if (segments[i] == segm) {
                ind = i;
                break;
            }
        }
        if (ind == -1) return;
        segments[ind] = null;
    }
}