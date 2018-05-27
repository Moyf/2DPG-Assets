// UPDATED 7-17-15 - Machine Functions Added
// Now inherits the Machine class
// Has functions to Start / Stop / Reset / Reverse / Speedup / Slowdown / SetSpeed


// RegalPrime 10-19-14 - SpikeTreadmill.cs

// This script spawns an object below the starting point, moves it above the starting point, then moves it twords the ending point, and finally moves it back under the ending point
// The object will spawn with its top left corner touching the StartingPosition Transform and ending with it touching the top right of the EndingPosition
// Offsets are used based on the sprite size (if no collider) - This is pretty bad because even if your viewable object is half the size of a 32x32 imported sprite it will assume 32x32
// PolygonCollider2D / CircleCollider2D / BoxCollider2D - Having one of these will overwrite the above offset calculations and are more accurate
// Having none will have 0 offsets
// Can disable all offsets via DisableOffsets bool - Object will move from starting to ending position

// MaintainParentRotation bool = true.  Allows the movement of the spikes to be based on the local transforms and thus the spikes will face relative to whichever way you rotate
// the parent object. Setting it to false will make the spikes face globally upwards and will not use width offsets

// It is possible that abnormally shaped polygon colliders may created poorly aligned results.
// Doing something like, stretching out a polygon node far out of the sprites area would be considered abnormal
// Worrying about such a rare occurance isnt worth figuring out the extra code to prevent this.
// If this becomes an issue, I may add the code later

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikeTreadmill : Machine
{
	public bool enableOnStart = true;				// Enable at startup
	public bool restartOnReset = true;				// Reset this object when scene Reset() is called
	public bool maintainParentRotation = true;		// Transform / rotation will be based on parents / global 
	
	public float startDelay = 0;					// Delay before initial start of object
	public float moveSpeed = 1;						// Spikes movement speed
	public float delayBetweenSpikes = 0.5f;			// Rate at which the spikes spawn
	
	public GameObject spikePrefab;					// Loaded gameobject to spawn
	public Transform startingPosition;				// Starting position
	public Transform endingPosition;				// Ending position
	
	public bool disableOffsets = false;				// Can disable offsets if needed
	
	
	private float spriteHeight = 0;					// Used in the calculation of offsets
	private float spriteWidth = 0;					// Used in the calculation of offsets
	private float offsetX = 0;						// The X_offset of the collider (if available)
	private float offsetY = 0;						// The Y_offset of the collider (if available)
	
	private Vector2[] wayPoints = new Vector2[4];	// Calculated waypoints for the spawned objects
	private bool isActive = false;					// Is the machine currently active
	private bool isPaused = false;					// If the object is currently "paused"
	private bool forward = true;					// Is this machine currently in forward mode
	private float speedMultiplier = 1;				// Current speed multiplier of this object
	private float currentDelayBetweenSpikes = 999;	// How long before the next object spawns
	
	
	private List<ObjectsData> PooledObjects = new List<ObjectsData>();	// List of pooled objects
	class ObjectsData													// Simple way to keep info on a gameobject and its heading index
	{
		public GameObject aPooledObject;
		public int headingToThisIndex;
	}
	
	void Start ()
	{
		if(restartOnReset)	EventManager.resetObjects += Reset;
		
		if(!disableOffsets)
		{
			// Calculating the width / height based on which component the spike prefab has
			if(spikePrefab.GetComponent<SpriteRenderer> () != null)
			{
				SpriteRenderer TEMP = spikePrefab.GetComponent<SpriteRenderer> ();
				spriteHeight = TEMP.sprite.bounds.size.y * spikePrefab.transform.localScale.y;
				spriteWidth = TEMP.sprite.bounds.size.x * spikePrefab.transform.localScale.x;
			}
			if(spikePrefab.GetComponent<PolygonCollider2D> () != null)
			{
				PolygonCollider2D Poly = spikePrefab.GetComponent<PolygonCollider2D> ();
				
				spriteWidth = Poly.bounds.size.x + (spriteWidth - Poly.bounds.size.x);
				spriteHeight = Poly.bounds.size.y + (spriteHeight - Poly.bounds.size.y);
			}
			if(spikePrefab.GetComponent<CircleCollider2D> () != null)
			{
				CircleCollider2D Poly = spikePrefab.GetComponent<CircleCollider2D> ();
				spriteWidth = Poly.bounds.size.x;
				spriteHeight = Poly.bounds.size.y;
				offsetX = Poly.offset.x;
				offsetY = Poly.offset.y;
			}
			if(spikePrefab.GetComponent<BoxCollider2D> () != null)
			{
				BoxCollider2D Poly = spikePrefab.GetComponent<BoxCollider2D> ();
				spriteWidth = Poly.bounds.size.x;
				spriteHeight = Poly.bounds.size.y;
				offsetX = Poly.offset.x;
				offsetY = Poly.offset.y;
			}
		}
		
		// Calculating the local waypoint based on calculated width / height and offsets
		if(maintainParentRotation)
		{
			wayPoints[0] = startingPosition.transform.localPosition + new Vector3(-offsetX+spriteWidth/2, -offsetY-spriteHeight/2);
			wayPoints[1] = startingPosition.transform.localPosition + new Vector3(-offsetX+spriteWidth/2, -offsetY+spriteHeight/2);
			wayPoints[2] = endingPosition.transform.localPosition + new Vector3(-offsetX-spriteWidth/2, -offsetY+spriteHeight/2);
			wayPoints[3] = endingPosition.transform.localPosition + new Vector3(-offsetX-spriteWidth/2, -offsetY-spriteHeight/2);
		}
		else
		{
			wayPoints[0] = startingPosition.transform.position - new Vector3(0, spriteHeight/2);
			wayPoints[0] = gameObject.transform.InverseTransformPoint (wayPoints[0]);
			
			wayPoints[1] = startingPosition.transform.position + new Vector3 (0, spriteHeight/2);
			wayPoints[1] = gameObject.transform.InverseTransformPoint (wayPoints[1]);
			
			wayPoints[2] = endingPosition.transform.position + new Vector3 (0, spriteHeight/2);
			wayPoints[2] = gameObject.transform.InverseTransformPoint (wayPoints[2]);
			
			wayPoints[3] = endingPosition.transform.position + new Vector3 (0, -spriteHeight/2);
			wayPoints[3] = gameObject.transform.InverseTransformPoint (wayPoints[3]);
		}
		
		if(enableOnStart) Start_Machine ();
	}
	void OnDestroy()
	{
		if(restartOnReset)	EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		Reset_Machine ();		
		if(enableOnStart) Start_Machine ();
	}
	
	
	ObjectsData GetPooledObject()
	{
		foreach (ObjectsData child in PooledObjects)		// Found a pooled object
		{
			if(!child.aPooledObject.activeInHierarchy)
			{
				child.aPooledObject.SetActive (true);
				return child;
			}
		}
		
		// Could not find a pooled object. Creating a new object
		GameObject clone;
		clone = Instantiate (spikePrefab, wayPoints[0], spikePrefab.transform.rotation) as GameObject;
		
		clone.transform.parent = gameObject.transform;
		
		ObjectsData temp = new ObjectsData();
		temp.aPooledObject = clone;
		temp.headingToThisIndex = 0;
		
		PooledObjects.Add (temp);
		
		return temp;
	}
	
	
	
	IEnumerator C_StartTreadmill()
	{
		if(!isPaused)	yield return new WaitForSeconds (startDelay);
		
		ObjectsData clone;
		// Create Object, Set it to the starting position, then send it to do its path movment via coroutine
		while(isActive)
		{
			while(currentDelayBetweenSpikes < delayBetweenSpikes/speedMultiplier)
			{
				currentDelayBetweenSpikes += Time.deltaTime;
				yield return new WaitForSeconds(Time.deltaTime);
			}
//			currentDelayBetweenSpikes -= DelayBetweenSpikes/speedMultiplier;
//			if(currentDelayBetweenSpikes < 0)	currentDelayBetweenSpikes = 0;
			currentDelayBetweenSpikes = 0;
			
			
			clone = GetPooledObject ();
			
			if(forward)
			{
				if(maintainParentRotation)
				{
					clone.aPooledObject.transform.rotation = gameObject.transform.rotation;
					clone.aPooledObject.transform.localPosition = wayPoints[0];
				}
				else
				{
					clone.aPooledObject.transform.rotation = Quaternion.identity;
					clone.aPooledObject.transform.localPosition = wayPoints[0];
				}
				
				clone.headingToThisIndex = 1;
			}
			else
			{
				if(maintainParentRotation)
				{
					clone.aPooledObject.transform.rotation = gameObject.transform.rotation;
					clone.aPooledObject.transform.localPosition = wayPoints[3];
				}
				else
				{
					clone.aPooledObject.transform.rotation = Quaternion.identity;
					clone.aPooledObject.transform.localPosition = wayPoints[3];
				}
				
				clone.headingToThisIndex = 2;
			}
			
			StartCoroutine (C_SpikeMovement (clone));
			yield return null;
		}
	}
	
	IEnumerator C_SpikeMovement(ObjectsData SpikeClone)
	{
		bool isMoving = true;
		
		while(isMoving)
		{
			yield return null;
			while(Vector2.Distance(SpikeClone.aPooledObject.transform.localPosition, wayPoints[SpikeClone.headingToThisIndex]) > 0.01f)
			{
				SpikeClone.aPooledObject.transform.localPosition = Vector3.MoveTowards(SpikeClone.aPooledObject.transform.localPosition, wayPoints[SpikeClone.headingToThisIndex], Time.deltaTime*moveSpeed*speedMultiplier);
				yield return null;
			}
			SpikeClone.aPooledObject.transform.localPosition = wayPoints[SpikeClone.headingToThisIndex];
			
			if(forward)
			{
				SpikeClone.headingToThisIndex++;
				if(SpikeClone.headingToThisIndex > 3)
				{
					SpikeClone.headingToThisIndex = 3;
					isMoving = false;
				}
			}
			else
			{
				SpikeClone.headingToThisIndex--;
				if(SpikeClone.headingToThisIndex < 0)
				{
					SpikeClone.headingToThisIndex = 0;
					isMoving = false;
				}
			}
			
			yield return null;
		}
		
		SpikeClone.aPooledObject.SetActive (false);
		yield return null;
	}
	
	
	// MACHINE OVERRIDES ----------------------------------------------------------------------------------------------------------------------
	public override void Start_Machine()
	{
		if(!isActive)
		{
			isActive = true;
			StartCoroutine ("C_StartTreadmill");
			foreach (ObjectsData child in PooledObjects)
			{
				if(child.aPooledObject.activeInHierarchy)
				{
					StartCoroutine (C_SpikeMovement (child));
				}
			}
		}
	}
	
	public override void Stop_Machine()
	{
		if(isActive)
		{
			StopAllCoroutines ();
			isActive = false;
			isPaused = true;
		}
	}
	
	public override void Reset_Machine()
	{
		StopAllCoroutines ();
		isActive = false;
		forward = true;
		isPaused = false;

		speedMultiplier = 1;
		currentDelayBetweenSpikes = 999;

		
		foreach (ObjectsData child in PooledObjects)
		{
			child.aPooledObject.SetActive (false);
		}
	}
	
	public override void Reverse_Machine()
	{
		if(forward)
			forward = false;
		else
			forward = true;
		
		foreach (ObjectsData child in PooledObjects)	// Change which waypoint this object is heading to
		{
			if(child.aPooledObject.activeInHierarchy)
			{
				if(forward)
				{
					child.headingToThisIndex++;
					if(child.headingToThisIndex > 3)
						child.headingToThisIndex = 3;
				}
				else
				{
					child.headingToThisIndex--;
					if(child.headingToThisIndex < 0)
						child.headingToThisIndex = 0;
				}
			}
		}
		
		// Flip the current delay between spawning an object
		currentDelayBetweenSpikes = delayBetweenSpikes/speedMultiplier - currentDelayBetweenSpikes;
		if(currentDelayBetweenSpikes < 0)	currentDelayBetweenSpikes = 0;
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
			currentDelayBetweenSpikes /= increaseInSpeed;
		}
		
//		print ("Speed Up Machine "+speedMultiplier + ", "+increaseInSpeed+"");
	}
	
	public override void SlowDown_Machine(float speedChange)
	{
		speedChange = Mathf.Abs (speedChange);
		float previousSpeedMultiplier = speedMultiplier;
		float decreaseInSpeed = 0;
		
		speedMultiplier -= speedChange;
		if(speedMultiplier <= 0) speedMultiplier = 0;
		
		
		if(speedMultiplier == 0)
			Stop_Machine();
		else
		{
			decreaseInSpeed = speedMultiplier / previousSpeedMultiplier;
			currentDelayBetweenSpikes /= decreaseInSpeed;
		}
		
//		print ("Slow Down Machine "+speedMultiplier + ", "+ decreaseInSpeed);
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
using System.Collections.Generic;

public class SpikeTreadmill : MonoBehaviour
{
	public bool RestartOnReset = true;
	public bool MaintainParentRotation = true;		// Transform / rotation will be based on parents / global 

	public float StartDelay = 0;
	public float MoveSpeed = 1;						// Spikes movement speed
	public float DelayBetweenSpikes = 0.5f;			// Rate at which the spikes spawn
	public bool IsEnabled = true;					// Enable at startup

	public GameObject SpikePrefab;					// Loaded gameobject to spawn
	public Transform StartingPosition;				// Starting position
	public Transform EndingPosition;				// Ending position

	public bool DisableOffsets = false;				// Can disable offsets if needed
	private float SpriteHeight = 0;					// Used in the calculation of offsets
	private float SpriteWidth = 0;					// Used in the calculation of offsets
	private float OffsetX = 0;						// The X_offset of the collider (if available)
	private float OffsetY = 0;						// The Y_offset of the collider (if available)

	private Vector3 Point0;							// Spawn point
	private Vector3 Point1;							// Waypoint 1 - above the spawn point
	private Vector3 Point2;							// Waypoint 2 - above the end point
	private Vector3 Point3;							// Waypoint 3 - at the end point

	public List<GameObject> PooledObjects = new List<GameObject>();

	void Start ()
	{
		if(RestartOnReset)
			EventManager.resetObjects += Reset;

		if(!DisableOffsets)
		{
			// Calculating the width / height based on which component the spike prefab has
			if(SpikePrefab.GetComponent<SpriteRenderer> () != null)
			{
				SpriteRenderer TEMP = SpikePrefab.GetComponent<SpriteRenderer> ();
				SpriteHeight = TEMP.sprite.bounds.size.y * SpikePrefab.transform.localScale.y;
				SpriteWidth = TEMP.sprite.bounds.size.x * SpikePrefab.transform.localScale.x;
			}
			if(SpikePrefab.GetComponent<PolygonCollider2D> () != null)
			{
				PolygonCollider2D Poly = SpikePrefab.GetComponent<PolygonCollider2D> ();

				SpriteWidth = Poly.bounds.size.x + (SpriteWidth - Poly.bounds.size.x);
				SpriteHeight = Poly.bounds.size.y + (SpriteHeight - Poly.bounds.size.y);

//				SpriteWidth = Poly.bounds.size.x;
//				SpriteHeight = Poly.bounds.size.y;

//				print(Poly.bounds.size.x+", "+Poly.bounds.size.y);				// Dimensions of the polygon collider
//				print(Poly.bounds.min.x+", "+Poly.bounds.min.y);				// Min global coordinates of the polygon collider
//				print(Poly.bounds.max.x+", "+Poly.bounds.max.y);				// Max global coordinates of the polygon collider
//				print(Poly.bounds.center.x+", "+Poly.bounds.center.y);			// Center global coordinates of the polygon collider
//				print ( OffsetX+", "+OffsetY);
			}
			if(SpikePrefab.GetComponent<CircleCollider2D> () != null)
			{
				CircleCollider2D Poly = SpikePrefab.GetComponent<CircleCollider2D> ();
				SpriteWidth = Poly.bounds.size.x;
				SpriteHeight = Poly.bounds.size.y;
				OffsetX = Poly.offset.x;
				OffsetY = Poly.offset.y;
			}
			if(SpikePrefab.GetComponent<BoxCollider2D> () != null)
			{
				BoxCollider2D Poly = SpikePrefab.GetComponent<BoxCollider2D> ();
				SpriteWidth = Poly.bounds.size.x;
				SpriteHeight = Poly.bounds.size.y;
				OffsetX = Poly.offset.x;
				OffsetY = Poly.offset.y;
			}
		}

		// Calculating the waypoint based on calculated width / height and offsets
		if(MaintainParentRotation)
		{
			Point0 = StartingPosition.transform.localPosition + new Vector3(-OffsetX+SpriteWidth/2, -OffsetY-SpriteHeight/2);
			Point1 = StartingPosition.transform.localPosition + new Vector3(-OffsetX+SpriteWidth/2, -OffsetY+SpriteHeight/2);
			Point2 = EndingPosition.transform.localPosition + new Vector3(-OffsetX-SpriteWidth/2, -OffsetY+SpriteHeight/2);
			Point3 = EndingPosition.transform.localPosition + new Vector3(-OffsetX-SpriteWidth/2, -OffsetY-SpriteHeight/2);
		}
		else
		{
			Point0 = StartingPosition.transform.position - new Vector3(0, SpriteHeight/2);

			Point1 = StartingPosition.transform.position + new Vector3 (0, SpriteHeight/2);
			Point1 = gameObject.transform.InverseTransformPoint (Point1);
			
			Point2 = EndingPosition.transform.position + new Vector3 (0, SpriteHeight/2);
			Point2 = gameObject.transform.InverseTransformPoint (Point2);
			
			Point3 = EndingPosition.transform.position + new Vector3 (0, -SpriteHeight/2);
			Point3 = gameObject.transform.InverseTransformPoint (Point3);
		}

		if(IsEnabled)
			StartCoroutine ("C_StartTreadmill");
	}

	void OnDestroy()
	{
		if(RestartOnReset)
			EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		StopAllCoroutines ();

		foreach (GameObject child in PooledObjects)
		{
			child.SetActive (false);
		}

		if(IsEnabled)
			StartCoroutine ("C_StartTreadmill");
	}




	GameObject GetPooledObject()
	{
		foreach (GameObject child in PooledObjects)
		{
			if(!child.activeInHierarchy)
			{
				child.SetActive (true);
				return child;
			}
		}

		GameObject clone;
		clone = Instantiate (SpikePrefab, Point0, SpikePrefab.transform.rotation) as GameObject;
		clone.transform.parent = gameObject.transform;
		PooledObjects.Add (clone);

		return clone;
	}

	IEnumerator C_StartTreadmill()
	{
		yield return new WaitForSeconds (StartDelay);

		GameObject clone;

		// Create Object, Set it to the starting position, then send it to do its path movment via coroutine
		while(gameObject != null)
		{
			clone = GetPooledObject ();

			if(MaintainParentRotation)
			{
//				clone.transform.rotation = clone.transform.rotation * gameObject.transform.rotation;
				clone.transform.rotation = gameObject.transform.rotation;
				clone.transform.localPosition = Point0;
			}
			else
				clone.transform.position = Point0;

			StartCoroutine (C_SpikeMovement (clone));

			yield return new WaitForSeconds(DelayBetweenSpikes);
			yield return null;
		}
	}

	IEnumerator C_SpikeMovement(GameObject SpikeClone)
	{
		yield return null;
		// Move Up via vertical offset
		while(Vector3.Distance(SpikeClone.transform.localPosition, Point1) > 0.01f)
		{
			SpikeClone.transform.localPosition = Vector3.MoveTowards(SpikeClone.transform.localPosition, Point1, Time.deltaTime*MoveSpeed);
			yield return null;
		}
		SpikeClone.transform.localPosition = Point1;
		yield return null;

		// Move Twords ending point while still up
		while(Vector3.Distance(SpikeClone.transform.localPosition, Point2) > 0.01f)
		{
			SpikeClone.transform.localPosition = Vector3.MoveTowards(SpikeClone.transform.localPosition, Point2, Time.deltaTime*MoveSpeed);
			yield return null;
		}
		SpikeClone.transform.localPosition = Point2;
		yield return null;

		// Move down
		while(Vector3.Distance(SpikeClone.transform.localPosition, Point3) > 0.01f)
		{
			SpikeClone.transform.localPosition = Vector3.MoveTowards(SpikeClone.transform.localPosition, Point3, Time.deltaTime*MoveSpeed);
			yield return null;
		}
		SpikeClone.transform.localPosition = Point3;

		if (GameController.control.GetPlayerGameObject().transform.IsChildOf (SpikeClone.transform))
			GameController.control.GetPlayerGameObject().transform.parent = null;

		SpikeClone.SetActive (false);
		yield return null;
	}
}
*/
