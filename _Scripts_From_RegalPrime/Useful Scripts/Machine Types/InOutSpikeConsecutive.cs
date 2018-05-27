// UPDATED 7-23-15 - Machine Functions Added
// Now inherits the Machine class
// Has functions to Start / Stop / Reset / Reverse / Speedup / Slowdown / SetSpeed


// RegalPrime 11-11-14 - InOutSpikeConsecutive.cs

// Takes the children of the scripted gameobject and makes them go up and down in consecutive order (with a delay of ObjectDelay inbetween)
// This takes into consideration the sprites size / scale and moves them upwards (locally) their height and back down to the starting position.

// WaitForReset = false = Default - Objects will continue to move up and down (will wait ObjectResetDelay time inbetween)
// WaitForReset = true - Will wait until the last object is done moving before starting from the beginning (will wait DelayBetweenReset time inbtween full reset)

// I think there is a slight problem when this script is running at high speedMultipliers due to the lazy way I calculated moving of objects.
// Will fix it at a later time if it becomes a problem.


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InOutSpikeConsecutive : Machine
{
	public bool enableOnStart = true;				// Enable at startup
	public bool restartOnReset = true;				// Reset this object when scene Reset() is called
	
	public float delayBetweenObjects = 0.5f;		// Delay between objects
	public float speed = 1;							// Speed of objects
	public float objectResetDelay = 0;				// Delay between updown movment on individual objects
	
	public bool waitForReset = true;				// Should the objects wait until the last object is finished before restarting
	public float delayBetweenReset = 0;				// Dlay between reset when WaitForReset = true
	
	
	private Vector3 startingPosition;				// Backup of the starting position
	private bool isActive = false;					// Is this object active
	private bool forward = true;					// Is this object going forward
	private int currentIndex = 0;					// Current index that this object is at
	private int maxIndex = 0;
	private float speedMultiplier = 1;				// Current speed multiplier
	private float currentDelayBetweenObjects = 0;	// The current delay timing between object triggers
	
	
	private List<ObjectsData> SubObjects = new List<ObjectsData>();	// List of objects
	class ObjectsData
	{
		public Transform objectsPosition;
		public Vector3 startingPosition;
		public Vector3 endingPositon;
		public bool extending = true;
		public bool isActive = false;
		public bool isLastObject = false;
		public float duration = 0;
	}
	
	
	
	void Start ()
	{
		if(restartOnReset)	EventManager.resetObjects += Reset;
		
		float spriteHeight = 0;
		
		foreach (Transform child in transform)		// Populate the SubObjects list
		{
			ObjectsData temp = new ObjectsData ();
			
			if(child.GetComponent<SpriteRenderer> () != null)	// Populate the SpriteHeight list
			{
				SpriteRenderer TEMP = child.GetComponent<SpriteRenderer> ();
				spriteHeight = (TEMP.sprite.bounds.size.y * child.transform.localScale.y);
			}
			else
				spriteHeight = 0;
			
			temp.objectsPosition = child.transform;
			temp.startingPosition = child.transform.localPosition;
			temp.endingPositon = child.transform.localPosition + new Vector3 (0, spriteHeight, 0);
			SubObjects.Add (temp);
		}
		
		maxIndex = SubObjects.Count - 1;
		SubObjects [maxIndex].isLastObject = true;
		startingPosition = gameObject.transform.position;
		if(enableOnStart) Start_Machine();
	}
	
	void OnDestroy()
	{
		if(restartOnReset)	EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		Reset_Machine ();
		if(enableOnStart) Start_Machine();
	}
	
	
	
	
	
	public IEnumerator C_LoopThroughList()
	{
		
		while(isActive)
		{
			if(!SubObjects[currentIndex].isActive)
			{
				StartCoroutine (C_In_Out(SubObjects[currentIndex]));
			}
			
			if(forward)
			{
				currentIndex++;
				if(currentIndex > maxIndex)
				{
					currentIndex = maxIndex;
					break;
				}
			}
			else
			{
				currentIndex--;
				if(currentIndex < 0)
				{
					currentIndex = 0;
					break;
				}
			}
			
			
			// Calculate the delay between objects
			if(delayBetweenObjects != 0 && !SubObjects[currentIndex].isActive)
			{
				while(currentDelayBetweenObjects < delayBetweenObjects/speedMultiplier)
				{
					currentDelayBetweenObjects += Time.deltaTime;
					yield return new WaitForSeconds(Time.deltaTime);
				}
			}
			currentDelayBetweenObjects = 0;
			yield return null;
		}
	}
	
	
	
	
	
	IEnumerator C_In_Out(ObjectsData singleObject)
	{
		// Calcuate start and ending position
		singleObject.isActive = true;
		do
		{
			if(singleObject.extending)
			{
				while(singleObject.duration <= 1)	// Move Out
				{
					singleObject.duration += Time.deltaTime*speed*speedMultiplier;
					singleObject.objectsPosition.localPosition = Vector3.Lerp(singleObject.startingPosition, singleObject.endingPositon, singleObject.duration);
					yield return null;
				}
				
				singleObject.extending = false;
				yield return null;
				
				while(singleObject.duration >= 0)	// Move In
				{
					singleObject.duration -= Time.deltaTime*speed*speedMultiplier;
					singleObject.objectsPosition.localPosition = Vector3.Lerp(singleObject.startingPosition, singleObject.endingPositon, singleObject.duration);
					yield return null;
				}
				
				singleObject.extending = true;
			}
			else
			{
				while(singleObject.duration >= 0)	// Move In
				{
					singleObject.duration -= Time.deltaTime*speed*speedMultiplier;
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
		singleObject.isActive = false;
		
		
		if(singleObject.isLastObject)
		{
			if(forward)
				currentIndex = 0;
			else
				currentIndex = maxIndex;
			
			yield return new WaitForSeconds(delayBetweenReset/speedMultiplier);
			StartCoroutine ("C_LoopThroughList");
		}
	}
	
	
	
	// MACHINE OVERRIDES ----------------------------------------------------------------------------------------------------------------------
	public override void Start_Machine()
	{
		if(!isActive)
		{
			foreach(ObjectsData anObject in SubObjects)
			{
				if(anObject.isActive)
				{
					StartCoroutine (C_In_Out(anObject));
				}
			}
			
			isActive = true;
			StartCoroutine ("C_LoopThroughList");
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
		foreach(ObjectsData anObject in SubObjects)
		{
			anObject.objectsPosition.localPosition = anObject.startingPosition;
			anObject.extending = true;
			anObject.isActive = false;
			anObject.duration = 0;
		}
		
		
		SubObjects [maxIndex].isLastObject = true;
		forward = true;
		currentIndex = 0;
		isActive = false;
		speedMultiplier = 1;
		currentDelayBetweenObjects = 0;
	}
	
	public override void Reverse_Machine()
	{
		if(isActive) StopAllCoroutines ();
		
		if (forward)
		{
			SubObjects [0].isLastObject = true;
			SubObjects [maxIndex].isLastObject = false;
			forward = false;
			
			currentIndex --;
			if(currentIndex < 0)
				currentIndex = 0;
		}
		else
		{
			SubObjects [maxIndex].isLastObject = true;
			SubObjects [0].isLastObject = false;
			forward = true;
			
			currentIndex ++;
			if(currentIndex > maxIndex)
				currentIndex = maxIndex;
		}
		
		foreach(ObjectsData anObject in SubObjects)
		{
			if(anObject.isActive)
			{
				if(anObject.extending == true)
					anObject.extending = false;
				else
					anObject.extending = true;
			}
			
			if(anObject.isActive && isActive)
				StartCoroutine (C_In_Out(anObject));
		}
		
		currentDelayBetweenObjects = delayBetweenObjects - currentDelayBetweenObjects;
		if(currentDelayBetweenObjects < 0)
			currentDelayBetweenObjects = 0;
		
		if(isActive) StartCoroutine ("C_LoopThroughList");
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
			currentDelayBetweenObjects = 0;
		}
		else
		{
			increaseInSpeed = speedMultiplier / previousSpeedMultiplier;
			currentDelayBetweenObjects /= increaseInSpeed;
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
			currentDelayBetweenObjects /= decreaseInSpeed;
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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InOutSpikeConsecutive : MonoBehaviour
{
	public bool RestartOnReset = true;

	public float ObjectDelay = 1;				// Delay between objects
	public float Speed = 1;						// Speed of objects
	public float ObjectResetDelay = 0;			// Delay between updown movment on individual objects

	public bool WaitForReset = false;			// Should the objects wait until the last object is finished before restarting
	public float DelayBetweenReset = 0;			// Dlay between reset when WaitForReset = true

	private List<Transform> SubObjects = new List<Transform>();			// List of all the child objects
	private List<float> SpriteHeight = new List<float>();				// Height of all of the child objects

	void Start ()
	{
		if(RestartOnReset)
			EventManager.resetObjects += Reset;

		foreach (Transform child in transform)		// Populate the SubObjects list
		{
			SubObjects.Add (child);

			if(SubObjects[0].GetComponent<SpriteRenderer> () != null)	// Populate the SpriteHeight list
			{
				SpriteRenderer TEMP = SubObjects[0].GetComponent<SpriteRenderer> ();
				SpriteHeight.Add (TEMP.sprite.bounds.size.y * child.transform.localScale.y);
			}
			else
				SpriteHeight.Add (0);
		}
		StartCoroutine (C_LoopThroughList ());
	}

	void OnDestroy()
	{
		if(RestartOnReset)
			EventManager.resetObjects -= Reset;
	}
	
	void Reset()
	{
		StopAllCoroutines ();
		foreach(Transform childPosition in SubObjects)
		{
			childPosition.transform.localPosition = new Vector2(childPosition.transform.localPosition.x ,0);
		}
		StartCoroutine (C_LoopThroughList ());
	}


	public IEnumerator C_LoopThroughList()
	{
		do
		{
			for(int I=0; I<SubObjects.Count; I++)
			{
				if(WaitForReset && I==(SubObjects.Count-1))			// If last object and WaitForReset - continue coroutine after last object is finished
				{
					yield return StartCoroutine (C_In_Out(I));
					yield return new WaitForSeconds(DelayBetweenReset);
				}
				else
				{
					StartCoroutine (C_In_Out(I));						// Make subobject move
					if(ObjectDelay != 0)
						yield return new WaitForSeconds(ObjectDelay);	// Delay between object
				}

				yield return null;
			}

			yield return null;
		}while(WaitForReset);
	}


	public IEnumerator C_In_Out(int Index)
	{
		// Calcuate start and ending position
		Vector3 StartingPosition = SubObjects [Index].transform.localPosition;
		Vector3 EndingPosition = StartingPosition + new Vector3 (0, SpriteHeight[Index], 0);

		do
		{
			float Duration = 0;
			while(Duration <= 1)	// Move Out
			{
				Duration += Time.deltaTime*Speed;
				SubObjects[Index].transform.localPosition = Vector3.Lerp(StartingPosition, EndingPosition, Duration);
				yield return null;
			}

			yield return null;

			Duration = 0;
			while(Duration <= 1)	// Move In
			{
				Duration += Time.deltaTime*Speed;
				SubObjects[Index].transform.localPosition = Vector3.Lerp(EndingPosition, StartingPosition, Duration);
				yield return null;
			}

			if(ObjectResetDelay != 0 && !WaitForReset)
				yield return new WaitForSeconds(ObjectResetDelay);
			else
				yield return null;
		}while(!WaitForReset);
	}
}
*/









