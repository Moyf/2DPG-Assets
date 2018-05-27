// RegalPrime 11-25-14 - ObjectTrap.cs

// Takes all of the children objects of the TrapLocation Gameobject and prepare to launch them when a trigger is hit
// Can launch them at the player
// Can launch them up / down / left / right (multiple selections will launch them in both directions (ie up and right), but left + rigth would cause it to not move)

// I kept the above options for simplicity, but it would be fairly easy to allow the user to input the Xvelocity / Yvelocity themselves in the editor
// This might be needed later, and I will change it when necessary

// On startup, the script will un-parent the child objects of TrapLocation, add them to a vector, and add a rigidbody2d to them
// On trigger, there is an initial delay variable, then an individual delay between each object in the vector when launching the objects

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectTrap : MonoBehaviour
{
	private Transform[] traps;
	public List<Vector3> trapsLocation = new List<Vector3>();

	public GameObject TrapLocation;					// Location of the objects you want to launch

	public float trapSpeed = 1;						// Speed at which the individual objects will launch

	public bool up = true;							// Direction to launch
	public bool down = false;						// Direction to launch
	public bool left = false;						// Direction to launch
	public bool right = false;						// Direction to launch

	private float Xvelocity = 0;
	private float Yvelocity = 0;

	public bool atPlayer;							// Shoot the objects at the player (this trumps the other options)

	public float delayStartTimer = 0;				// Time delay between trigger and launching the first object
	public float objectDeathTimer = 5;				// When the object is disabled after launch
	public float objectDelays = 0;					// Delay between each object in the vector

	public AudioClip triggerSound;					// Sound that is played when triggered

	private bool tripped = false;					// Disallows multriggering of the trigger



	void Start ()
	{
		Rigidbody2D TEMP;
		EventManager.resetObjects += Reset;
		traps = TrapLocation.GetComponentsInChildren<Transform> (gameObject);		// Get all child objects

//		foreach (Transform itslocation in traps)
		for(int I=0; I<traps.Length; I++)											// Set the objects variables
		{
			traps[I].transform.parent = null;
			TEMP = traps[I].gameObject.AddComponent<Rigidbody2D>() as Rigidbody2D;
			TEMP.freezeRotation = true;
			TEMP.isKinematic = true;

			trapsLocation.Add (traps[I].position);
		}

//		for(int I=0; I< traps.Length; I++)
//			traps[I].transform.parent = null;


//		X GOES RIGHT - Y GOES UP
		if(up)
			Yvelocity += 1;
		if(down)
			Yvelocity -= 1;
		if(right)
			Xvelocity += 1;
		if(left)
			Xvelocity -= 1;



//		print (traps.Length);
//		print (traps [0].name);
	}

	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		StopAllCoroutines ();
		for(int I=1; I<traps.Length; I++)
		{
			traps[I].GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			traps[I].position = trapsLocation[I];
			traps[I].gameObject.SetActive(true);

			tripped = false;
		}
		gameObject.GetComponent<BoxCollider2D> ().enabled = false;
		gameObject.GetComponent<BoxCollider2D> ().enabled = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == ("Player") && tripped == false)			// Trigger was hit by player
		{
			tripped = true;
			StartCoroutine(triggerObjects());
		}
	}


	IEnumerator triggerObjects()											// Launch the objects at the target
	{
		yield return new WaitForSeconds(delayStartTimer);

		Rigidbody2D TEMP;

		MusicController.control.OneShotAudio(triggerSound);

		for(int I=1; I<traps.Length; I++)
		{
			TEMP = traps[I].GetComponent<Rigidbody2D>();
			if(atPlayer)
				TEMP.velocity = (GameController.control.GetPlayerGameObject().transform.position - TEMP.transform.position).normalized * trapSpeed; 
			else
				TEMP.velocity = new Vector2(Xvelocity,Yvelocity)*trapSpeed;

			StartCoroutine (DisableObject (TEMP.gameObject, objectDeathTimer));

			if(objectDelays > 0.02f)
				yield return new WaitForSeconds(objectDelays);

		}

//		Destroy (this.gameObject, objectDeathTimer);
	}

	IEnumerator DisableObject(GameObject aGameObject, float DeathTimer)
	{
		yield return new WaitForSeconds (DeathTimer);
		aGameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		aGameObject.SetActive (false);
	}
}
