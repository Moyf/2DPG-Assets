// RegalPrime 11-10-14 - LoopThroughSprites.cs

// Loops through a vector of sprites
// This was created so I dont have to make animation controllers / states for very simple "default only" animations.

// Default looping goes from the (start -> end) of the sprite vector. 4 sprite vector would go from index 0 -> 1 -> 2 -> 3 -> Start Over
// BackForthLooping = true - Loop from (start -> end -> start). 4 sprite vector would go from index 0 -> 1 -> 2 -> 3 -> 2 -> 1 -> Start Over

// TopLeftOffset = false = Default - the sprites will be centered on the gameobject
// TopLeftOffset = true - the sprites' top left corner will be the position of the gameobject. This makes it useful for snapping to other objects

using UnityEngine;
using System.Collections;

public class LoopThroughSprites : MonoBehaviour
{
	public int FramesPerSec = 10;				// How fast it loops through the sprites
	public bool TopLeftOffset = false;			// Selecting this will allow the user to place the sprites upper left point and the script will offset it.
	public bool BackForthLooping = false;		// Change how it loops through the sprites
	public Sprite[] SpriteSet;					// Location of the sprites to loop through

	private float SpriteHeight;
	private float SpriteWidth;

	private SpriteRenderer SpriteHolder;

	void Start ()
	{
		if (GetComponent<SpriteRenderer> ())
			SpriteHolder = GetComponent<SpriteRenderer> ();
		else
			SpriteHolder = gameObject.AddComponent<SpriteRenderer>();

		if(TopLeftOffset)						// Calculate the offset
		{
			SpriteHeight = SpriteSet[0].bounds.size.y * gameObject.transform.localScale.y;
			SpriteWidth = SpriteSet[0].bounds.size.x * gameObject.transform.localScale.x;
			gameObject.transform.position += new Vector3 (SpriteWidth/2, -SpriteHeight/2, 0);
		}

		if(!BackForthLooping)
			StartCoroutine (C_SpriteLooping ());
		else
			StartCoroutine (C_SpriteLooping_BackForth ());
	}

	IEnumerator C_SpriteLooping()
	{
		while(gameObject != null)
		{
			for(int I=0; I<SpriteSet.Length;I++)
			{
				SpriteHolder.sprite = SpriteSet [I];
				yield return new WaitForSeconds ((1f/FramesPerSec));
			}
			yield return null;
		}
	}


	IEnumerator C_SpriteLooping_BackForth()
	{
		while(gameObject != null)
		{
			for(int I=0; I<SpriteSet.Length;I++)
			{
				SpriteHolder.sprite = SpriteSet [I];
				yield return new WaitForSeconds ((1f/FramesPerSec));
			}
			yield return null;

			for(int I=SpriteSet.Length-2; I>=1; I--)
			{
				SpriteHolder.sprite = SpriteSet [I];
				yield return new WaitForSeconds ((1f/FramesPerSec));
			}
		}
	}
}
