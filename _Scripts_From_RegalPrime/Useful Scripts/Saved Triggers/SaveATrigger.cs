// RegalPrime 4-21-15 - SaveATrigger.cs

// When this trigger is hit, the GameController saves the trigger index to the saved file.

// HardSavePosition = true  - Will update that trigger number and save your game at that position
// HardSavePosition = false - Will update that trigger number and not save your game at that position (it will update your save file though) 


using UnityEngine;
using System.Collections;

public class SaveATrigger : MonoBehaviour
{
	public int SaveThisTriggerIndex = 0;
	public bool TriggerBooleanValue = false;

	public bool HardSavePosition = false;	// If this is true, it will save your game at that position


	// Checks to see if the value is already the same as in the saved array. If so, it disables itself.
	void Start()
	{
		bool[] TempArray = GameController.control.Get_SavedTriggers ();

		if(0 <= SaveThisTriggerIndex && SaveThisTriggerIndex < TempArray.Length)
		{
			if(TempArray[SaveThisTriggerIndex] == TriggerBooleanValue)
				gameObject.SetActive (false);
		}
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			GameController.control.UpdateSavedTrigger(SaveThisTriggerIndex, TriggerBooleanValue);

			if(HardSavePosition)
				GameController.control.Save ();
			else
				GameController.control.SaveTimerDeath ();

			gameObject.SetActive (false);
		}
	}
}
