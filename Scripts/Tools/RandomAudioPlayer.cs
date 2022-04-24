using Godot;
using System;

public class RandomAudioPlayer : AudioStreamPlayer {
    [Export]
    public Godot.Collections.Array<AudioStreamSample> samples = new Godot.Collections.Array<AudioStreamSample>();

    private RandomNumberGenerator rnd = new RandomNumberGenerator();

    public override void _Ready() {
        rnd.Randomize();
    }

    public void PlayRandom() {
        if (samples.Count == 0) return;
        int count = samples.Count;
        int numb = rnd.RandiRange(0, count - 1);
        this.Stream = samples[numb];
        this.Play();
    }
}
