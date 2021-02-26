using Godot;
using System;
using Array = Godot.Collections.Array;
using LightDictionary = System.Collections.Generic.Dictionary<System.String, Godot.Light2D>;

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

    private Area2D meleeCollisionFists;
    private Area2D meleeCollisionTube;

    private AudioStreamPlayer audioMeleeHit;
    private AudioStreamPlayer audioMeleeMiss;
    private AudioStreamPlayer audioPistolNoAmmo;
    private AudioStreamPlayer audioLightToggle;
    private RandomAudioPlayer audioPistolShoot;

    private Node2D bulletHolder;
    private PackedScene bulletScene = ResourceLoader.Load<PackedScene>("res://Objects/Bullet.tscn");
    private Bullet bulletInstance = null;
    private RandomNumberGenerator rnd = new RandomNumberGenerator();

    private bool isAttacking = false;
    private bool isReloading = false;

    private ItemPawn reloadItemPawnHolder = null;

    private Light2D lightFlashlight;
    private Light2D lightLamp;
    private Timer lightBatteryTimer;

    private ItemPawn itemInHand;

    private LightDictionary lightDictionary = new LightDictionary();

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

        meleeCollisionFists = GetNode<Area2D>("MeleeCollisions/Fists");
        meleeCollisionTube = GetNode<Area2D>("MeleeCollisions/Tube");

        audioMeleeHit = GetNode<AudioStreamPlayer>("AnimationAudio/AudioMeleeHit");
        audioMeleeMiss = GetNode<AudioStreamPlayer>("AnimationAudio/AudioMeleeMiss");
        audioPistolNoAmmo = GetNode<AudioStreamPlayer>("AnimationAudio/AudioGunNoAmmo");
        audioPistolShoot = GetNode<RandomAudioPlayer>("AnimationAudio/AudioPistol");
        audioLightToggle = GetNode<AudioStreamPlayer>("AnimationAudio/AudioLightToggle");

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

        lightFlashlight = GetNode<Light2D>("LightFlashlight");
        lightLamp = GetNode<Light2D>("LightLamp");

        lightDictionary.Add("Flashlight", lightFlashlight);
        lightDictionary.Add("Lamp", lightLamp);
    }

    public override void _Process(float delta) {
        String anim = animationPlayer.CurrentAnimation;
        if (anim.StartsWith("idle") || anim.StartsWith("walk")) {
            ProcessLight(itemInHand, delta);
            //GD.Print(itemInHand);
        } else {
            lightFlashlight.Visible = false;
            lightLamp.Visible = false;
        }
    }

    public void SetItemInHand(ItemPawn itemInHand) {
        this.itemInHand = itemInHand;
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
            // GD.Print(newAnimation); ///////////////// DEBUG /////////////////
            animationPlayer.Play(newAnimation);
        }
    }

    public String GetAnimationItemName(String animation, ItemPawn itemInHand) {
        switch (itemInHand.Name) {
            case "Flashlight":
                return animation + (itemInHand.IsActive ? "_flash_on" : "_flash_off");
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
        lightFlashlight.Scale = new Vector2 ( ( flip ? -1 : 1 ), 1);
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
        switch (itemInHand.Name) {
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

    public void AimTube() {
        meleeCollisionTube.Scale = new Vector2(spriteMovement.FlipH ? -1 : 1, 1);
        meleeCollisionTube.Position = new Vector2(spriteMovement.FlipH ? -12 : 12, 1);
    }

    public void AimFists() {
        meleeCollisionFists.Scale = new Vector2(spriteMovement.FlipH ? -1 : 1, 1);
        meleeCollisionFists.Position = new Vector2(spriteMovement.FlipH ? -11 : 11, -5);
    }

    public void Attack(ItemPawn itemInHand) {
        if (!GetCanMove()) return;
        // isAttacking = true;
        if (itemInHand is null) {
            PlayAnimation("attack_fists", null);
            return;
        }
        switch (itemInHand.Name) {
            case "Handgun":
                AimGun();
                AttackGun(itemInHand);
            break;
            case "Tube":
                PlayAnimation("attack_tube", null);
            break;
        }
    }
    
    public void AttackGun(ItemPawn itemInHand) {
            int currentAmmo = itemInHand.Ammo;
            if (currentAmmo == 0) {
                audioPistolNoAmmo.Play();
                return;
            }
            currentAmmo --;
            itemInHand.Ammo = currentAmmo;
            PlayAnimation("attack_handgun", null);
            audioPistolShoot.PlayRandom();
            Vector2 dir = handFrontPosition.GlobalPosition.DirectionTo(this.GetGlobalMousePosition() + new Vector2(rnd.Randf(), rnd.Randf()));
            player.camera.Translate(new Vector2(Mathf.Floor(-dir.x * 7f), 0f));

            player.GetParent().AddChild(bulletInstance);
            bulletInstance.PauseMode = PauseModeEnum.Inherit;
            bulletInstance.direction = dir;
            bulletInstance.Rotation = Mathf.Atan2(dir.y, dir.x);
            bulletInstance.GlobalPosition = bulletSpawnPosition.GlobalPosition;
            bulletInstance = (Bullet) bulletScene.Instance();
            bulletInstance.PauseMode = PauseModeEnum.Stop;
    }

    public void AttackTube() {
        Array collisions = meleeCollisionTube.GetOverlappingBodies();
        if (collisions.Count == 0) {
            audioMeleeMiss.Play();
        } else {
            audioMeleeHit.Play();
        }
    }

    public void AttackFists() {
        Array collisions = meleeCollisionFists.GetOverlappingBodies();
        if (collisions.Count == 0) {
            audioMeleeMiss.Play();
        } else {
            audioMeleeHit.Play();
        }
    }

    public void Reload(ItemPawn itemInHand) {
        if (!GetCanMove()) return;
        if (itemInHand is null) return;

        reloadItemPawnHolder = itemInHand;

        switch (itemInHand.Name) {
            case "Handgun":
                if ( reloadItemPawnHolder.Ammo == reloadItemPawnHolder.AmmoMax ) break;
                if ( player.ammoHandgun == 0 ) break; 
                isReloading = true;
                PlayAnimation("reload_handgun", null);
            break;
            case "Flashlight":
                if ( reloadItemPawnHolder.Ammo == reloadItemPawnHolder.AmmoMax ) break;
                if ( player.ammoBattery == 0 ) break; 
                isReloading = true;
                PlayAnimation("reload_flash", null);
            break;
            case "Lamp":
                if ( reloadItemPawnHolder.Ammo == reloadItemPawnHolder.AmmoMax ) break;
                if ( player.ammoBattery == 0 ) break; 
                isReloading = true;
                PlayAnimation("reload_lamp", null);
            break;
        }
    }

    public void ReloadHandgun() {
        int ammoMax = reloadItemPawnHolder.AmmoMax;
        int ammo = reloadItemPawnHolder.Ammo;
        int ammoInv = player.ammoHandgun;
        int difference = Mathf.Min(ammoMax - ammo, ammoInv);
        player.ammoHandgun -= difference;
        reloadItemPawnHolder.Ammo = ammo + difference;
    }  

    public void ReloadFlash() {
        int ammoMax = reloadItemPawnHolder.AmmoMax;
        int ammo = reloadItemPawnHolder.Ammo;
        int ammoInv = player.ammoBattery;
        int difference = Mathf.Min(ammoMax - ammo, ammoInv);
        player.ammoBattery -= difference;
        reloadItemPawnHolder.Ammo = ammo + difference;
    }

    public void ReloadLamp() {
        int ammoMax = reloadItemPawnHolder.AmmoMax;
        int ammo = reloadItemPawnHolder.Ammo;
        int ammoInv = player.ammoBattery;
        int difference = Mathf.Min(ammoMax - ammo, ammoInv);
        player.ammoBattery -= difference;
        reloadItemPawnHolder.Ammo = ammo + difference;
    }

    /// <summary> Function that using item (weaapon) in hardcoded way. Do not mistake with Item Script Language actions use </summary>
    public void UseItem(ItemPawn itemInHand) {
        if (!GetCanMove()) return;
        if (itemInHand is null) return;

        switch (itemInHand.Name) {
            case "Flashlight":
                UseLightSource(itemInHand);
            break;
            case "Lamp":
                UseLightSource(itemInHand);
            break;
        }
    }

    public void UseLightSource(ItemPawn itemInHand) {
        if (itemInHand.IsActive) {
            itemInHand.IsActive = false;
            audioLightToggle.Play();
        } else {
            if ((int) itemInHand.Ammo > 0) {
                itemInHand.IsActive = true;
                audioLightToggle.Play();
            }
        }
    }

    public void ProcessLight(ItemPawn itemInHand, float delta) {
        lightFlashlight.Visible = false;
        lightLamp.Visible = false;
        if (!GetCanMove()) return;
        if (itemInHand is null) return;
        // GD.Print(itemInHand.name, " ", itemInHand.textField, " ", itemInHand.intArray);
        if (!lightDictionary.ContainsKey(itemInHand.Name)) return;

        bool isLightOn = false;
        int ammo = (int) itemInHand.Ammo;
        if (ammo > 0 && itemInHand.IsActive) {
            isLightOn = true;
            ammo -= Mathf.RoundToInt(delta * 100f);
            itemInHand.Ammo = ammo;
        }
        if (ammo <= 0 && itemInHand.IsActive) {
            audioLightToggle.Play();
            itemInHand.IsActive = false;
            isLightOn = false;
        }

        if (!isLightOn) return;
        lightDictionary[itemInHand.Name].Visible = true;
    }
}
