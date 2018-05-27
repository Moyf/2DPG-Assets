// RegalPrime 11-25-14 - NavigateRooms.cs

// When triggered, the player will be warped to a certain scene depending on which options are selected (if able)
// warpForwardARoom   - Will move +1 a scene
// warpBackwardsARoom - Will move -1 a scene
// warpToACertainRoom - Will warp to a certain scene (scene index will be the value of warpToThisRoomIndex)

// Warping in the same room will allow to jump to a certain index in that room
// It would be easier to use the TeleportInRoom script to do exact in-scene warpings though

// warpToThisSpawnPointIndex is at which index the player will spawn at in the new scene
// It talks to the GameController so that it will know which index to spawn at in the next screen
// This index is set in the PlayerSpawnController object and has the option to expand the vector of waypoints to allow for additional options
// warpToThisSpawnPointIndex = 0 - is the default spawn position that is built into the PlayerSpawnController

using UnityEngine;
using System.Collections;

public class NavigateRooms : MonoBehaviour
{
	public int warpToThisSpawnPointIndex = 0;		// This is the index in an array of locations to warp at - Default = 0;

	// Only one of the following bool may be selected at once.
	public bool warpInSameRoom = false;				// Stay in the same room (but goto the above index)
	public bool warpForwardARoom = false;			// Waprs the player forward a scene
	public bool warpBackwardsARoom = false;			// Warps the player back a scene

	public bool warpToACertainRoom = false;			// Warp to this exact room index (be careful when using this option)
	public int warpToThisRoomIndex = 0;				// index of the room

	private int currentLevel;
	private int maxLevelIndex;

	private int triggerCount = 0;					// Checks how many options are selected
	private int collisionCount = 0;					// Makes sure multi-collision player objects only trigger this object once

	private Transform[] spawnPoints;				// All of the spawn points on the screen


	void Start()
	{
		if(PlayerSpawnController.spawnguy != null)
			spawnPoints = PlayerSpawnController.spawnguy.spawnPoints;		// Get the spawnpoints on this screen

		// Make sure only one option is selected
		if(warpInSameRoom == true)
			triggerCount++;
		if(warpForwardARoom == true)
			triggerCount++;
		if(warpBackwardsARoom == true)
			triggerCount++;
		if(warpToACertainRoom == true)
			triggerCount++;

		if(triggerCount == 0)
			print ("ERROR in NavigateRoom.cs object - You need to select at least one option for this script to work");

		if(triggerCount > 1)
			print ("ERROR in NavigateRoom.cs object - Too many options turned to true - Only pick one per object");

		currentLevel = (Application.loadedLevel);
		maxLevelIndex = Application.levelCount - 1;

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player"&&collisionCount<1)			// Only the player may trigger this
		{
			collisionCount++;

			if(warpInSameRoom)
			{
				WarpingToSameRoom(other.gameObject);
			}

			if(warpForwardARoom)
			{
				if((currentLevel+1) <= maxLevelIndex)
				{
					GameController.control.SetRememberIndex (warpToThisSpawnPointIndex);
					Application.LoadLevel (currentLevel+1);
				}
				else
					print ("ERROR in NavigateRoom.cs object - Trying to move forward a scene and cannot. Trying to access index ("+ (currentLevel+1) + "). Max level index = ("+maxLevelIndex+")");
			}

			if(warpBackwardsARoom)
			{
				if(currentLevel>0)
				{
					GameController.control.SetRememberIndex (warpToThisSpawnPointIndex);
					Application.LoadLevel (currentLevel-1);
				}
				else
					print ("ERROR in NavigateRoom.cs object - Trying to move backwards a scene and cannot");
			}

			if(warpToACertainRoom)
			{
				if(warpToThisRoomIndex>=0 && warpToThisRoomIndex<=maxLevelIndex)	// If this room index is in bounds
				{
					if(currentLevel == warpToThisRoomIndex)							// If warping to the same screen - move the character
					{
						WarpingToSameRoom(other.gameObject);
					}
					else 															// else warp to the map and remember which index you want to spawn at
					{
						GameController.control.SetRememberIndex (warpToThisSpawnPointIndex);
						Application.LoadLevel (warpToThisRoomIndex);
					}
				}
				else
					print ("ERROR in NavigateRoom.cs object - Trying to access a scene that does not exist or is not added to the build settings (Scene Index "+warpToThisRoomIndex+")");
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player" && collisionCount>0)
		{
			collisionCount--;

			if(collisionCount <0)
				collisionCount = 0;
		}
	}


	// Stay in the same room and change the players location
	void WarpingToSameRoom(GameObject thePlayer)
	{
		if(warpToThisSpawnPointIndex >= spawnPoints.Length)	// If the index is too high for the number of spawn points - error and set to default spawn index = 0;
		{
			print ("ERROR in navigateRoom.cs object - Looking for Index (" +warpToThisSpawnPointIndex+ "). Array max index = ("+ ((int)(spawnPoints.Length)-1) +")" );
			warpToThisSpawnPointIndex = 0;
		}

		thePlayer.transform.position = spawnPoints[warpToThisSpawnPointIndex].position;
	}

}

/*
 * using UnityEngine;
using System.Collections;

public class NavigateRooms : MonoBehaviour
{
	public int warpToThisSpawnPointIndex = 0;		// This is the index in an array of locations to warp at - Default = 0;

	// Only one of the following bool may be selected at once.
	public bool warpInSameRoom = false;	
	public bool warpForwardARoom = false;
	public bool warpBackwardsARoom = false;

	public bool warpToACertainRoom = false;
	public int warpToThisRoomIndex = 0;

	private int currentLevel;
	private int maxLevelIndex;

	private int triggerCount = 0;					// Checks how many options are selected
	private int collisionCount = 0;					// Makes sure multi-collision player objects only trigger this object once

	private GameObject[] spawnPoints;				// All of the spawn points on the screen


	void Start()
	{
		spawnPoints = PlayerSpawnController.spawnguy.spawnPoints;		// Get array from the static spawnController

		// Make sure only one option is selected
		if(warpInSameRoom == true)
			triggerCount++;
		if(warpForwardARoom == true)
			triggerCount++;
		if(warpBackwardsARoom == true)
			triggerCount++;
		if(warpToACertainRoom == true)
			triggerCount++;

		if(triggerCount == 0)
			print ("ERROR in NavigateRoom.cs object - You need to select at least one option for this script to work");

		if(triggerCount > 1)
			print ("ERROR in NavigateRoom.cs object - Too many options turned to true - Only pick one per object");

		currentLevel = (Application.loadedLevel);
		maxLevelIndex = Application.levelCount - 1;

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player"&&collisionCount<1)			// Only the player may trigger this
		{
			collisionCount++;

			if(warpInSameRoom)
			{
				WarpingToSameRoom();
			}

			if(warpForwardARoom)
			{
				if((currentLevel+1) <= maxLevelIndex)
				{
					GameController.control.RememberIndex = warpToThisSpawnPointIndex;
					Application.LoadLevel (currentLevel+1);
				}
				else
					print ("ERROR in NavigateRoom.cs object - Trying to move forward a scene and cannot. Trying to access index ("+ (currentLevel+1) + "). Max level index = ("+maxLevelIndex+")");
			}

			if(warpBackwardsARoom)
			{
				if(currentLevel>0)
				{
					GameController.control.RememberIndex = warpToThisSpawnPointIndex;
					Application.LoadLevel (currentLevel-1);
				}
				else
					print ("ERROR in NavigateRoom.cs object - Trying to move backwards a scene and cannot");
			}

			if(warpToACertainRoom)
			{
				if(warpToThisRoomIndex>=0 && warpToThisRoomIndex<=maxLevelIndex)	// If this room index is in bounds
				{
					if(currentLevel == warpToThisRoomIndex)							// If warping to the same screen - move the character
					{
						WarpingToSameRoom();
					}
					else 															// else warp to the map and remember which index you want to spawn at
					{
						GameController.control.RememberIndex = warpToThisSpawnPointIndex;
						Application.LoadLevel (warpToThisRoomIndex);
					}
				}
				else
					print ("ERROR in NavigateRoom.cs object - Trying to access a scene that does not exist or is not added to the build settings (Scene Index "+warpToThisRoomIndex+")");
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			collisionCount--;

			if(collisionCount <0)
				collisionCount = 0;
		}
	}


	// Stay in the same room and change the players location
	void WarpingToSameRoom()
	{
		if(warpToThisSpawnPointIndex >= spawnPoints.Length)	// If the index is too high for the number of spawn points - error and set to default spawn index = 0;
		{
			print ("ERROR in navigateRoom.cs object - Looking for Index (" +warpToThisSpawnPointIndex+ "). Array max index = ("+ ((int)(spawnPoints.Length)-1) +")" );
			warpToThisSpawnPointIndex = 0;
		}

		GameController.control.GuyLocation.transform.position = spawnPoints[warpToThisSpawnPointIndex].transform.position;
	}

}

*/