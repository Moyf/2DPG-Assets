// RegalPrime 6-02-14 - ProjectileSpawner.cs

// Updated version of the projectile spawner
// Choose a spawn type and set the variables for amount / sets and it will spawn them (make sure to set your projectile)

// This script has a custom editor attached to it (see ProjectileSpawner_Editor.cs for more info)
// Makes it a lot easier to manipulate the script in the editor now


using UnityEngine;
using System.Collections;

public class ProjectileSpawner : MonoBehaviour
{
	public GameObject objectToSpawn;			// The object that will spawn

	public bool Kinematic = true;				// Will the objects be kinematic
	public bool RandomColor = false;			// Will the objects spawn as a random color


	public bool IsEnabled = true;				// Enable this object as default
	public float InitialStartDelay = 0;			// Delay between the script loading and the first set
	public int NumberOfSets = 1;				// Number of sets. 0 = Infinite sets
	public float DelayBetweenSets = 1;			// Delay between each set


	public bool TargetPlayer = true;			// Will find the player if able and use that as the target
	public GameObject TargetObject;				// When not using targetplayer, this is the target

	public Vector2 TargetDirection = new Vector2 (0,1);		// The direction that the objects will spawn

	public float AppearDelay = 0.2f;			// Delay between objects spawning
	public int NumberOfObjects = 5;				// How many objects per set			0 = infinite objects
	public float ObjectSpeed = 1f;				// Speed of the object
	public float ObjectDeathTimer = 3f;			// Death timer of the objects		0 = no death

	public float SprayVariance = 0;				// How varied certain shots are (scatter)

	public float DegreeChange = 10f;			// For Shoot_CircularSpray - Degree change between each objects spawn
	public float DegreeCap = 0f;				// For Shoot_CircularSpray - How many degrees before reversing. 0 = no cap

	public float Amplitude_X = 0;				// For Shoot_WaveSpray - the amplitude / period of x / y 
	public float PeriodSpeed_X = 0;
	public float Amplitude_Y = 0;
	public float PeriodSpeed_Y = 0;
	public bool LerpMovement = true;			// Are the objects moved via lerping or exact movements

	private ObjectCreation objectSpawner;		// Script reference

	public int CurrentSpawnSelection = 0;			// Which object spawn it will use
	delegate IEnumerator ObjectSpawnerDelegate();	// Delegate of the spawn type
	ObjectSpawnerDelegate[] ObjectSpawnerType;		// Array of all the spawn types

	private float TimeToSpawnObjects = 0;			// Calculated time to spawn a set of objects

	void Start()
	{
		EventManager.resetObjects += Reset;

		if (!objectToSpawn.GetComponent<SpriteRenderer> ())
			objectToSpawn.AddComponent<SpriteRenderer> ();
		
		objectSpawner = gameObject.AddComponent<ObjectCreation> ();

//		objectSpawner.EnablePooling = EnablePooling;
		objectSpawner.SetProjectile (0, objectToSpawn);
		objectSpawner.SetKinematic (0, Kinematic);
		if(RandomColor)	objectSpawner.SetColor_Random (0);

		TimeToSpawnObjects = NumberOfObjects * (AppearDelay+0.01f);

		ObjectSpawnerType = new ObjectSpawnerDelegate[]
		{
			Shoot_AtTarget,
			Shoot_InDirection,
			Shoot_FollowTarget,
			Shoot_RandomSprayOutwards,
			Shoot_CircularSpray,
			Shoot_WaveSpray,
            Shoot_FromScreenEdge,
			Shoot_CircleBurst,
		};

		if(!TargetPlayer && TargetObject == null)
		{
			TargetObject = gameObject;
		}

        if(IsEnabled)	StartCoroutine ("C_StartSpawner");
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		StopAllCoroutines ();
		objectSpawner.ResetObjectSpawner ();
		
		if(IsEnabled)	StartCoroutine ("C_StartSpawner");
	}

	IEnumerator C_StartSpawner()
	{
		yield return new WaitForSeconds (InitialStartDelay);

		if(TargetPlayer)
		{
			yield return null;
			TargetObject = GameController.control.GetPlayerGameObject ();
			if(TargetObject == null)
				TargetObject = gameObject;
		}

		int I = 0;
		while(I<NumberOfSets || NumberOfSets==0)
		{
        	yield return StartCoroutine (ObjectSpawnerType[CurrentSpawnSelection] ());
			I++;
			yield return new WaitForSeconds(DelayBetweenSets);
		}

		print ("I AM DONE");
		yield return new WaitForSeconds(1);
	}


	IEnumerator Shoot_AtTarget()
	{
		objectSpawner.Shoot_AtTarget (0, gameObject, Vector2.zero, TargetObject, Vector2.zero, AppearDelay, NumberOfObjects, ObjectSpeed, SprayVariance, ObjectDeathTimer);
		yield return new WaitForSeconds (TimeToSpawnObjects);
	}
	IEnumerator Shoot_InDirection()
	{
		objectSpawner.Shoot_InDirection (0, gameObject, Vector2.zero, TargetDirection, AppearDelay, NumberOfObjects, ObjectSpeed, SprayVariance, ObjectDeathTimer);
		yield return new WaitForSeconds (TimeToSpawnObjects);
    }
	IEnumerator Shoot_FollowTarget()
	{
		objectSpawner.Shoot_FollowTarget (0, gameObject, Vector2.zero, TargetObject, AppearDelay, NumberOfObjects, ObjectSpeed, ObjectDeathTimer);
		yield return new WaitForSeconds (TimeToSpawnObjects);
    }
	IEnumerator Shoot_RandomSprayOutwards()
	{
		objectSpawner.Shoot_RandomSprayOutwards (0, gameObject, Vector2.zero, AppearDelay, NumberOfObjects, ObjectSpeed, ObjectDeathTimer);
		yield return new WaitForSeconds (TimeToSpawnObjects);
    }
	IEnumerator Shoot_CircularSpray()
	{
		objectSpawner.Shoot_CircularSpray (0, gameObject, Vector2.zero, TargetDirection, 1, 0.0f, DegreeChange, DegreeCap, AppearDelay, NumberOfObjects, ObjectSpeed, ObjectDeathTimer);
		yield return new WaitForSeconds (TimeToSpawnObjects);
    }
	IEnumerator Shoot_WaveSpray()
	{
		objectSpawner.Shoot_WaveSpray (0, gameObject, Vector2.zero, TargetDirection, AppearDelay, NumberOfObjects, ObjectSpeed, Amplitude_X, PeriodSpeed_X, Amplitude_Y, PeriodSpeed_Y, LerpMovement, ObjectDeathTimer);
		yield return new WaitForSeconds (TimeToSpawnObjects);
    }
	IEnumerator Shoot_FromScreenEdge()
	{
		objectSpawner.Shoot_FromScreenEdge (0, TargetDirection, AppearDelay, NumberOfObjects, ObjectSpeed, ObjectDeathTimer);
		yield return new WaitForSeconds (TimeToSpawnObjects);
    }
	IEnumerator Shoot_CircleBurst()
	{
		objectSpawner.Shoot_CircleBurst (0, gameObject, Vector2.zero, NumberOfObjects, ObjectSpeed, ObjectDeathTimer);
		yield return null;
    }
    
    public string[] GetListOfSpawns()
	{
		string[] tempList = new string[]
		{
		"AtTarget",
		"InDirection",
		"FollowTarget",
		"RandomSprayOutwards",
		"CircularSpray",
		"WaveSpray",
		"FromScreenEdge",
		"CircleBurst"
		};

		return tempList;
	}

	// Some overrides testing for Machine inheritance
//	public override void Start_Machine ()
//	{
//		StartCoroutine ("C_StartSpawner");
//	}
//	public override void Stop_Machine ()
//	{
//
//	}
//	public override void Reset_Machine ()
//	{
//		Reset();
//	}
}




/*
public float InitialStartDelay = 0;			// Delay between the script loading and the first set
public int NumberOfSets = 1;				// Number of sets. 0 = Infinite sets
public float DelayBetweenSets = 0;			// Delay between each set
public float DeathTimer = 5;				// Time between launch and object being disabled

GameObject TargetObject
Vector3 TargetDirection

float AppearDelay
int NumberOfObjects
float ObjectSpeed
float ObjectDeathTimer

float SprayVariance

float DegreeChange
float DegreeCap

float Amplitude_X
float PeriodSpeed_X
float Amplitude_Y
float PeriodSpeed_Y
bool LerpMovement



C_Shoot_AtTarget
GameObject TargetObject
float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer

C_Shoot_InDirection
Vector3 TargetDirection
float AppearDelay, int NumberOfObjects, float ObjectSpeed, float SprayVariance, float ObjectDeathTimer

Shoot_FollowTarget
GameObject TargetObject
float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer

Shoot_RandomSprayOutwards
float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer

C_Shoot_CircularSpray
Vector3 TargetDirection
float ObjectSpeed, float DegreeChange, float DegreeCap, int NumberOfObjects, float AppearDelay, float ObjectDeathTimer

C_Shoot_WaveSpray
Vector2 TargetDirection
float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer
float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y, bool LerpMovement

C_Shoot_FromScreenEdge
Vector3 Direction
float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer

C_Shoot_CircleBurst
int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer







//		objectSpawner.Shoot_AtTarget (0, gameObject, Vector2.zero, playersObject, Vector2.zero, 0.2f, 10, 1, 0, 3);
//		objectSpawner.Shoot_InDirection (0, gameObject, Vector2.zero, new Vector2 (1, 1), 0.2f, 10, 1, 0, 3);
//		objectSpawner.Shoot_FollowTarget (0, gameObject, Vector2.zero, playersObject, 0.2f, 2, 1, 3);
//		objectSpawner.Shoot_RandomSprayOutwards (0, gameObject, Vector2.zero, 0.2f, 10, 1, 3);
//		objectSpawner.Shoot_CircularSpray (0, gameObject, Vector2.zero, new Vector2 (0, 1), 1, 0.0f, 10, 0, 0.02f, 100, 1, 3);
//		objectSpawner.Shoot_WaveSpray (0, gameObject, Vector2.zero, new Vector2 (0, 1), 0.2f, 10, 1, 1, 1, 0, 0, true, 3);
//		objectSpawner.Shoot_FromScreenEdge (0, new Vector2 (1, 1), 0.2f, 100, 1, 3);
//		objectSpawner.Shoot_CircleBurst (0, gameObject, new Vector2 (1, 1), 10, 1, 3);



























*/