// UPDATED 7-15-15 - Machine Functions Added
// Now inherits the Machine class
// Has functions to Start / Stop / Reset / Reverse / Speedup / Slowdown / SetSpeed


// RegalPrime 10-24-14 - SpikeChopper.cs

// Fairly simple script that moves an object to a certain offset and returns it back to the starting position
// Able to make chopping like actions or in / out actions for gameobject sets (like spikes)
// Variables change delay / speed / distance (both X and Y offset distances)
// Script effects only the gameobject it is attached to. Attaching children will allow them to be effected as well (via the parent child relationship)

// Deals with local transforms, so nesting this script on multiple children objects will work
// You could have a script on the parent main object that goes back and forth on the Xaxis,
// then add a script on the child object to move up and down on the Yaxis to create a wavy effect

// In order to make an object go the opposite direction, use negative extend distance values
// ExtendSpeed / RetractSpeed cannot be negative

using UnityEngine;
using System.Collections;

public class SpikeChopper : Machine
{
	public bool enableOnStart = true;		// Should this object be enabled on startup
	public bool restartOnReset = true;		// Should this object reset when the scene is reset?

	public float extendDelay = 1;			// Time before the initial extrend
	public float extendSpeed = 2;			// Speed of extending action
	
	public float retractDelay = 1;			// Delay of retraction
	public float retractSpeed = 1;			// Speed of retraction
	
	public float extendDistance_X = 0;		// Offset distance of extension X
	public float extendDistance_Y = 1;		// Offset distance of extension Y
	
	
	
	
	private float speedMultiplier = 1f;		// The speed multiplier
	private float currentPosition = 0;		// Current position along the distance line
	private bool extending = true;			// Is this machine currently extending or retracting?
	private bool isActive = false;			// Is the object currently active
	
	private Vector3 StartingPosition;		// Where this object will reset to
	private Vector3 extendPosition;			// Where this object would end up (used in calculating extendLength)
	private float extendLength;				// Amount of distance it can travel in the direction it is supposed to go

	private float currentExtrendDelay = 0;	// Current time left before extending
	private float currentRetractDelay = 0;	// Current time left before retracting
	
	
	void Start ()
	{
		if(restartOnReset) EventManager.resetObjects += Reset;
		StartingPosition = gameObject.transform.position;
		extendPosition = new Vector2 (extendDistance_X, extendDistance_Y);
		extendLength = Vector2.Distance (Vector2.zero, extendPosition);
		
		extendSpeed = Mathf.Abs (extendSpeed);
		retractSpeed = Mathf.Abs (retractSpeed);
		
		if(enableOnStart) Start_Machine();
	}
	void OnDestroy()
	{
		if(restartOnReset) EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		Reset_Machine ();
		if(enableOnStart) Start_Machine();
	}

	IEnumerator C_Start_Chopper()
	{
		while(gameObject != null)
		{
			if(extending)
			{
				if(currentPosition <= 0 || currentPosition >= extendLength)
				{
					while(currentExtrendDelay < extendDelay/speedMultiplier)
					{
						currentExtrendDelay += Time.deltaTime;
						yield return new WaitForSeconds(Time.deltaTime);
					}
				}
				currentExtrendDelay = 0;
//				yield return new WaitForSeconds(extendDelay);
				
				while(currentPosition < extendLength)
				{
					Vector3 FinalPosition = gameObject.transform.localPosition + extendPosition;
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, FinalPosition, Time.deltaTime*extendSpeed*speedMultiplier);
					currentPosition += Time.deltaTime*extendSpeed*speedMultiplier;
					
					yield return null;
				}
				
				extending = false;
			}
			else
			{
				if(currentPosition <= 0 || currentPosition >= extendLength)
				{
					while(currentRetractDelay < retractDelay/speedMultiplier)
					{
						currentRetractDelay += Time.deltaTime;
						yield return new WaitForSeconds(Time.deltaTime);
					}
				}
				currentRetractDelay = 0;
//				yield return new WaitForSeconds(retractDelay);
				
				while(currentPosition > 0)
				{
					Vector3 FinalPosition = gameObject.transform.localPosition - extendPosition;
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, FinalPosition, Time.deltaTime*retractSpeed*speedMultiplier);
					currentPosition -= Time.deltaTime*retractSpeed*speedMultiplier;

					yield return null;
				}


				extending = true;
			}
			
			yield return null;
		}
	}

// MACHINE OVERRIDES ----------------------------------------------------------------------------------------------------------------------
	public override void Start_Machine()
	{
		if(!isActive)
		{
			isActive = true;
			StartCoroutine ("C_Start_Chopper");
		}
	}
	public override void Stop_Machine()
	{
		if(isActive)
		{
			isActive = false;
			StopAllCoroutines ();
		}
	}
	
	public override void Reset_Machine()
	{
		StopAllCoroutines ();
		gameObject.transform.position = StartingPosition;
		extending = true;
		isActive = false;
		speedMultiplier = 1;
		currentPosition = 0;
	}
	
	public override void Reverse_Machine()
	{
		if(extending)
			extending = false;
		else
			extending = true;

		currentExtrendDelay = extendDelay - currentExtrendDelay;
		if(currentExtrendDelay < 0)	currentExtrendDelay = 0;

		currentRetractDelay = retractDelay - currentRetractDelay;
		if(currentRetractDelay < 0) currentRetractDelay = 0;


		StopAllCoroutines ();
		isActive = false;
		Start_Machine ();
	}
	public override void SpeedUp_Machine(float speedChange)
	{
		speedChange = Mathf.Abs (speedChange);
		float previousSpeedMultiplier = speedMultiplier;
		float increaseInSpeed = 0;
		
		speedMultiplier += speedChange;
		
		if(previousSpeedMultiplier == 0)
		{
			Start_Machine ();
		}
		else
		{
			increaseInSpeed = speedMultiplier / previousSpeedMultiplier;
			currentExtrendDelay /= increaseInSpeed;
			currentRetractDelay /= increaseInSpeed;
		}
	}
	public override void SlowDown_Machine(float speedChange)
	{
		speedChange = Mathf.Abs (speedChange);
		float previousSpeedMultiplier = speedMultiplier;
		float decreaseInSpeed = 0;
		
		speedMultiplier -= speedChange;
		if(speedMultiplier <= 0) speedMultiplier = 0;

		if(speedMultiplier == 0)
			Stop_Machine ();
		else
		{
			decreaseInSpeed = speedMultiplier / previousSpeedMultiplier;
			currentExtrendDelay /= decreaseInSpeed;
			currentRetractDelay /= decreaseInSpeed;
		}
	}

	public override void SetSpeed_Machine(float newSpeedMultiplier)
	{
		if(newSpeedMultiplier >= 0)
		{
			if(newSpeedMultiplier > speedMultiplier)
			{
				SpeedUp_Machine (newSpeedMultiplier-speedMultiplier);
			}
			
			if(newSpeedMultiplier < speedMultiplier)
			{
				SlowDown_Machine (speedMultiplier - newSpeedMultiplier);
			}
		}
	}
}




/*
Previous version of this script before adding in machine functions - DELETE LATER
using UnityEngine;
using System.Collections;

public class SpikeChopper : MonoBehaviour
{
	public bool RestartOnReset = true;

	public bool IsEnabled = true;		// Enable on startup

	public float ExtendDelay = 0;		// Time before the initial extrend
	public float ExtendSpeed = 2;		// Speed of extending action

	public float ExtendDistance_X = 0;	// Offset distance of extension X
	public float ExtendDistance_Y = 1;	// Offset distance of extension Y

	public float RetractDelay = 0;		// Delay of retraction
	public float RetractSpeed = 1;		// Speed of retraction

	private Vector3 StartingPosition;



	void Start ()
	{
		if(RestartOnReset)
			EventManager.resetObjects += Reset;
		StartingPosition = gameObject.transform.position;

		ExtendSpeed = Mathf.Abs (ExtendSpeed);
		RetractSpeed = Mathf.Abs (RetractSpeed);

		if(IsEnabled)
			StartCoroutine ("C_StartCoroutine");
	}
	void OnDestroy()
	{
		if(RestartOnReset)
			EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		StopAllCoroutines ();
		gameObject.transform.position = StartingPosition;

		if(IsEnabled)
			StartCoroutine ("C_StartCoroutine");
	}

//	public override void Start_Machine ()
//	{
//		StartCoroutine ("C_StartCoroutine");
//	}
//	public override void Stop_Machine ()
//	{
//		StopAllCoroutines ();
//	}
//	public override void Reset_Machine()
//	{
//		StopAllCoroutines ();
//		gameObject.transform.position = StartingPosition;		
//	}


	
	IEnumerator C_StartCoroutine()
	{
		while(gameObject != null)
		{
			yield return new WaitForSeconds(ExtendDelay);
			yield return StartCoroutine (C_ChopValue(ExtendDistance_X, ExtendDistance_Y, ExtendSpeed));
			yield return new WaitForSeconds(RetractDelay);
			yield return StartCoroutine (C_ChopValue(-ExtendDistance_X, -ExtendDistance_Y, RetractSpeed));
		}

		yield return null;
	}

	IEnumerator C_ChopValue (float DistanceX, float DistanceY, float Speed)
	{
//		print (" I AM MOVING " + Distance + " At Speed " + Speed);
		Vector3 FinalPosition = gameObject.transform.localPosition + new Vector3 (DistanceX, DistanceY);

		while(Vector3.Distance(transform.localPosition, FinalPosition)> 0.01f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, FinalPosition, Time.deltaTime*Speed);
			yield return null;
		}
		transform.localPosition = FinalPosition;
		
		yield return null;
	}
}


*/