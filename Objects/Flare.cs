using Godot;
using System;

public class Flare : RigidBody2D { 
    [Export]
    public float burnTime = 300;

    private AudioStreamPlayer2D audioFire;
    private Timer timerBurned;
    private Timer timerIgnite;

    private Particles2D particles;
    private Light2D light;

    private bool timerRunning;

    public override void _Ready() {
        audioFire = GetNode<AudioStreamPlayer2D>("AudioFire");
        timerBurned = GetNode<Timer>("TimerBurned");
        timerIgnite = GetNode<Timer>("TimerIgnite");

        particles = GetNode<Particles2D>("Particles2D");
        light = GetNode<Light2D>("Light2D");

        timerIgnite.Connect("timeout", this, nameof(PlayFireSound));
        timerBurned.Connect("timeout", this, nameof(BurnOut));
    }

	public override void _Process(float delta) {
        if (timerBurned.TimeLeft < 10f && timerRunning) {
            float t = timerBurned.TimeLeft;
            
            Color modul = particles.Modulate;
            modul.a = t / 10f;
            particles.Modulate = modul;

            Color lcol = light.Color;
            lcol.a = t / 10f;
            light.Color = lcol;

            audioFire.VolumeDb = - (1f - t / 10f) * 20f;
        }
	}

    public void PlayFireSound() {
        audioFire.Play();
        timerBurned.Start(burnTime);
        timerRunning = true;
    }

    public void BurnOut() {
        particles.Emitting = false;
        light.Visible = false;
        audioFire.Playing = false;
        if (particles.Emitting != false) {
            RemoveChild(light);
            light.QueueFree();
            RemoveChild(particles);
            particles.QueueFree();
        }
    }

    public void HitByBullet(Vector2 pos, Vector2 direction, float speed) {
        BurnOut();
        timerBurned.Stop();
        this.ApplyImpulse(this.GlobalPosition - pos, direction * speed * 2f);
    }
}