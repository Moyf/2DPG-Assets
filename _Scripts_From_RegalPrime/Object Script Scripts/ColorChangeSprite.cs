// RegalPrime 6-02-14 - ColorChangeSprite.cs

// This script creates a color changing effect on the object
// It uses the Coroutine found in NewObjectSpawner.cs

// Default is random color change. This is done via the NewObjectSpawner - C_ChangeColor Coroutine
// Random color change will maintain starting alpha of the object

// LoopThroughVector = true, Will allow the object to loop through the colors of the vector ColorLoop[]
// To only go through the vector once, select OneTimeVectorLoop = true
// Any Color / Alpha will work in the vector. Thus you can loop through and create a fade / unfade effect by having different alpha values
// Make sure when increasing the vector size in the editor to increase the alpha of the color (default is Black + Alpha = 0)

// Updated to use the ObjectManipllation script now
// Time change is now in seconds
// Other than that, the script is pretty much the same.

// The IsEnabled really isnt connected the way I would like it. Will worry about that later.

using UnityEngine;
using System.Collections;

public class ColorChangeSprite : MonoBehaviour
{
	public bool RestartOnReset = false;
	
	public bool IsEnabled = true;			// Is the object enabled
	public float StartDelay = 0;			// Start Delay
	public float ColorSpeed = 1;			// How many seconds to change one color
	
	public bool EnableVectorLoop = false;	// Go through color vector
	public bool OneTimeVectorLoop = false;	// Go through the vector once and stop
	public Color[] ColorLoop;				// Set of colors to loop through
	
	private ObjectManipulation SuperScript;	// Script that is used to do the color changing
	private Color startingColor;
	
	
	void Awake ()
	{
		if(RestartOnReset)
			EventManager.resetObjects += Reset;
		
		startingColor = GetComponent<SpriteRenderer> ().color;
		
		if(!gameObject.GetComponent<ObjectManipulation> ())
			SuperScript = gameObject.AddComponent<ObjectManipulation> ();
		else
			SuperScript = gameObject.GetComponent<ObjectManipulation> ();
		// Cannot allow speed changes of 0 or less
		if(ColorSpeed <= 0)
			ColorSpeed = 1;
		
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
		SuperScript.StopAllCoroutines ();
		GetComponent<SpriteRenderer> ().color = startingColor;

		if(gameObject.activeInHierarchy)
			StartCoroutine ("C_ChangingColor");
	}
	
	void OnEnable()
	{
//		if(!gameObject.GetComponent<ObjectManipulation> ())
//			SuperScript = gameObject.AddComponent<ObjectManipulation> ();
		
		StartCoroutine ("C_ChangingColor");
	}
	
	IEnumerator C_ChangingColor()
	{
		yield return new WaitForSeconds (StartDelay);

		if(!EnableVectorLoop || ColorLoop.Length == 0)
		{
			SuperScript.ChangeColor(0, gameObject, Color.white, ColorSpeed, true);		// Start the random color changing effect
		}
		else
		{
			do
			{
				for(int I=0; I<ColorLoop.Length; I++)						// Go through the vector and change to the next color
				{
					SuperScript.ChangeColor (0, gameObject, ColorLoop[I], ColorSpeed, false);
					
					yield return null;
					yield return null;
					yield return new WaitForSeconds(ColorSpeed);
				}
				yield return null;
			}while(IsEnabled && !OneTimeVectorLoop);
		}
		
		yield return null;
	}
}
