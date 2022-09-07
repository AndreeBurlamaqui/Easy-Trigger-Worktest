using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoolGameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public GameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<bool> Response;

    private void OnEnable()
    {
        Event.AddListener<BoolEvent>(OnEventRaised);
    }

    private void OnDisable()
    {
        Event.RemoveListener<BoolEvent>(OnEventRaised);
    }

    public void OnEventRaised(BoolEvent se)
    {
        Response.Invoke(se.state);
    }
}

public class BoolEvent : Event
{
    public bool state;

    public BoolEvent(bool value) => state = value;
}