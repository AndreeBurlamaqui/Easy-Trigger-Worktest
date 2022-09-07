using System.Collections;
using UnityEngine;
using TinyCacto.Effects;

public class Enemy : MonoBehaviour
{
    [Header("ENEMY INFORMATIONS")]
    [SerializeField] UnitData Unit;

    [Header("DEATH")]
    [SerializeField] float destroyDelay = 2;
    [SerializeField] GameEvent OnInstaPunchEvent;

    // MODULES
    MovementModule Movement;
    AttackModule Attack;
    HealthModule Health;

    // BEHAVIOURS
    PatrolBehaviour Patrol;
    SightBehaviour Sight;

    // COMPONENTS
    Animator anim;
    BoxCollider2D EnemyCollider;

    private void Start()
    {
        // Components

        if (EnemyCollider == null)
            EnemyCollider = GetComponentInChildren<BoxCollider2D>();

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        // Unit
        Unit = Instantiate(Unit); // On enemies, the scriptable needs to be unique, so we need to instantiate it
        Unit.InitializeUnit(gameObject);

        // Modules

        if (Movement == null)
        {
            Movement = GetComponentInChildren<MovementModule>();
            Movement.InitializeModule(Unit);
        }

        if (Attack == null)
        {
            Attack = GetComponentInChildren<AttackModule>();
            Attack.InitializeModule(Unit);
        }

        if (Health == null)
        {
            Health = GetComponentInChildren<HealthModule>();
            Health.InitializeModule(Unit);

            Health.OnDeathEvent.AddListener(OnDeathListener);
            Health.OnHitEvent.AddListener(OnHitListener);
        }

        // Behaviours

        if (Patrol == null)
        {
            Patrol = GetComponentInChildren<PatrolBehaviour>();
            Patrol.InitializePatrol(Unit, Movement);
        }

        if (Sight == null)
        {
            Sight = GetComponentInChildren<SightBehaviour>();
            Sight.OnSightEvent += (OnSightListener);
        }

    }

    private void OnDisable()
    {
        if (Sight != null)
            Sight.OnSightEvent -= (OnSightListener);

        if(Health != null)
            Health.OnDeathEvent.RemoveListener(OnDeathListener);

    }

    private void Update()
    {
        if (Unit.IsDead)
            return;

        UpdateAnimation();

        // In case we don't have Sight Module
        bool stopOnSight = false;

        if (Sight != null && Sight.IsActive)
        {
            Sight.UpdateBehaviour();

            stopOnSight = Sight.SomethingOnSight && Sight.ShouldStopOnSight;
        }


        // Patrol at last because it's a movement module in the end
        if (Patrol != null && Patrol.IsActive)
        {
            if(Patrol.IsPatrolling)
                Unit.Horizontal = !Movement.IsGrounded || Unit.IsHurting || stopOnSight ? 0 : Patrol.curDir;

            Patrol.UpdateBehaviour();

        }

    }

    void UpdateAnimation()
    {
        // Update animator accordingly

        anim.SetBool("WalkParam", !Unit.IsAttacking && Unit.IsMoving);
        anim.SetBool("ShootParam", !Unit.IsReloading && Unit.IsShooting);
        anim.SetBool("HitParam", Unit.IsHurting);

    }

    public void OnSightListener(Transform target)
    {
        if (Unit.IsAttacking)
            return;

        // TODO: Check where it is in case we need to flip/turn towards them
        // By doing this we can improve the sight part so that we have more freedom on enemies types

        // Shoot
        Attack.TryShoot();
    }

    void OnDeathListener()
    {
        Unit.Horizontal = 0;
        Patrol.UpdateBehaviour();

        // TODO: Make front and back param

        anim.SetBool("FrontDeathParam", true);

        // We could use DOTween to avoid needing a routine
        // So we could do a blink fade-out effect and remove enemy after that
        Unit.uVisual.FadeOut(destroyDelay, 1, DestroyEnemy);

        // Remove colliders so no unit can mess with it anymore
        if (Unit.uCollider != null)
            Destroy(Unit.uCollider);

        // Remove gravity because without collider it'll start to fall
        if (Unit.uRigibody != null)
            Unit.uRigibody.bodyType = RigidbodyType2D.Static;

    
    }

    void OnHitListener()
    {
        // Check if enemy is almost dying
        // If so, make it so it can be killed by insta punch
        if (Health != null && Health.CurrentLife <= 1)
        {
            OnInstaPunchEvent.Raise(new TransformEvent(transform));

            if (Unit.uVisual != null)
                Unit.uVisual.color = Color.red;
        }
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);

        // Check if it was the only one that could be insta punchable
        OnInstaPunchEvent.Raise(new TransformEvent(transform));
    }
}