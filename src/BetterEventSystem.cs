using System;
using System.Collections.Generic;

public abstract class BetterEventSystem {
	public abstract void CleanEventSystem();
}

/// <summary>
/// the actual generic event system class via which you can raise and get events and pass data
/// </summary>
/// <typeparam name="T">The type of the event for this system</typeparam>
public class BetterEventSystem<T> : BetterEventSystem {
	private static BetterEventSystem<T> eventSystemInstance;
	private Action<T> eventBackingDelegate;

	public static event Action<T> EventHappened {
		add {
			// if it's our first subscriber
			if (EventSystemInstance.eventBackingDelegate == null) {
				// we register this (newly) closed generic class in the Maintenance class
				// so we can easily clean it up later
				BetterEventSystemMaintenance.RegisterNewEventSystem(EventSystemInstance);
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

	static BetterEventSystem<T> EventSystemInstance {
		get { return eventSystemInstance ?? (eventSystemInstance = new BetterEventSystem<T>()); }
	}

	public override void CleanEventSystem() {
		// notice that we call a static method here
		BetterEventSystem<T>.CleanCurrentEventSystem();
	}

	void CleanSubscribersList() {
		eventBackingDelegate = null;
	}

	void SafeRaise(T eventData) {
		if (eventBackingDelegate != null) {
			eventBackingDelegate(eventData);
		}
	}

	BetterEventSystem() { }
}

/// <summary>
/// a maintenance class used to cleanup every used event system
/// </summary>
public static class BetterEventSystemMaintenance {
	static readonly List<BetterEventSystem> betterEventSystems = new List<BetterEventSystem>();

	public static void RegisterNewEventSystem(BetterEventSystem eventSystem) {
		// if we don't have this event system in our list yet
		if (!betterEventSystems.Contains(eventSystem)) {
			// only then we add it to the list
			betterEventSystems.Add(eventSystem);
		}
	}

	public static void CleanupEventSystem() {
		// for every registered event system
		foreach (var betterEventSystem in betterEventSystems) {
			// we let it clean itself of the subscribers and stuff
			betterEventSystem.CleanEventSystem();
		}

		// and clear our list, so it becomes fresh and empty
		betterEventSystems.Clear();
	}
}