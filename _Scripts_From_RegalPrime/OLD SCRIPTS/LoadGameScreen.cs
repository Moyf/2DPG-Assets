// RegalPrime 11-26-14 - LoadGameScreen.cs
// This was replaced with the new GUI from 4.6 and the LoadGameScreenNew.cs script

// This script is used to display a simple GUI that allows the user to load a file / change effect settings
// Upon running this script, data is gathered from the saved game files (located in the "Saved Games" folder) and used to populate a vector of struct SaveFile
// Any changes will reload the data again using the refreshData() function

// The information gathered is used to display / restrict files from loading
// Upon selection an option, the files will be updated accordingly and the user will be allowed to enter the game

// There is also a slider for both music volume and sound effect volume
// Pressing the save button, will save their settings to the "GameSettings.dat" file


using UnityEngine;
using System.Collections;

public class LoadGameScreen : MonoBehaviour
{
	public AudioClip effectClipTest;				// Effect that is previewed when changing the sound effect slidebar

	public string[] DifficultyString = {"Easy", "Medium", "Hard"};		// Used to detect which button is selected for difficulty
	public int[] ChooseDifficulty = {0,0,0};

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
	}

	SaveFiles[] currentSaveFiles = new SaveFiles[3];	// A vector of the above struct

	private float MusicSlider = 1;						// Music slider variable
	private float EffectSlider = 1;						// Effect slider variable

	private float MusicChange = 1;						// Checks if the value has been changed
	private float EffectChange = 1;						// Checks if the value has been changed

	private float LevelSlider = 0;						// Music slider variable



	private float lengthOfLabel = 150;
	private float heightOfLabel = 30;

	private float lengthOfButton = 150;
	private float heightOfButton = 16;

	float virtualWidth = 407.0f;						// Used to scale the boxes / font incase the resolution is different
	float virtualHeight = 305.0f;						// Used to scale the boxes / font incase the resolution is different

	float WidthMultiplier;
	float HeightMultiplier;

	public GUIStyle styleGuide;

//	int screenWidth;
//	int screenHeight;
//	int labelWidth;
//	int labelHeight;
//	int rowStagger;


	void Start ()
	{
		refreshData ();
		MusicController.control.Load ();

		MusicSlider = MusicController.control.musicVolume;
		EffectSlider = MusicController.control.effectVolume;

		MusicChange = MusicSlider;
		EffectChange = EffectSlider;

		WidthMultiplier = (Screen.width / virtualWidth);
		HeightMultiplier = (Screen.height / virtualHeight);
		
		styleGuide.fontSize = (int)(styleGuide.fontSize * WidthMultiplier);
		lengthOfLabel = lengthOfLabel * WidthMultiplier;
		lengthOfButton = lengthOfButton * WidthMultiplier;
		
		heightOfLabel = heightOfLabel * HeightMultiplier;
		heightOfButton = heightOfButton * HeightMultiplier;

//		GameController.control.Load (0);
//		GameController.control.StopTimer ();
//		screenWidth = Screen.width;
//		screenHeight = Screen.height;
//		labelWidth = 200;
//		labelHeight = 50;
//		rowStagger = 20;
	}
	
	void Update ()
	{
		
	}

	void OnDestroy()
	{
		MusicController.control.Load ();
		MusicController.control.RefreshVolume ();
	}

	void OnGUI()
	{
		Music_Effect_GUI ();
		Save_Load_GUI ();
		TestRooms_GUI ();
	}

	void Save_Load_GUI()			// Display all the information to load / enter a new game
	{
		float rowSkip = 0;
		float rowSkipAmount = 15 * HeightMultiplier;
		
		for(int I=0; I<currentSaveFiles.Length; I++)
		{
			GUI.Box(new Rect(0,rowSkip,lengthOfLabel, rowSkipAmount * 5), "");

			if(currentSaveFiles[I].ableToLoadFile)
			{
				if(currentSaveFiles[I].CodeMatch)
				{
					float CurrentX = (int)(currentSaveFiles[I].playerXPosition*100);
					CurrentX /= 100;
					float CurrentY = (int)(currentSaveFiles[I].playerYPosition*100);
					CurrentY /= 100;

					if(currentSaveFiles[I].GameCompleted)
						GUI.Label (new Rect (0, rowSkip, lengthOfLabel, heightOfLabel), "GAME COMPLETED - "+DifficultyString[currentSaveFiles[I].difficulty], styleGuide);
					else
						GUI.Label (new Rect (0, rowSkip, lengthOfLabel, heightOfLabel), "Loaded Room =  " + currentSaveFiles[I].loadedRoom + " (" + CurrentX + ", "+ CurrentY + ") - "+DifficultyString[currentSaveFiles[I].difficulty], styleGuide);
					rowSkip += rowSkipAmount;
					GUI.Label (new Rect (0, rowSkip, lengthOfLabel, heightOfLabel), "Current Time : " + currentSaveFiles[I].currentTime + "s.   Deaths =  " + currentSaveFiles[I].numberOfDeaths, styleGuide);
				}
				else
					GUI.Label (new Rect (0, rowSkip, lengthOfLabel, heightOfLabel), "Unsupported Save File", styleGuide);
			}
			else
			{
				GUI.Label (new Rect (0, rowSkip, lengthOfLabel, heightOfLabel), "Loaded Room =  EMPTY FILE", styleGuide);
				rowSkip += rowSkipAmount;
			}
			rowSkip += rowSkipAmount;
			
			// Button - Load the file
			if(currentSaveFiles[I].ableToLoadFile)
			{
				if(currentSaveFiles[I].CodeMatch)
				{
					if(GUI.Button (new Rect (0, rowSkip, lengthOfButton, heightOfButton), "Load this file"))
					{
						GameController.control.SaveFile = I;
						GameController.control.Load(I);
						GameController.control.SetLoadingFile (true);
						Application.LoadLevel(currentSaveFiles[I].loadedRoom);
					}
				}else
					GUI.Label (new Rect (0, rowSkip, lengthOfLabel, heightOfLabel), "Unsupported Save File", styleGuide);
			}else
					GUI.Label (new Rect (0, rowSkip, lengthOfLabel, heightOfLabel), "NO FILE TO LOAD", styleGuide);
			rowSkip += rowSkipAmount;
			
			// Button - Start at beginning
			if(GUI.Button (new Rect (0, rowSkip, lengthOfButton, heightOfButton), "Start from beginning"))
			{
				GameController.control.SetLoadingFile(false);
				GameController.control.SaveFile = I;
				GameController.control.Difficulty = ChooseDifficulty[I];
				GameController.control.ResetGameControllerValues();
				GameController.control.StartTimer ();
				GameController.control.StartAtBeginning = true;
				Application.LoadLevel(2);
			}
			rowSkip += rowSkipAmount;

//			GUI.Label (new Rect (0, rowSkip, lengthOfButton, heightOfButton), "Difficulty "+currentSaveFiles[I].difficulty, styleGuide);

			ChooseDifficulty[I] = GUI.SelectionGrid(new Rect (0, rowSkip, lengthOfButton, heightOfButton), ChooseDifficulty[I], DifficultyString, 3);

			rowSkip += rowSkipAmount;
			rowSkip += rowSkipAmount;
		}
	}

	void Music_Effect_GUI()				// Display the music / effect slider
	{
		float Offset = 200 * WidthMultiplier;

		GUI.Label (new Rect (Offset, 0, 300*WidthMultiplier, 50*HeightMultiplier), "Music Volume - " + MusicSlider*10, styleGuide);
		GUI.Label (new Rect (Offset, 30*HeightMultiplier, 300*WidthMultiplier, 50*HeightMultiplier), "Effect Volume - " + EffectSlider*10, styleGuide);
		
		MusicSlider = GUI.HorizontalSlider(new Rect(Offset, 15*HeightMultiplier, 100*WidthMultiplier, 15*HeightMultiplier), MusicSlider, 0.0F, 1.0F);
		MusicSlider = (Mathf.Round (MusicSlider*10))/10;
		
		EffectSlider = GUI.HorizontalSlider(new Rect(Offset, 45*HeightMultiplier, 100*WidthMultiplier, 15*HeightMultiplier), EffectSlider, 0.0F, 1.0F);
		EffectSlider = (Mathf.Round (EffectSlider*10))/10;
		
		if(MusicSlider != MusicChange)
		{
			MusicController.control.musicVolume = MusicSlider;
			MusicChange = MusicSlider;
			MusicController.control.RefreshVolume();
		}
		if(EffectSlider != EffectChange)
		{
			MusicController.control.effectVolume = EffectSlider;
			EffectChange = EffectSlider;
			MusicController.control.OneShotAudio(effectClipTest);
		}
		
		if(GUI.Button (new Rect (Offset, 60*HeightMultiplier, 100*WidthMultiplier, 15*HeightMultiplier), "Save Settings"))
		{
			MusicController.control.musicVolume = MusicSlider;
			MusicController.control.effectVolume = EffectSlider;
			MusicController.control.Save();
			MusicController.control.RefreshVolume();
		}
	}

	void TestRooms_GUI ()
	{
		float Offset = 200 * WidthMultiplier;

		GUI.Label (new Rect (Offset, 160*HeightMultiplier, 300*WidthMultiplier, 50*HeightMultiplier), "LevelSelect - " + LevelSlider, styleGuide);
		LevelSlider = GUI.HorizontalSlider(new Rect(Offset, 170*HeightMultiplier, 100*WidthMultiplier, 15*HeightMultiplier), LevelSlider, 1f, Application.levelCount-1);
		LevelSlider = (int) (LevelSlider);
		if(GUI.Button (new Rect (Offset, 180*HeightMultiplier, 100*WidthMultiplier, 15*HeightMultiplier), "Jump To Scene"))
		{
			Application.LoadLevel((int)LevelSlider);
		}

		if(GUI.Button (new Rect (Offset, 200*HeightMultiplier, 100*WidthMultiplier, 15*HeightMultiplier), "New Scenes"))
		{
			Application.LoadLevel(44);
		}

	}

	void refreshData()				// Repopulates the data from the saved files
	{
		for(int I=0; I<currentSaveFiles.Length; I++)
		{
			if(GameController.control.AbleToLoadFile (I))
			{
				GameController.control.Load(I);
				GameController.control.StopTimer();

//				print("File "+I+" " +GameController.control.SecretCode+", "+GameController.control.LoadedCode+": "+GameController.control.CheckCodeMatch());

				currentSaveFiles[I].ableToLoadFile = true;
				currentSaveFiles[I].difficulty = GameController.control.Difficulty;
				currentSaveFiles[I].loadedRoom = GameController.control.LoadedRoom;
				currentSaveFiles[I].playerXPosition = GameController.control.LoadedX;
				currentSaveFiles[I].playerYPosition = GameController.control.LoadedY;
				currentSaveFiles[I].numberOfDeaths = GameController.control.DeathCounter;
				currentSaveFiles[I].currentTime = GameController.control.GameTimer;
				currentSaveFiles[I].GameCompleted = GameController.control.IsCompleted();

				if(GameController.control.CheckCodeMatch())
					currentSaveFiles[I].CodeMatch = true;
				else
					currentSaveFiles[I].CodeMatch = false;
			
			}else
			{
				currentSaveFiles[I].ableToLoadFile = false;
				currentSaveFiles[I].loadedRoom = 0;
				currentSaveFiles[I].playerXPosition = 0;
				currentSaveFiles[I].playerYPosition = 0;
			}
		}

//		GameController.control.Load (0);
//		GameController.control.StopTimer ();
	}
}




/* 
 * 

	void Save_Load_GUI()
	{
		float rowSkip = 0;
		float rowSkipAmount = 20 * (Screen.height / virtualHeight);
		
		for(int I=0; I<currentSaveFiles.Length; I++)
		{
			// Label - Can file be loaded?
			//			GUI.Label (new Rect (0, rowSkip, 300, 50), "Loaded =  " +currentSaveFiles[I].ableToLoadFile);
			//			rowSkip += 20;
			
			// Label - What room is the save file on (x,y) coordinates as well
			if(currentSaveFiles[I].ableToLoadFile)
			{
				GUI.Label (new Rect (0, rowSkip, 300, 50), "Loaded Room =  " + currentSaveFiles[I].loadedRoom + " (" + currentSaveFiles[I].playerXPosition + ", "+ currentSaveFiles[I].playerYPosition + ")", styleGuide);
				rowSkip += rowSkipAmount;
				GUI.Label (new Rect (0, rowSkip, 300, 50), "Current Time : " + currentSaveFiles[I].currentTime + "s.   Deaths =  " + currentSaveFiles[I].numberOfDeaths);
			}
			else
				GUI.Label (new Rect (0, rowSkip, 300, 50), "Loaded Room =  EMPTY FILE");
			rowSkip += 20;
			
			// Button - Load the file
			if(currentSaveFiles[I].ableToLoadFile)
			{
				if(GUI.Button (new Rect (0, rowSkip, 300, 20), "Load this file"))
				{
					GameController.control.SaveFile = I;
					GameController.control.Load(I);
					GameController.control.SetLoadingFile (true);
					Application.LoadLevel(currentSaveFiles[I].loadedRoom);
				}
			}else
				GUI.Label (new Rect (0, rowSkip, 300, 50), "NO FILE TO LOAD");
			rowSkip += 20;
			
			// Button - Delete data and start from scratch
			if(GUI.Button (new Rect (0, rowSkip, 300, 20), "Reset data"))
			{
				GameController.control.DeleteData(I);
				refreshData();
			}
			rowSkip += 20;
			
			// Button - Start at beginning
			if(GUI.Button (new Rect (0, rowSkip, 300, 20), "Start from beginning"))
			{
				GameController.control.SetLoadingFile(false);
				GameController.control.SaveFile = I;
				GameController.control.StartTimer ();
				Application.LoadLevel(2);
			}
			rowSkip += 20;
			
			rowSkip += 20;
		}
	}

	void Music_Effect_GUI()
	{
		GUI.Label (new Rect (400, 0, 300, 50), "Music Volume - " + MusicSlider*10);
		GUI.Label (new Rect (400, 50, 300, 50), "Effect Volume - " + EffectSlider*10);
		
		MusicSlider = GUI.HorizontalSlider(new Rect(400, 25, 100, 30), MusicSlider, 0.0F, 1.0F);
		MusicSlider = (Mathf.Round (MusicSlider*10))/10;
		
		EffectSlider = GUI.HorizontalSlider(new Rect(400, 75, 100, 30), EffectSlider, 0.0F, 1.0F);
		EffectSlider = (Mathf.Round (EffectSlider*10))/10;
		
		if(MusicSlider != MusicChange)
		{
			MusicController.control.musicVolume = MusicSlider;
			MusicChange = MusicSlider;
			MusicController.control.RefreshVolume();
		}
		if(EffectSlider != EffectChange)
		{
			MusicController.control.effectVolume = EffectSlider;
			EffectChange = EffectSlider;
			MusicController.control.OneShotAudio(effectClipTest);
		}
		
		
		if(GUI.Button (new Rect (400, 100, 100, 30), "Save Settings"))
		{
			MusicController.control.musicVolume = MusicSlider;
			MusicController.control.effectVolume = EffectSlider;
			MusicController.control.Save();
			MusicController.control.RefreshVolume();
		}
	}

*/