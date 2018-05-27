// RegalPrime 5-20-15 - AddSelfToPlayer.cs

// Adds this object to the player at a certain offset when a certain button is pressed
// This was written for picking up / dropping the torch object

// Attaches to the EventManager to detect when the action button is pressed

using UnityEngine;
using System.Collections;

public class AddSelfToPlayer : MonoBehaviour
{	
	public float X_Offset = 0.16f;		// X-Offset when object is attached
	public float Y_Offset = 0.16f;		// Y-Offset when object is attached

	private bool PickedUp = false;		// Tells the script if the object is picked up
	private Collider2D playersObject;	// The player that is standing in the collider

	private Vector3 startingPosition;	// Initial position of this object

	void Start()
	{
		EventManager.CreateEventManagerIfNeeded();
		EventManager.resetObjects += Reset;
		startingPosition = gameObject.transform.position;
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void OnEnable()
	{
		EventManager.actionButton += OnButtonPressed_actionButton;
	}
	void OnDisable()
	{
		EventManager.actionButton -= OnButtonPressed_actionButton;
	}
	void Reset()
	{
		PickedUp = false;
		gameObject.transform.parent = null;
		gameObject.transform.position = startingPosition;
		gameObject.SetActive (true);
		gameObject.GetComponent<CircleCollider2D>().enabled = true;
	}

	void OnButtonPressed_actionButton()
	{
		if(playersObject != null)					// If a player is near this object
		{
			if(!PickedUp)
				StartCoroutine (C_AttachObject(playersObject));
			else
				StartCoroutine (C_DetachObject());
		}
	}

	// Find when the player enters and exits this object
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
			playersObject = other;
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (other == playersObject)
			playersObject = null;		
	}

	IEnumerator C_AttachObject(Collider2D other)
	{
		gameObject.transform.parent = other.transform;
		gameObject.transform.localPosition = new Vector3(X_Offset, Y_Offset, gameObject.transform.localPosition.z);
		gameObject.GetComponent<CircleCollider2D>().enabled = false;
		
		yield return null;
		PickedUp = true;
	}
	IEnumerator C_DetachObject()
	{
		gameObject.transform.parent = null;
		gameObject.GetComponent<CircleCollider2D>().enabled = true;

		yield return null;
		PickedUp = false;
	}




}



/*

	void Update()
	{
		if(PickedUp && Input.GetKeyDown(KeyCode.X))		// Drop the torch
		{
			gameObject.transform.parent = null;
			gameObject.GetComponent<CircleCollider2D>().enabled = true;

			PickedUp = false;
		}
	}
	void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "Player" && Input.GetKeyDown(KeyCode.X))		// Pick up the torch
		{
			StartCoroutine (C_AttachedObject(other));
		}
	}


	IEnumerator C_AttachedObject(Collider2D other)
	{
		gameObject.transform.parent = other.transform;
		gameObject.transform.localPosition = new Vector3(X_Offset, Y_Offset, gameObject.transform.localPosition.z);
		gameObject.GetComponent<CircleCollider2D>().enabled = false;

		yield return null;
		PickedUp = true;
	}

*/