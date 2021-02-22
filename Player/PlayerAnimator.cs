using Godot;
using System;

public class PlayerAnimator : Node2D {

    private Player player;

    private AnimationPlayer animationPlayer;

    private Position2D handBackPosition;
    private Position2D handFrontPosition;
    private Position2D bulletSpawnPosition;

    private Sprite spriteMovement;
    private Sprite spriteMovementHands;
    private Sprite spriteWeapon;
    private Sprite handsWeaponsBack;
    private Sprite handsWeaponsFront;

    private Node2D bulletHolder;
    private PackedScene bulletScene = ResourceLoader.Load<PackedScene>("res://Objects/Bullet.tscn");
    private Bullet bulletInstance = null;
    private RandomNumberGenerator rnd = new RandomNumberGenerator();

    private bool isAttacking = false;
    private bool isReloading = false;

    public override void _Ready() {
        player = GetParent<Player>();

        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        handBackPosition = GetNode<Position2D>("PositionWeaponBack");
        handFrontPosition = GetNode<Position2D>("PositionWeaponFront");
        bulletSpawnPosition = GetNode<Position2D>("PositionWeaponFront/BulletHolder/BulletSpawn");

        spriteMovement = GetNode<Sprite>("SpriteMovement");
        spriteMovementHands = GetNode<Sprite>("SpriteMovementHands");
        spriteWeapon = GetNode<Sprite>("SpriteWeapon");
        handsWeaponsBack = GetNode<Sprite>("PositionWeaponBack/SpriteWeaponBack");
        handsWeaponsFront = GetNode<Sprite>("PositionWeaponFront/SpriteWeaponFront");

        bulletHolder = GetNode<Node2D>("PositionWeaponFront/BulletHolder");

        bulletInstance = (Bullet) bulletScene.Instance();
        bulletInstance.PauseMode = PauseModeEnum.Stop;
        rnd.Randomize();

        animationPlayer.AnimationSetNext("attack_fists", "aim_fists");
        animationPlayer.AnimationSetNext("attack_handgun", "aim_handgun");
        animationPlayer.AnimationSetNext("attack_tube", "aim_tube");
        
        animationPlayer.AnimationSetNext("reload_flash", "idle_flash");
        animationPlayer.AnimationSetNext("reload_handgun", "idle_handgun");
        animationPlayer.AnimationSetNext("reload_lamp", "idle_lamp");
        
        animationPlayer.AnimationSetNext("hit", "idle");
        animationPlayer.AnimationSetNext("smoke", "idle");
    }

    public void Play() {
        animationPlayer.Play();
    }

    public void Pause() {
        animationPlayer.Stop(false);
    }

    public void PlayAnimation(String animation, ItemPawn itemInHand) {
        String newAnimation = itemInHand is null ? animation : GetAnimationItemName(animation, itemInHand);

        if (animationPlayer.CurrentAnimation != newAnimation) {
            animationPlayer.Play(newAnimation);
        }
    }

    public String GetAnimationItemName(String animation, ItemPawn itemInHand) {
        switch (itemInHand.name) {
            case "Flashlight":
                return animation + itemInHand.textField == "on" ? "_flash_on" : "flash_off";
            case "Handgun":
                return animation + "_handgun";
            case "Lamp":
                return animation + "_lamp";
            case "Tube":
                return animation + "_tube";
        }
        return "";
    }

    public bool GetCanMove() {
        return !(isAttacking || isReloading);
    }

    public void SetIsAttacking(bool value) {
        isAttacking = value;
    }

    public bool GetIsAttacking() {
        return isAttacking;
    }

    public void SetIsReloading(bool value) {
        isReloading = value;
    }

    public bool GetIsReloading() {
        return isReloading;
    }

    public void SetSpriteFlipH(bool flip) {
        spriteMovement.FlipH = flip;
        spriteMovementHands.FlipH = flip;
        spriteWeapon.FlipH = flip;
        handsWeaponsBack.FlipH = flip;
        handsWeaponsFront.FlipH = flip;
    }

    public bool GetSpriteFlipH() {
        return spriteMovement.FlipH;
    }

    public void Aim(ItemPawn itemInHand) {
        if (!GetCanMove()) return;
        SetSpriteFlipH(this.GetGlobalMousePosition().x <= this.GlobalPosition.x);
        if (itemInHand is null) {
            PlayAnimation("aim_fists", null);
            return;
        }
        switch (itemInHand.name) {
            case "Handgun":
                AimGun();
            break;
            case "Tube":
                PlayAnimation("aim_tube", null);
            break;
        }
    }

    public void AimGun() {
        PlayAnimation("aim_handgun", null);
        Vector2 shoulderPos = handFrontPosition.Position;
        shoulderPos.x = GetSpriteFlipH() ? 4 : -4;

        handFrontPosition.Position = shoulderPos;
        handBackPosition.Position = shoulderPos;

        handsWeaponsFront.Position = shoulderPos * -1;
        handsWeaponsBack.Position = shoulderPos * -1;

        handFrontPosition.Rotation = handFrontPosition.GlobalPosition.AngleToPoint(this.GetGlobalMousePosition()) + (GetSpriteFlipH() ? 0 : Mathf.Pi);
        handBackPosition.Rotation = handFrontPosition.Rotation;

        Vector2 spawnPos = bulletHolder.Position;
        spawnPos.x = GetSpriteFlipH() ? -20 : 20;
        bulletHolder.Position = spawnPos;
        bulletHolder.Rotation = (GetSpriteFlipH() ? Mathf.Pi : 0);
    }

    public void Attack(ItemPawn itemInHand) {
        if (!GetCanMove()) return;
        // isAttacking = true;
        if (itemInHand is null) {
            PlayAnimation("attack_fists", null);
            return;
        }
        switch (itemInHand.name) {
            case "Handgun":
                AttackGun(itemInHand);
            break;
            case "Tube":
                PlayAnimation("attack_tube", null);
            break;
        }
    }
    
    public void AttackGun(ItemPawn itemInHand) {
            PlayAnimation("attack_handgun", null);
            ///////////////////////////////////////////////////////////////// TODO CHECK & REMOVE BULLET //////////////////////////////////////////////////////////////////////
            Vector2 dir = handFrontPosition.GlobalPosition.DirectionTo(this.GetGlobalMousePosition() + new Vector2(rnd.Randf(), rnd.Randf()));
            this.GetParent().AddChild(bulletInstance);
            bulletInstance.PauseMode = PauseModeEnum.Inherit;
            bulletInstance.direction = dir;
            bulletInstance.Rotation = Mathf.Atan2(dir.y, dir.x);
            bulletInstance.GlobalPosition = bulletSpawnPosition.GlobalPosition;
            player.camera.Translate(new Vector2(Mathf.Floor(-bulletInstance.direction.x * 7f), 0f));
            bulletInstance = (Bullet) bulletScene.Instance();
            bulletInstance.PauseMode = PauseModeEnum.Stop;  
    }

    public void Reload(ItemPawn itemInHand) {
        // isReloading = true;
        ///////////////////////////////////////////////////////////////// TODO RELOAD //////////////////////////////////////////////////////////////////////
    }
}
