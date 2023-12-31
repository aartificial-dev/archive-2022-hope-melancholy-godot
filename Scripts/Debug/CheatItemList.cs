using Godot;
using System;

public class CheatItemList : ItemList { 
    [Export]
    public Godot.Collections.Array<Resource> items = new Godot.Collections.Array<Resource>();

    private PackedScene itemScene = ResourceLoader.Load<PackedScene>("res://Scenes/Items/ItemFloor.tscn");

    public override void _Ready() {
        int i = 0;
        foreach (Resource res in items) {
            this.AddItem((String) res.Get("name"));
            this.SetItemTooltipEnabled(i, false);
            i ++;
        }

        this.Connect("item_activated", this, nameof(SelectItem));
        this.GetParent().Connect("pressed", this, nameof(ChangeVisibility));

    }

	public override void _Process(float delta) {
	}

    public void ChangeVisibility() {
        this.Visible = !this.Visible;
    }

    public void SelectItem(int index) {
        ItemFloor item = (ItemFloor) itemScene.Instance();
        item.itemPawnResource = items[index];
        item.Position = this.GetParent<Button>().GetParent<HBoxContainer>().GetParent<CheatSheet>().GetParent<CanvasLayer>().GetParent<PlayerCamera>().GetPlayer().GlobalPosition;
        
        Godot.Collections.Array arr = this.GetTree().GetNodesInGroup("ItemHolder");
        foreach (Node node in arr) {
            if (node is Node2D) {
                node.AddChild(item);
                break;
            }
        }

    }
}