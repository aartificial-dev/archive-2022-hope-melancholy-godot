tool
extends Resource
class_name ItemPawnGD, "res://item_resource.svg"

signal value_changed

var item_type: String = "Any,None,Weapon,Quest,Garbage,Chip,Ammo,Medicine,Notes,Tool,Usable"

var property_list = {
	"ItemPawn": {
		"name": "ItemPawn",	"type": TYPE_NIL,
		"usage": PROPERTY_USAGE_CATEGORY | PROPERTY_USAGE_SCRIPT_VARIABLE
	},
	"name": {
		"name": "name",
		"type": TYPE_STRING,
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": "item" 
	},
	"itemType": {
		"name": "itemType",
		"type": TYPE_INT,
		"hint": PROPERTY_HINT_ENUM,
		"hint_string": item_type,
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": 0
	},
	"itemID": {
		"name": "itemID",
		"type": TYPE_STRING,
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": "" 
	},
	"Sprite_Floor": {
		"name": "Sprite_Floor",	"type": TYPE_NIL,
		"usage": PROPERTY_USAGE_GROUP | PROPERTY_USAGE_SCRIPT_VARIABLE, "hint_string": "spriteFloor"
	},
	"spriteFloorFrames": {
		"name": "spriteFloorFrames",
		"type": TYPE_OBJECT,
		"hint": PROPERTY_HINT_RESOURCE_TYPE,
		"hint_string": "SpriteFrames",
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": null
	},
	"spriteFloorFrame": {
		"name": "spriteFloorFrame",
		"type": TYPE_INT,
		"hint": PROPERTY_HINT_RANGE,
		"hint_string": "0,999,1",
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": 0
	},
	"spriteFloorSize": {
		"name": "spriteFloorSize",
		"type": TYPE_VECTOR2,
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": Vector2.ZERO
	},
	"Sprite_Inventory": {
		"name": "Sprite_Inventory",	"type": TYPE_NIL,
		"usage": PROPERTY_USAGE_GROUP | PROPERTY_USAGE_SCRIPT_VARIABLE, "hint_string": "spriteInventory"
	},
	"spriteInventoryFrames": {
		"name": "spriteInventoryFrames",
		"type": TYPE_OBJECT,
		"hint": PROPERTY_HINT_RESOURCE_TYPE,
		"hint_string": "SpriteFrames",
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": null
	},
	"spriteInventoryFrame": {
		"name": "spriteInventoryFrame",
		"type": TYPE_INT,
		"hint": PROPERTY_HINT_RANGE,
		"hint_string": "0,999,1",
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": 0
	},
	"spriteInventorySize": {
		"name": "spriteInventorySize",
		"type": TYPE_VECTOR2,
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": Vector2.ZERO
	},
	"spriteInventoryGridSize": {
		"name": "spriteInventoryGridSize",
		"type": TYPE_VECTOR2,
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": Vector2.ZERO
	},
	"Weapon_Stats": {
		"name": "Weapon_Stats",	"type": TYPE_NIL, "usage": PROPERTY_USAGE_GROUP | PROPERTY_USAGE_SCRIPT_VARIABLE
	},
	"ammo": {
		"name": "ammo",
		"type": TYPE_INT,
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": 0
	},
	"ammoMax": {
		"name": "ammoMax",
		"type": TYPE_INT,
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": 0
	},
	"isActive": {
		"name": "isActive",
		"type": TYPE_BOOL,
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": false
	},
	"Inventory_Preview": {
		"name": "Inventory_Preview",	"type": TYPE_NIL, "usage": PROPERTY_USAGE_GROUP | PROPERTY_USAGE_SCRIPT_VARIABLE
	},
	"model": {
		"name": "model",
		"type": TYPE_OBJECT,
		"hint": PROPERTY_HINT_RESOURCE_TYPE,
		"hint_string": "Mesh",
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": null
	},
	"description": {
		"name": "description",
		"type": TYPE_STRING,
		"hint": PROPERTY_HINT_MULTILINE_TEXT,
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": ""
	},
	"Item_Script": {
		"name": "Item_Script",	"type": TYPE_NIL, "usage": PROPERTY_USAGE_GROUP | PROPERTY_USAGE_SCRIPT_VARIABLE
	},
	"itemScript": {
		"name": "itemScript",
		"type": TYPE_STRING,
		"hint": PROPERTY_HINT_MULTILINE_TEXT,
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": ""
	},
	"audioPickup": {
		"name": "audioPickup",
		"type": TYPE_OBJECT,
		"hint": PROPERTY_HINT_RESOURCE_TYPE,
		"hint_string": "AudioStreamSample",
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": null
	},
	"audioUse": {
		"name": "audioUse",
		"type": TYPE_OBJECT,
		"hint": PROPERTY_HINT_RESOURCE_TYPE,
		"hint_string": "AudioStreamSample",
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": null
	},
	"audioDrop": {
		"name": "audioDrop",
		"type": TYPE_OBJECT,
		"hint": PROPERTY_HINT_RESOURCE_TYPE,
		"hint_string": "AudioStreamSample",
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": null
	},
	"audioActivate": {
		"name": "audioActivate",
		"type": TYPE_OBJECT,
		"hint": PROPERTY_HINT_RESOURCE_TYPE,
		"hint_string": "AudioStreamSample",
		"usage": PROPERTY_USAGE_DEFAULT | PROPERTY_USAGE_SCRIPT_VARIABLE,
		"value": null
	},
}

func _get(property):
	if property_list.has(property):
		return property_list.get(property)["value"]
	return null

func _set(property, value):
	if property_list.has(property):
		property_list.get(property)["value"] = value
		emit_signal("value_changed")
	pass

func _get_property_list():
	return property_list.values()
