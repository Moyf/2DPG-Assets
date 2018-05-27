// RegalPrime 12-02-14 - GameController.cs

// There can only be one of these objects in a scene at a time (others will be destroyed)
// Deals with saving and loading of game data (current saved positions / deaths / timer / etc)
// Creates a subfolder '/Saved Games' if not available

// Detects button presses for
// Reloading the scene (S)
// ReloadingSpecial (Q) - Reloads the scene and spawns you at the first spawn point - Used incase the player saves into a deadly object
// BacktoMenu (F2)

// IMPORTANT INFORMATION !!
// To differentiate between saved files I included a special variable that must be hard set via code
// private string SecretCode = "Bacon";
// The secret code is put into the save file, and when loaded, it must match the secret code of the game you are running.
// If it does not, the loading file screen will give you a file unsupported error for that file.
// This is very simplistic (and Im sure easy to circumvent) but for now, it is good enough

using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour
{
	public static GameController control;		// Reference to itself

	public int SaveFile;						// What current save slot are we in
												// Is set by the save / load screen
	public GameObject GuyLocation;				// Attaches the character prefab when guy is loaded
												// Easy way to grab this gameobject from other scripts

	bool loadingFile;							// Are we loading from a file? Used in the PlayerSpawnController
	int rememberIndex = 0;						// This remember which spawn point to spawn at across scene change


	public int currentScene;					// What scene are we in? This is only useful in viewing within the editor (for testing)
												// Any saving calls on Application.loadedLevel and does not rely on this variable

	private string SecretCode = "Bacon";		// This is a secret code that allows saved files to be loaded
	private string LoadedCode;					// This is the loaded code from the saved file

	public int Difficulty = 0;					// 0 = Easy, 1 = Medium, 2 = Hard

	public int DeathCounter;					// Number of deaths
	public float GameTimer = 0;					// Time saved in file
	public float TimeSpent = 0;					// Time spent since the last save

	public int LoadedRoom;						// Value stored in the file
	public float LoadedX;						// X Position stored in the file
	public float LoadedY;						// Y Position stored in the file

	private bool[] SavedTriggers;				// Array of triggers that are saved to file
	private int NumberOfSavedTriggers = 10;		// How many saved triggers are in the array
	private bool GameComplete = false;			// has the game been cleared
	public bool StartAtBeginning = false;		// Did the player just start at the beginning

	void Start()
	{
		if(!Directory.Exists("Saved Games"))
		{
			Directory.CreateDirectory("Saved Games");
		}
	}
	public GameObject GetPlayerGameObject()
	{
		return GuyLocation;
	}

	// Used in calculating the time spent ingame
	// Incremented in 1s intervals instead of doing in every frame (less calculations)
	// StopTimer - Is primarily used when player dies
	public void StartTimer()
	{
		StopCoroutine ("C_StartTimer");
		StartCoroutine ("C_StartTimer");
	}
	public void StopTimer()
	{
		StopCoroutine ("C_StartTimer");
	}
	IEnumerator C_StartTimer()
	{
		TimeSpent = 0;
		while(gameObject != null)
		{
			yield return new WaitForSeconds(1);
			TimeSpent += 1;
		}
	}

	public bool CheckCodeMatch()
	{
		if(SecretCode == LoadedCode)
			return true;
		else
			return false;
	}

	void Awake ()
	{
		if (control == null)		// If there are no control objects - this is the main one
		{
//			if(EventManager.control == null)
//					gameObject.AddComponent<EventManager> ();

			EventManager.CreateEventManagerIfNeeded();
	
			DontDestroyOnLoad (this.gameObject);
			control = this;
			Load (SaveFile);
			currentScene = Application.loadedLevel;
			gameObject.transform.parent = null;

			// Attaches button presses to the following functions
			EventManager.resetButton += OnButtonPressed_ResetScreen;
			EventManager.resetButton_Hard += OnButtonPressed_HardResetScreen;
			EventManager.resetGameButton += OnButtonPressed_ResetGame;
		}
		else if(control != this)
		{
			Destroy(gameObject);	// If there is already a control object - delete the duplicate
		}
		loadingFile = false;
	}


	void OnButtonPressed_ResetScreen()
	{
		if(Application.loadedLevel != 0 && Application.loadedLevel != 1 && !StartAtBeginning)
		{
			loadingFile = true;
			SaveTimerDeath();						// Updates Timer
//			Load (SaveFile);						// Do I need to load again after saving to update values now that I fixed it in Save()???
			
			if(GameController.control.LoadedRoom == Application.loadedLevel)
			{
				EventManager.control.MasterReset();
				GuyLocation.transform.position = new Vector2(GameController.control.LoadedX, GameController.control.LoadedY);
				GuyLocation.SetActive (true);
				loadingFile = false;
			}
			else
				Application.LoadLevel(LoadedRoom);
		}
	}
	void OnButtonPressed_HardResetScreen()
	{
		if(Application.loadedLevel != 0 && Application.loadedLevel != 1 && !StartAtBeginning)
		{
			loadingFile = false;
			SaveTimerDeath();					// Updates Timer
//			Load (SaveFile);					// Do I need to load again after saving to update values now that I fixed it in Save()???
			Application.LoadLevel(LoadedRoom);
		}
	}
	void OnButtonPressed_ResetGame()
	{
		if(Application.loadedLevel != 0 && Application.loadedLevel != 1 && !StartAtBeginning)
			SaveTimerDeath();
		Application.LoadLevel (0);
	}



	// Updates the timer if the game is shut down
	void OnApplicationQuit()
	{
		if(Application.loadedLevel != 0 && Application.loadedLevel != 1 && !StartAtBeginning)
			SaveTimerDeath ();
	}

	// Removes the references to the event manager
	void OnDestroy()
	{
		EventManager.resetButton -= OnButtonPressed_ResetScreen;
		EventManager.resetButton_Hard -= OnButtonPressed_HardResetScreen;
		EventManager.resetGameButton -= OnButtonPressed_ResetGame;
	}

	// Updates the editor view of which scene we are in
	void OnLevelWasLoaded()
	{
		currentScene = Application.loadedLevel;
	}

	// Saves all data to the file. Is used when hitting a save point
	// All current information is updated and saved to the file
	public void Save()
	{
		Save (SaveFile);
	}

	// Saves the player data
	public void Save(int saveIndex)
	{
//		print ("Saved at Room - " + Application.loadedLevel + " " + GuyLocation.transform.position.x + ", " + GuyLocation.transform.position.y);
		StartAtBeginning = false;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create ("Saved Games" + "/Player"+saveIndex+".dat");

		PlayerData data = new PlayerData ();

		data.secretCode = SecretCode;
		LoadedCode = SecretCode;

		data.difficulty = Difficulty;
		data.roomSaveNum = Application.loadedLevel;
		data.playerX = GuyLocation.transform.position.x;
		data.PlayerY = GuyLocation.transform.position.y;

		LoadedRoom = Application.loadedLevel;
		LoadedX = GuyLocation.transform.position.x;
		LoadedY = GuyLocation.transform.position.y;

		data.deaths = DeathCounter;
		GameTimer += TimeSpent;
		data.timeInGame = GameTimer;
		StartTimer ();

		data.GameComplete = GameComplete;
		data.savedTriggers = SavedTriggers;

		bf.Serialize (file, data);
		file.Close ();
	}


	// Saves data to the file, but only updates the deathcounter and ingame time
	// The other information is saved using the previous data (last known saved position / room)
	public void SaveTimerDeath()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create ("Saved Games" + "/Player"+SaveFile+".dat");

		PlayerData data = new PlayerData ();

		data.secretCode = LoadedCode;

		data.difficulty = Difficulty;
		data.roomSaveNum = LoadedRoom;
		data.playerX = LoadedX;
		data.PlayerY = LoadedY;
		
		data.deaths = DeathCounter;
		GameTimer += TimeSpent;
		data.timeInGame = GameTimer;
		StartTimer ();

		data.GameComplete = GameComplete;
		data.savedTriggers = SavedTriggers;

		bf.Serialize (file, data);
		file.Close ();
	}

	// Loads data from the file
	public void Load(int saveIndex)
	{
		if(File.Exists("Saved Games" + "/Player"+saveIndex+".dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open ("Saved Games" + "/Player"+saveIndex+".dat", FileMode.Open);

			PlayerData data = (PlayerData)bf.Deserialize(file);
			file.Close();

			SaveFile = saveIndex;
			LoadedCode = data.secretCode;

			Difficulty = data.difficulty;
			LoadedRoom = data.roomSaveNum;
			LoadedX = data.playerX;
			LoadedY = data.PlayerY;

			DeathCounter = data.deaths;
			GameTimer = data.timeInGame;

			GameComplete = data.GameComplete;
			SavedTriggers = data.savedTriggers;

			StartTimer ();
//			print ("Loading at Room - " + LoadedRoom + " " + LoadedX + ", " + LoadedY);
		}
	}


	// Resets all data to a starting value
	public void ResetGameControllerValues()
	{
		DeathCounter = 0;
		GameTimer = 0;
		GameComplete = false;
		SavedTriggers = new bool[NumberOfSavedTriggers];
		for(int I=0; I<SavedTriggers.Length; I++)
		{
			SavedTriggers[I] = false;
		}
	}

	// Resets all data to a starting value
	// This is not used and probably should never be used
	public void DeleteData(int saveIndex)
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create ("Saved Games" + "/Player"+saveIndex+".dat");
		
		PlayerData data = new PlayerData ();

		data.secretCode = SecretCode;

		data.difficulty = 0;
		data.roomSaveNum = 1;
		data.playerX = 0;
		data.PlayerY = 0;

		data.deaths = 0;
		data.timeInGame = 0;

		data.GameComplete = false;

		bf.Serialize (file, data);
		file.Close ();		
	}

	public bool AbleToLoadFile(int saveSlot)
	{
		return File.Exists("Saved Games" + "/Player"+saveSlot+".dat");
	}

	// Functions are used so the variables are no longer public
	public void SetRememberIndex(int TEMPint)
	{
		rememberIndex = TEMPint;
	}
	public int RememberIndex()
	{
		return rememberIndex;
	}
	public bool LoadingFile()
	{
		return loadingFile;
	}
	public void SetLoadingFile(bool TempBool)
	{
		loadingFile = TempBool;
	}
	public bool IsCompleted()
	{
		return GameComplete;
	}
	public void CompleteGame()
	{
		GameComplete = true;
	}
	public bool[] Get_SavedTriggers()
	{
		return SavedTriggers;
	}
	public bool UpdateSavedTrigger(int position, bool value)
	{
		if(0 <= position && position < SavedTriggers.Length)
		{
			SavedTriggers [position] = value;
			return true;
		}
		else
		{
			print (string.Format("UpdateSavedTrigger() - WARNING - {0} is out of bounds for array SavedTriggers. Max index = {1}", position, SavedTriggers.Length-1));
			return false;
		}
	}

}

// Class used in saving and loading game information
[Serializable]
class PlayerData
{
	public string secretCode;

	public int difficulty;
	public int roomSaveNum;
	public float playerX;
	public float PlayerY;

	public int deaths;
	public float timeInGame;

	public bool GameComplete;
	public bool[] savedTriggers;
}