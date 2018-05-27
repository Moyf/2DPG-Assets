// RegalPrime 11-26-14 - JumpRefresher.cs

// When the player hits this trigger, their double jump is set to true
// Has options to re-enable this object after a certain respawn rate

using UnityEngine;
using System.Collections;

public class JumpRefresher : MonoBehaviour
{
	public int AddToExtraJump = 0;
	public bool GoOverExtraJumpCap = false;

	public bool destroySelfOnHit = false;		// Object will re-enable after a certain amount of time if false
	public float respawnRate = 1f;				// Delay between object re-enable

	public GameObject refresherEffect;			// Effect that plays when object is triggered
	public AudioClip refresherSound;			// Sound that plays when object is triggered

	private GameObject effectClone;

	void Start ()
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
		gameObject.GetComponent<SpriteRenderer>().enabled = true;
		gameObject.GetComponent<CircleCollider2D>().enabled = true;
		if (effectClone != null)
			Destroy (effectClone);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == ("Player") && other.gameObject.GetComponent<PlatformerCharacter2D> () != null)
		{
			if(AddToExtraJump > 0)
				other.gameObject.GetComponent<PlatformerCharacter2D> ().AddToMultiJump(AddToExtraJump, GoOverExtraJumpCap);
			else
				other.gameObject.GetComponent<PlatformerCharacter2D> ().Refresh_MultiJump();

			gameObject.GetComponent<SpriteRenderer>().enabled = false;
			gameObject.GetComponent<CircleCollider2D>().enabled = false;

			createEffect(transform);

			if(destroySelfOnHit==false)
				StartCoroutine (Reenable ());
		}

	}

	void createEffect(Transform TEMP)
	{
		if(refresherEffect != null)
			effectClone = Instantiate(refresherEffect, TEMP.position, Quaternion.identity)as GameObject;
		
		if(refresherSound != null)
			MusicController.control.OneShotAudio(refresherSound);
			
	}

	IEnumerator Reenable()
	{
		yield return new WaitForSeconds (respawnRate);
		gameObject.GetComponent<SpriteRenderer>().enabled = true;
		gameObject.GetComponent<CircleCollider2D>().enabled = true;
	}
}
