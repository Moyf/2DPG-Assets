﻿// RegalPrime 11-25-14 - ColorChangeSprite.cs

// This script creates a color changing effect on the object
// It uses the Coroutine found in NewObjectSpawner.cs

// Default is random color change. This is done via the NewObjectSpawner - C_ChangeColor Coroutine
// Random color change will maintain starting alpha of the object

// LoopThroughVector = true, Will allow the object to loop through the colors of the vector ColorLoop[]
// To only go through the vector once, select OneTimeVectorLoop = true
// Any Color / Alpha will work in the vector. Thus you can loop through and create a fade / unfade effect by having different alpha values
// Make sure when increasing the vector size in the editor to increase the alpha of the color (default is Black + Alpha = 0)

// Speed change 
// 1 = over 1 second
// 2 = over 0.5s second
// 0.5f = over 2 second
// Etc etc

using UnityEngine;
using System.Collections;

public class ColorChangeSprite_OLD : MonoBehaviour
{
	public bool RestartOnReset = false;

	public bool IsEnabled = true;			// Is the object enabled
	public float StartDelay = 0;			// Start Delay
	public float SpeedChange = 1;			// Change Speed

	public bool EnableVectorLoop = false;	// Go through color vector
	public bool OneTimeVectorLoop = false;	// Go through the vector once and stop
	public Color[] ColorLoop;				// Set of colors to loop through

	private NewObjectSpawner SuperScript;	// Script that is used to do the color changing
	private Color startingColor;


	void Awake ()
	{
		if(RestartOnReset)
			EventManager.resetObjects += Reset;

		startingColor = GetComponent<SpriteRenderer> ().color;

		if(!gameObject.GetComponent<NewObjectSpawner> ())
			SuperScript = gameObject.AddComponent<NewObjectSpawner> ();
		else
			SuperScript = gameObject.GetComponent<NewObjectSpawner> ();
		// Cannot allow speed changes of 0 or less
		if(SpeedChange <= 0)
			SpeedChange = 1;

//		if(IsEnabled && gameObject.GetComponent<SpriteRenderer>() != null)
//			StartCoroutine ("C_ChangingColor");
	}
	void OnDestroy()
	{
		if(RestartOnReset)
			EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		StopAllCoroutines ();
		GetComponent<SpriteRenderer> ().color = startingColor;
		SuperScript.ResetObjectSpawner ();

		if(gameObject.activeInHierarchy)
			StartCoroutine ("C_ChangingColor");
	}

	void OnEnable()
	{
		if(!gameObject.GetComponent<NewObjectSpawner> ())
			SuperScript = gameObject.AddComponent<NewObjectSpawner> ();

		StartCoroutine ("C_ChangingColor");
	}

	IEnumerator C_ChangingColor()
	{
		if(!EnableVectorLoop || ColorLoop.Length == 0)
		{
			SuperScript.ChangeColor_Random(StartDelay, SpeedChange);		// Start the random color changing effect
		}
		else
		{
			do
			{
				for(int I=0; I<ColorLoop.Length; I++)						// Go through the vector and change to the next color
				{
					SuperScript.ChangeColor (0, ColorLoop[I], SpeedChange);

					yield return null;
					yield return new WaitForSeconds((1/SpeedChange));
				}
				yield return null;
			}while(IsEnabled && !OneTimeVectorLoop);
		}

		yield return null;
	}
}
