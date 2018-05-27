// RegalPrime 11-08-14 - TimeModifierTrigger.cs

// This script creates a time slow / speed effect. This effects the entire scene.
// It does it by modifying the timescale / physics / audio pitch. This changes how often fixed update is called.

// Selecting MaintainNormalMovement will cause the stats of the player to be changed to give the illusion that he is still able to move at the same speed
// There seems to be a problem when lerping and maintaining normal movement is selected. There may be variances in relative speed / jumping ability during the Lerp
// I belive this is because of the velocity based movement + the lerping of the physics timescale causes the velocities to be calculated oddly during the lerp
// The player will jump slightly higher than normal when speeding up time during the first part of the lerp and lower when slowing down time.
// At the end of the lerp, the player will move at the "normal" speed

// It has the option to do the effect over a certain period of time (this is called Lerping)
// Be aware that triggering multiple of these scripts while others are Lerping, will cause them to fight each other
// Since the speed changes are fixed times, they will end when they are supposed to and not fight each other forever.

// Be aware that speeding up time increases the calls to fixed update and thus is most costly in terms of performance

// Speed change 
// 1 = over 1 second
// 2 = over 0.5s second
// 0.5f = over 2 second
// Etc etc

using UnityEngine;
using System.Collections;

public class TimeModifierTrigger : MonoBehaviour
{
	public float DelayStart = 0;					// Time to delay the start
	public float speedScale = 0.4f;					// Rate at which the game is slowed down / spead up

	public float LerpSpeed = 1;						// Speed at which the timescale changes
	public bool LerpEnter = true;					// Lerp on enter (false = instant timescale change)
	public bool LerpExit = true;					// Lerp on exit (false = instant timescale change)

	public bool maintainNormalMovement = false;		// Changes the players stats to give the illusion he is moving at the same speed / jump as before
	public bool RevertOnExit = true;				// If the player reverts to normal on exiting the collider

	private float FixedTime;						// Backup of what the default fixed time is

	void Start()
	{
		EventManager.resetObjects += Reset;
		FixedTime = Time.fixedDeltaTime;
	}
	void Reset()
	{
		StopAllCoroutines ();
		StartCoroutine (C_RevertTimeModifier ());
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
				StopAllCoroutines ();			// This will stop this objects script from multi triggering 

			if(LerpEnter)
				StartCoroutine ("C_ChangeTimeModifier_Lerp");
			else
				StartCoroutine ("C_ChangeTimeModifier");
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player" && RevertOnExit)
		{
			StopAllCoroutines ();				// This will stop this objects script from multi triggering 

			if(LerpExit)
				StartCoroutine ("C_RevertTimeModifier_Lerp");
			else
				StartCoroutine ("C_RevertTimeModifier");
		}
	}

	// Reverts music / timescales back to normal when this object is destroyed (for a scene change)
	// I guess in theory this is not a good idea because the player may destroy this gameobject for other reasons than scene change
	// Be aware of this and possibly change if needed later.
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;

		if(MusicController.control != null)
			MusicController.control.GetComponent<AudioSource>().pitch = 1f;

//		if (GameController.control.GuyLocation != null)
//			GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D>().ResetGravityMultiplier();

		Time.timeScale = 1f;
		Time.fixedDeltaTime = FixedTime;
	}

	// Modifies the area / player stats instantly
	IEnumerator C_ChangeTimeModifier()
	{
		yield return new WaitForSeconds (DelayStart);
		Animator anim = GameController.control.GuyLocation.GetComponent<Animator> ();

		Time.timeScale = speedScale;
		Time.fixedDeltaTime = FixedTime*speedScale;
		MusicController.control.GetComponent<AudioSource>().pitch = speedScale;
		
		if(maintainNormalMovement)
		{
			GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D>().movementMultiplier = (1/speedScale);
			GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D>().gravityMultiplier = (1/speedScale);
			GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D>().jumpforceMultiplier = (1/speedScale);

			anim.speed = 1/speedScale;
		}
		else
		{
			GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D>().movementMultiplier = (speedScale);
			GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D>().gravityMultiplier = (speedScale);
//			GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D>().jumpforceMultiplier = (speedScale);
		}


		yield return null;
	}

	// Reverts the area / player stats instantly
	IEnumerator C_RevertTimeModifier()
	{
		Animator anim = GameController.control.GuyLocation.GetComponent<Animator> ();
		anim.speed = 1;

		Time.timeScale = 1f;
		Time.fixedDeltaTime = FixedTime;
		GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D>().ResetGravityMultiplier();
		GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D>().ResetJumpforceMultipler();
		GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D>().ResetMovementMultiplier();
		MusicController.control.GetComponent<AudioSource>().pitch = 1f;	

		yield return null;
	}


	// Change the area / player stats over time
	IEnumerator C_ChangeTimeModifier_Lerp()
	{
		yield return new WaitForSeconds (DelayStart);

		PlatformerCharacter2D PlayerStats = GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D> ();
		Animator anim = GameController.control.GuyLocation.GetComponent<Animator> ();

		float B_timescale = Time.timeScale;
		float B_fixedDeltaTime = Time.fixedDeltaTime;
		float B_fixedDeltaTime_End = FixedTime * speedScale;

		float B_Pitch = MusicController.control.GetComponent<AudioSource> ().pitch;
		float B_AnimSpeed = anim.speed;

		float B_GravityMultiplier = PlayerStats.gravityMultiplier;
		float B_MovementMultiplier = PlayerStats.movementMultiplier;
		float B_JumpforceMultiplier = PlayerStats.jumpforceMultiplier;

		for(float I=0; I<=1; I+=FixedTime*LerpSpeed)
		{
			Time.timeScale = Mathf.Lerp(B_timescale, speedScale, I);
			Time.fixedDeltaTime = Mathf.Lerp (B_fixedDeltaTime, B_fixedDeltaTime_End, I);
			MusicController.control.GetComponent<AudioSource>().pitch = Mathf.Lerp(B_Pitch, speedScale, I);

			if(maintainNormalMovement)
			{
				PlayerStats.gravityMultiplier = (Mathf.Lerp(B_GravityMultiplier, 1/speedScale, I));
				PlayerStats.movementMultiplier = (Mathf.Lerp (B_MovementMultiplier, 1/speedScale, I));
				PlayerStats.jumpforceMultiplier = (Mathf.Lerp (B_JumpforceMultiplier, 1/speedScale, I));
				anim.speed = (Mathf.Lerp (B_AnimSpeed, 1/speedScale, I));
			}
			else
			{
				PlayerStats.gravityMultiplier = (Mathf.Lerp(B_GravityMultiplier, speedScale, I));
				PlayerStats.movementMultiplier = (Mathf.Lerp (B_MovementMultiplier, speedScale, I));
//				PlayerStats.jumpforceMultiplier = (Mathf.Lerp (B_JumpforceMultiplier, speedScale, I));
				anim.speed = (Mathf.Lerp (B_AnimSpeed, speedScale, I));
			}

			yield return null;
		}

		yield return null;
	}

	// Revert the area / player stats over time
	IEnumerator C_RevertTimeModifier_Lerp()
	{
		PlatformerCharacter2D PlayerStats = GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D> ();
		Animator anim = GameController.control.GuyLocation.GetComponent<Animator> ();
		
		float B_timescale = Time.timeScale;
		float B_fixedDeltaTime = Time.fixedDeltaTime;

		float B_Pitch = MusicController.control.GetComponent<AudioSource> ().pitch;
		float B_AnimSpeed = anim.speed;
		
		float B_GravityMultiplier = PlayerStats.gravityMultiplier;
		float B_MovementMultiplier = PlayerStats.movementMultiplier;
		float B_JumpforceMultiplier = PlayerStats.jumpforceMultiplier;
		
		
		for(float I=0; I<1; I+=FixedTime*LerpSpeed)
		{
			Time.timeScale = Mathf.Lerp(B_timescale, 1, I);
			Time.fixedDeltaTime = Mathf.Lerp (B_fixedDeltaTime, FixedTime ,I);
			MusicController.control.GetComponent<AudioSource>().pitch = Mathf.Lerp(B_Pitch, 1, I);
			
			if(maintainNormalMovement)
			{
				PlayerStats.gravityMultiplier = (Mathf.Lerp(B_GravityMultiplier, 1, I));
				PlayerStats.movementMultiplier = (Mathf.Lerp (B_MovementMultiplier, 1, I));
				PlayerStats.jumpforceMultiplier = (Mathf.Lerp (B_JumpforceMultiplier, 1, I));
				anim.speed = (Mathf.Lerp (B_AnimSpeed, 1, I));
			}
			else
			{
				PlayerStats.gravityMultiplier = (Mathf.Lerp(B_GravityMultiplier, 1, I));
				PlayerStats.movementMultiplier = (Mathf.Lerp (B_MovementMultiplier, 1, I));
				PlayerStats.jumpforceMultiplier = (Mathf.Lerp (B_JumpforceMultiplier, 1, I));
				anim.speed = (Mathf.Lerp (B_AnimSpeed, 1, I));
			}
			
			yield return null;
		}
		
		yield return null;
	}
}