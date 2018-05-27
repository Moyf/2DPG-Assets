// RegalPrime 11-29-14 - ExistForDifficulty.cs

// Finds which difficulty the player is in
// If this object does not have the correct permissions, it is disabled


using UnityEngine;
using System.Collections;

public class ExistForDifficulty : MonoBehaviour
{
	public bool AllowForEasy = true;
	public bool AllowForMedium = true;
	public bool AllowForHard = true;

	void Start ()
	{
		int Difficulty = GameController.control.Difficulty;

		if(!AllowForEasy && Difficulty == 0)	gameObject.SetActive(false);
		if(!AllowForMedium && Difficulty == 1)	gameObject.SetActive(false);
		if(!AllowForHard && Difficulty == 2)	gameObject.SetActive(false);
	}
}
