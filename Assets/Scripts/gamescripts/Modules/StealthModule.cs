using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StealthModule : MainModule
{
    [SerializeField] LayerMask hideLayer;
    public GameEvent OnCanHideEvent;

    Vector2 checkSize;
    bool hidingHere = false;
    bool canHideHere = true; // Start as true so it's always sending that we can't hide here at start

    readonly Collider2D[] hideCols = new Collider2D[1];

    public override void InitializeModule(UnitData _unit)
    {
        base.InitializeModule(_unit);

        checkSize = Unit.uCollider.bounds.size;
    }

    public override void UpdateModule()
    {
        // Check if player can hide
        int newHideableSpots = Physics2D.OverlapBoxNonAlloc(Unit.uCollider.bounds.center, checkSize, 0, hideCols, hideLayer);

        if(newHideableSpots > 0)
        {
            // Warn UI
            if(IsPlayer && !canHideHere)    
                OnCanHideEvent.Raise(new BoolEvent(true));

            // Can hide, check if input is pressed
            if (hidingHere != Unit.TryHide || Unit.IsAttacking)
            {
                hidingHere = Unit.TryHide && !Unit.IsAttacking;
                Unit.IsHiding = hidingHere;
            }

            canHideHere = true;
        }
        else if(canHideHere)
        {
            canHideHere = false;

            if (IsPlayer)
                OnCanHideEvent.Raise(new BoolEvent(false));
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Unit != null)
            Gizmos.DrawWireCube(Unit.uCollider.bounds.center, Unit.uCollider.bounds.size);
    }
}
