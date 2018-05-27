// Testing script so I dont break the others while tearing them apart
// This script can be deleted at any time


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class A_NewMachineObject : Machine
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
		speedMultiplier = 1;
		currentDelayBetweenSpikes = 999;
		isPaused = false;
		
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

public IEnumerator C_LoopThroughList()
	{
		do
		{
			for(int I=0; I<SubObjects.Count; I++)
			{
				if(waitForReset && I==(SubObjects.Count-1))			// If last object and WaitForReset - continue coroutine after last object is finished
				{
					yield return StartCoroutine (C_In_Out(SubObjects[I]));
					yield return new WaitForSeconds(delayBetweenReset);
				}
				else
				{
					StartCoroutine (C_In_Out(SubObjects[I]));						// Make subobject move
					if(delayBetweenObjects != 0) 
						yield return new WaitForSeconds(delayBetweenObjects);	// Delay between object
				}
				
				yield return null;
			}
			
			yield return null;
		}while(waitForReset);
	}
	
	
	IEnumerator C_In_Out(ObjectsData singleObject)
	{
		// Calcuate start and ending position
		float Duration;

		do
		{

			if(singleObject.extending)
			{
				while(singleObject.duration <= 1)	// Move Out
				{
					singleObject.duration += Time.deltaTime*speed;
					singleObject.objectsPosition.localPosition = Vector3.Lerp(singleObject.startingPosition, singleObject.endingPositon, singleObject.duration);
					yield return null;
				}

				singleObject.extending = false;
				yield return null;
			}
			else
			{
				while(singleObject.duration >= 0)	// Move In
				{
					singleObject.duration -= Time.deltaTime*speed;
					singleObject.objectsPosition.localPosition = Vector3.Lerp(singleObject.startingPosition, singleObject.endingPositon, singleObject.duration);
					yield return null;
				}

				singleObject.extending = true;
			}


			{
				if(objectResetDelay != 0 && !waitForReset)
					yield return new WaitForSeconds(objectResetDelay);
				else
					yield return null;
			}

		}while(!waitForReset);
	}



*/
