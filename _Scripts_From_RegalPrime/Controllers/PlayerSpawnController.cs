// RegalPrime 12-02-14 - PlayerSpawnController.cs

// Used when spawning the character from a save file / between scenes

// WARNING - This uses a static reference to itself, which means if more than one of these objects are on the screen at a time, there may be problems
// A this point and time, it is not an issue, but something may come up later and I will rewrite the code as necessary

// There is 2 commented out lines in the Awake() function
// These will force a certain guy prefab to spawn instead of what was put in the GuyPrefab location
// It is an easy way to change who spawns throughout each scene.


using UnityEngine;
using System.Collections;

public class PlayerSpawnController : MonoBehaviour
{
	public static PlayerSpawnController spawnguy;			// Reference to this script
	public GameObject GuyPrefab;							// Object to spawn

	public Transform[] spawnPoints;							// Location of the available spawnpoints

	void Awake()
	{
		spawnguy = this;				// Set the static reference
//		tag = "PlayerSpawner";			// Set an object with this script to the PlayerSpawner tag

		// THIS IS AN EASY WAY TO CHANGE WHICH GUY IS USED IN THE ENTIRE GAME
//		GuyPrefab =  (GameObject)Resources.Load("GuyCharacterPrefab_Physics_Resource") as GameObject;
//		GuyPrefab =  (GameObject)Resources.Load("GuyCharacterPrefab_Pixel_Resource") as GameObject;

		if(spawnPoints.Length == 0)		// If there are no spawn points added to the array, the object itself becomes the default spawnpoint
			spawnPoints = new Transform[] {gameObject.transform};

	}
	
	void Start()
	{
		if(GameController.control.LoadingFile ()) // If loading from file - spawn at saved position and reset trigger
		{
			GameController.control.GuyLocation = Instantiate (GuyPrefab, new Vector3(GameController.control.LoadedX, GameController.control.LoadedY, 0), transform.rotation) as GameObject;
			GameController.control.SetLoadingFile(false);
		}
		else // This spawns the character usually when switching scenes
		{
			if(GameController.control.RememberIndex () >= spawnPoints.Length)	// If the index is too high for the number of spawn points - error and set to default spawn index = 0;
			{
				print ("ERROR in navigateRoom.cs object - Looking for Index (" +GameController.control.RememberIndex()+ "). Array max index = ("+ ((int)(spawnPoints.Length)-1) +")" );
				GameController.control.SetRememberIndex (0);
			}

			Vector3 spawnPosition = spawnPoints[GameController.control.RememberIndex()].position;
			spawnPosition.z = 0;
			GameController.control.GuyLocation = Instantiate (GuyPrefab, spawnPosition, Quaternion.identity) as GameObject;

			GameController.control.SetRememberIndex (0);
		}
	}
}