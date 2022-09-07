using System.Collections;
using TinyCacto.Effects;
using UnityEngine;


public class Player : MonoBehaviour
{

    [Header("PLAYER INFORMATIONS")]
    [SerializeField] InputReader Input;
    [SerializeField] UnitData Unit;

    [Header("DEATH")]
    [SerializeField] float respawnDuration = 5;
    [SerializeField] GameEvent OnPlayerHitEvent;
    
    [Header("COLLIDER")]
    [SerializeField] ColliderInformations normalCollider;
    [SerializeField] ColliderInformations duckCollider;

    // MODULES
    MovementModule Movement;
    StealthModule Stealth;
    AttackModule Attack;
    HealthModule Health;
    InstaPunchModule InstaPunch;

    Animator anim;

    BoxCollider2D PlayerCollider;

    private void Start()
    {
        // Components

        if (PlayerCollider == null)
        {
            PlayerCollider = GetComponentInChildren<BoxCollider2D>();

            normalCollider.Setup(PlayerCollider.offset, PlayerCollider.size);

        }

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        // Unit

        Unit.InitializeUnit(gameObject);
        Input.Initialize(Unit);

        // Modules

        if (Movement == null)
        {
            Movement = GetComponentInChildren<MovementModule>();
            Movement.InitializeModule(Unit);
        }

        if (Stealth == null)
        {
            Stealth = GetComponentInChildren<StealthModule>();
            Stealth.InitializeModule(Unit);
        }

        if (Attack == null)
        {
            Attack = GetComponentInChildren<AttackModule>();
            Attack.InitializeModule(Unit);

            // Update Ammo UI
            Attack.OnShootEvent.Raise(new FloatEvent(Attack.MaximumAmmo, Attack.MaximumAmmo));

            // Set the clip speed of the reload animation to be equal as the reload delay
            anim.SetFloat("ReloadSpeed", 1 / Attack.ReloadSpeed);
        }

        if (Health == null)
        {
            Health = GetComponentInChildren<HealthModule>();
            Health.InitializeModule(Unit);

            Health.OnDeathEvent.AddListener(OnDeathListener);
            Health.OnHitEvent.AddListener(OnHitListener);

            OnPlayerHitEvent.Raise(new FloatEvent(Health.MaximumLife, Health.MaximumLife));

        }


        if (InstaPunch == null)
        {
            InstaPunch = GetComponentInChildren<InstaPunchModule>();
            InstaPunch.InitializeModule(Unit);

            InstaPunch.OnCanInstaPunchEvent.Raise(new FloatEvent(0));
            InstaPunch.OnInstaPunchEvent.AddListener(OnInstaPunchListener);
        }

    }

    private void OnDisable()
    {
        if (Health != null)
            Health.OnDeathEvent.RemoveListener(OnDeathListener);
    
        if(InstaPunch != null)
            InstaPunch.OnInstaPunchEvent.RemoveListener(OnInstaPunchListener);

    }



    private void Update()
    {
        if (Unit.IsDead)
            return;

        Input.UpdateInput();

        UpdateAnimation();

        if (Stealth != null && Stealth.IsActive)
            Stealth.UpdateModule();


        if (InstaPunch != null && InstaPunch.IsActive)
            InstaPunch.UpdateModule();

        if (Attack != null && Attack.IsActive)
            Attack.UpdateModule();

        // Movement needs to be the last one because some of the other actions might block the movement
        if (Movement != null && Movement.IsActive)
            Movement.UpdateModule();

        if(Unit.IsDucking)
        {
            if (!duckCollider.IsEqual(PlayerCollider))
            {
                duckCollider.Set(PlayerCollider);
            }
        }
        else if (!normalCollider.IsEqual(PlayerCollider))
        {
            normalCollider.Set(PlayerCollider);
        }

    }

    void UpdateAnimation()
    {
        // Update animator accordingly

        bool inAir = Movement.IsFalling || Movement.IsJumping;
        bool isDuck = !inAir && Unit.TryDuck;
        Unit.IsDucking = isDuck;

        anim.SetBool("FallParam", !Unit.IsReloading && Movement.IsFalling); // We're falling
        anim.SetBool("WalkParam", !Unit.IsReloading && !Unit.IsAttacking && !inAir && Unit.IsMoving);
        anim.SetBool("JumpParam", !Unit.IsReloading && !Movement.IsFalling && !Unit.IsAttacking && Unit.TryJump);
        anim.SetBool("ShootParam", !Unit.IsReloading && !inAir && Unit.IsShooting);
        anim.SetBool("PunchParam", !Unit.IsReloading && !inAir && Unit.IsPunching);
        anim.SetBool("ReloadParam", Unit.IsReloading);
        anim.SetBool("HurtParam", Unit.IsHurting);


        // We can reload while ducking or hiding for game design purposes
        anim.SetBool("DuckParam", isDuck);
        anim.SetBool("HideParam", !inAir && Unit.IsHiding);
    }

    void OnHitListener()
    {
        OnPlayerHitEvent.Raise(new FloatEvent(Health.CurrentLife, Health.MaximumLife));

        // TODO: Visual indication to show that player is almost dying 
    }

    void OnDeathListener()
    {
        // TODO: Make front and back param

        anim.SetBool("FrontDeathParam", true);

        // TODO: Checkpoint
        // Restarting level for now
        Unit.uVisual.FadeOut(respawnDuration, 1, () => UnityEngine.SceneManagement.SceneManager.LoadScene(gameObject.scene.buildIndex));

    }

    void OnInstaPunchListener()
    {
        if (Attack != null && Attack.IsActive)
        {
            // Do the actual attack
            Attack.DoPunch();

            // Then Insta reload
            Attack.DoFastReload();
        }
    }
}

[System.Serializable]
struct ColliderInformations
{
    public Vector2 offset;
    public Vector2 size;

    public void Setup(Vector2 _offset, Vector2 _size)
    {
        offset = _offset;
        size = _size;
    }

    public void Set(BoxCollider2D otherCollider)
    {
        otherCollider.offset = offset;
        otherCollider.size = size;
    }

    public bool IsEqual(BoxCollider2D otherCollider)
    {
        return otherCollider.size == size && otherCollider.offset == offset;
    }
}