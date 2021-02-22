tool
extends AudioStreamPlayer

export (Array, AudioStreamSample) var samples

func play_random():
	if (samples.empty()):
		pass
	var num: int = floor(rand_range(0, samples.size() - 1))
	var sample: AudioStreamSample = samples[num]
	stream = sample
	play()
	pass
