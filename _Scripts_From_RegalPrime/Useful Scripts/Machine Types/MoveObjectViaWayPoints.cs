// UPDATED 7-20-15 - Machine Functions Added
// Now inherits the Machine class
// Has functions to Start / Stop / Reset / Reverse / Speedup / Slowdown / SetSpeed


// RegalPrime 11-25-14 - MoveObjectViaWayPoints.cs

// Moves an object between waypoints
// Additional waypoints can be added in the editor by expanding the wayPoints[] vector and pulling additional objects onto it
// Direction and starting waypoint are options to make the object move in the way you want

// FullLoop = false - Will make the object go backwards through the vector after reaching the end of the vector
// FullLoop = true - Will make the object go back to the start after reaching the end of the vector

// showLineRendering = true - Is more of a for-fun option
// It allows you to see the pathways of the waypoints via a line drawn between them
// You can also change the color of the lines

// This was created initially to make platforms go back and forth, but it can be used for any other object (fruit / etc)
// Adding extra waypoints, allows for multiple pathway options

// Had an issue with the waypoints being snapped to wierd Z values and thus caused the movement to appear slower than it should
// If this becomes a major problem, I will snap the Z waypoints to the gameobjects initial Z point at startup
// Distance between object and its target is now Vector2.Distance to fix this problem

using UnityEngine;
using System.Collections;

public class MoveObjectViaWayPoints  : Machine
{
	public bool enableOnStart = true;			// Enable at startup
	public bool restartOnReset = true;			// Reset this object when scene Reset() is called
	public bool forward = true;					// Traverses forward or backwards through the array
	public bool fullLoop = false;				// Setting this to true will loop the array
	
	public float moveSpeed = 1f;				// Move Speed
	public int startingIndex = 1;				// The index to start at
	
	public Transform[] wayPoints;				// Array of waypoints that the object traverses through
	
	
	public bool showLineRendering = false;		// Creates a line between waypoints if selected
	public Color firstColor = Color.white;		// Line renderer settings
	public Color secondColor = Color.blue;		// Line renderer settings
	
	
	private Vector3 startingPosition;			// Backup of the starting position
	private bool startingDirection;				// Backup of the starting direction	
	private bool isActive = false;				// Is this object active
	private float speedMultiplier = 1;			// Current speed multiplier
	private int currentWaypoint = 0;			// Current waypoint
	
	
	void Start ()
	{
		startingPosition = gameObject.transform.position;
		startingDirection = forward;
		
		if(moveSpeed<=0)	// using the MoveTwords function does not allow for negative numbers
		{
			print ("MovingPlatform.cs ERROR - (moveSpeed = "+moveSpeed+") is set below 0. This script does not take negative numbers. moveSpeed is now set to 1f");
			moveSpeed = 1;
		}

		if(GetComponent<Rigidbody2D>() != null)
			GetComponent<Rigidbody2D>().isKinematic = true;
		
		if(showLineRendering)	// If line rendering was selected - set it up
		{
			gameObject.GetComponent<SpriteRenderer> ().sortingOrder = 1;
			
			LineRenderer TEMP;
			for(int I=0; I<wayPoints.Length-1; I++)
			{
				TEMP = wayPoints[I].gameObject.AddComponent<LineRenderer>();
				
				TEMP.SetColors(firstColor, secondColor);
				TEMP.SetWidth(0.1F, 0.1F);
				TEMP.useWorldSpace = true;
				TEMP.material = new Material(Shader.Find("Particles/Additive"));
				
				TEMP.SetPosition(0, wayPoints[I].transform.position);
				TEMP.SetPosition(1, wayPoints[I+1].transform.position);
			}
			
			if(fullLoop)
			{
				TEMP = wayPoints[(wayPoints.Length-1)].gameObject.AddComponent<LineRenderer>();
				
				TEMP.SetColors(firstColor, secondColor);
				TEMP.SetWidth(0.1F, 0.1F);
				TEMP.useWorldSpace = true;
				TEMP.SetPosition(0, wayPoints[(wayPoints.Length-1)].transform.position);
				TEMP.material = new Material(Shader.Find("Particles/Additive"));
				TEMP.SetPosition(1, wayPoints[0].transform.position);
			}
		}
		
		if(startingIndex < 0)
			startingIndex = 0;
		if(startingIndex > wayPoints.Length-1)
			startingIndex = wayPoints.Length-1;
		
		currentWaypoint = startingIndex;
		
		if(restartOnReset)	EventManager.resetObjects += Reset;
		if(enableOnStart) Start_Machine ();
	}
	
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	
	void Reset()
	{
		Reset_Machine ();
		if(enableOnStart) Start_Machine ();
	}
	
	
	IEnumerator C_MovePlatform()
	{
		yield return null;
		while(isActive)
		{
			while(Vector2.Distance(transform.position, wayPoints[currentWaypoint].position)> 0.05f)
			{
				transform.position = Vector3.MoveTowards(transform.position, wayPoints[currentWaypoint].position, Time.deltaTime*moveSpeed*speedMultiplier);
				yield return null;
			}
			
			CalculateWaypoint ();
			
			yield return null;
		}
	}
	
	
	void CalculateWaypoint()
	{
		if(forward)													// If moving forward through the array
		{
			if(currentWaypoint+1 >= wayPoints.Length)				// If going forward is out of bounds either - go backwards or head to start
			{														// depending if fullLoop is selected
				if(fullLoop)
					currentWaypoint=0;
				else
				{	
					forward=false;
					currentWaypoint--;
				}
			}
			else
				currentWaypoint++;									// Goto next waypoint
			
		}
		else
		{
			if(currentWaypoint-1 < 0)								// If going backwards through the array
			{	
				if(fullLoop)										// If going backwards is out of bounds - go forward or head to end of list
					currentWaypoint=(wayPoints.Length)-1;			// depending if fullLoop is selected
				else
				{
					forward=true;
					currentWaypoint++;
				}
			}
			else
				currentWaypoint--;
		}
	}
	
	
	// MACHINE OVERRIDES ----------------------------------------------------------------------------------------------------------------------
	public override void Start_Machine()
	{
		if(!isActive)
		{
			isActive = true;
			StartCoroutine ("C_MovePlatform");
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
		
		gameObject.transform.position = startingPosition;
		currentWaypoint = startingIndex;
		forward = startingDirection;
		
		isActive = false;
		speedMultiplier = 1;
	}
	
	public override void Reverse_Machine()
	{
		if(forward)
			forward = false;
		else
			forward = true;
		CalculateWaypoint ();
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

		if(speedMultiplier == 0)
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



/*
using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (Rigidbody2D))]
public class MoveObjectViaWayPoints  : MonoBehaviour
{
	public float moveSpeed = 1f;				// Move Speed

	public bool isEnabled = true;				// Stops / Starts the platform
	public bool showLineRendering = false;		// Creates a line between waypoints if selected
	public Color firstColor = Color.white;
	public Color secondColor = Color.blue;

	public Transform[] wayPoints;				// Array of waypoints that the object traverses through
	public int startingIndex = 0;				// The index to start at
	private int currentWaypoint = 0;

	public bool forward = true;					// Traverses forward or backwards through the array
	private bool StartingDirection;
	public bool fullLoop = false;				// Setting this to true will loop the array

	private Vector3 startingPosition;

	void Start ()
	{
		EventManager.resetObjects += Reset;
		startingPosition = gameObject.transform.position;
		StartingDirection = forward;

		gameObject.GetComponent<Rigidbody2D> ().isKinematic = true;

		if(moveSpeed<0)	// using the MoveTwords function does not allow for negative numbers
		{
			print ("MovingPlatform.cs ERROR - (moveSpeed = "+moveSpeed+") is set below 0. This script does not take negative numbers. moveSpeed is now set to 1f");
			moveSpeed = 1;
		}

		if(showLineRendering)	// If line rendering was selected - set it up
		{
			gameObject.GetComponent<SpriteRenderer> ().sortingOrder = 1;

			LineRenderer TEMP;
			for(int I=0; I<wayPoints.Length-1; I++)
			{
				TEMP = wayPoints[I].gameObject.AddComponent<LineRenderer>();

				TEMP.SetColors(firstColor, secondColor);
				TEMP.SetWidth(0.1F, 0.1F);
				TEMP.useWorldSpace = true;
				TEMP.material = new Material(Shader.Find("Particles/Additive"));

				TEMP.SetPosition(0, wayPoints[I].transform.position);
				TEMP.SetPosition(1, wayPoints[I+1].transform.position);
			}

			if(fullLoop)
			{
				TEMP = wayPoints[(wayPoints.Length-1)].gameObject.AddComponent<LineRenderer>();
				
				TEMP.SetColors(firstColor, secondColor);
				TEMP.SetWidth(0.1F, 0.1F);
				TEMP.useWorldSpace = true;
				TEMP.SetPosition(0, wayPoints[(wayPoints.Length-1)].transform.position);
				TEMP.material = new Material(Shader.Find("Particles/Additive"));
				TEMP.SetPosition(1, wayPoints[0].transform.position);
			}
		}

		if(startingIndex < 0)
			startingIndex = 0;
		if(startingIndex > wayPoints.Length-1)
			startingIndex = wayPoints.Length-1;

		currentWaypoint = startingIndex;
		StartCoroutine ("C_Start");
	}

	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	
	void Reset()
	{
		StopAllCoroutines ();
		gameObject.transform.position = startingPosition;
		currentWaypoint = startingIndex;
		forward = StartingDirection;
		StartCoroutine ("C_Start");
	}


	public void StartPlatform()
	{
		StopCoroutine ("C_Start");
		StartCoroutine ("C_Start");
	}


	IEnumerator C_Start()
	{
		yield return null;
		while(isEnabled)
		{
			if(forward)													// If moving forward through the array
			{
				if(currentWaypoint+1 >= wayPoints.Length)				// If going forward is out of bounds either - go backwards or head to start
				{														// depending if fullLoop is selected
					if(fullLoop)
						currentWaypoint=0;
					else
					{	
						forward=false;
						currentWaypoint--;
					}
				}
				else
					currentWaypoint++;									// Goto next waypoint
				
			}
			else
			{
				if(currentWaypoint-1 < 0)								// If going backwards through the array
				{	
					if(fullLoop)										// If going backwards is out of bounds - go forward or head to end of list
						currentWaypoint=(wayPoints.Length)-1;			// depending if fullLoop is selected
					else
					{
						forward=true;
						currentWaypoint++;
					}
				}
				else
					currentWaypoint--;
			}

			yield return StartCoroutine (C_GotoWaypoint(currentWaypoint));
		}

		yield return null;
	}

	IEnumerator C_GotoWaypoint(int GotoThisWayPoint)		// Make the object goto the next waypoint
	{
//		print ("Heading To " + GotoThisWayPoint);

		while(Vector3.Distance(transform.position, wayPoints[GotoThisWayPoint].position)> 0.05f)
		{
			transform.position = Vector3.MoveTowards(transform.position, wayPoints[GotoThisWayPoint].position, Time.deltaTime*moveSpeed);
//			transform.rigidbody2D.velocity = (wayPoints[GotoThisWayPoint].position - transform.position).normalized * moveSpeed;
			yield return null;
		}
//		transform.position = wayPoints [GotoThisWayPoint].position;

//		yield return new WaitForEndOfFrame();
		yield return null;
	}
}
*/

