extends Control


func _ready():
	if (OS.is_debug_build()):
		visible = true
	
	$VBoxContainer/OS.text = "OS: " + OS.get_name()
	$VBoxContainer/VideoCard.text = "Video: " + VisualServer.get_video_adapter_name()
	$VBoxContainer/Build.text = "Build: " + ProjectSettings.get_setting("application/config/version")
	var engine = Engine.get_version_info()
	$VBoxContainer/Engine.text = "Engine: " + str(engine.major, ".", engine.minor, ".", engine.patch)
	pass
