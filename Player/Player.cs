using Godot;
using System;

public class Player : KinematicBody2D {

    private float gravity = 9.8f;
    private float gravityMultiplier = 19f;
    private float speed = 50f;
    private float climbSpeed = 25f;

    public float interactDistance = 30f;

    private AnimatedSprite spriteMovement;
    private AnimatedSprite spriteWeapons;

    public PlayerCamera camera;

    private CollisionShape2D collisionShapePlayer;

    private bool isOnLadder = false;
    private bool isLadderEndCollide = false;
    private uint ladderTopBitmask    = 0b010000;
    private uint ladderBottomBitmask = 0b100000;
    private uint ladderEndBitmask = 0b0;

    public RayCast2D itemDropRayCast;

    private Position2D handBackPos;
    private Position2D handFrontPos;
    private Position2D bulletSpawnPos;

    private PackedScene bulletScene = ResourceLoader.Load<PackedScene>("res://Objects/Bullet.tscn");
    private Bullet bulletInstance = null;
    private RandomNumberGenerator rnd = new RandomNumberGenerator();

    public override void _Ready() {
        rnd.Randomize();
        spriteMovement = GetNode<AnimatedSprite>("MoveSprites/SpriteMovement");
        spriteWeapons = GetNode<AnimatedSprite>("WeaponSprites/SpriteWeapons");

        collisionShapePlayer = GetNode<CollisionShape2D>("CollisionShapePlayer");

        itemDropRayCast = GetNode<RayCast2D>("ItemDropRayCast");

        GetNode<Area2D>("LadderEndCollider").Connect("area_entered", this, nameof(LadderEndCollision));
        GetNode<Area2D>("LadderEndCollider").Connect("area_exited", this, nameof(NoLadderEndCollision));

        handBackPos = GetNode<Position2D>("WeaponSprites/BackHandPos");
        handFrontPos = GetNode<Position2D>("WeaponSprites/FrontHandPos");
        bulletSpawnPos = GetNode<Position2D>("WeaponSprites/FrontHandPos/HandsWeaponsFront/BulletSpawn");

        bulletInstance = (Bullet) bulletScene.Instance();
        bulletInstance.PauseMode = PauseModeEnum.Stop;
    }

    public override void _Process(float delta) {
        if (Input.IsActionJustPressed("key_space")) {
            GetNode<Particles2D>("WeaponSprites/FrontHandPos/HandsWeaponsFront/ParticlesGunSmoke").Emitting = false;
            GetNode<Particles2D>("WeaponSprites/FrontHandPos/HandsWeaponsFront/ParticlesGunFire").Emitting = false;
            Vector2 dir = handFrontPos.GlobalPosition.DirectionTo(this.GetGlobalMousePosition());
            this.GetParent().AddChild(bulletInstance);
            bulletInstance.PauseMode = PauseModeEnum.Inherit;
            bulletInstance.direction = dir;
            bulletInstance.GlobalPosition = bulletSpawnPos.GlobalPosition;
            GetNode<Particles2D>("WeaponSprites/FrontHandPos/HandsWeaponsFront/ParticlesGunSmoke").Emitting = true;
            GetNode<Particles2D>("WeaponSprites/FrontHandPos/HandsWeaponsFront/ParticlesGunFire").Emitting = true;
            bulletInstance = (Bullet) bulletScene.Instance();
            bulletInstance.PauseMode = PauseModeEnum.Stop;
            //camera.Position = camera.Position - bullet.direction * 3f;
        }
    }

    public override void _PhysicsProcess(float delta) {
        if (isOnLadder) {
            collisionShapePlayer.Disabled = true;
            ProcessLadderMove(delta);
        } else {
            collisionShapePlayer.Disabled = false;
            ProcessMove(delta);
        }

        camera.UpdateCamera(delta);
    }

    private void ProcessMove(float delta) {
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

        //GD.Print(this.IsOnFloor());

        if (dir.x != 0f) {
            if (this.TestMove(this.Transform, new Vector2(dir.x / 2f * delta, 0f), false)) {
                SetMoveAnimation("wall");
                dir.x = 0;
            } else {
                SetMoveAnimation("walk");
            }
        } else if (dir.x == 0f) {
            SetMoveAnimation("idle");
            Transform2D _tr = this.Transform;
            _tr.origin = _tr.origin.Round();
            this.Transform = _tr;
        }

        if (dir.x < 0f) {
            spriteMovement.FlipH = true;
        } 
        if (dir.x > 0f) {
            spriteMovement.FlipH = false;
        }

        this.MoveAndSlide(dir, Vector2.Up);

        if (isLadderEndCollide) {
            // GD.Print("trying to climb, bit check: ", ladderEndBitmask, ", top: ", ladderTopBitmask, ", bottom: ", ladderBottomBitmask);
            if ((ladderEndBitmask & ladderBottomBitmask) != 0 
                && Input.IsActionPressed("key_up")) {
                    isOnLadder = true;
            }
            if ((ladderEndBitmask & ladderTopBitmask) != 0
                && Input.IsActionPressed("key_down")) {
                    isOnLadder = true;
            }
        }
    }

    private void ProcessLadderMove(float delta) {
        spriteMovement.FlipH = false;
        if (isLadderEndCollide) {
            if ((ladderEndBitmask & ladderBottomBitmask) != 0 
                && Input.IsActionPressed("key_down")) {
                    isOnLadder = false;
                    return;
            }
            if ((ladderEndBitmask & ladderTopBitmask) != 0
                && Input.IsActionPressed("key_up")) {
                    isOnLadder = false;
                    return;
            }
        }

        Vector2 dir = Vector2.Zero;

        if (Input.IsActionPressed("key_up")) {
            SetMoveAnimation("climb_up");
            dir.y += -1;
        }
        if (Input.IsActionPressed("key_down")) {
            SetMoveAnimation("climb_down");
            dir.y += 1;
        }
        if (Input.IsActionPressed("key_down") || Input.IsActionPressed("key_up")) {
            spriteMovement.Playing = true;
        } else {
            spriteMovement.Playing = false;
        }
        
        this.MoveAndSlide(dir * climbSpeed, Vector2.Up);
    }

    private void SetMoveAnimation(String anim) {
        if (spriteMovement.Animation != anim) {
            spriteMovement.Animation = anim;
        }
    }

    private void LadderEndCollision(Area2D _area) {
        isLadderEndCollide = true;
        ladderEndBitmask = _area.CollisionLayer;
    } 
    
    private void NoLadderEndCollision(Area2D _area) {
        isLadderEndCollide = false;
    }
}
