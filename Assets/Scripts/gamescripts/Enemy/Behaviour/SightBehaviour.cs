using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SightBehaviour : MainBehaviour
{
    [SerializeField] LayerMask detectLayer;
    [SerializeField] Transform eyePoint;
    [SerializeField] Vector2 sightSize;
    [Tooltip("Does this Unit stop when seeing any target?")]
    public bool ShouldStopOnSight = true;

    // UnityEvents with parameters will not show on inspector due to some error in 2019 versions apparently
    // And also seems to break
    // So I'll use UnityAction instead
    public UnityAction<Transform> OnSightEvent;

    public bool SomethingOnSight => _onSightCount > 0;


    int _onSightCount;
    readonly Collider2D[] eyeSight = new Collider2D[5];

    public override void UpdateBehaviour()
    {
        // TODO: Maybe put a cooldown here to not check it every frame?

        _onSightCount = Physics2D.OverlapBoxNonAlloc(eyePoint.position, sightSize, 0, eyeSight, detectLayer);

        if (!SomethingOnSight)
            return;

        foreach (Collider2D col in eyeSight)
        {
            if (col == null)
                continue; 

            if (col.transform == null)
                continue;

            // Check if it has a stealth module
            // If so, check if it's hiding or not
            if (col.TryGetComponent(out StealthModule sm))
            {
                if (sm.Unit.IsHiding)
                {
                    _onSightCount--;
                    continue;
                }
            }

            OnSightEvent?.Invoke(col.transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (eyePoint != null)
            Gizmos.DrawWireCube(eyePoint.position, sightSize);
    }
}
