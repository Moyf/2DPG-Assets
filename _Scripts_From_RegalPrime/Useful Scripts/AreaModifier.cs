// RegalPrime 11-25-14 - AreaModifier.cs

// This is a simple script that accesses the character stats and changes them when this trigger is hit.
// Can change gravity / jumpforce / movespeed / maxfallspeed
// Can enable autofire / infinityjump
// Can add a trailrenderer (I just put that in and have no clue the usefulness of doing this at the moment)

// This script changes the multiplier of these stats (except maxfallspeed)
// For example 2 = 2x the speed and 0.5f = 1/2 the speed.

// RevertOnExit = true - This will revert the character stats back to default values (ie 1x gravity ...)
// This is done for simplicity. I felt that saving the current stats and defaulting back to them would cause issues when multitriggering 
// between many different area modifier scripts.

// WARNING !!
// Butting 2 of these triggers against each other with revertOnExit = true will cause it to act stupidly
// Upon entering the 2nd object, you will recieve its stats change, but when your butt leaves the first object, you will go back to default settings
// I think I have a way to fix this, but I will do that later (sorry).


using UnityEngine;
using System.Collections;

public class AreaModifier : MonoBehaviour
{
	public bool ChangeGravity = false;			// Can the trigger change the gravity?
	public float gravityMultiplier = 1;

	public bool ChangeJumpforce = false;		// Can the trigger change the jumpforce?
	public float jumpforceMultiplier = 1;

	public bool ChangeSpeed = false;			// Can the trigger change the speed?
	public float speedMultiplier = 1;

	public bool ChangeMaxFallSpeed = false;		// Can the trigger change the maxfallspeed?
	public float newMaxFallSpeed = -9.0f;

	public bool ChangeMaxNumberOfExtraJumps = false;	// Can the trigger change the number of extra jumps?
	public int newMaxNumberOfExtraJumps = 1;

	public bool autoFire = false;				// Enable autofire (hold button = constant firing)
	public bool infinityJump = false;			// Enable infinity jumping (can jump unlimited times)

	public bool OneTimeActivate = false;		// Disable after trigger is hit
	public bool RevertOnExit = false;			// Reset the stats of the player back to default values on exit
	public bool RefreshMultiJumpOnExit = false;	// Makes sure double jump is true on exit

	private PlatformerCharacter2D GuysScript;	// Reference to the player that hits the trigger

	static int numberOfTriggered = 0;

	void Awake()
	{
		numberOfTriggered = 0;
	}
	void Start()
	{
		EventManager.resetObjects += Reset;
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
//	void OnLevelWasLoaded(int level)
//	{
//
//	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == ("Player") && other.gameObject.GetComponent<PlatformerCharacter2D> () != null)				// If player hits the collider, then change its stats
		{
			GuysScript = other.gameObject.GetComponent<PlatformerCharacter2D> ();

			if(ChangeGravity)
				GuysScript.gravityMultiplier = gravityMultiplier;

			if(ChangeJumpforce)
				GuysScript.jumpforceMultiplier = jumpforceMultiplier;

			if(ChangeSpeed)
				GuysScript.movementMultiplier = speedMultiplier;

			if(ChangeMaxFallSpeed)
				GuysScript.maxFallSpeed = newMaxFallSpeed;

			if(ChangeMaxNumberOfExtraJumps)
				GuysScript.Set_MultiJumps(newMaxNumberOfExtraJumps);

			if(autoFire && GuysScript.GetComponent<Shooting>() != null)
				GuysScript.GetComponent<Shooting>().AutoFire = true;

			if(infinityJump)
				GuysScript.infinityJump = true;

			if(OneTimeActivate)
				gameObject.SetActive (false);

			numberOfTriggered ++;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == ("Player") && other.gameObject.GetComponent<PlatformerCharacter2D> () != null && RevertOnExit)	// If player hits the collider, then revert the stats (if revertOnExit = true)
		{
			if(numberOfTriggered == 1)		// Only revert if you are not touching any other area modifier
			{
				Reset (other.GetComponent<PlatformerCharacter2D> ());
			}

			numberOfTriggered --;
		}
	}

	void Reset(PlatformerCharacter2D aPlayer)	// Set all the stats back to default values
	{
		if(ChangeGravity)
			aPlayer.ResetGravityMultiplier();
		
		if(ChangeJumpforce)
			aPlayer.ResetJumpforceMultipler ();
		
		if(ChangeSpeed)
			aPlayer.ResetMovementMultiplier();
		
		if(ChangeMaxFallSpeed)
			aPlayer.ResetMaxFallSpeed();

		if(ChangeMaxNumberOfExtraJumps)
			aPlayer.Reset_MultiJump();
		
		if(autoFire && aPlayer.GetComponent<Shooting>() != null)
			aPlayer.GetComponent<Shooting>().AutoFire = false;
		
		if(infinityJump)
		{
			aPlayer.infinityJump = false;
		}
		
		if(RefreshMultiJumpOnExit)
			aPlayer.Refresh_MultiJump();

		if(OneTimeActivate)
			gameObject.SetActive (true);

		gameObject.SetActive (false);
		gameObject.SetActive (true);
	}
	void Reset()
	{
		numberOfTriggered = 0;
		if(GuysScript != null)
			Reset (GuysScript);
	}

}



/*
void Reset()	// Set all the stats back to default values
	{
		if(GuysScript == null)
			GuysScript = GameController.control.GetPlayerGameObject().GetComponent<PlatformerCharacter2D> ();

		if(ChangeGravity)
			GuysScript.ResetGravityMultiplier();
		
		if(ChangeJumpforce)
			GuysScript.ResetJumpforceMultipler ();
		
		if(ChangeSpeed)
			GuysScript.ResetMovementMultiplier();
		
		if(ChangeMaxFallSpeed)
			GuysScript.ResetMaxFallSpeed();

		if(ChangeMaxNumberOfExtraJumps)
			GuysScript.Reset_MultiJump();
		
		if(AddTrailRenderer)
		{
			Destroy(CloneTrailRenderer);
		}
		
		if(autoFire)
			GuysScript.GetComponent<Shooting>().AutoFire = false;
		
		if(infinityJump)
			GuysScript.infinityJump = false;
		
		if(RefreshDoubleJumpOnExit)
			GuysScript.Refresh_MultiJump();

		if(OneTimeActivate)
			gameObject.SetActive (true);

		gameObject.SetActive (false);
		gameObject.SetActive (true);
	}


 */