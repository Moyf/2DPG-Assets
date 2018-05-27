// RegalPrime 12-19-14 - CameraManipulation.cs

// A player based trigger that uses the camera controller functions to change where / how the camera is used.
// TargetThisObject - The object the camera will follow. This is a free follow camera focused on that object
// TargetThePlayer = true - will target the player
// If you choose no target, it will attempt to lock onto the player.
// If the player does not exist, then it will lock onto this gameobject itself.

// Locking onto the player will cause the camera to be somewhat centered on the player
// (there is a small buffer so that the camera moves more smoothly when the player move small distances)
// Be aware that doing so could cause the camera to show an area outside of your screen edge.

// To prevent this, use the ObjectLock_X / ObjectLock_Y
// Having a value for either of these will cause the camera to still follow the target, but will lock that axis to the X/Y-value of that gameobject.
// Having a null value will allow for free movement along that axis.
// This will allow the camera to follow the character but keep it from dipping outside of the screen bounds.

// To make the camera instantly jump to the position, use NoLerpSnap = true

// See the functions in the CameraController.cs for more information.

using UnityEngine;
using System.Collections;

public class CameraManipulation : MonoBehaviour
{
	public bool TargetThePlayer = false;			// Camera will taget the player

	public GameObject TargetThisObject;				// Object for the camera to target
	public GameObject ObjectLock_X;					// Lock the camera to this objects X-Axis
	public GameObject ObjectLock_Y;					// Lock the camera to this objects Y-Axis

	public bool NoLerpSnap = false;					// Instant move to the wanted location
	public bool OneTimeActivate = false;			// Only trigger once
	public bool RevertOnExit = false;				// Will go back to the previous camera settings when leaving the collider

	private BoxCollider2D triggerBox;

	void Start ()
	{
		EventManager.resetObjects += Reset;
		triggerBox = GetComponent<BoxCollider2D> ();
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		CameraController.control.ResetCamera ();
		if(gameObject.activeInHierarchy)
			StartCoroutine (EndOfFrame ());
	}
	void OnEnable()
	{
		gameObject.GetComponent<BoxCollider2D> ().enabled = false;
		gameObject.GetComponent<BoxCollider2D> ().enabled = true;
	}
	IEnumerator EndOfFrame()
	{
		yield return new WaitForEndOfFrame ();
		gameObject.GetComponent<BoxCollider2D> ().enabled = false;
		gameObject.GetComponent<BoxCollider2D> ().enabled = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			if(RevertOnExit)
				CameraController.control.BackupCameraTargets(NoLerpSnap);

			if(TargetThePlayer)
				CameraController.control.MoveCamera(GameController.control.GuyLocation, ObjectLock_X, ObjectLock_Y, NoLerpSnap);
			else
				CameraController.control.MoveCamera(TargetThisObject, ObjectLock_X, ObjectLock_Y, NoLerpSnap);

			if(OneTimeActivate)
			{
				triggerBox.enabled = false;
			}
		
		}
	}


	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			if(RevertOnExit)
				CameraController.control.ReloadCameraTargets();
		}
	}

}

/*
// RegalPrime 11-25-14 - CameraManipulation.cs

// A player based trigger that uses the camera controller functions to change where / how the camera is used.
// TargetThisObject - The object the camera will follow. This is a free follow camera focused on that object
// TargetThePlayer = true - will target the player
// If you choose no target, it will attempt to lock onto the player.
// If the player does not exist, then it will lock onto this gameobject itself.

// Locking onto the player will cause the camera to be somewhat centered on the player
// (there is a small buffer so that the camera moves more smoothly when the player move small distances)
// Be aware that doing so could cause the camera to show an area outside of your screen edge.

// To prevent this, use the ObjectLock_X / ObjectLock_Y
// Having a value for either of these will cause the camera to still follow the target, but will lock that axis to the X/Y-value of that gameobject.
// Having a null value will allow for free movement along that axis.
// This will allow the camera to follow the character but keep it from dipping outside of the screen bounds.

// To make the camera instantly jump to the position, use NoLerpSnap = true


// Please see the functions in the CameraController.cs for more information.

using UnityEngine;
using System.Collections;

public class CameraManipulation : MonoBehaviour
{
	public bool TargetThePlayer = false;			// Camera will taget the player

	public Transform TargetThisObject;				// Object for the camera to target
	public Transform ObjectLock_X;					// Lock the camera to this objects X-Axis
	public Transform ObjectLock_Y;					// Lock the camera to this objects Y-Axis

	public bool NoLerpSnap = false;					// Instant move to the wanted location
	public bool OneTimeActivate = false;			// Only trigger once
	public bool RevertOnExit = false;				// Will go back to the previous camera settings when leaving the collider

	private BoxCollider2D triggerBox;

	void Start ()
	{
		ResetManager.resetObjects += Reset;
		triggerBox = GetComponent<BoxCollider2D> ();
	}
	void OnDestroy()
	{
		ResetManager.resetObjects -= Reset;
	}
	void Reset()
	{
		CameraController.control.ResetCamera ();
		if(gameObject.activeInHierarchy)
			StartCoroutine (EndOfFrame ());
	}
	void OnEnable()
	{
		gameObject.GetComponent<BoxCollider2D> ().enabled = false;
		gameObject.GetComponent<BoxCollider2D> ().enabled = true;
	}
	IEnumerator EndOfFrame()
	{
		yield return new WaitForEndOfFrame ();
		gameObject.GetComponent<BoxCollider2D> ().enabled = false;
		gameObject.GetComponent<BoxCollider2D> ().enabled = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			if(RevertOnExit)
				CameraController.control.SaveCamera();

			if(TargetThePlayer)
			{
				CameraController.control.Do_FollowObject(GameController.control.GuyLocation.transform);
			}
			else
				CameraController.control.Do_FollowObject(TargetThisObject);

			if(ObjectLock_X != null)
				CameraController.control.Do_LockToX(ObjectLock_X);

			if(ObjectLock_Y != null)
				CameraController.control.Do_LockToY(ObjectLock_Y);

			if(NoLerpSnap)
				CameraController.control.Do_NoLerpSnap();

			if(OneTimeActivate)
			{
				triggerBox.enabled = false;
//				Destroy (gameObject);
			}
		
		}
	}


	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			if(RevertOnExit)
				CameraController.control.ReloadCamera();
		}
	}

}

*/