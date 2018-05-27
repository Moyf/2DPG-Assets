// RegalPrime 11-29-14 - ProjectileSpawner.cs

// Created to be able to use the NewObjectSpawner script in a simple to use Prefab
// Choose a spawn type and set the variables for amount / sets and it will spawn them (make sure to set your projectile)

using UnityEngine;
using System.Collections;

public class ProjectileSpawner_old : MonoBehaviour
{
	public bool IsEnabled = true;

	public GameObject Projectile;				// Projectile to spawn
	public bool EnablePooling = true;			// Will reuse objects when they die
	public bool IsKinematic = true;				// Sets the kinematic state
	public float StartDelay = 0;				// Delay between the script loading and the first set

	public int NumberOfSets = 1;				// Number of sets. 0 = Infinite sets
	public float DelayBetweenSets = 0;			// Delay between each set
	public float DeathTimer = 5;				// Time between launch and object being disabled
	public bool RandomColor = false;			// Will change the color of each spawned object randomly

	public int ObjectAmount = 0;				// Number of objects
	public float ObjectSpeed = 0;				// Speed of the objects
	public float ObjectDelay = 0;				// Delay between each object

	public float Xdirection = 0;				// Used for directional targeted spawning - X direction
	public float Ydirection = 0;				// Used for directional targeted spawning - Y direction

	public float Amplitude_X = 0;				// Variables for wavy spawned objects
	public float PeriodSpeed_X = 0;				// Variables for wavy spawned objects
	public float Amplitude_Y = 0;				// Variables for wavy spawned objects
	public float PeriodSpeed_Y = 0;				// Variables for wavy spawned objects
	private Vector3 Target;

	public bool ShootDirection = false;			// Shoot the objects in direction
	public bool ShootAtPlayer = false;			// Shoot the objects at the player
	public bool WaveyLine = false;				// Spawn a wavy line - Xdirection / Ydirection - Only vertical / horizontal lines can be spawned

	public bool SprayInDirection = false;		// Spray objects in a general direction
	public bool RandomSprayOutwards = false;	// Spray objects randomly outwards
	public bool CircleBurst = false;			// Create a circle burst

	public bool RandomRain = false;				// Rain projectiles from the top of the screen

	public bool FollowTarget = false;			// Shoot an object that follows the player till death
	public bool RandomSprayBounce = false;		// Create an object that bounces off of walls

	private NewObjectSpawner SuperScript;
	private float TimeToSpawnObjects = 0;



	void Start ()
	{
		EventManager.resetObjects += Reset;

		if (!Projectile.GetComponent<SpriteRenderer> ())
			Projectile.AddComponent<SpriteRenderer> ();

		SuperScript = gameObject.AddComponent<NewObjectSpawner> ();
		Target = new Vector3 (Xdirection, Ydirection);

		SuperScript.EnablePooling = EnablePooling;
		SuperScript.SetProjectile (0, Projectile);
		SuperScript.SetDeathTimer (0, DeathTimer);
		SuperScript.SetKinematicObjects (0, IsKinematic);

		if(RandomColor)
			SuperScript.SetColor_Random (0);

		TimeToSpawnObjects = ObjectAmount * ObjectDelay;

		if(IsEnabled)
			StartCoroutine ("C_StartCoroutine");
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	
	void Reset()
	{
		StopAllCoroutines ();
		SuperScript.ResetObjectSpawner ();

		if(IsEnabled)
			StartCoroutine ("C_StartCoroutine");
	}

	

	IEnumerator C_StartCoroutine()
	{
		yield return new WaitForSeconds(StartDelay);

//		if (NumberOfSets == 0)
//			NumberOfSets = 1000;

		int I = 0;

//		for(int I=0; I<NumberOfSets; I++)
		while (I<NumberOfSets || NumberOfSets == 0)
		{
			if (ShootDirection)		SuperScript.ShootDirection(0, ObjectAmount, Target, ObjectDelay, ObjectSpeed);
			if (ShootAtPlayer)		SuperScript.ShootAtTarget_Player(0, ObjectDelay, ObjectAmount, ObjectSpeed);
			if (WaveyLine)
			{
					if(Xdirection != 0)
						SuperScript.WaveSpray_Horizontal(0, ObjectAmount, ObjectDelay, Amplitude_Y, PeriodSpeed_Y, ObjectSpeed);
					if(Ydirection != 0)
						SuperScript.WaveSpray_Vertical(0, ObjectAmount, ObjectDelay, Amplitude_X, PeriodSpeed_X, ObjectSpeed);
			}
			
			if (SprayInDirection)	SuperScript.SprayInDirection (0, ObjectAmount, Target, ObjectDelay,ObjectSpeed);
			if (RandomSprayOutwards)	SuperScript.RandomSprayOutwards(0, ObjectDelay, ObjectAmount, ObjectSpeed);

			// Next 3 count as one object
			if (CircleBurst)		SuperScript.CircleBurst (0, ObjectAmount, ObjectSpeed);
			if (RandomRain)			SuperScript.ShootFromTopScreen(0, ObjectAmount, ObjectDelay, ObjectSpeed);
			if (FollowTarget)		SuperScript.FollowTarget_Player(0, DeathTimer, ObjectSpeed);
			if (RandomSprayBounce)	SuperScript.RandomSprayBounce(0, ObjectDelay, ObjectAmount, ObjectSpeed);


			yield return new WaitForSeconds(TimeToSpawnObjects);
			yield return new WaitForSeconds(DelayBetweenSets);

			I++;
		}

		print (" I AM DONE");
		yield return null;
	}
}