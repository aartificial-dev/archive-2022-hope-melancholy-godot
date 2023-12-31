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
    private PackedScene bulletScene = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/Bullet.tscn");
    private PackedScene casingScene = ResourceLoader.Load<PackedScene>("res://Scenes/Decals/BulletCasing.tscn");
    private PackedScene emptyMagScene = ResourceLoader.Load<PackedScene>("res://Scenes/Decals/EmptyMag.tscn");
    private Bullet bulletInstance = null;
    private RandomNumberGenerator rnd = new RandomNumberGenerator();

    private StreamTexture spriteEmptyClip = ResourceLoader.Load<StreamTexture>("res://Assets/VFX_Sprites/spr_empty_clip.png");
    private StreamTexture spriteEmptyBat = ResourceLoader.Load<StreamTexture>("res://Assets/VFX_Sprites/spr_empty_bat.png");
    
    private PackedScene flareScene = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/Flare.tscn");

    private bool isInAnimation = false;

    private ItemPawn reloadItemPawnHolder = null;

    private Light2D lightFlashlight;
    private Light2D lightLamp;

    private Position2D emptyMagPos;

    private ItemPawn itemInHand;

    private LightDictionary lightDictionary = new LightDictionary();

    private Flare flare = null;

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

        emptyMagPos = GetNode<Position2D>("EmptyMagDrop");

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
        
        animationPlayer.AnimationSetNext("inv_open", "inv_opened");
        animationPlayer.AnimationSetNext("inv_close", "idle");
        
        animationPlayer.AnimationSetNext("flare_throw", "idle");

        lightFlashlight = GetNode<Light2D>("LightFlashlight");
        lightLamp = GetNode<Light2D>("LightLamp");

        lightDictionary.Add("w_flashlight", lightFlashlight);
        lightDictionary.Add("w_lamp", lightLamp);
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

        if (player.GetIsInCutscene()) {
            PlayAnimation("idle", itemInHand);
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
        switch (itemInHand.Id) {
            case "w_flashlight":
                return animation + (itemInHand.IsActive ? "_flash_on" : "_flash_off");
            case "w_handgun":
                return animation + "_handgun";
            case "w_lamp":
                return animation + "_lamp";
            case "w_tube":
                return animation + "_tube";
        }
        return "";
    }

    public bool GetCanMove() {
        String curAnim = animationPlayer.CurrentAnimation;
        return !(isInAnimation) && !(curAnim == "inv_close" || curAnim == "inv_open") && !(player.GetIsInCutscene());
    }

    public void SetIsInAnimation(bool value) {
        isInAnimation = value;
    }

    public bool GetIsInAnimation() {
        return isInAnimation;
    }

    public void SetSpriteFlipH(bool flip) {
        spriteMovement.FlipH = flip;
        spriteMovementHands.FlipH = flip;
        spriteWeapon.FlipH = flip;
        handsWeaponsBack.FlipH = flip;
        handsWeaponsFront.FlipH = flip;
        lightFlashlight.Scale = new Vector2 ( ( flip ? -1 : 1 ), 1);
        emptyMagPos.Position = new Vector2 ( ( flip ? -4 : 4 ), 4);
    }

    public bool GetSpriteFlipH() {
        return spriteMovement.FlipH;
    }

    public void Aim(ItemPawn itemInHand) {
        if (!GetCanMove()) return;
        SetSpriteFlipH(GameHelper.GetMousePosScene(this).x <= this.GlobalPosition.x);
        if (itemInHand is null) {
            PlayAnimation("aim_fists", null);
            return;
        }
        switch (itemInHand.Id) {
            case "w_handgun":
                AimGun();
            break;
            case "w_tube":
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

        handFrontPosition.Rotation = handFrontPosition.GlobalPosition.AngleToPoint(GameHelper.GetMousePosScene(this)) + (GetSpriteFlipH() ? 0 : Mathf.Pi);
        handBackPosition.Rotation = handFrontPosition.Rotation;
        // GD.Print(handFrontPosition.GlobalPosition, "  :  ", GameHelper.GetMousePosScene(this));
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
        if (IsInventoryOpen()) return;
        if (!GetCanMove()) return;
        if (itemInHand is null) {
            PlayAnimation("attack_fists", null);
            isInAnimation = true;
            return;
        }
        switch (itemInHand.Id) {
            case "w_handgun":
                AimGun();
                AttackGun(itemInHand);
            break;
            case "w_tube":
                PlayAnimation("attack_tube", null);
                isInAnimation = true;
            break;
        }
    }
    
    public void AttackGun(ItemPawn itemInHand) {
        int currentAmmo = itemInHand.Ammo;
        if (currentAmmo == 0) {
            audioPistolNoAmmo.Play();
            return;
        }
        isInAnimation = true;
        currentAmmo --;
        itemInHand.Ammo = currentAmmo;
        PlayAnimation("attack_handgun", null);
        audioPistolShoot.PlayRandom();
        Vector2 dir = handFrontPosition.GlobalPosition.DirectionTo(GameHelper.GetMousePosScene(this) + new Vector2(rnd.Randf(), rnd.Randf()));
        player.camera.Translate(new Vector2(Mathf.Floor(-dir.x * 7f), 0f));

        player.GetParent().AddChild(bulletInstance);
        bulletInstance.PauseMode = PauseModeEnum.Inherit;
        bulletInstance.direction = dir;
        bulletInstance.Rotation = Mathf.Atan2(dir.y, dir.x);
        bulletInstance.GlobalPosition = bulletSpawnPosition.GlobalPosition;
        bulletInstance = (Bullet) bulletScene.Instance();
        bulletInstance.PauseMode = PauseModeEnum.Stop;

        RigidBody2D casingInstance = (RigidBody2D) casingScene.Instance();
        player.GetParent().AddChild(casingInstance);
        casingInstance.GlobalPosition = bulletSpawnPosition.GlobalPosition;
        casingInstance.ApplyImpulse(new Vector2(GetSpriteFlipH() ? -0.15f : 0.15f, 0f) ,bulletSpawnPosition.GlobalPosition.DirectionTo(new Vector2(player.GlobalPosition.x, player.GlobalPosition.y - 20f)) * (80f + rnd.Randf() * 40f));
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
        if (IsInventoryOpen()) return;
        if (itemInHand is null) return;

        reloadItemPawnHolder = itemInHand;

        switch (itemInHand.Id) {
            case "w_handgun":
                if ( reloadItemPawnHolder.Ammo == reloadItemPawnHolder.AmmoMax ) break;
                if ( player.GetAmmo("a_handgun") is null ) break; 
                isInAnimation = true;
                PlayAnimation("reload_handgun", null);
                CreateEmptyMag(spriteEmptyClip);
            break;
            case "w_flashlight":
                if ( reloadItemPawnHolder.Ammo == reloadItemPawnHolder.AmmoMax ) break;
                if ( player.GetAmmo("a_battery") is null ) break; 
                isInAnimation = true;
                PlayAnimation("reload_flash", null);
                CreateEmptyMag(spriteEmptyBat);
            break;
            case "w_lamp":
                if ( reloadItemPawnHolder.Ammo == reloadItemPawnHolder.AmmoMax ) break;
                if ( player.GetAmmo("a_battery") is null ) break; 
                isInAnimation = true;
                PlayAnimation("reload_lamp", null);
                CreateEmptyMag(spriteEmptyBat);
            break;
        }
    }

    private void CreateEmptyMag(StreamTexture sprite) {
        RigidBody2D emptyMag = (RigidBody2D) emptyMagScene.Instance();
        player.GetParent().AddChild(emptyMag);
        emptyMag.GlobalPosition = emptyMagPos.GlobalPosition;
        emptyMag.GetNode<Sprite>("Sprite").Texture = sprite;
    }

    public void ReloadHandgun() {
        int ammoMax = reloadItemPawnHolder.AmmoMax;
        int ammo = reloadItemPawnHolder.Ammo;
        reloadItemPawnHolder.Ammo = ammoMax;
        player.RemoveAmmo("a_handgun");
    }  

    public void ReloadFlash() {
        int ammoMax = reloadItemPawnHolder.AmmoMax;
        int ammo = reloadItemPawnHolder.Ammo;
        reloadItemPawnHolder.Ammo = ammoMax;
        player.RemoveAmmo("a_battery");
    }

    public void ReloadLamp() {
        int ammoMax = reloadItemPawnHolder.AmmoMax;
        int ammo = reloadItemPawnHolder.Ammo;
        reloadItemPawnHolder.Ammo = ammoMax;
        player.RemoveAmmo("a_battery");
    }

    /// <summary> Function that using item (weaapon) in hardcoded way. Do not mistake with Item Script Language actions use </summary>
    public void UseItem(ItemPawn itemInHand) {
        if (IsInventoryOpen()) return;
        if (!GetCanMove()) return;
        if (itemInHand is null) return;

        switch (itemInHand.Id) {
            case "w_flashlight":
                UseLightSource(itemInHand);
            break;
            case "w_lamp":
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
        if (!lightDictionary.ContainsKey(itemInHand.Id)) return;

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
        lightDictionary[itemInHand.Id].Visible = true;
    }

    public bool IsInventoryOpen() {
        return player.IsInventoryOpen();
    }

    public void ToggleGUI() {
        if (!GetCanMove()) return;
        if (animationPlayer.CurrentAnimation == "inv_opened") {
            PlayAnimation("inv_close", null);
        } else {
            PlayAnimation("inv_open", null);
        }
    }

    public void SetGUIVisible(bool value) {
        player.camera.SetGUIVisible(value);
    }

    public void SpawnFlare() {
        Position2D fPos = GetNode<Position2D>("FlareSpawnPos");
        Vector2 localPos = fPos.Position;
        Vector2 pos = fPos.GlobalPosition;
        pos.x = GetSpriteFlipH() ? (pos.x - localPos.x * 2f) : pos.x;
        flare = (Flare) flareScene.Instance();

        Godot.Collections.Array arr = this.GetTree().GetNodesInGroup("ItemHolder");
        foreach (Node node in arr) {
            if (node is Node2D) {
                node.AddChild(flare);
                break;
            }
        }

        flare.GlobalPosition = pos;
        flare.GlobalRotation = fPos.GlobalRotation * (GetSpriteFlipH() ? -1 : 1);
        flare.ApplyCentralImpulse(new Vector2((GetSpriteFlipH() ? -1 : 1), -0.35f) * 120f);
    }

    public void ApplyFlareTorque() {
        if (!(flare is null)) {
            flare.ApplyTorqueImpulse((GetSpriteFlipH() ? -1 : 1) * 6f);
            flare = null;
        }
    }

    public void UseUsableItem() {
        if (IsInventoryOpen()) return;
        if (!GetCanMove()) return;
        PlayerCamera cam = player.camera;

        ItemPawn item = cam.GetUsableItem();
        //GD.Print(item);
        if (item is null) return;

        switch (item.Id) {
            case "u_flare_pack":
                UseFlarePack();
                cam.UseUsableItem();
            break;
        }
    }

    private void UseFlarePack() {
        SetSpriteFlipH(GameHelper.GetMousePosScene(this).x <= this.GlobalPosition.x);
        PlayAnimation("flare_throw", null);
        isInAnimation = true;
    }
}
