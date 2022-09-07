using System.Collections;
using System.Collections.Generic;
using TinyCacto.Effects;
using UnityEngine;
using UnityEngine.Events;

public class InstaPunchModule : MainModule
{
    public GameEvent OnCanInstaPunchEvent;

    [SerializeField] float movePunchSpeed = 1;
    [SerializeField] float trailInterval = 0.3f;
    [SerializeField] float trailFadeOut = 0.75f;

    public UnityEvent OnInstaPunchEvent;
    public bool CanInstaPunch => AvailableToPunch.Count > 0;

    List<Transform> AvailableToPunch = new List<Transform>();
    bool instaPunchTooltip = false;

    public override void InitializeModule(UnitData _unit)
    {
        base.InitializeModule(_unit);

        OnCanInstaPunchEvent.AddListener<TransformEvent>(AddToList);
    }

    private void OnDisable()
    {
        OnCanInstaPunchEvent.RemoveListener<TransformEvent>(AddToList);
    }

    void AddToList(TransformEvent t)
    {
        if (AvailableToPunch.Contains(t.transform))
        {
            // If it's a clone, remove it, as we're *probably* sending to remove the warning
            AvailableToPunch.Remove(t.transform);
        }
        else
        {
            AvailableToPunch.Add(t.transform);
        }

        if (IsPlayer && !instaPunchTooltip)
        {
            OnCanInstaPunchEvent.Raise(new FloatEvent(AvailableToPunch.Count));

            instaPunchTooltip = true;
        }
    }

    public override void UpdateModule()
    {
        base.UpdateModule();

        if (CanInstaPunch)
        {

            // if we click to punch when there's someone to insta punch, teleport to it

            if (Unit.TryPunch && !Unit.IsHiding)
            {
                // Move
                for(int t = 0; t < AvailableToPunch.Count; t++)
                {
                    if (AvailableToPunch[t] == null)
                    {
                        AvailableToPunch.RemoveAt(t);
                        break;
                    }

                    Unit.IsPunching = true;

                    // Move
                    transform.ForceMove(AvailableToPunch[t].position, movePunchSpeed, DoInstaPunch);

                    // Effects
                    if (Unit.uVisual != null)
                        Unit.uVisual.SpawnGhostTrail(trailInterval, movePunchSpeed, trailFadeOut);

                    AvailableToPunch.RemoveAt(t);
                    
                    break;
                }
            }
        }
        else if(instaPunchTooltip)
        {
            // Do a cleanup check
            foreach (Transform cleanupT in AvailableToPunch)
                if (cleanupT == null)
                    AvailableToPunch.Remove(cleanupT);

            if (IsPlayer)
                OnCanInstaPunchEvent.Raise(new FloatEvent(0));

            instaPunchTooltip = false;
        }
    }

    public void DoInstaPunch()
    {
        // Then punch
        OnInstaPunchEvent?.Invoke();

        // UI will check only for the Float Events, so we can send it without worry
        if(IsPlayer)
            OnCanInstaPunchEvent.Raise(new FloatEvent(0));

        instaPunchTooltip = false;
    }
}

public class TransformEvent : Event
{
    public Transform transform;

    public TransformEvent(Transform t) => transform = t;
}
