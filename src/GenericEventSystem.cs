using System;
using System.Collections.Generic;

public abstract class GenericEventSystem {
	public abstract void CleanEventSystem();
}

/// <summary>
/// the actual generic event system class via which you can raise and get events and pass data
/// </summary>
/// <typeparam name="T">The type of the event for this system</typeparam>
public class GenericEventSystem<T> : GenericEventSystem {
	private static GenericEventSystem<T> eventSystemInstance;
	private Action<T> eventBackingDelegate;

	public static event Action<T> EventHappened {
		add {
			// if it's our first subscriber
			if (EventSystemInstance.eventBackingDelegate == null) {
				// we register this (newly) closed generic class in the Maintenance class
				// so we can easily clean it up later
				GenericEventSystemMaintenance.RegisterNewEventSystem(EventSystemInstance);
			}

			// and of cource we subscribe the provided method to this particular event
			EventSystemInstance.eventBackingDelegate += value;
		}

		remove { EventSystemInstance.eventBackingDelegate -= value; }
	}

	public static void Raise(T eventData) {
		EventSystemInstance.SafeRaise(eventData);
	}

	static void CleanCurrentEventSystem() {
		if (eventSystemInstance != null) {
			eventSystemInstance.CleanSubscribersList();
			// we set our instance to null, so we can check whether we have to create a new instance next time
			eventSystemInstance = null;
		}
	}

	static GenericEventSystem<T> EventSystemInstance {
		get { return eventSystemInstance ?? (eventSystemInstance = new GenericEventSystem<T>()); }
	}

	public override void CleanEventSystem() {
		// notice that we call a static method here
		GenericEventSystem<T>.CleanCurrentEventSystem();
	}

	void CleanSubscribersList() {
		eventBackingDelegate = null;
	}

	void SafeRaise(T eventData) {
		if (eventBackingDelegate != null) {
			eventBackingDelegate(eventData);
		}
	}

	GenericEventSystem() { }
}

/// <summary>
/// a maintenance class used to cleanup every used event system
/// </summary>
public static class GenericEventSystemMaintenance {
	static readonly List<GenericEventSystem> genericEventSystems = new List<GenericEventSystem>();

	public static void RegisterNewEventSystem(GenericEventSystem eventSystem) {
		// if we don't have this event system in our list yet
		if (!genericEventSystems.Contains(eventSystem)) {
			// only then we add it to the list
			genericEventSystems.Add(eventSystem);
		}
	}

	public static void CleanupEventSystem() {
		// for every registered event system
		foreach (var genericEventSystem in genericEventSystems) {
			// we let it clean itself of the subscribers and stuff
			genericEventSystem.CleanEventSystem();
		}

		// and clear our list, so it becomes fresh and empty
		genericEventSystems.Clear();
	}
}