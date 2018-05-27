// RegalPrime 11-25-14 - TeleportInRoom.cs

// Teleports the player to the target location
// By default, the player will warp to the exact position of the warpTO object
// By selecting either the MaintainXoffset / MaintainYoffset, the players entry will determine the offset of the exit
// This allows for a more believable screen wrapping effect

// For now, this is the only means of creating screen wrapping


using UnityEngine;
using System.Collections;

public class TeleportInRoom : MonoBehaviour 
{
	public Transform warpTO;					// Target to warp to

	public float warpDelay = 0;					// Delay between intial trigger and warp destination
	public bool MaintainXoffset = false;		// Use the X-offset of the trigger area to calculate an offset on exit
	public bool MaintainYoffset = false;		// Use the Y-offset of the trigger area to calculate an offset on exit

	public GameObject teleportEffect;			// Effect that is played when triggered
	public AudioClip teleportSound;				// Sound that is played when triggered

	private bool isWarping = false;

	private Collider2D otherObject;
	private GameObject effectClone1;
	private GameObject effectClone2;

	void Start ()
	{
		EventManager.resetObjects += Reset;

		if (warpDelay < 0)
			warpDelay = 0;
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	
	void Reset()
	{
		StopAllCoroutines ();
		if(effectClone1 != null)
			Destroy (effectClone1);
		if(effectClone2 != null)
			Destroy (effectClone2);
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player" && !isWarping)
		{
			isWarping = true;

			effectClone1 = createWarpEffect(transform);
			otherObject = other;

			other.gameObject.SetActive(false);
			if(warpDelay > 0)				// If there is a delay between warp / exit
			{
				StartCoroutine(warp (warpDelay, other.gameObject));

			}else
			{
				StartCoroutine(warp (0, other.gameObject));
			}
		}
	}

	IEnumerator warp(float delay, GameObject ObjectToWarp)
	{
		yield return new WaitForSeconds (delay);

		float tempX = warpTO.transform.position.x;
		float tempY = warpTO.transform.position.y;

		// Calculate offsets
		if(MaintainXoffset)
			tempX += (otherObject.transform.position.x - gameObject.transform.position.x);

		if(MaintainYoffset)
			tempY += (otherObject.transform.position.y - gameObject.transform.position.y);

		ObjectToWarp.transform.position = new Vector2(tempX, tempY);
		effectClone2 = createWarpEffect (warpTO.transform);
		ObjectToWarp.SetActive(true);

		isWarping = false;
	}

	GameObject createWarpEffect(Transform TEMP)
	{
//		if(TEMP == warpTO.transform && warpDelay<0.5)
		if(teleportSound != null && (TEMP == transform || warpDelay>0.5))
		{
			MusicController.control.OneShotAudio(teleportSound);
		}

		if(teleportEffect != null)
			return Instantiate(teleportEffect, TEMP.position, Quaternion.identity) as GameObject;

		return null;
	}
}


/*
// RegalPrime 11-25-14 - TeleportInRoom.cs

// Teleports the player to the target location
// By default, the player will warp to the exact position of the warpTO object
// By selecting either the MaintainXoffset / MaintainYoffset, the players entry will determine the offset of the exit
// This allows for a more believable screen wrapping effect

// For now, this is the only means of creating screen wrapping


using UnityEngine;
using System.Collections;

public class TeleportInRoom : MonoBehaviour 
{
	public Transform warpTO;					// Target to warp to

	public float warpDelay = 0;					// Delay between intial trigger and warp destination
	public bool MaintainXoffset = false;		// Use the X-offset of the trigger area to calculate an offset on exit
	public bool MaintainYoffset = false;		// Use the Y-offset of the trigger area to calculate an offset on exit

	public GameObject teleportEffect;			// Effect that is played when triggered
	public AudioClip teleportSound;				// Sound that is played when triggered

	private bool isWarping = false;

	private Collider2D otherObject;
	private GameObject effectClone1;
	private GameObject effectClone2;

	void Start ()
	{
		ResetManager.resetObjects += Reset;

		if (warpDelay < 0)
			warpDelay = 0;
	}
	void OnDestroy()
	{
		ResetManager.resetObjects -= Reset;
	}
	
	void Reset()
	{
		StopAllCoroutines ();
		if(effectClone1 != null)
			Destroy (effectClone1);
		if(effectClone2 != null)
			Destroy (effectClone2);
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player" && !isWarping)
		{
			isWarping = true;

			effectClone1 = createWarpEffect(transform);
			otherObject = other;

			GameController.control.GuyLocation.SetActive(false);
			if(warpDelay > 0)				// If there is a delay between warp / exit
			{
//				GameController.control.GuyLocation.transform.position = warpTO.transform.position;
				StartCoroutine(warp (warpDelay));

			}else
			{
				StartCoroutine(warp (0));
			}
		}
	}

	IEnumerator warp(float delay)
	{
		yield return new WaitForSeconds (delay);

		float tempX = warpTO.transform.position.x;
		float tempY = warpTO.transform.position.y;

		// Calculate offsets
		if(MaintainXoffset)
			tempX += (otherObject.transform.position.x - gameObject.transform.position.x);

		if(MaintainYoffset)
			tempY += (otherObject.transform.position.y - gameObject.transform.position.y);

		GameController.control.GuyLocation.transform.position = new Vector2(tempX, tempY);
		effectClone2 = createWarpEffect (warpTO.transform);
		GameController.control.GuyLocation.SetActive(true);

		isWarping = false;
	}

	GameObject createWarpEffect(Transform TEMP)
	{
//		if(TEMP == warpTO.transform && warpDelay<0.5)
		if(teleportSound != null && (TEMP == transform || warpDelay>0.5))
		{
			MusicController.control.OneShotAudio(teleportSound);
		}

		if(teleportEffect != null)
			return Instantiate(teleportEffect, TEMP.position, Quaternion.identity) as GameObject;

		return null;
	}
}

*/