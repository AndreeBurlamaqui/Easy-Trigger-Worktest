using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Will not use custom classes. So it just receives the raise call with no parameter inside it.
/// </summary>
public class SimpleGameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public GameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent Response;

    private void OnEnable()
    {
        Event.AddListener<SimpleEvent>(OnEventRaised);
    }

    private void OnDisable()
    {
        Event.RemoveListener<SimpleEvent>(OnEventRaised);
    }

    public void OnEventRaised(SimpleEvent se)
    {
        Response.Invoke();
    }
}