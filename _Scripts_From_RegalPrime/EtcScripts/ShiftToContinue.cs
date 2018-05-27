// RegalPrime 12-02-14 - ShiftToContinue.cs

// Moves to the next scene when shift is pressed

using UnityEngine;
using System.Collections;

public class ShiftToContinue : MonoBehaviour
{

	private int currentLevel;
	private int maxLevelIndex;
	
	void Start()
	{
		currentLevel = (Application.loadedLevel);
		maxLevelIndex = Application.levelCount - 1;
		
		
		//		print ("Current Level   = " + currentLevel);
		//		print ("Level Count     = " + Application.levelCount);
		//		print ("Max Level Index = " + maxLevelIndex);
	}
	
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
		{
			if((currentLevel+1) <= maxLevelIndex)
				Application.LoadLevel (currentLevel+1);
			else
				print ("NOT ENOUGH LEVELS");
		}
	}
}
