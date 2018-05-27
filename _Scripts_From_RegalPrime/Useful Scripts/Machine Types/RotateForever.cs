// UPDATED 7-18-15 - Machine Functions Added
// Now inherits the Machine class
// Has functions to Start / Stop / Reset / Reverse / Speedup / Slowdown / SetSpeed

// Complete rewrite of script
// Simple script that rotates an object around their axis


using UnityEngine;
using System.Collections;

public class RotateForever : Machine
{
	public bool enableOnStart = true;			// Should this object be enabled on startup
	public bool restartOnReset = true;			// Does the object reset its rotation on reset?

	public float Xaxis = 0;						// Rotate this speed around the X axis
	public float Yaxis = 0;						// Rotate this speed around the Y axis
	public float Zaxis = 25;					// Rotate this speed around the Z axis
	
	private bool isActive = false;				// Is this object active
	private float speedMultiplier = 1;			// Speed multiplier of this object
	private bool forward = true;				// Is this object spinning forward
	private Quaternion InitialRotation;			// Used to reset the rotation back to the default value
	
	
	void Start ()
	{
		InitialRotation = gameObject.transform.rotation;
		
		if(restartOnReset)	EventManager.resetObjects += Reset;
		if(enableOnStart)	Start_Machine();
	}
	void OnDestroy()
	{
		if(restartOnReset)	EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		Reset_Machine ();
		if(enableOnStart)	Start_Machine();
	}
	
	IEnumerator C_StartRotating()
	{
		while(isActive)
		{
			if(forward)
				transform.Rotate(Xaxis*speedMultiplier*Time.deltaTime, Yaxis*speedMultiplier*Time.deltaTime, Zaxis*speedMultiplier*Time.deltaTime);
			else
				transform.Rotate(-Xaxis*speedMultiplier*Time.deltaTime, -Yaxis*speedMultiplier*Time.deltaTime, -Zaxis*speedMultiplier*Time.deltaTime);
			yield return null;
		}
	}
	
	// MACHINE OVERRIDES ----------------------------------------------------------------------------------------------------------------------
	public override void Start_Machine()
	{
		if(!isActive)
		{
			isActive = true;
			StartCoroutine ("C_StartRotating");
		}
	}
	public override void Stop_Machine()
	{
		if(isActive)
		{
			StopAllCoroutines ();
			isActive = false;
		}
	}
	
	public override void Reset_Machine()
	{
		StopAllCoroutines ();
		isActive = false;
		speedMultiplier = 1;
		gameObject.transform.localRotation = InitialRotation;
	}
	
	public override void Reverse_Machine()
	{
		if(forward)
			forward = false;
		else
			forward = true;
	}
	
	public override void SpeedUp_Machine(float speedChange)
	{
		speedChange = Mathf.Abs (speedChange);
		float previousSpeedMultiplier = speedMultiplier;
		speedMultiplier += speedChange;

		if(previousSpeedMultiplier == 0)
			Start_Machine ();
	}
	
	public override void SlowDown_Machine(float speedChange)
	{
		speedChange = Mathf.Abs (speedChange);
		speedMultiplier -= speedChange;
		if(speedMultiplier <= 0) speedMultiplier = 0;

		if (speedMultiplier == 0)
			Stop_Machine ();
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


