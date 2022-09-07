using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementModule))]
/// <summary>
/// Patrol by checking if is grounded or if its hitting a wall
/// </summary>
public class PatrolBehaviour : MainBehaviour
{
    [Tooltip("Time that it'll be idling when reaching the border of the patrol waypoint")]
    [SerializeField] float restDuration = 2;
    [SerializeField] Vector2 wallCheckSize;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform wallCheckPoint;

    MovementModule Movement;
    UnitData Unit;

    bool patrolling = true;
    [HideInInspector] public int curDir = 1;
    bool canRest = true;

    readonly Collider2D[] wallsAtFront = new Collider2D[1];

    WaitForSeconds waitRest;


    public bool IsPatrolling => patrolling;
    //public bool Is

    public void InitializePatrol(UnitData _unit, MovementModule _movModule)
    {
        Unit = _unit;
        Movement = _movModule;

        waitRest = new WaitForSeconds(restDuration);
        
        patrolling = true;
        curDir = 1;

    }

    public override void UpdateBehaviour()
    {
        if (Movement != null && Movement.IsActive)
            Movement.UpdateModule();

        if (patrolling && canRest && NeedToTurn())
            StartCoroutine(PatrolRestRoutine()); // If unit was patrolling but isn't grounded anymore, it needs to stop and turn

    }

    public bool NeedToTurn()
    {
        // If not grounded or there's a wall in front
        return !Movement.IsGrounded || Physics2D.OverlapBoxNonAlloc(wallCheckPoint.position, wallCheckSize, 0, wallsAtFront, groundMask) > 0;
    }


    private IEnumerator PatrolRestRoutine()
    {
        // Turn and then rest for a bit

        patrolling = false;
        canRest = false;
        Unit.Horizontal = 0;
        curDir *= -1;

        yield return waitRest;

        Movement.Turn();
        patrolling = true;

        yield return waitRest;

        canRest = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (wallCheckPoint != null)
            Gizmos.DrawWireCube(wallCheckPoint.position, wallCheckSize);
    }
}
