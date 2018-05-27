// RegalPrime 12-02-14 - KinematicToggle.cs

// Used to explode the title screen apart when a trigger is hit

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinematicToggle : MonoBehaviour
{
	public GameObject kinematicDestruction;
	public AudioClip clip;

	public Transform[] TitleObjects;
	public List<Vector3> TitleLocation = new List<Vector3>();
	public List<Quaternion> TitleRotation = new List<Quaternion>();

	void Start ()
	{
		EventManager.resetObjects += Reset;
		TitleObjects = kinematicDestruction.GetComponentsInChildren<Transform> (gameObject);		// Get all child objects

		foreach(Transform child in TitleObjects)
		{
			TitleLocation.Add (child.transform.position);
			TitleRotation.Add (child.transform.rotation);
		}
	}
	
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		gameObject.SetActive (true);
		for(int I=0; I<TitleObjects.Length; I++)
		{
			TitleObjects[I].transform.position = TitleLocation[I];
			TitleObjects[I].transform.rotation = TitleRotation[I];

			if(TitleObjects[I].GetComponent<Rigidbody>())
				TitleObjects[I].GetComponent<Rigidbody>().isKinematic = true;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{

		if (other.gameObject.tag == "Player")
		{
			Rigidbody[] bodies = kinematicDestruction.GetComponentsInChildren<Rigidbody> (gameObject);
			foreach (Rigidbody r in bodies)
			{
					r.isKinematic = false;
					r.AddForce (Vector3.forward);
			}

//			AudioSource.PlayClipAtPoint (clip, new Vector3 (1, 1, 1));
			MusicController.control.OneShotAudio(clip);
			gameObject.SetActive (false);
		}
	}
}



/*
Rigidbody[] bodies = kinematicDestruction.GetComponentsInChildren<Rigidbody>(gameObject);
foreach (Rigidbody r in bodies)
{
    r.isKinematic = false;
}
*/


/* Old code
using UnityEngine;
using System.Collections;

public class KinematicToggle : MonoBehaviour
{

	public Transform kinematicDestruction;

	void Start ()
	{
//		Uncommenting this made the title break apart on startup
//		RecursiveChild (kinematicDestruction);
	}
	
	void Update ()
	{

	}

	void OnTriggerEnter2D(Collider2D other)
	{
//		This does not activate the function
		RecursiveChild (kinematicDestruction);
		print ("TOGGLE");
	}

	void RecursiveChild(Transform anObject)
	{
		int children = anObject.transform.childCount;		// Checks the number of children
//		print ("This object has - " + children);

		if (children == 0)
		{									// If there are no children
			anObject.rigidbody.isKinematic = false;			// remove Kinematic state
			anObject.rigidbody.AddForce (Vector3.up * 10);
		}
		else
		for (int i = 0; i < children; ++i)					// For every child call the function again
		{
			RecursiveChild(anObject.transform.GetChild(i));
		}
	}
}
	*/