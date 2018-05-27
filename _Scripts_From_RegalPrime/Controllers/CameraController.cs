// RegalPrime 1-21-15 - GameController.cs

// This contains functions to manipluate the cameras position
// In general, each axis is controlled separatly. This way locking an axis is easier to accomplish.
// When it receives a locked axis value, it will move to that position then stop moving that axis
// Normal camera movment is done on a frame by frame basis otherwise.

// Check out CameraManipulation.cs script
// It uses this script to move the camera when triggers are hit

// WARNING !!
// This script uses a public static reference to itself, but does not destroy any competitors
// This is done for easy of locating the camera and manipluating it
// I would assume if more than one camera with this script on it appears, it may cause problems
// I have not come to the point when I need multicameras so I will worry about it when the need arises


// Just did a total rewrite of this script. Things are way better than before (in terms of code / efficiency-wise)
// Be aware of bugs that may happen


using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public static CameraController control;
//	private float offset_X = 0f;			// Offset of character to target (NOT USED RIGHT NOW)
//	private float offset_Y = 0f;			// Offset of character to target (NOT USED RIGHT NOW)
	
	private float damping_X = 2.0f;				// Speed of camera movement
	private float damping_Y = 3.0f;				// Speed of camera movement
	
	private float xBuffer	= 0.5f;				// How much null space before camera moves
	private float yBuffer	= 0.5f;				// How much null space before camera moves
	
	private Vector3 cameraResetPosition;		// Allows for resetting of the cameras position
	
	// The next 8 variables are used in remembering the previous camera state
	// For example, you can have a camera look at a target, then go back to what it was doing using SaveCameraTargets() and ReloadCameraTargets()
	private GameObject C_TargetThisObject = null;	// Current targeted object
	private GameObject C_Lock_X = null;				// Current lock_X
	private GameObject C_Lock_Y = null;				// Current lock_Y
//	private bool C_SnapMovement = false;			// Current snap variable
	
	private GameObject B_TargetThisObject;		// Backup of targeted object
	private GameObject B_Lock_X;				// Backup of lock_X
	private GameObject B_Lock_Y;				// Backup of lock_Y
	private bool B_SnapMovement;				// Backup of snap variable
	
	void Awake()
	{
		if (control == null)
		{
			control = this;
		}
		cameraResetPosition = gameObject.transform.position;
	}

	public void ResetCamera()
	{
		gameObject.transform.position = cameraResetPosition;
		StopAllCoroutines ();
	}
	public void SetNewCameraResetPosition()
	{
		cameraResetPosition = gameObject.transform.position;
	}
	
	public void MoveCamera(GameObject targetThisObject, GameObject lockX, GameObject lockY, bool SnapMovement)
	{
		StopAllCoroutines ();
		
		C_TargetThisObject = targetThisObject;
		C_Lock_X = lockX;
		C_Lock_Y = lockY;
//		C_SnapMovement = SnapMovement;
		
		StartCoroutine (C_MoveCamera (targetThisObject, lockX, lockY, SnapMovement));
	}

	// Move certain axis based on the lock x / y inputs
	IEnumerator C_MoveCamera(GameObject targetThisObject, GameObject lockX, GameObject lockY, bool SnapMovement)
	{
		if(lockX == null)
			StartCoroutine (C_Follow_X(targetThisObject, SnapMovement));
		else
			StartCoroutine (C_MovetoX (lockX.transform.position, SnapMovement));
		
		if(lockY == null)
			StartCoroutine (C_Follow_Y(targetThisObject, SnapMovement));
		else
			StartCoroutine (C_MovetoY (lockY.transform.position, SnapMovement));
		
		yield return null;
	}
	
	// Constant movement twords a certain X axis position
	IEnumerator C_Follow_X(GameObject targetThisObject_X,  bool SnapMovement)
	{
		float newX;
		
		if(SnapMovement && gameObject.activeInHierarchy && targetThisObject_X != null)
			gameObject.transform.position = new Vector3(targetThisObject_X.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
		
		while(gameObject.activeInHierarchy && targetThisObject_X != null)
		{
			if(Mathf.Abs (gameObject.transform.position.x - targetThisObject_X.transform.position.x) > xBuffer)
			{
				newX = Mathf.Lerp(gameObject.transform.position.x, targetThisObject_X.transform.position.x, Time.deltaTime*damping_X);
				gameObject.transform.position = new Vector3(newX, gameObject.transform.position.y, gameObject.transform.position.z);
			}
			
			yield return null;
		}
	}
	// Constant movement twords a certain Y axis position
	IEnumerator C_Follow_Y(GameObject targetThisObject_Y, bool SnapMovement)
	{
		float newY;
		
		if(SnapMovement && gameObject.activeInHierarchy && targetThisObject_Y != null)
			gameObject.transform.position = new Vector3(gameObject.transform.position.x, targetThisObject_Y.transform.position.y, gameObject.transform.position.z);
		
		while(gameObject.activeInHierarchy && targetThisObject_Y != null)
		{
			if(Mathf.Abs (gameObject.transform.position.y - targetThisObject_Y.transform.position.y) > yBuffer)
			{
				newY = Mathf.Lerp(gameObject.transform.position.y, targetThisObject_Y.transform.position.y, Time.deltaTime*damping_Y);
				gameObject.transform.position = new Vector3(gameObject.transform.position.x, newY, gameObject.transform.position.z);			
			}
			
			yield return null;
		}
	}
	
	// One time movement to a certain X axis position
	IEnumerator C_MovetoX(Vector3 lockX, bool SnapMovement)
	{
		float newX;
		float XDistance = Mathf.Abs (gameObject.transform.position.x - lockX.x);
		
		//		while(XDistance > 0.01f && !SnapMovement && gameObject.activeInHierarchy && lockX != null)
		while(XDistance > 0.01f && !SnapMovement && gameObject.activeInHierarchy)
		{
			newX = Mathf.Lerp(gameObject.transform.position.x, lockX.x, Time.deltaTime*damping_X);
			gameObject.transform.position = new Vector3(newX, gameObject.transform.position.y, gameObject.transform.position.z);
			
			XDistance = Mathf.Abs (gameObject.transform.position.x - lockX.x);
			yield return null;
		}
		
		gameObject.transform.position = new Vector3(lockX.x, gameObject.transform.position.y, gameObject.transform.position.z);
	}
	// One time movement to a certain Y axis position
	IEnumerator C_MovetoY(Vector3 lockY, bool SnapMovement)
	{
		float newY;
		float YDistance = Mathf.Abs (gameObject.transform.position.y - lockY.y);
		
		//		while(YDistance > 0.01f && !SnapMovement && gameObject.activeInHierarchy && lockY != null)
		while(YDistance > 0.01f && !SnapMovement && gameObject.activeInHierarchy)
		{
			newY = Mathf.Lerp(gameObject.transform.position.y, lockY.y, Time.deltaTime*damping_Y);
			gameObject.transform.position = new Vector3(gameObject.transform.position.x, newY, gameObject.transform.position.z);			
			
			YDistance = Mathf.Abs (gameObject.transform.position.y - lockY.y);
			yield return null;
		}
		
		gameObject.transform.position = new Vector3(gameObject.transform.position.x, lockY.y, gameObject.transform.position.z);
	}


	// Will backup the current camera targets for use later
	public void BackupCameraTargets(bool snapback)
	{
		B_TargetThisObject = C_TargetThisObject;
		B_Lock_X = C_Lock_X;
		B_Lock_Y = C_Lock_Y;
		B_SnapMovement = snapback;
	}
	// Will take the above backup targets and move twords them
	public void ReloadCameraTargets()
	{
		StopAllCoroutines ();
		if (B_TargetThisObject == null && B_Lock_X == null && B_Lock_Y == null)
		{
			if(B_SnapMovement)
			{
				C_TargetThisObject = null;
				C_Lock_X = null;
				C_Lock_Y = null;
//				C_SnapMovement = B_SnapMovement;
				gameObject.transform.position = cameraResetPosition;
			}
			else
			{
				C_TargetThisObject = null;
				C_Lock_X = null;
				C_Lock_Y = null;
//				C_SnapMovement = B_SnapMovement;
				StartCoroutine (C_MovetoX (cameraResetPosition, B_SnapMovement));
				StartCoroutine (C_MovetoY (cameraResetPosition, B_SnapMovement));
			}
		}
		else
			MoveCamera (B_TargetThisObject, B_Lock_X, B_Lock_Y, B_SnapMovement);
	}
}