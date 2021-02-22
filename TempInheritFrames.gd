tool
extends AnimatedSprite

export(NodePath) var parent

var parentNode: AnimatedSprite;

func _ready():
	pass

func _process(_delta):
	parentNode = get_node_or_null(parent)
	if (parentNode != null):
		frame = parentNode.frame
		flip_h = parentNode.flip_h
