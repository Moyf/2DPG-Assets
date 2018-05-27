using UnityEngine;
using System.Collections;

public class FORWARDaRoom : MonoBehaviour
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
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			if((currentLevel+1) <= maxLevelIndex)
				Application.LoadLevel (currentLevel+1);
			else
				print ("NOT ENOUGH LEVELS");
		}
	}

}