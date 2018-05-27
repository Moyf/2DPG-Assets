// RegalPrime 11-04-14 - BlockTrap.cs

// Simple trap that has two options
// 1. DisappearOnTouch = true - Object will start visible and disable itself when touched by the player
// 2. AppearOnTouch = true - Object will start invisible and become visible when touched by the player

// An audio clip is played on the triggering of the script (you do not need to add a audioclip though)

// Setting tag to ground will prevent bullets from passing through the object
// AppearOnTouch object will automatically set the objects layer to "Ground"
// This will allow the player to jump off of the object / bullets will be stopped by the hidden object

using UnityEngine;
using System.Collections;

public class BlockTrap : MonoBehaviour
{
	public bool DisappearOnTouch = true;
	public bool AppearOnTouch = false;
	public AudioClip activateSound;			// Trigger sound if wanted

	private int GroundLayerIndex;
	private int TransparentLayer;

	private static AudioSource playingSound;
	private float delayBetweenAudio = 0;

	void Start ()
	{
		EventManager.resetObjects += Reset;

		GroundLayerIndex = LayerMask.NameToLayer("SolidObjects");
		TransparentLayer = LayerMask.NameToLayer("Default");

		delayBetweenAudio = activateSound.length/10f;

		if(DisappearOnTouch && AppearOnTouch)
			print ("BlockTrap.cs WARNING : Selecting both options will result in undesirable effects");

		// Disables the sprite renderer to make the object appear invisible
		if(AppearOnTouch)
			gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}

	void Reset()
	{
		gameObject.SetActive(true);
		gameObject.layer = TransparentLayer;
		gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
		if(AppearOnTouch)
			gameObject.GetComponent<SpriteRenderer>().enabled = false;
		else
			gameObject.GetComponent<SpriteRenderer>().enabled = true;
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			// Trigger has been hit and object needs to disappear. Done via destroying the object
			if(DisappearOnTouch)
			{
				if(activateSound != null)
				{
					if(playingSound == null)
						playingSound = MusicController.control.OneShotAudio(activateSound);
					else
					{
						if(playingSound.time > delayBetweenAudio)
						{
							playingSound = MusicController.control.OneShotAudio(activateSound);
						}
					}
				}

				gameObject.SetActive(false);
			}

			// Trigger has been hit and the object needs to appear. Collider isTrigger is set to false to make the object solid
			if(AppearOnTouch)
			{
				if(playingSound == null)
					playingSound = MusicController.control.OneShotAudio(activateSound);
				else
				{
					if(playingSound.time > delayBetweenAudio)
					{
						playingSound = MusicController.control.OneShotAudio(activateSound);
					}
				}

				gameObject.layer = GroundLayerIndex;
				gameObject.GetComponent<SpriteRenderer>().enabled = true;
				gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
			}
		}
	}
}

