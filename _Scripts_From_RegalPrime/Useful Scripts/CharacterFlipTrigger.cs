// RegalPrime 11-25-14 - CharacterFlipTrigger.cs

// When the player hits this trigger, they will flip upsidedown
// If both UprightFlip / UpsideDownFlip are the same bool, the character will flip to the opposite
// Selecting either UprightFlip / UpsideDownFlip as true will enable you to choose how you want the trigger to act

// By default InstaFlip = false, which means the players current fallspeed will be maintained even when flipping
// Of course after flipping, the gravity will act on the player in the reverse direction.
// In order to instantly change direction when triggering, InstaFlip = true.

// Be aware, with all the changes to the PlatformerCharacter2D controller and other script changes
// there may be certain bugs when interacting with the world while upsidedown (calculations are not necessarly the same while upside down and I may have missed something).

using UnityEngine;
using System.Collections;

public class CharacterFlipTrigger : MonoBehaviour
{
	public float ReuseDelay = 0.5f;				// How often this object can be triggered

	public bool UprightFlip = false;			// If not already - Flips the character upright
	public bool UpsideDownFlip = false;			// If not already - Flips the character upside down

	public bool InstaFlip = false;				// Will zero out the vertical velocity of the character
	public bool DoubleJumpRefresh = false;		// Refreshes double jump when the trigger is hit

	private bool AbleToFlip = true;				// Use to prevent multi triggering of this object


	void Start()
	{
		EventManager.resetObjects += Reset;
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		StopAllCoroutines ();
		gameObject.GetComponent<BoxCollider2D> ().enabled = false;
		gameObject.GetComponent<BoxCollider2D> ().enabled = true;
		AbleToFlip = true;
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == ("Player") && AbleToFlip && other.GetComponent<PlatformerCharacter2D> () != null)
		{
			StartCoroutine (C_Flip (other.GetComponent<PlatformerCharacter2D> ()));
		}
	}

	IEnumerator C_Flip(PlatformerCharacter2D GuysScript)
	{
		AbleToFlip = false;

		if (InstaFlip)
			GuysScript.ZeroFallSpeed();

		if(UpsideDownFlip == UprightFlip)
		{
			GuysScript.FlipCharacter();
			if(DoubleJumpRefresh)
				GuysScript.Refresh_MultiJump();
		}
		else
		{
			if(UprightFlip)
				if(GuysScript.FlipCharacter_Upright() && DoubleJumpRefresh)
					GuysScript.Refresh_MultiJump();
			
			if(UpsideDownFlip)
				if(GuysScript.FlipCharacter_UpsideDown() && DoubleJumpRefresh)
					GuysScript.Refresh_MultiJump();
		}

		yield return new WaitForSeconds(ReuseDelay);
		AbleToFlip = true;
	}
}
