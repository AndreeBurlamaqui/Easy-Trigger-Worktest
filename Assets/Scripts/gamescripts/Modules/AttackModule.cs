using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackModule : MainModule
{
    [Tooltip("Time in seconds that will freeze the unit while attacking")]
    [SerializeField] float attackRest = 1;

    [Header("SHOOTING")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform gunPoint;
    [SerializeField] float shootMaxCooldown = 3;
    [SerializeField] float maxAmmo = 8;
    [SerializeField] float reloadDelay = 2;

    [Header("PUNCHING")]
    [SerializeField] Transform punchPoint;
    [SerializeField] float punchMaxCooldown = 3;
    [SerializeField] string punchTag;
    [SerializeField] Vector2 punchSize;


    [Header("READ-ONLY")]
    [SerializeField] float curAmmo;

    [Header("PLAYER")]
    public GameEvent OnShootEvent;

    float _attackCooldown;
    float _shootCooldown;
    float _punchCooldown;
    WaitForSeconds waitReload;

    public bool CanShoot => _shootCooldown <= 0;
    public bool CanPunch => _punchCooldown <= 0;
    public bool HasAmmo => curAmmo > 0;

    public float MaximumAmmo => maxAmmo;
    public float CurrentAmo => curAmmo;
    public float ReloadSpeed => reloadDelay;

    private void Update()
    {
        if (_shootCooldown >= 0)
            _shootCooldown -= Time.deltaTime;

        if (_punchCooldown >= 0)
            _punchCooldown -= Time.deltaTime;

        if (_attackCooldown >= 0)
        {
            _attackCooldown -= Time.deltaTime;
        }
        else if (Unit.IsShooting)
        {
            Unit.IsShooting = false;
        } 
        else if (Unit.IsPunching)
        {
            Unit.IsPunching = false;
        }
    }

    public override void InitializeModule(UnitData _unit)
    {
        base.InitializeModule(_unit);
        curAmmo = maxAmmo;

        waitReload = new WaitForSeconds(reloadDelay);
    }

    public override void UpdateModule()
    {
        if (Unit.IsAttacking)
            return; // already shooting or punching


        // PUNCHING
        // If we have insta punch, check if we're going to insta punch instead of a normal punch
        if (Unit.TryPunch )
        {
            TryPunch();
        }

        // SHOOTING
        if (Unit.TryShoot) 
        {
            TryShoot();
        }
    }


    #region PUNCHING

    public void TryPunch()
    {
        if (!CanPunch)
            return;

        // Punch will override any state, so nothing can block it
        DoPunch();
    }
    public void DoPunch()
    {
        Unit.IsPunching = true;

        // restart cooldown
        _punchCooldown = punchMaxCooldown;
        _attackCooldown = attackRest;

        foreach (Collider2D col in Physics2D.OverlapBoxAll(punchPoint.position, punchSize, 0))
        {
            if (col.TryGetComponent(out HealthModule hm))
                hm.TryGetHurtBy(punchTag);
        }

    }

    #endregion

    #region SHOOTING

    public void TryShoot()
    {
        if (!CanShoot)
            return; // Unit on cooldown or not trying to shoot 

        if (!HasAmmo )
        {
            if (!Unit.IsReloading)
            {
                // Reload
                StartCoroutine(DoReloadRoutine());
            }

            // And wait until Unit has ammo
            return;
        }

        // These states will only block the actual shooting, so the Unit can still recharge while doing them
        bool availableToAttack = !Unit.IsJumping && !Unit.IsFalling && !Unit.IsDucking;

        if (availableToAttack)
            DoShoot();
        
    }

    public void DoShoot()
    {
        Unit.IsShooting = true;

        // Restart cooldown
        _shootCooldown = shootMaxCooldown;
        _attackCooldown = attackRest;

        // Play sound
        PlayAudio("Gun");

        // Discount bullet
        curAmmo--;

        // Instantiate bullet
        float spawnDir = transform.localScale.x > 0 ? 0f : 180f;
        Instantiate(bulletPrefab, gunPoint.position, Quaternion.Euler(Quaternion.identity.x, Quaternion.identity.y, spawnDir));

        // Warn whoever wants to listen that this unit shot and send it's current ammo
        if (IsPlayer)
            OnShootEvent.Raise(new FloatEvent(CurrentAmo, MaximumAmmo));
        
    }
    public void DoFastReload()
    {
        curAmmo = maxAmmo;

        if (IsPlayer)
            OnShootEvent.Raise(new FloatEvent(MaximumAmmo, MaximumAmmo));
    }

    public IEnumerator DoReloadRoutine()
    {
        if (IsPlayer)
            OnShootEvent.Raise(new FloatEvent(0, MaximumAmmo));

        Unit.IsReloading = true;

        yield return waitReload;

        Unit.IsReloading = false;

        curAmmo = maxAmmo;

        if (IsPlayer)
            OnShootEvent.Raise(new FloatEvent(MaximumAmmo, MaximumAmmo));
        
    }

    #endregion



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if(punchPoint != null)
            Gizmos.DrawWireCube(punchPoint.position, punchSize);
    }

}
