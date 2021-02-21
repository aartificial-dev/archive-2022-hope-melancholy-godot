using Godot;
using System;
using ItemArray = Godot.Collections.Array<ItemInventory>;

[Tool]
public class Inventory : Node2D {
    [Export]
    public Vector2 GridSize { set { gridSize = value; UpdateGrid();} get { return gridSize; } }
    private Vector2 gridSize = new Vector2(1f, 1f);
    [Export]
    public Vector2 CellSize { set { cellSize = value; UpdateGrid();} get { return cellSize; } }
    public Vector2 cellSize = new Vector2(1f, 1f);
    [Export]
    public Vector2 CellSpace { set { cellSpace = value; UpdateGrid();} get { return cellSpace; } }
    public Vector2 cellSpace = new Vector2(0f, 0f);
    [Export]
    public Vector2 CellOffset { set { cellOffset = value; UpdateGrid();} get { return cellOffset; } }
    public Vector2 cellOffset = new Vector2(0f, 0f);
    [Export]
    public bool updateGrid = false;
    [Export]
    public String name = "inv";
    [Export]
    public ItemPawn.ItemType allowedType = ItemPawn.ItemType.any;
    [Export]
    public int allowedAmount = 999;

    public InventoryGUI inventoryGUI;
    
    private Vector2 leftTopPos = Vector2.Zero;
    private Vector2 rightBottomPos = Vector2.Zero;

    private ItemArray itemList = new ItemArray();

    public override void _Ready() {
        UpdateGrid();
    }

    public override void _Process(float delta) {
        Vector2 mousePos = this.GetLocalMousePosition();
        if (Input.IsActionJustPressed("mb_left")) {
            //GetMouseGridPos(mousePos);
        }
        
        if (updateGrid) {
            updateGrid = false;
            Update();
        }
    }

    public bool PlaceItem(ItemInventory item) { // return true if cursor on inv
        if (this.Visible == false) return false;
        if (GetMouseGridPosFloor( this.GetLocalMousePosition() ) is null) return false;
        if (allowedType != ItemPawn.ItemType.any  && ItemList.GetItemType(item.itemType) != allowedType) return true;

        // CALCULATE ITEM PLACEMENT
        Vector2? gridPosRaw = GetMouseGridPosRaw( this.GetLocalMousePosition());
        if (gridPosRaw is null) return true;
        Vector2 gridPos = (Vector2) gridPosRaw - new Vector2(item.gridSize / 2f);
        gridPos = gridPos.Round();

        // if cannot place item
        if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x > gridSize.x || gridPos.y > gridSize.y) return true;
        if (gridPos.x + item.gridSize.x > gridSize.x + 0.1f || gridPos.y + item.gridSize.y > gridSize.y + 0.1f) return true;

        Vector2 actCellSize = new Vector2(cellSize + cellSpace);
        ItemArray itemIntersect = GridGetItemIntersectArray((Vector2) gridPos, item.gridSize);
        bool forceSwap = false;
        if (itemIntersect.Count == 0) {
            if (itemList.Count + 1 > allowedAmount) {
                forceSwap = true;
            } else {
                itemList.Add(item);
                item.gridPos = (Vector2) gridPos;
                item.GetParent().RemoveChild(item);
                this.AddChild(item);
                item.Position = (Vector2) gridPos * actCellSize;
                inventoryGUI.itemInHand = null;
            }
        }
        if (itemIntersect.Count == 1 || forceSwap) {
            // swap items
            ItemInventory itemPick;
            if (forceSwap) {
                itemPick = itemList[itemList.Count - 1];
            } else {
                itemPick = itemIntersect[0];
            }
            itemList.Add(item);
            item.gridPos = (Vector2) gridPos;
            item.GetParent().RemoveChild(item);
            this.AddChild(item);
            item.Position = (Vector2) gridPos * actCellSize;
            itemList.Remove(itemPick);
            itemPick.GetParent().RemoveChild(itemPick);
            inventoryGUI.inventoryHand.AddChild(itemPick);
            inventoryGUI.itemInHand = itemPick;
            inventoryGUI.itemInHand.GlobalPosition = this.GetGlobalMousePosition() - inventoryGUI.itemInHand.spriteSize / 2f;
        }
        return true;
    }

    public void TakeItem() {
        if (this.Visible == false) return;
        Vector2? gridPos = GetMouseGridPosFloor( this.GetLocalMousePosition());
        if (gridPos is null) return;
        ItemArray itemIntersect = GridGetItemIntersectArray((Vector2) gridPos, new Vector2(1f, 1f));
        if (itemIntersect.Count == 1) {
            ItemInventory itemPick = itemIntersect[0];
            itemList.Remove(itemPick);
            itemPick.GetParent().RemoveChild(itemPick);
            inventoryGUI.inventoryHand.AddChild(itemPick);
            inventoryGUI.itemInHand = itemPick;
            inventoryGUI.itemInHand.GlobalPosition = this.GetGlobalMousePosition() - inventoryGUI.itemInHand.spriteSize / 2f;
        }
    }

    private ItemArray GridGetItemIntersectArray(Vector2 pos, Vector2 size) {
        ItemArray itemIntersect = new ItemArray();
        foreach (ItemInventory item in itemList) {
            Rect2 iItemRect = new Rect2(item.gridPos, item.gridSize);
            Rect2 nItemRect = new Rect2(pos, size);
            if (iItemRect.Intersects(nItemRect)) {
                itemIntersect.Add(item);
            }
        }
        return itemIntersect;
    }

    private Vector2? GetMouseGridPosRound(Vector2 mousePos) {
        Vector2? gridPos = GetMouseGridPosRaw(mousePos);
        if (gridPos is null) return null;
        return gridPos?.Round();
    }

    private Vector2? GetMouseGridPosFloor(Vector2 mousePos) {
        Vector2? gridPos = GetMouseGridPosRaw(mousePos);
        if (gridPos is null) return null;
        return gridPos?.Floor();
    }

    private Vector2? GetMouseGridPosRaw(Vector2 mousePos) {
        //GD.Print(name, ": ", mousePos);
        bool mouseInInvX = mousePos.x >= leftTopPos.x && mousePos.x <= rightBottomPos.x;
        bool mouseInInvY = mousePos.y >= leftTopPos.y && mousePos.y <= rightBottomPos.y;
        bool mouseInInv = mouseInInvX && mouseInInvY;
        if (!mouseInInv) {
             //GD.Print(name, ": Out of bounds");
            return null;
        }
        Vector2 actCellSize = new Vector2(cellSize + cellSpace);
        Vector2 gridPos = mousePos / actCellSize;
        // GD.Print(name, ": ", gridPos);
        return gridPos;
    }

    private void UpdateGrid() {
        leftTopPos.x = cellOffset.x;
        leftTopPos.y = cellOffset.y;
        
        rightBottomPos.x = cellOffset.x + cellSize.x * gridSize.x + cellSpace.x * gridSize.x;
        rightBottomPos.y = cellOffset.y + cellSize.y * gridSize.y + cellSpace.y * gridSize.y;
        Update();
    }

    public override void _Draw() {
        
        // if (!Engine.EditorHint) return;
        DrawCircle(leftTopPos, 1f, new Color("#00aaaa"));
        DrawCircle(rightBottomPos, 1f, new Color("#00aaaa"));
        for (int y = 0; y < Mathf.FloorToInt(gridSize.y); y ++) {
            for (int x = 0; x < Mathf.FloorToInt(gridSize.x); x ++) {
                Rect2 rect = new Rect2(cellOffset.x + cellSize.x * x + cellSpace.x * x, cellOffset.y + cellSize.y * y + cellSpace.y * y, cellSize);
                this.DrawRect(rect, new Color("#aa00aa"), false, 1, false);
            }
        }
    }

    public ItemInventory GetItem(int index) {
        if (index < itemList.Count) {
            return itemList[index];
        }
        return null;
    }
}
