using System;
using System.Collections.Generic;

namespace GenericEventSystem
{
	public abstract class EventSystem {
		public abstract void CleanEventSystem();
	}

	/// <summary>
	/// the actual generic event system class via which you can raise and get events and pass data
	/// </summary>
	/// <typeparam name="T">The type of the event for this system</typeparam>
	public class EventSystem<T> : EventSystem {
		static EventSystem<T> eventSystemInstance;
		Action<T> eventBackingDelegate;

		public static event Action<T> EventHappened {
			add {
				// if it's our first subscriber
				if (EventSystemInstance.eventBackingDelegate == null) {
					// we register this (newly) closed generic class in the Maintenance class
					// so we can easily clean it up later
					EventSystemMaintenance.RegisterNewEventSystem(EventSystemInstance);
				}

				// and of cource we subscribe the provided method to this particular event
				EventSystemInstance.eventBackingDelegate += value;
			}

			remove { EventSystemInstance.eventBackingDelegate -= value; }
		}

		public static void Emit(T eventData) {
			EventSystemInstance.SafeEmit(eventData);
		}

		static void CleanCurrentEventSystem() {
			if (eventSystemInstance != null) {
				eventSystemInstance.CleanSubscribersList();
				// we set our instance to null, so we can check whether we have to create a new instance next time
				eventSystemInstance = null;
			}
		}

		static EventSystem<T> EventSystemInstance {
			get { return eventSystemInstance ?? (eventSystemInstance = new EventSystem<T>()); }
		}

		public override void CleanEventSystem() {
			// notice that we call a static method here
			EventSystem<T>.CleanCurrentEventSystem();
		}

		void CleanSubscribersList() {
			eventBackingDelegate = null;
		}

		void SafeEmit(T eventData) {
			if (eventBackingDelegate != null) {
				eventBackingDelegate(eventData);
			}
		}

		EventSystem() {
		}
	}

	/// <summary>
	/// a maintenance class used to cleanup every used event system
	/// </summary>
	public static class EventSystemMaintenance {
		static readonly List<EventSystem> eventSystems = new List<EventSystem>();

		public static void RegisterNewEventSystem(EventSystem eventSystem) {
			// if we don't have this event system in our list yet
            if (!eventSystems.Contains(eventSystem)) {
				// only then we add it to the list
                eventSystems.Add(eventSystem);
			}
		}

		public static void CleanupEventSystem() {
			// for every registered event system
            foreach (var eventSystem in eventSystems) {
				// we let it clean itself of the subscribers and stuff
                eventSystem.CleanEventSystem();
			}

			// and clear our list, so it becomes fresh and empty
            eventSystems.Clear();
		}
	}
}
