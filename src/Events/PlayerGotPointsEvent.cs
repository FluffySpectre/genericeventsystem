using UnityEngine;
using System.Collections;

public class PlayerGotPointsEvent : Event {
	public int Points { get; set; }
	public string Action { get; set; }
}
