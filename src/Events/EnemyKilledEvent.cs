using UnityEngine;
using System.Collections;

public class EnemyKilledEvent : Event {
	public string EnemyName { get; set; }
	public string KillerName { get; set; }
}
