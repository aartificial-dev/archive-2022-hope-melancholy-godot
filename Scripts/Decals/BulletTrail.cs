using Godot;
using System;

public class BulletTrail : Line2D {

    private Timer timer;

    private RandomNumberGenerator rnd = new RandomNumberGenerator();
    private float distortAmount = 0f;

    private ShaderMaterial shader;

    public Bullet bullet;

    private float timerLength = 2f;

    private float offset;

    // private ShaderMaterial shader = ResourceLoader.Load("res://Shaders/shader_bullettrail.tres");

    public override void _Ready() {
        rnd.Randomize();

        timer = GetNode<Timer>("Timer");
        timer.Connect("timeout", this, nameof(Destroy));
        timerLength += rnd.Randf() - 0.5f;
        timer.Start(timerLength);

        shader = (ShaderMaterial) this.Material;
        this.Material.ResourceLocalToScene = true;
        shader.SetShaderParam("offset", new Vector2(rnd.Randf() * 100f, rnd.Randf() * 100f));

        offset = rnd.Randf() * Mathf.Tau;
    }

    public override void _Process(float delta) {
        distortAmount = 0.16f * (1f - timer.TimeLeft / timerLength);
        shader.SetShaderParam("alpha", 1 - Mathf.SmoothStep(1f, 0f, timer.TimeLeft / timerLength));

        for (int i = 0 ; i < this.Points.Length; i ++) {
            Vector2 point = this.Points[i];
            point.x += Mathf.Cos(offset + (float) i * 4f) * rnd.Randf() * distortAmount * (delta * 60);
            point.y += Mathf.Sin(offset + (float) i * 4f) * rnd.Randf() * distortAmount * (delta * 60)  - delta * 2f;
            this.SetPointPosition(i, point);
        }
    }

    public void Destroy() {
        //GD.Print( this.Points.Length);
        bullet.bulletTrail = null;
        this.Visible = false;
        this.GetParent().RemoveChild(this);
        this.QueueFree();
    }
}
