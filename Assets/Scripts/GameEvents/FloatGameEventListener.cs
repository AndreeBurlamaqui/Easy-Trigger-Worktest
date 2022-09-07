using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FloatGameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public GameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<float[]> Response;

    private void OnEnable()
    {
        Event.AddListener<FloatEvent>(OnEventRaised);
    }

    private void OnDisable()
    {
        Event.RemoveListener<FloatEvent>(OnEventRaised);
    }

    public void OnEventRaised(FloatEvent se)
    {
        Response.Invoke(se.value);
    }
}

public class FloatEvent : Event
{
    public float[] value;

    public FloatEvent(params float[] v) => value = v;
}
