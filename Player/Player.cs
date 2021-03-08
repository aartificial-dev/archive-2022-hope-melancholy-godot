using Godot;
using System;

public class Player : KinematicBody2D {

    private float gravity = 9.8f;
    private float gravityMultiplier = 19f;
    private float speed = 50f;
    private float climbSpeed = 25f;

    public float interactDistance = 30f;

    public PlayerCamera camera;
    public RayCast2D itemDropRayCast;
    private CollisionShape2D collisionShapePlayer;
    private PlayerAnimator animator;

    private bool isOnLadder = false;
    private bool isLadderEndCollide = false;
    private uint ladderTopBitmask    = 0b010000;
    private uint ladderBottomBitmask = 0b100000;
    private uint ladderEndBitmask = 0b0;

    public int health = 20;
    public int healthMax = 20;
    public int sanity = 40;
    public int sanityMax = 40;

    private bool isInCutscene = false;

    public override void _Ready() {

        collisionShapePlayer = GetNode<CollisionShape2D>("CollisionShapePlayer");

        itemDropRayCast = GetNode<RayCast2D>("ItemDropRayCast");

        animator = GetNode<PlayerAnimator>("PlayerAnimator");

        GetNode<Area2D>("LadderEndCollider").Connect("area_entered", this, nameof(LadderEndCollision));
        GetNode<Area2D>("LadderEndCollider").Connect("area_exited", this, nameof(NoLadderEndCollision));
    }

    public override void _Process(float delta) {
        if (!GetCanMove()) return;
        ItemPawn itemInHand = (camera.selectedWeapon is null) ? null : camera.selectedWeapon.itemPawn;
        animator.SetItemInHand(itemInHand);
        if (!isOnLadder) {
            if (Input.IsActionPressed("key_aim")) {
                if (Input.IsActionJustPressed("mb_left") && !camera.IsCursorOnInventory()) {
                    animator.Attack(itemInHand);
                } else {
                    animator.Aim(itemInHand);
                }
            } else {
                if (Input.IsActionJustPressed("mb_left") && !camera.IsCursorOnInventory()) {
                    animator.UseItem(itemInHand);
                }
            }
            if (Input.IsActionJustPressed("key_reload")) {
                animator.Reload(itemInHand);
            }
            if (Input.IsActionJustPressed("key_inventory")) {
                animator.ToggleGUI();
            }
            if (Input.IsActionJustPressed("key_usable")) {
                animator.UseUsableItem();
            }
        }
        health = Mathf.Clamp(health, 0, healthMax);
        sanity = Mathf.Clamp(sanity, 0, sanityMax);

        camera.UpdateCamera(delta);
    }

    public override void _PhysicsProcess(float delta) {
        if (isOnLadder) {
            collisionShapePlayer.Disabled = true;
            ProcessLadderMove(delta);
        } else {
            collisionShapePlayer.Disabled = false;
            ProcessMove(delta);
        }
    }

    private void ProcessMove(float delta) {
        if (Input.IsActionPressed("key_aim") || !animator.GetCanMove() || IsInventoryOpen()) return;
        ItemPawn itemInHand = (camera.selectedWeapon is null) ? null : camera.selectedWeapon.itemPawn;
        Vector2 dir = Vector2.Zero;
        if (Input.IsActionPressed("key_right")) {
            dir.x += speed;
        }
        if (Input.IsActionPressed("key_left")) {
            dir.x -= speed;
        }
        if (!this.IsOnFloor()) {
            dir.y += gravity * gravityMultiplier;
        }

        if (dir.x != 0f) {
            if (this.TestMove(this.Transform, new Vector2(dir.x / 2f * delta, 0f), false)) {
                animator.PlayAnimation("wall", null);
                dir.x = 0;
            } else {
                animator.PlayAnimation("walk", itemInHand);
            }
        } else if (dir.x == 0f) {
            animator.PlayAnimation("idle", itemInHand);
            Transform2D _tr = this.Transform;
            _tr.origin = _tr.origin.Round();
            this.Transform = _tr;
        }

        if (dir.x < 0f) {
            animator.SetSpriteFlipH(true);
        } 
        if (dir.x > 0f) {
            animator.SetSpriteFlipH(false);
        }

        this.MoveAndSlide(dir, Vector2.Up);

        isOnLadder = CheckLadderCanClimb();
    }

    private void ProcessLadderMove(float delta) {
        animator.SetSpriteFlipH(false);
        isOnLadder = !CheckLadderCanGetOff();
        if (isOnLadder == false) {
            animator.Play();
            return;
        }

        Vector2 dir = Vector2.Zero;

        if (Input.IsActionPressed("key_up")) {
            animator.PlayAnimation("climb_up", null);
            dir.y += -1;
        }
        if (Input.IsActionPressed("key_down")) {
            animator.PlayAnimation("climb_down", null);
            dir.y += 1;
        }
        if (Input.IsActionPressed("key_down") || Input.IsActionPressed("key_up")) {
            animator.Play();
        } else {
            animator.Pause();
        }
        
        this.MoveAndSlide(dir * climbSpeed, Vector2.Up);
    }

    public bool GetCanMove() {
        return animator.GetCanMove();
    }

    private void LadderEndCollision(Area2D _area) {
        isLadderEndCollide = true;
        ladderEndBitmask = _area.CollisionLayer;
    } 
    
    private void NoLadderEndCollision(Area2D _area) {
        isLadderEndCollide = false;
        ladderEndBitmask = 0b0;
    }

    private bool CheckLadderCollisionTop() {
        return (ladderEndBitmask & ladderTopBitmask) != 0;
    } 

    private bool CheckLadderCollisionBottom() {
        return (ladderEndBitmask & ladderBottomBitmask) != 0 ;
    } 

    private bool CheckLadderCanClimb() {
        if (CheckLadderCollisionBottom() && Input.IsActionPressed("key_up")) {
            return true;
        }
        if (CheckLadderCollisionTop() && Input.IsActionPressed("key_down")) {
            return true;
        }
        return false;
    }

    private bool CheckLadderCanGetOff() {
        if (CheckLadderCollisionBottom() && Input.IsActionPressed("key_down")) {
            return true;
        }
        if (CheckLadderCollisionTop() && Input.IsActionPressed("key_up")) {
            return true;
        }
        return false;
    }

    public void StatsAddValue(String stat, int value) {
        switch (stat) {
            case "health":
                health += value;
                health = Mathf.Clamp(health, 0, healthMax);
            break;
            case "sanity":
                sanity += value;
                sanity = Mathf.Clamp(sanity, 0, sanityMax);
            break;
        }
    }

    public void AmmoAddValue(String type, int value) {
        switch (type) {
            case "handgun":
                //ammoHandgun += value;
            break;
            case "battery":
                //ammoBattery += value;
            break;
        }
    }

    public ItemInventory GetAmmo(String id) {
        return camera.GetItemByID(id);
    }

    public void RemoveAmmo(String id) {
        camera.RemoveItem(id);
    }

    public bool IsInventoryOpen() {
        return camera.IsInventoryOpen();
    }

    public void SetIsInCutscene(bool value) {
        isInCutscene = value;
    }

    public bool GetIsInCutscene() {
        return isInCutscene;
    }

}
