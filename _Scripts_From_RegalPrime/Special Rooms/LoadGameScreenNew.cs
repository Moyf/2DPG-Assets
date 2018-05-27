// RegalPrime 12-20-14 - LoadGameScreenNew.cs

// This is the replacement of the file loading script LoadGameScreen.cs using the new GUI in Version 4.6
// This holds all of the button / functions calls / etc for the GUI made loadgamescreen

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadGameScreenNew : MonoBehaviour
{
	public int SaveSlot = 0;						// Which save slot to try and gather information about
	public struct SaveFiles							// This struct is used to gather inportant information from a saved file
	{
		public bool CodeMatch;

		public bool ableToLoadFile;

		public int difficulty;
		public int loadedRoom;
		public float playerXPosition;
		public float playerYPosition;

		public int numberOfDeaths;
		public float currentTime;

		public bool GameCompleted;
		public bool[] savedTriggers;
	}
	private SaveFiles SaveSlotInformation;
	private string[] DifficultyString = {"Easy", "Medium", "Hard"};		// Used to change the difficulty int into a string for display purposes

	// References to particular GUI elements
	public GameObject VersionMismatchPanel;
	public GameObject NoSaveFilePanel;
	public GameObject SavedGameInfoPanel;

	public Text LoadedRoom;
	public Text LoadedPosition;
	public Text Difficulty;
	public Text CurrentTime;
	public Text Deaths;

	public Toggle EasyToggle;
	public Toggle MediumToggle;
	public Toggle HardToggle;

	public GameObject GameCompleteIcon;
	public GameObject[] AchievementIcons;


	void Start()
	{
		refreshData ();			// Grab this save slots information and populate SaveSlotInformation
		updateGUI ();			// Update the GUI elements using the SaveSlotInformation
	}

	void updateGUI ()
	{
		SavedGameInfoPanel.SetActive(false);
		NoSaveFilePanel.SetActive (false);
		VersionMismatchPanel.SetActive (false);


		// Enable the correct Panel and populate if needed
		if(SaveSlotInformation.ableToLoadFile)
		{
			if(SaveSlotInformation.CodeMatch)
			{
				float CurrentX = (int)(SaveSlotInformation.playerXPosition*100);
				CurrentX /= 100;
				float CurrentY = (int)(SaveSlotInformation.playerYPosition*100);
				CurrentY /= 100;

				SavedGameInfoPanel.SetActive(true);
				LoadedRoom.text = SaveSlotInformation.loadedRoom.ToString();
				LoadedPosition.text = "("+CurrentX + ", " +CurrentY+")";

				Difficulty.text = DifficultyString[SaveSlotInformation.difficulty];


				TimeSpan t = TimeSpan.FromSeconds(SaveSlotInformation.currentTime);
				string formatTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", 
				                              t.Hours, 
				                              t.Minutes, 
				                              t.Seconds, 
				                              t.Milliseconds);
				CurrentTime.text = formatTime;

				Deaths.text = SaveSlotInformation.numberOfDeaths.ToString();

				GameCompleteIcon.SetActive (SaveSlotInformation.GameCompleted);
				for(int I=0; I<SaveSlotInformation.savedTriggers.Length; I++)
				{
					AchievementIcons[I].SetActive (SaveSlotInformation.savedTriggers[I]);
				}
			}
			else
				VersionMismatchPanel.SetActive (true);
		}
		else
		{
			NoSaveFilePanel.SetActive (true);
		}

	}

	// Function to load the file from the saved file data
	public void LoadFile()
	{
		GameController.control.SaveFile = SaveSlot;
		GameController.control.Load(SaveSlot);
		GameController.control.SetLoadingFile (true);
		Application.LoadLevel(SaveSlotInformation.loadedRoom);

	}

	// Function to start a new game at the saved file index
	public void StartNewGame()
	{
		GameController.control.SetLoadingFile(false);
		GameController.control.SaveFile = SaveSlot;

		if(EasyToggle.isOn)
			GameController.control.Difficulty = 0;
		if(MediumToggle.isOn)
			GameController.control.Difficulty = 1;
		if(HardToggle.isOn)
			GameController.control.Difficulty = 2;

		GameController.control.ResetGameControllerValues();
		GameController.control.StartTimer ();
		GameController.control.StartAtBeginning = true;
		Application.LoadLevel((Application.loadedLevel+1));
//		Application.LoadLevel(2);
	}


	// Repopulates the data for that saved files
	void refreshData()
	{

		if(GameController.control.AbleToLoadFile (SaveSlot))
		{
			GameController.control.Load(SaveSlot);
			GameController.control.StopTimer();

			SaveSlotInformation.ableToLoadFile = true;
			SaveSlotInformation.difficulty = GameController.control.Difficulty;
			SaveSlotInformation.loadedRoom = GameController.control.LoadedRoom;
			SaveSlotInformation.playerXPosition = GameController.control.LoadedX;
			SaveSlotInformation.playerYPosition = GameController.control.LoadedY;
			SaveSlotInformation.numberOfDeaths = GameController.control.DeathCounter;
			SaveSlotInformation.currentTime = GameController.control.GameTimer;
			SaveSlotInformation.GameCompleted = GameController.control.IsCompleted();
			SaveSlotInformation.savedTriggers = GameController.control.Get_SavedTriggers();
			
			if(GameController.control.CheckCodeMatch())
				SaveSlotInformation.CodeMatch = true;
			else
				SaveSlotInformation.CodeMatch = false;
			
		}else
		{
			SaveSlotInformation.ableToLoadFile = false;
			SaveSlotInformation.loadedRoom = 0;
			SaveSlotInformation.playerXPosition = 0;
			SaveSlotInformation.playerYPosition = 0;
		}
	}
}