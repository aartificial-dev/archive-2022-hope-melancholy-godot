tool
extends Resource
class_name ItemPawnGD, "res://item_resource.svg"

signal value_changed

enum ITEM_TYPE {
	ANY, 
	WEAPON, 
	QUEST, 
	GARBAGE, 
	CHIP,
	AMMO, 
	MEDICINE, 
	KEYCARD, 
	NOTES, 
	TOOL
}
 
export (String) 		var  name 			= "item"			setget set_name
export (int) 			var  spriteFrame 	= 0					setget set_spriteFrame
export (Vector2) 		var  sizeFloor 		= Vector2(1, 1)		setget set_sizeFloor
export (Vector2) 		var  sizeGrid 		= Vector2(1, 1)		setget set_sizeGrid
export (Vector2) 		var  sizeSprite 	= Vector2(1, 1)		setget set_sizeSprite
export (ITEM_TYPE) 		var  type 			= ITEM_TYPE.ANY		setget set_type
export (Array, int)		var  intArray 		= []				setget set_intArray
export (String) 		var  textField 		= ""				setget set_textField
export (int)			var  guiFrame		= 0					setget set_guiFrame


# Make sure that every parameter has a default value.
# Otherwise, there will be problems with creating and editing
# your resource via the inspector.
func _init(p_name = "item", p_spriteFrame = 0, 
			p_sizeFloor = Vector2(1, 1), p_sizeGrid = Vector2(1, 1), p_sizeSprite = Vector2(1, 1), 
			p_type = ITEM_TYPE.ANY, p_intArray = [], p_textField = "", p_guiFrame = 0):
	name = p_name
	spriteFrame = p_spriteFrame
	sizeFloor = p_sizeFloor
	sizeGrid = p_sizeGrid
	sizeSprite = p_sizeSprite
	type = p_type;
	intArray = p_intArray
	textField = p_textField
	guiFrame = p_guiFrame
	pass

func set_name(p_name: String):
	name = p_name
	emit_signal("value_changed")
	pass

func set_spriteFrame(p_spriteFrame: int):
	spriteFrame = p_spriteFrame
	emit_signal("value_changed")
	pass

func set_sizeFloor(p_sizeFloor: Vector2):
	sizeFloor = p_sizeFloor
	emit_signal("value_changed")
	pass

func set_sizeGrid(p_sizeGrid: Vector2):
	sizeGrid = p_sizeGrid
	emit_signal("value_changed")
	pass

func set_sizeSprite(p_sizeSprite: Vector2):
	sizeSprite = p_sizeSprite
	emit_signal("value_changed")
	pass

func set_type(p_type):
	type = p_type
	emit_signal("value_changed")
	pass

func set_intArray(p_intArray: Array):
	intArray = p_intArray
	emit_signal("value_changed")
	pass

func set_textField(p_textField: String):
	textField = p_textField
	emit_signal("value_changed")
	pass
	
func set_guiFrame(p_guiFrame: int):
	guiFrame = p_guiFrame
	emit_signal("value_changed")
	pass
