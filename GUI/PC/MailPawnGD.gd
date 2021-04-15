tool
extends Resource
class_name MailPawnGD, "res://item_resource.svg"

export (String) var from = "from"
export (String) var to = "to"
export (String, MULTILINE) var text = ""
export (bool) var isRead = false
