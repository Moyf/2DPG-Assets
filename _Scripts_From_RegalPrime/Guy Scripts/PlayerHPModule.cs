// RegalPrime 4-23-15 - PlayerHPModule.cs

// When this object touches a deadly tagged object, it will decrement the HP. If the player has 0 then it will
// create the gameover popup / sound / and effect
// Sound effects will only be played if they are attached

using UnityEngine;
using System.Collections;

public class PlayerHPModule : MonoBehaviour
{
	public GameObject gameOverScreen;				// Prefab that displays the gameover text
	public GameObject bloodParticleEffect;			// Prefab of a particle effect

	public AudioClip hitSound;						// Sound when player is hit and still has hp
	public AudioClip deathSound;					// Death sound to play
	public AudioClip deathMusic;					// Death music to play

	public int PlayerMaxHP = 1;						// How much hp the player will have upon spawn
	public float DamageDelay = 1f;					// How often the player can be damaged
	private int PlayerCurrentHP = 0;				// Current HP the player has

	private bool CanPlayerBeDamaged = true;			// Tells the script if the player can be damage
	private GameObject gameOverScreen_Clone;		// Reference to the gameover screen
	private GameObject bloodParticleEffect_Clone;	// Reference to the particle effect
	private AudioSource deathSound_Clone;			// Reference to the death sound clip

	void Start()
	{
		PlayerCurrentHP = PlayerMaxHP;
		EventManager.resetObjects += Reset;
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	
	void Reset()
	{
		PlayerCurrentHP = PlayerMaxHP;
		CanPlayerBeDamaged = true;
		StopAllCoroutines ();

		if(gameOverScreen_Clone != null)
			Destroy (gameOverScreen_Clone);

		if(bloodParticleEffect_Clone != null)
			Destroy (bloodParticleEffect_Clone);

		if(deathSound_Clone != null)
			Destroy (deathSound_Clone);

		MusicController.control.UnFadeMusic();
	}


	void OnTriggerStay2D(Collider2D other)
	{

		if (other.gameObject.tag == "Deadly" && CanPlayerBeDamaged)
		{
			CanPlayerBeDamaged = false;
			StartCoroutine (ResetDamageDelay());
			PlayerCurrentHP --;

			if(PlayerCurrentHP <= 0)
			{
				if(gameOverScreen != null)
					gameOverScreen_Clone = Instantiate(gameOverScreen, new Vector2(0,0), Quaternion.identity) as GameObject;

				if(bloodParticleEffect != null)
					bloodParticleEffect_Clone = Instantiate(bloodParticleEffect, transform.position, transform.rotation) as GameObject;

				if(deathSound != null)
					MusicController.control.OneShotAudio(deathSound);

				if(deathMusic != null)
				{
					MusicController.control.FadeMusic();
					deathSound_Clone = MusicController.control.OneShotAudio(deathMusic);
				}

				EventManager.control.OnPlayerDeath(other.gameObject);		// Tell the event manager the player has died (or others can know)

				gameObject.SetActive(false);

				GameController.control.DeathCounter++;						// +1 death
				if(GameController.control.StartAtBeginning == false)		// If the player has a save file going this run save the death
					GameController.control.SaveTimerDeath ();
				GameController.control.StopTimer();							// Stop ingame timer
			}
			else
			{
				if(hitSound != null)
					MusicController.control.OneShotAudio(hitSound);
			}
		}
	}



	IEnumerator ResetDamageDelay ()
	{
		yield return new WaitForSeconds(DamageDelay);
		CanPlayerBeDamaged = true;
	}


}