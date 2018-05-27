// RegalPrime 4-21-15 - GameCompletedTrigger.cs

// When this trigger is hit, the GameController sets the game completed trigger to true.

// HardSavePosition = true  - Will update that trigger number and save your game at that position
// HardSavePosition = false - Will update that trigger number and not save your game at that position (it will update your save file though) 


using UnityEngine;
using System.Collections;

public class GameCompletedTrigger : MonoBehaviour
{
	public bool HardSavePosition = false;	// If this is true, it will save your game at that position
	
	void Start()
	{
		if(GameController.control.IsCompleted() == true)
			gameObject.SetActive (false);
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			GameController.control.CompleteGame();
			
			if(HardSavePosition)
				GameController.control.Save ();
			else
				GameController.control.SaveTimerDeath ();
			
			gameObject.SetActive (false);
		}
	}
}
