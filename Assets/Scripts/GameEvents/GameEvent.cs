using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
	public delegate void EventDelegate<T>(T e) where T : Event;
	private delegate void EventDelegate(Event e);

	private Dictionary<System.Type, EventDelegate> delegates = new Dictionary<System.Type, EventDelegate>();
	private Dictionary<System.Delegate, EventDelegate> delegateLookup = new Dictionary<System.Delegate, EventDelegate>();

	public void AddListener<T>(EventDelegate<T> del) where T : Event
	{
		// Early-out if we've already registered this delegate
		if (delegateLookup.ContainsKey(del))
			return;

		// Create a new non-generic delegate which calls our generic one.
		// This is the delegate we actually invoke.
		EventDelegate internalDelegate = (e) => del((T)e);
		delegateLookup[del] = internalDelegate;

		EventDelegate tempDel;
		if (delegates.TryGetValue(typeof(T), out tempDel))
		{
			delegates[typeof(T)] = tempDel += internalDelegate;
		}
		else
		{
			delegates[typeof(T)] = internalDelegate;
		}
	}

	public void RemoveListener<T>(EventDelegate<T> del) where T : Event
	{
		EventDelegate internalDelegate;
		if (delegateLookup.TryGetValue(del, out internalDelegate))
		{
			EventDelegate tempDel;
			if (delegates.TryGetValue(typeof(T), out tempDel))
			{
				tempDel -= internalDelegate;
				if (tempDel == null)
				{
					delegates.Remove(typeof(T));
				}
				else
				{
					delegates[typeof(T)] = tempDel;
				}
			}

			delegateLookup.Remove(del);
		}
	}

	/// <summary>
	/// Generic raise call. Can send specific informations.
	/// </summary>
	/// <param name="e">Specific <see cref="Event"/> type to send informations.</param>
	public void Raise(Event e)
	{
		EventDelegate del;
		if (delegates.TryGetValue(e.GetType(), out del))
		{
			del.Invoke(e);
		}
	}

	/// <summary>
	/// Empty raise. Calls a <see cref="SimpleEvent"/>
	/// </summary>
	public void Raise()
	{
		Raise(new SimpleEvent());
	}
}

public class Event { }
public class SimpleEvent : Event
{
    // Empty event for null usages
}
