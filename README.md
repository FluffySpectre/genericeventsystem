# Generic Event System

This approach keeps the event data and the event management logic separate. It makes use of a templated static class which removes the need for a specialized event base class and explicit type casting.

## Usage:

### Emit Event

    using BetterEventSystem;

    // define a event class
    public class EnemyKilledEvent {
	    public string EnemyName { get; set; }
	    public string KillerName { get; set; }
    }

    // setup event object
    EnemyKilledEvent e = new EnemyKilledEvent();
    e.EnemyName = "Goblin";
    e.KillerName = "Slayer X";

    // emit the event
    EventSystem<EnemyKilledEvent>.Emit(e);

### Listen for Event

    using BetterEventSystem;

    EventSystem<EnemyKilledEvent>.EventHappened += (e) => {
        Console.WriteLine(e.EnemyName + " killed by " + e.KillerName);

        // do awesome stuff...
    };