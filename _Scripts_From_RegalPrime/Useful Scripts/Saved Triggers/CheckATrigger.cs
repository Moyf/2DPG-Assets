// RegalPrime 4-21-15 - CheckATrigger.cs

// Checks to see if a certain index in the GameController's SavedTriggers array is valid.

using UnityEngine;
using System.Collections;

public class CheckATrigger : MonoBehaviour
{
	public int SavedTriggersIndex = 0;			// Check which index of the SavedTriggers array in the GameController
	public bool SaveTriggersValue = true;		// Check which value of the above returned trigger

	public bool EnableOrDisable = false;		// Enable or disable this object if the above statement is true


	void Start ()
	{
		bool[] TempArray = GameController.control.Get_SavedTriggers ();

		if(0 <= SavedTriggersIndex && SavedTriggersIndex < TempArray.Length)
		{
			if(TempArray[SavedTriggersIndex] == SaveTriggersValue)
				gameObject.SetActive (EnableOrDisable);
		}
		else
			print (string.Format("CheckATrigger.cs - WARNING - {0} is out of bounds for array SavedTriggers. Max index = {1}", SavedTriggersIndex, TempArray.Length-1));
	}
	
}
