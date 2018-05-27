// RegalPrime 11-25-14 - Enable_Disable_OnHit.cs

// When the trigger is activated, this script:
// enables all objects in the vector ObjectsToEnable
// disables all objects in the vector ObjectsToDisable

// Size of the vector and the amount of objects can be changed in the editor

// This was created to be a onestopshop for dealing with both enabling and disabling objects via trigger


using UnityEngine;
using System.Collections;

public class Enable_Disable_OnHit : MonoBehaviour
{
	public string TriggerTagName;						// What tag can trigger this - "Player" "Bullet" are 2 of the main ones
	public bool DisableSelfOnTrigger = false;			// Will disable self after triggering

	public AudioClip OnHitSound;						// A sound to play when triggering

	public GameObject[] ObjectsToEnable;				// Vector of objects to enable
	public GameObject[] ObjectsToDisable;				// Vector of objects to disable

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
		foreach (GameObject child in ObjectsToEnable)
			child.SetActive(false);
		
		foreach (GameObject child in ObjectsToDisable)
			child.SetActive(true);

		gameObject.SetActive(false);
		gameObject.SetActive(true);
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == TriggerTagName)
		{
			if(OnHitSound != null)
				MusicController.control.OneShotAudio(OnHitSound);

			foreach (GameObject child in ObjectsToEnable)
				child.SetActive(true);

			foreach (GameObject child in ObjectsToDisable)
				child.SetActive(false);

			if(DisableSelfOnTrigger)
				gameObject.SetActive(false);
		}
	}
}
