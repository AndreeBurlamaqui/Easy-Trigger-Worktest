using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Health module requires some collider2D to work
/// </summary>
public class HealthModule : MainModule
{
    [Tooltip("Whoever is able to hurt this unit")]
    public string[] hazardTags;

    [SerializeField] float maxLife;
    [SerializeField] float stunDuration;
    [SerializeField] float invencibilityDuration;

    [Header("DEATH DROP")]
    [SerializeField] GameObject objectSpawnOnDeath;
    [Tooltip("Range of chance for this to drop, from 0% to 100%")]
    [SerializeField] [Range(0,100)] float dropChance = 50;

    [Header("READ-ONLY")]
    [SerializeField] float curLife;

    [Header("EVENTS")]
    public UnityEvent OnHitEvent;
    public UnityEvent OnDeathEvent;

    WaitForSeconds waitStun;
    float _invencibilityTimer;


    public float CurrentLife => curLife;
    public float MaximumLife => maxLife;

    public bool CanBeHurt => _invencibilityTimer <= 0 && !Unit.IsDead && !Unit.IsHiding;

    public override void InitializeModule(UnitData _unit)
    {
        base.InitializeModule(_unit);
        curLife = maxLife;

        waitStun = new WaitForSeconds(stunDuration);
    }

    private void Update()
    {
        if (_invencibilityTimer >= 0)
            _invencibilityTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Unit.IsHurting || !CanBeHurt)
            return; // Already being hit


        if (TryGetHurtBy(collision.tag))
        {
            // If it's a bullet, let's remove it

            if(collision.TryGetComponent(out Bullet b))
            {
                Destroy(b.gameObject);
            }
        }
    }

    public bool TryGetHurtBy(string tag)
    {
        bool isHurt = System.Array.Exists(hazardTags, x => x == tag);

        if (isHurt)
            ApplyDamage();

        return isHurt;
    }

    public void ApplyDamage()
    {
        curLife -= 1;

        if (curLife <= 0)
        {
            // Death

            ApplyDeath();
        }
        else
        {
            // Normal Hit

            // If there's a invencibility period, activate it
            _invencibilityTimer = invencibilityDuration;

            // Could do a DOTween here to make unit blink in a yo-yo style
            // Also spawn some hit effect

            StartCoroutine(ApplyStunRoutine());
            OnHitEvent?.Invoke();
        }

    }

    public void ApplyDeath()
    {
        OnDeathEvent?.Invoke();
        Unit.IsDead = true;

        if (objectSpawnOnDeath != null && Random.Range(0f, 100f) <= dropChance)
            Instantiate(objectSpawnOnDeath, transform.position, Quaternion.identity);
    }

    private IEnumerator ApplyStunRoutine()
    {
        Unit.IsHurting = true;

        yield return waitStun;

        Unit.IsHurting = false;
    }
}
