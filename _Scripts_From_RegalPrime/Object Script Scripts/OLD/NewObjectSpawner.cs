// RegalPrime 11-29-14 - NewObjectSpawner.cs

// This is a very long script that does many many things
// Creating circles / squares / triangles / lines 
// Rotation / movement / scaling
// and tonnnns more

// For the most part, this was me learning how to do certain tasks and putting it into one super script
// Many other scripts use this script to make their life easier
// Due to the size of this script there may be wording inconsistencies and other issues
// Im sure in the future, I will go thought this entire script and smooth things out

// Most of the Coroutines have comments, so I wont mention about them here
// Check out the CatBoss.cs / ColorChangeSprite.cs / ProjectileSpawner.cs script for some examples of using this script

// Unlike other scripts, this does not connect to the ResetManager
// This is because sometimes it will stop the coroutines before / after another script at the wrong timing, thus giving you wierd results
// Using this script in another script's Reset() function, you will need to manually call ResetObjectSpawner() function
// This way you can control the timing of when it resets


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewObjectSpawner : MonoBehaviour
{
	private PhysicsMaterial2D BouncyMaterial;				// Creates a bouncy physics material

	public GameObject projectile;							// Projectile to launch

	private bool RandomColorSpawn = false;					// Should the objects spawn as random colors?
	private Color SpawnColor = Color.white;					// Default spawn color
	private float ObjectDeathTimer = 5;						// Default death timer

	private bool AllowRotateAroundObject = false;			// Used to tell when to stop
	private bool ForeverRotate = false;						// Used to tell when to stop
	private bool AllowWave = false;							// Used to tell when to stop
	private bool ForeverColorChange = false;				// Used to tell when to stop
	private bool KinematicObjects = true;					// Should the objects spawn as kinematic?

	private List<GameObject> SpawnedObjects = new List<GameObject>();		// Set Objects connected to this script

	public bool EnablePooling = true;										// Should we reuse our projectiles?
	private List<GameObject> PooledObjects = new List<GameObject>();		// List of pooled objects
	private List<bool> PooledObjectsActiveState = new List<bool>();			// List of current stats of pooled objects
	private List<GameObject> NonPooledObjects = new List<GameObject> ();	// List of non-pooled objects

	public static int TotalPooledObjects = 0;				// Counts the total pooled objects over every NewObjectSpawner script


	void Start()
	{
		BouncyMaterial = new PhysicsMaterial2D ();
		BouncyMaterial.bounciness = 1;
	}
	void OnDestroy()
	{

	}
	void FixedUpdate()
	{

	}

	// Reset all objects
	public void ResetObjectSpawner()
	{
		StopAllCoroutines ();

		AllowRotateAroundObject = false;
		ForeverRotate = false;
		AllowWave = false;
		ForeverColorChange = false;

		for(int I=0; I<PooledObjects.Count; I++)
		{
			if(PooledObjects[I].GetComponent<Rigidbody2D>() == true)
				PooledObjects[I].GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			PooledObjects[I].transform.rotation = Quaternion.identity;
			PooledObjects[I].SetActive(false);
			PooledObjectsActiveState[I] = false;
		}

		for(int I=0; I<NonPooledObjects.Count; I++)
		{
			Destroy (NonPooledObjects[I]);
		}

		NonPooledObjects.Clear ();
		SpawnedObjects.Clear ();
	}


	// Find a pooled object. If there is not one available, create a new object
	GameObject GetPooledObject()
	{
		foreach (GameObject child in PooledObjects)			// Look for a pooled object to use
		{
			if(!child.activeInHierarchy)
			{
				PooledObjectsActiveState [PooledObjects.IndexOf (child)] = true;
				child.transform.position = gameObject.transform.position;

				if(KinematicObjects)
					child.GetComponent<Rigidbody2D>().isKinematic = true;
				else
					child.GetComponent<Rigidbody2D>().isKinematic = false;

				child.SetActive(true);
				return child;
			}
		}

		// Couldnt find one, so make a new one and add it to the end of the list
		GameObject newclone;

		newclone = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
		newclone.name = gameObject.name + " - clone -"+PooledObjects.Count;
		if(newclone.GetComponent<Rigidbody2D>() == false)
			newclone.AddComponent <Rigidbody2D>();

		newclone.GetComponent<Rigidbody2D>().freezeRotation = true;

		if(KinematicObjects)
			newclone.GetComponent<Rigidbody2D>().isKinematic = true;
		else
			newclone.GetComponent<Rigidbody2D>().isKinematic = false;

		TotalPooledObjects++;
		PooledObjects.Add (newclone);
		PooledObjectsActiveState.Add (true);

		return newclone;
	}
	// Alternate to destroying an object. Instead it disables it
	IEnumerator RepoolObject(GameObject ObjectToDestroy, float ObjectDeathTimer)	// "Destroy" an object after a set time (repooling)
	{
		yield return new WaitForSeconds(ObjectDeathTimer);

		int LocationOfObject = PooledObjects.IndexOf (ObjectToDestroy);

		ObjectToDestroy.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
//		ObjectToDestroy.transform.position = gameObject.transform.position;
		ObjectToDestroy.SetActive (false);

		if(LocationOfObject >= 0)
			PooledObjectsActiveState [PooledObjects.IndexOf (ObjectToDestroy)] = false;
	}
	// Checks if a certain object is active
	bool IsPooledObjectActive(GameObject pooledObject)
	{
		if(pooledObject != null)
		{
			int LocationOfObject = PooledObjects.IndexOf (pooledObject);
			if(PooledObjectsActiveState.Count > 1 && PooledObjects.Count >1 && LocationOfObject >= 0)	// If found in the pooled objects list, return state
			{
				return PooledObjectsActiveState [LocationOfObject];
			}
			else
				return pooledObject.activeInHierarchy;		// Return active state in hierarchy
		}
		else
			return false;
	}
	// Enable / Disable object pooling
	IEnumerator C_SetToEnablePooling(float StartDelay, bool enablePooling)
	{
		yield return new WaitForSeconds(StartDelay);
		EnablePooling = enablePooling;
	}




	// Create a single object that bounces off of objects (needs further testing)
	IEnumerator C_RandomSprayBounce(float StartDelay, float AppearDelay, int NumberOfObjects, float ObjectSpeed)
	{
		yield return new WaitForSeconds(StartDelay);
		
		float RandomX;
		float RandomY;

		GameObject newclone = CreateClone(false);
//		newclone = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
		if(newclone.GetComponent<Rigidbody2D>() == false)
			newclone.AddComponent <Rigidbody2D>();
		newclone.GetComponent<Rigidbody2D>().freezeRotation = true;
		if(KinematicObjects)
			newclone.GetComponent<Rigidbody2D>().isKinematic = true;
		
		if(RandomColorSpawn == true)
			newclone.GetComponent<SpriteRenderer>().color = new Color (Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f), 1F);
		else
			newclone.GetComponent<SpriteRenderer>().color = SpawnColor;

		if(ObjectDeathTimer != 0)
		{
			if(EnablePooling)
				StartCoroutine (RepoolObject(newclone, ObjectDeathTimer));
			else
				Destroy (newclone, ObjectDeathTimer);
		}
			
		RandomX = Random.Range (-1f, 1);
		RandomY = Random.Range (-1f, 1);			
			
		newclone.GetComponent<Rigidbody2D>().velocity = new Vector3 (RandomX, RandomY, 0).normalized * ObjectSpeed;
		newclone.GetComponent<Rigidbody2D>().isKinematic = false;
		newclone.GetComponent<Rigidbody2D>().gravityScale = 0;
		newclone.GetComponent<Rigidbody2D>().angularDrag = 0;
		newclone.layer = LayerMask.NameToLayer ("Monsters");
		CircleCollider2D TempCircle = newclone.AddComponent<CircleCollider2D> ();
		TempCircle.radius = 0.1f;
		TempCircle.sharedMaterial = BouncyMaterial;
		TempCircle.enabled = false;
		TempCircle.enabled = true;

		yield return null;
	}
/*
//	Alternate code for creating a projectile that bounces off of objects
// Done using calculations and not bouncy material
	IEnumerator C_RandomSprayBounce(float StartDelay, float AppearDelay, int NumberOfObjects, float ObjectSpeed)
	{
		yield return new WaitForSeconds(StartDelay);
		
		float RandomX;
		float RandomY;
		GameObject TEMP;
		
		TEMP = CreateClone (false);
		
		if(ObjectDeathTimer != 0)
		{
			if(EnablePooling)
				StartCoroutine (RepoolObject(newclone, ObjectDeathTimer));
			else
				Destroy (newclone, ObjectDeathTimer);
		}
		
		RandomX = Random.Range (-1f, 1);
		RandomY = Random.Range (-1f, 1);			
		
		TEMP.rigidbody2D.velocity = new Vector3 (RandomX, RandomY, 0).normalized * ObjectSpeed;
		
		while(IsPooledObjectActive(TEMP))
		{
			float rayDistance = 0.16f;
			
			RaycastHit2D hitTarget = Physics2D.Raycast(TEMP.transform.position, TEMP.rigidbody2D.velocity+(new Vector2(TEMP.transform.position.x, TEMP.transform.position.y)), rayDistance, 1 << LayerMask.NameToLayer ("Ground"));
			
			Debug.DrawLine(TEMP.transform.position, TEMP.rigidbody2D.velocity.normalized*rayDistance+(new Vector2(TEMP.transform.position.x, TEMP.transform.position.y)), Color.red);
			
			if(hitTarget.transform != null)
			{
				float Xdistance = Mathf.Abs(TEMP.transform.position.x - hitTarget.point.x);
				float Ydistance = Mathf.Abs(TEMP.transform.position.y - hitTarget.point.y);

				if(Xdistance > Ydistance)
					TEMP.rigidbody2D.velocity = new Vector3(-TEMP.rigidbody2D.velocity.x, TEMP.rigidbody2D.velocity.y);
				else
					TEMP.rigidbody2D.velocity = new Vector3(TEMP.rigidbody2D.velocity.x, -TEMP.rigidbody2D.velocity.y);
			}
			
			yield return null;
		}
		yield return null;
	}
*/

	// This takes another NewObjectSpawner's object list and add its to this one. This is a test function 
	// However, I do use this in the CatBoss.cs script
	public void TESTCOMBINE(float StartDelay, NewObjectSpawner OtherObjectSpawner)
	{
		StartCoroutine (C_TESTCOMBINE (StartDelay, OtherObjectSpawner));
	}
	IEnumerator C_TESTCOMBINE(float StartDelay, NewObjectSpawner OtherObjectSpawner)
	{
		yield return new WaitForSeconds(StartDelay);

		for(int i=0; i<OtherObjectSpawner.SpawnedObjects.Count; i++)
		{
			SpawnedObjects.Add (OtherObjectSpawner.SpawnedObjects[i]);
			OtherObjectSpawner.SpawnedObjects[i].transform.parent = this.transform;
//			PooledObjects.Add (OtherObjectSpawner.SpawnedObjects[i]);
//			PooledObjectsActiveState.Add(OtherObjectSpawner.SpawnedObjects[i].activeInHierarchy);
		}
	}

	// Add the gameobject to the list - do not use this
	public void AddSelfToList(float StartDelay)
	{
		StartCoroutine (C_AddSelfToList (StartDelay));
	}
	IEnumerator C_AddSelfToList(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);
		
		PooledObjects.Add (gameObject);
		PooledObjectsActiveState.Add (true);
		SpawnedObjects.Add (gameObject);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// This point and above are the newer functions
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




	// Sets the death timer of one shot objects. Default is 10s
	// Setting this to 0 will disable the destruction of one shot objects
	// This is useful in spawning non projectile objects (like monsters)
	IEnumerator C_SetDeathTimer(float StartDelay, float DeathTimer)
	{
		yield return new WaitForSeconds(StartDelay);
		
		ObjectDeathTimer = DeathTimer;
	}

	// Changes if the objects are spawned as kinematic or not
	IEnumerator C_SetKinematicObjects(float StartDelay, bool areObjectsKinematic)
	{
		yield return new WaitForSeconds(StartDelay);
		
		KinematicObjects = areObjectsKinematic;
	}	

	// Sets the projectile used when a clone is created
	IEnumerator C_SetProjectile(float StartDelay, GameObject ProjectileSet)
	{
		yield return new WaitForSeconds(StartDelay);
		projectile = ProjectileSet;
	}

	// Play a clip at a certain time.
	IEnumerator C_PlayClip(float StartDelay, AudioClip ClipToPlay)
	{
		yield return new WaitForSeconds(StartDelay);
		
		if(ClipToPlay)
			MusicController.control.OneShotAudio (ClipToPlay);
		
		yield return null;
	}

	// Enables a gameobject when called
	IEnumerator C_EnableObject(float StartDelay, GameObject ObjectToEnable)
	{
		yield return new WaitForSeconds(StartDelay);

		if(ObjectToEnable)
			ObjectToEnable.SetActive (true);

		yield return null;
	}

	// Enables a gameobject when called
	IEnumerator C_DisableObject(float StartDelay, GameObject ObjectToDisable)
	{
		yield return new WaitForSeconds(StartDelay);
		
		if(ObjectToDisable)
			ObjectToDisable.SetActive (false);
		
		yield return null;
	}

	// Destroys all of the objects in the list, has a delay between destructions
	// After the last object is destroyed, the list is cleared
	IEnumerator C_DestroyChildren(float StartDelay, float ObjectDelay)
	{
		yield return new WaitForSeconds(StartDelay);
		foreach (GameObject child in SpawnedObjects)
		{
			if(EnablePooling)
				StartCoroutine (RepoolObject(child, 0));
			else
				Destroy (child);

			yield return new WaitForSeconds(ObjectDelay);
		}
		
		SpawnedObjects.Clear ();
	}

	// Attaches the objects in the list to this gameobject
	// Useful after C_Break functions
	IEnumerator C_ReattachChildren(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);
		foreach (GameObject child in SpawnedObjects)
		{
			child.transform.parent = this.transform;
		}
	}

	// Set the color of the objects that are spawning
	// Not setting the color will default to random colors
	IEnumerator C_SetColor (float StartDelay, Color newColor)
	{
		yield return new WaitForSeconds(StartDelay);
		RandomColorSpawn = false;
		SpawnColor = newColor;
	}
	IEnumerator	C_SetColor_Random (float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);
		RandomColorSpawn = true;
	}

	// Fades the objects in the list over time
	// At the lowest visibilty (Alpha) it will disable the objects
	IEnumerator C_FadeObject_Children(float StartDelay, float Speed)
	{
		if(StartDelay != 0)
			yield return new WaitForSeconds(StartDelay);
		
		foreach (GameObject child in SpawnedObjects)
		{
			StartCoroutine (C_FadeObject(0, child, Speed));
		}
		yield return null;
	}
	IEnumerator C_FadeObject(float StartDelay, GameObject AGameObject, float Speed)
	{
		yield return new WaitForSeconds(StartDelay);
		
		SpriteRenderer TEMP;
		float Duration = 0;
		float Alpha = 0;

		TEMP = AGameObject.GetComponent<SpriteRenderer>();

		while (Duration < 1)
		{
			Alpha = Mathf.Lerp(1,0,Duration);
			TEMP.color = new Color(TEMP.color.r,TEMP.color.g,TEMP.color.b, Alpha);
			Duration += Time.deltaTime*Speed;
		
			yield return null;
		}
		AGameObject.SetActive (false);
	}




	// Unfades the objects in the list over time
	// It starts by enabling the objects before increasing their alpha
	IEnumerator C_UnfadeObject_Children(float StartDelay, float Speed)
	{
		if(StartDelay != 0)
			yield return new WaitForSeconds(StartDelay);
		
		foreach (GameObject child in SpawnedObjects)
		{
			StartCoroutine (C_UnfadeObject(0, child, Speed));
		}
		yield return null;
	}
	IEnumerator C_UnfadeObject(float StartDelay, GameObject AGameObject, float Speed)
	{
		yield return new WaitForSeconds(StartDelay);

		SpriteRenderer TEMP;
		float Duration = 0;
		float Alpha = 0;

		TEMP = AGameObject.GetComponent<SpriteRenderer>();
		AGameObject.SetActive (true);

		while (Duration < 1)
		{
			Alpha = Mathf.Lerp(0,1,Duration);
			TEMP.color = new Color(TEMP.color.r,TEMP.color.g,TEMP.color.b, Alpha);
			Duration += Time.deltaTime*Speed;
			yield return null;
		}
	}

	// Changes the color of all the objects in the list
	// Overloaded function - no color option = random color
	IEnumerator C_ChangeColor_Children(float StartDelay, float Speed)
	{
		yield return new WaitForSeconds(StartDelay);
		Color NewColor;

		foreach (GameObject child in SpawnedObjects)
		{
			NewColor = new Color (Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f), child.GetComponent<SpriteRenderer>().color.a);
			StartCoroutine (C_ChangeColor(0, child, NewColor, Speed, true));
		}
		yield return null;
	}
	IEnumerator C_ChangeColor_Children(float StartDelay, Color NewColor, float Speed)
	{
		if(StartDelay != 0)
			yield return new WaitForSeconds(StartDelay);

		foreach (GameObject child in SpawnedObjects)
		{
			StartCoroutine (C_ChangeColor(0, child, NewColor, Speed, false));
		}
		yield return null;
	}

	// Used in lerping the color of each individual object
	IEnumerator C_ChangeColor(float StartDelay, GameObject AGameObject, Color NewColor, float Speed, bool ChangeForever)
	{
		if(StartDelay != 0)
			yield return new WaitForSeconds(StartDelay);

		ForeverColorChange = ChangeForever;

		float Duration = 0;
		Color OldColor;

		do{
			Duration = 0;
			OldColor = AGameObject.GetComponent<SpriteRenderer>().color;

			while(Duration < 1 && IsPooledObjectActive(AGameObject))
			{
				Duration += Time.deltaTime*Speed;
				AGameObject.GetComponent<SpriteRenderer>().color  = Color.Lerp(OldColor, NewColor, Duration);
				yield return null;
			}

			NewColor = new Color (Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f), AGameObject.GetComponent<SpriteRenderer>().color.a);

			yield return null;
		}while(ForeverColorChange);
	}
	IEnumerator C_StopColorChangeEffect(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);
		
		ForeverColorChange = false;
		
		yield return null;
	}

	// Stop all wave type motions via the AllowWave booleasn
	// This will stop every wave motion object using this script
	IEnumerator C_StopWaveEffect(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);

		AllowWave = false;

		yield return null;
	}

	// Takes all the objects in the current list applies a wave type motion to each of them
	IEnumerator C_WaveEffect_Children(float StartDelay, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y)
	{
		yield return new WaitForSeconds(StartDelay);

			int I = 0;
			foreach (GameObject child in SpawnedObjects)
			{
				if(child == true)
				StartCoroutine (C_WaveEffectObject(0, child, I, Amplitude_X, PeriodSpeed_X, Amplitude_Y, PeriodSpeed_Y));

				I++;
			}

			yield return null;
	}

	// Takes a single object and makes them do a wavy like motion based on the variables
	// If gameobject = null then it will become the object the script is attached to
	IEnumerator C_WaveEffectObject(float StartDelay, GameObject AGameObject, float I, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y)
	{
		yield return new WaitForSeconds(StartDelay);

		if(AGameObject == null)
			AGameObject = gameObject;

		float YLerp;
		float XLerp;
		float X = AGameObject.transform.localPosition.x;
		float Y = AGameObject.transform.localPosition.y;

		AllowWave = true;

		while(IsPooledObjectActive(AGameObject) && AllowWave)
		{
			if(Amplitude_Y != 0)
			{
				YLerp = Mathf.Lerp(AGameObject.transform.localPosition.y, Y+Amplitude_Y*Mathf.Sin (I+ Time.time*PeriodSpeed_Y), Time.deltaTime);
//				YLerp = Y+Amplitude_Y*Mathf.Sin (I+ Time.time*PeriodSpeed_Y);
				AGameObject.transform.localPosition = new Vector3(AGameObject.transform.localPosition.x, YLerp, 0);;
			}

			if(Amplitude_X != 0)
			{
				XLerp = Mathf.Lerp(AGameObject.transform.localPosition.x, X+Amplitude_X*Mathf.Sin (I+ Time.time*PeriodSpeed_X), Time.deltaTime);
//				XLerp = X+Amplitude_X*Mathf.Sin (I+ Time.time*PeriodSpeed_X);
				AGameObject.transform.localPosition = new Vector3(XLerp, AGameObject.transform.localPosition.y, 0);;
			}
			yield return null;
		}
		yield return null;
	}

	// Takes all the objects in the current list and forms them in the shape of a circle
	IEnumerator C_MorphToCircle(float StartDelay, float X_Width, float Y_Width,  float ObjectDelay, float Speed)
	{
		yield return new WaitForSeconds(StartDelay);
		Speed = Mathf.Abs (Speed);
		Vector3 pointPos;
		
		for(int i = 0; i<SpawnedObjects.Count; i++)
		{
			float pointNum = (i*1.0f)/SpawnedObjects.Count;
			float angle = pointNum*Mathf.PI*2;
			
			float x = Mathf.Sin (angle)*X_Width;
			float y = Mathf.Cos (angle)*Y_Width;
			
			pointPos = new Vector3(x, y);

			StartCoroutine(C_MoveObjectToPoint(0, SpawnedObjects[i], pointPos, Speed));

			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}

		yield return null;
	}

	// Takes all the objects in the current list and forms them in the shape of a square
	IEnumerator C_MorphToSquare(float StartDelay, float X_Width, float Y_Width, float ObjectDelay, float Speed)
	{
		yield return new WaitForSeconds(StartDelay);
		Speed = Mathf.Abs (Speed);
		Vector3 pointPos;

		float pointNum = 0;
		int counter = 0;

		int EvenObjects = SpawnedObjects.Count / 4 * 4;

		for(int i = 0; i<SpawnedObjects.Count/4; i++)
		{
			pointNum = -1f*X_Width + i*(4/(float)EvenObjects*2*X_Width);
			pointPos = new Vector3(pointNum, 1*Y_Width);
			StartCoroutine(C_MoveObjectToPoint(0, SpawnedObjects[counter], pointPos, Speed));
			counter++;
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}

		for(int i = 0; i<SpawnedObjects.Count/4; i++)
		{
			pointNum = 1f*Y_Width - i*(4/(float)EvenObjects*2*Y_Width);
			pointPos = new Vector3(1*X_Width, pointNum);
			StartCoroutine(C_MoveObjectToPoint(0, SpawnedObjects[counter], pointPos, Speed));
			counter++;
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}

		for(int i = 0; i<SpawnedObjects.Count/4; i++)
		{
			pointNum = 1f*X_Width - i*(4/(float)EvenObjects*2*X_Width);
			pointPos = new Vector3(pointNum, -1*Y_Width);
			StartCoroutine(C_MoveObjectToPoint(0, SpawnedObjects[counter], pointPos, Speed));
			counter++;
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}
		
		for(int i = 0; i<SpawnedObjects.Count/4; i++)
		{
			pointNum = -1f*Y_Width + i*(4/(float)EvenObjects*2*Y_Width);
			pointPos = new Vector3(-1*X_Width, pointNum);
			StartCoroutine(C_MoveObjectToPoint(0, SpawnedObjects[counter], pointPos, Speed));
			counter++;
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}

		while(counter < SpawnedObjects.Count)
		{
			pointPos = new Vector3(-1*X_Width, pointNum);
			StartCoroutine(C_MoveObjectToPoint(0, SpawnedObjects[counter], pointPos, Speed));
			counter++;
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}

		yield return null;
	}

	// Moves an object locally to a certain vector3 point
	// Used mostly for shape change function
	IEnumerator C_MoveObjectToPoint(float StartDelay, GameObject ObjectToMove, Vector3 Target, float Speed)
	{
		yield return new WaitForSeconds(StartDelay);
		if(ObjectToMove == null)
			ObjectToMove = gameObject;

		float Duration = 0;
		Vector3 StartingPostion = ObjectToMove.transform.localPosition;

		if(Speed == 0)
			ObjectToMove.transform.localPosition = Target;
		else
			while(Duration < 1 && IsPooledObjectActive(ObjectToMove))
			{
				Duration += Time.deltaTime*Speed;
				ObjectToMove.transform.localPosition = Vector3.Lerp(StartingPostion, Target, Duration);
				yield return null;
			}
		yield return null;
	}




////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////
	// Creates a wave like line from the scripted object
	// Can be call from the function to make either a vertical or a horizontal wave line
	// Speed allows the line to travel in both directions
	IEnumerator C_WaveSpray(float StartDelay, int NumberOfObjects, float ObjectDelay, Vector2 Target, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y, float Speed)
	{
		yield return new WaitForSeconds(StartDelay);

		for(int I=0; I<NumberOfObjects; I++)
		{
			GameObject newclone = CreateClone(false);
			newclone.transform.position = gameObject.transform.position;
			newclone.GetComponent<Rigidbody2D>().velocity = Target.normalized * Speed;

			StartCoroutine(C_WaveEffectObject(0, newclone, I, Amplitude_X, PeriodSpeed_X, Amplitude_Y, PeriodSpeed_Y));

			if(ObjectDeathTimer != 0)
			{
				if(EnablePooling)
					StartCoroutine (RepoolObject(newclone, ObjectDeathTimer));
				else
					Destroy (newclone, ObjectDeathTimer);
			}

			yield return new WaitForSeconds(ObjectDelay);
		}
	}

	// Create a circle burst at a certain location
	// A circle burst is an object that starts out at a single point and expands outward in a circular shape
	IEnumerator C_CircleBurst(float StartDelay, Transform TargetLocation, int NumberOfPoints, float ObjectSpeed)
	{
		yield return new WaitForSeconds(StartDelay);

		Vector3 pointPos;
		float X_Width = 0.01f;
		float Y_Width = 0.01f;

		
		for(int i = 0; i<NumberOfPoints; i++)
		{
			float pointNum = (i*1.0f)/NumberOfPoints+(gameObject.transform.rotation.z);
			float angle = pointNum*Mathf.PI*2;
			
			float x = Mathf.Sin (angle)*X_Width;
			float y = Mathf.Cos (angle)*Y_Width;
			
			pointPos = new Vector3(x, y)+TargetLocation.position;
			
			GameObject newclone = CreateClone(false);
			newclone.transform.position = pointPos;

			newclone.GetComponent<Rigidbody2D>().velocity = (newclone.transform.position - TargetLocation.position).normalized * ObjectSpeed; 

			if(ObjectDeathTimer != 0)
			{
				if(EnablePooling)
					StartCoroutine (RepoolObject(newclone, ObjectDeathTimer));
				else
					Destroy (newclone, ObjectDeathTimer);
			}
		}
		yield return null;
	}

	// Objects spawn randomly from the top side of the screen downwards
	IEnumerator C_ShootFromTopScreen(float StartDelay, int ObjectAmount, float ObjectDelay, float ObjectSpeed)
	{
		yield return new WaitForSeconds(StartDelay);

		for(int I=0; I<ObjectAmount; I++)
		{
			float RandomNumber = Random.Range (0, 1f);
			
			Vector3 p = Camera.main.ViewportToWorldPoint(new Vector3(RandomNumber, 1, 1));
			GameObject newclone = CreateClone(false);
			newclone.transform.position = p;
			newclone.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -1)*ObjectSpeed;

			if(ObjectDeathTimer != 0)
			{
				if(EnablePooling)
					StartCoroutine (RepoolObject(newclone, ObjectDeathTimer));
				else
					Destroy (newclone, ObjectDeathTimer);
			}

			yield return new WaitForSeconds(ObjectDelay);
		}
		yield return null;
	}

	// Objects spawn randomly from the rigth side of the screen and travel left
	IEnumerator C_ShootFromRightScreen(float StartDelay, int ObjectAmount, float ObjectDelay, float ObjectSpeed)
	{
		yield return new WaitForSeconds(StartDelay);

		for(int I=0; I<ObjectAmount; I++)
		{
			float RandomNumber = Random.Range (0, 1f);
			
			Vector3 p = Camera.main.ViewportToWorldPoint(new Vector3(1, RandomNumber, 1));
			GameObject newclone = CreateClone(false);
			newclone.transform.position = p;
			newclone.GetComponent<Rigidbody2D>().velocity = new Vector2(-1, 0)*ObjectSpeed;

			if(ObjectDeathTimer != 0)
			{
				if(EnablePooling)
					StartCoroutine (RepoolObject(newclone, ObjectDeathTimer));
				else
					Destroy (newclone, ObjectDeathTimer);
			}

			yield return new WaitForSeconds(ObjectDelay);
		}
		yield return null;
	}

	// Sprays a set of objects in a generalized direction (like a fountain)
	// These objects are affected by gravity
	IEnumerator C_SprayInDirection(float StartDelay, int ObjectAmount, Vector3 Target, float ObjectDelay, float ObjectSpeed)
	{
		yield return new WaitForSeconds(StartDelay);
		
		for(int I=0; I<ObjectAmount; I++)
		{
			GameObject newclone = CreateClone(false);
			newclone.GetComponent<Rigidbody2D>().isKinematic = false;

			Vector3 sprayRange = Target.normalized;
			sprayRange.x = sprayRange.x + Random.Range (-.1f,.1f);

			newclone.GetComponent<Rigidbody2D>().velocity = sprayRange * ObjectSpeed;
			if(ObjectDeathTimer != 0)
			{
				if(EnablePooling)
					StartCoroutine (RepoolObject(newclone, ObjectDeathTimer));
				else
					Destroy (newclone, ObjectDeathTimer);
			}			
			yield return new WaitForSeconds(ObjectDelay);
		}
		
		yield return null;
	}

	// Sprays a set of objects outward from the position of the object that this script is attached to
	IEnumerator C_RandomSprayOutwards(float StartDelay, float AppearDelay, int NumberOfObjects, float ObjectSpeed)
	{
		yield return new WaitForSeconds(StartDelay);
		
		float RandomX;
		float RandomY;
		GameObject newclone;
		
		for(int i=0; i<NumberOfObjects; i++)
		{
			newclone = CreateClone (false);
			if(ObjectDeathTimer != 0)
			{
				if(EnablePooling)
					StartCoroutine (RepoolObject(newclone, ObjectDeathTimer));
				else
					Destroy (newclone, ObjectDeathTimer);
			}
			
			RandomX = Random.Range (-1f, 1);
			RandomY = Random.Range (-1f, 1);			

			newclone.GetComponent<Rigidbody2D>().velocity = new Vector3 (RandomX, RandomY, 0).normalized * ObjectSpeed;
			yield return new WaitForSeconds(AppearDelay);
		}
	}

	// Shoots a set of objects at a certain target
	// If the target is null then it sends them at the player
	IEnumerator C_ShootAtTarget(float StartDelay, float ObjectDelay, int ObjectAmount, Transform Target, float ObjectSpeed)
	{
		yield return new WaitForSeconds(StartDelay);

		if(Target == null)
			Target = GameController.control.GuyLocation.transform;
		
		for(int I=0; I<ObjectAmount; I++)
		{
			GameObject newclone = CreateClone(false);
			
			newclone.GetComponent<Rigidbody2D>().velocity = (Target.position - transform.position).normalized * ObjectSpeed;
			if(ObjectDeathTimer != 0)
			{
				if(EnablePooling)
					StartCoroutine (RepoolObject(newclone, ObjectDeathTimer));
				else
					Destroy (newclone, ObjectDeathTimer);
			}			
			yield return new WaitForSeconds(ObjectDelay);
		}
		
		yield return null;
	}

	// Shoots a set of objects in a certain vector3 direction
	IEnumerator C_ShootDirection(float StartDelay, int ObjectAmount, Vector3 Target, float ObjectDelay, float ObjectSpeed)
	{
		yield return new WaitForSeconds(StartDelay);

		for(int I=0; I<ObjectAmount; I++)
		{
			GameObject newclone = CreateClone(false);
			newclone.GetComponent<Rigidbody2D>().velocity = Target.normalized * ObjectSpeed;
			if(ObjectDeathTimer != 0)
			{
				if(EnablePooling)
					StartCoroutine (RepoolObject(newclone, ObjectDeathTimer));
				else
					Destroy (newclone, ObjectDeathTimer);
			}			
			yield return new WaitForSeconds(ObjectDelay);
		}
		yield return null;
	}

	// Shoots a single projectile that continues to follow the target till the created objects death
	IEnumerator C_FollowTarget(float StartDelay, Transform Target, float DeathTimer, float ObjectSpeed)
	{
		yield return new WaitForSeconds(StartDelay);

		if(Target == null)
			Target = GameController.control.GuyLocation.transform;
		
		GameObject newclone = CreateClone(false);

		if(ObjectDeathTimer != 0)
		{
			if(EnablePooling)
				StartCoroutine (RepoolObject(newclone, ObjectDeathTimer));
			else
				Destroy (newclone, ObjectDeathTimer);
		}
		
		while(IsPooledObjectActive(newclone))
		{
			newclone.GetComponent<Rigidbody2D>().velocity = (Target.position - newclone.transform.position).normalized * ObjectSpeed; 
			yield return null;
		}
		yield return null;
	}

////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////
	// Detaches all the children and creates a circle burst at each child location
	IEnumerator C_BreakCircleBurst(float Delay, int CircleBurstSize, float ObjectDelay, float ObjectSpeed)
	{
		yield return new WaitForSeconds (Delay);
		
		foreach (GameObject child in SpawnedObjects)
		{
			if(IsPooledObjectActive(child))
			{
				child.transform.parent = null;
				StartCoroutine (C_CircleBurst(0, child.transform, CircleBurstSize, ObjectSpeed));
				child.SetActive(false);
//				Destroy(child, (1));

				if(ObjectDelay != 0)
					yield return new WaitForSeconds (ObjectDelay);
			}
		}
	}


	// Detaches all the children and sends them in a random direction
	IEnumerator C_BreakRandomly(float Delay, float ObjectDelay, float ObjectSpeed)
	{
		yield return new WaitForSeconds (Delay);
		float RandomX;
		float RandomY;

		foreach (GameObject child in SpawnedObjects)
		{
			child.transform.parent = null;
//			Destroy(child, 4);

			RandomX = Random.Range (-1f, 1);
			RandomY = Random.Range (-1f, 1);			
			
			child.GetComponent<Rigidbody2D>().velocity = new Vector3 (RandomX, RandomY, 0).normalized * ObjectSpeed;

			if(ObjectDelay != 0)
				yield return new WaitForSeconds (ObjectDelay);
		}
	}

	// Detaches all the children and sends them to a certain target
	// If the target is null then it sends them at the player
	IEnumerator C_BreakToTarget(float Delay, Transform Target, float ObjectDelay, float ObjectSpeed)
	{
		yield return new WaitForSeconds (Delay);

		if(Target == null)
			Target = GameController.control.GuyLocation.transform;

		foreach (GameObject child in SpawnedObjects)
		{
			child.transform.parent = null;
//			Destroy(child, 8);
			child.GetComponent<Rigidbody2D>().velocity = (Target.position - child.transform.position).normalized * ObjectSpeed; 

			if(ObjectDelay != 0)
				yield return new WaitForSeconds (ObjectDelay);
		}
	}


////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stops all velocity on this gameobject
	// Useful for stopping the launch coroutines
	IEnumerator C_StopMovement(float Delay)
	{
		yield return new WaitForSeconds (Delay);

		GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		foreach (GameObject child in SpawnedObjects)
		{
			child.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		}
		yield return null;	
	}

	// Launches the object that this script is attached to, twords a target location
	// A null target will launch this at the player
	// Can be stopped by the C_StopMovement coroutine
	IEnumerator C_LaunchAtTarget(float Delay, Transform Target, float Speed)
	{
		yield return new WaitForSeconds (Delay);

		if(Target == null)
			Target = GameController.control.GuyLocation.transform;
		
		GetComponent<Rigidbody2D>().velocity = (Target.transform.position - gameObject.transform.position).normalized * Speed; 

		yield return null;	
	}

	// Launches the object that this script is attached to, in a targeted Vector3 direction
	// Can be stopped by the C_StopMovement coroutine
	IEnumerator C_LaunchInDirection(float StartDelay, Vector3 Target, float Speed)
	{
		yield return new WaitForSeconds(StartDelay);

		GetComponent<Rigidbody2D>().velocity = Target.normalized * Speed;

		yield return null;
	}

	// Move the object that this script is attached to, to a target spot
	// Delay - Start of delay
	// Target - Changes it to a vector3 location so if the target moves it will not change its target
	// Speed - Speed at which this object moves
	IEnumerator C_MoveToPosition(float Delay, Transform Target, float Speed, bool XAxis, bool YAxis)
	{
		yield return new WaitForSeconds (Delay);
		Speed = Mathf.Abs (Speed);

		if(Target == null)
			Target = GameController.control.GuyLocation.transform;

		Vector3 StartingPosition = transform.position;
		Vector3 TargetLocation = Target.position;

		float Duration = 0;
		float XPosition = transform.position.x;
		float YPosition = transform.position.y;

		if(Speed == 0)
			transform.position = TargetLocation;
		while (Duration < 1)
		{
			if(XAxis == true)
			{
				XPosition = Mathf.Lerp (StartingPosition.x, TargetLocation.x, Duration);
				transform.position = new Vector2 (XPosition, gameObject.transform.position.y);
			}
			if(YAxis == true)
			{
				YPosition = Mathf.Lerp (StartingPosition.y, TargetLocation.y, Duration);
				transform.position = new Vector2 (gameObject.transform.position.x, YPosition);
			}

			Duration += Time.deltaTime*Speed;
			yield return null;
		}
	}

/*
	// Old version of C_MoveToPosition
	IEnumerator C_MoveToPosition(float Delay, Transform Target, float Speed, bool XAxis, bool YAxis)
	{
		yield return new WaitForSeconds (Delay);
		Speed = Mathf.Abs (Speed);

		if(Target == null)
			Target = GameController.control.GuyLocation.transform;

		Vector3 TargetLocation = Target.position;

		if(XAxis == false)
			TargetLocation = new Vector3(gameObject.transform.position.x, TargetLocation.y, TargetLocation.z);
		if(YAxis == false)
			TargetLocation = new Vector3(TargetLocation.x, gameObject.transform.position.y, TargetLocation.z);

		if(Speed == 0)
			transform.position = TargetLocation;
		else
			while(Vector3.Distance(gameObject.transform.position, TargetLocation)>0.01)
			{
				transform.position = Vector3.MoveTowards(transform.position, TargetLocation, Speed * Time.deltaTime);
				yield return null;	
			}
		
		transform.position = TargetLocation;
	}

*/


////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////
	// Scales the object that this script is attached to (+/-)
	// Delay - Start of delay
	// ScaleChange - 1 = Normal size
	// Speed - how fast this change takes place
	// MaintainChildSize - If true - the child object will not change size.
	IEnumerator C_ChangeScale(float Delay, float ScaleChange, float Speed, bool MaintainChildSize)
	{
		yield return new WaitForSeconds (Delay);

		if(Speed ==0)
		{
			transform.localScale = new Vector3(ScaleChange, ScaleChange, 1);
		}
		else
		{
			if(ScaleChange > transform.localScale.x)
			{
				while(transform.localScale.x < ScaleChange)
				{
					transform.localScale += new Vector3(Speed*Time.deltaTime, Speed*Time.deltaTime, 0);

					if(MaintainChildSize)
					{
						foreach (GameObject child in SpawnedObjects)
						{
							child.transform.localScale = new Vector3(1/transform.localScale.x, 1/transform.localScale.y, 1);
						}
					}
					yield return null;	
				}
			}
			else
			{
				while(transform.localScale.x > ScaleChange)
				{
					transform.localScale -= new Vector3(Speed*Time.deltaTime, Speed*Time.deltaTime, 0);

					if(MaintainChildSize)
					{
						foreach (GameObject child in SpawnedObjects)
						{
							child.transform.localScale = new Vector3(1/transform.localScale.x, 1/transform.localScale.y, 1);
						}
					}
					yield return null;	
				}
			}
		}
	}

	// Stops all rotations based on the "ForeverRotate" boolean
	IEnumerator C_StopRotation(float Delay)
	{
		yield return new WaitForSeconds (Delay);

		ForeverRotate = false;
		yield return null;
	}

	// Rotates an object X degrees
	// Delay - Start of delay
	// RotateSpeed - (+/- determines the direction)
	// RotateDegrees - The object will rotate X degrees before stopping - If = 0, it will rotate forever
	IEnumerator C_RotateDegrees_Children(float Delay, float RotateSpeed, float RotateDegrees)
	{
		yield return new WaitForSeconds (Delay);
		
		for(int I=0; I<SpawnedObjects.Count; I++)
		{
			StartCoroutine(C_RotateDegrees(0, SpawnedObjects[I], RotateSpeed, RotateDegrees, false));
		}
	}
	IEnumerator C_RotateDegrees(float Delay, GameObject AGameObject, float RotateSpeed, float RotateDegrees, bool MaintainChildRotation)
	{
		yield return new WaitForSeconds (Delay);
		if(AGameObject == null)
			AGameObject = gameObject;

		if(RotateDegrees == 0)
			ForeverRotate = true;
		else
			RotateDegrees = Mathf.Abs(RotateDegrees);

		if(RotateSpeed == 0)
			AGameObject.transform.Rotate(0,0, AGameObject.transform.rotation.z+RotateDegrees);
		else
			while(RotateDegrees >= 0 || ForeverRotate)
			{
				RotateDegrees -= Mathf.Abs ((RotateSpeed * Time.deltaTime*90));

				if(RotateDegrees > RotateSpeed * Time.deltaTime || ForeverRotate)
					AGameObject.transform.Rotate(0,0, RotateSpeed * Time.deltaTime*90);
				else
				{
					if(RotateSpeed < 0)
						AGameObject.transform.Rotate(0,0, (RotateSpeed * Time.deltaTime*90) - RotateDegrees);
					else
						AGameObject.transform.Rotate(0,0, (RotateSpeed * Time.deltaTime*90) + RotateDegrees);
				}

				if(MaintainChildRotation)
				{
					foreach (GameObject child in SpawnedObjects)
					{
						if(child != null)
							child.transform.rotation = Quaternion.Euler(0,0,0);
					}
				}
				yield return null;
			}
		yield return null;
	}

	// Rotate an object to a certain degree
	// Delay - Start of Delay
	// Rotate Speed - (+/- determines the direction)
	// RotateToThisDegree - Rotates the object to this degree setting
	IEnumerator C_RotateToDegree_Children(float Delay, float RotateSpeed, float RotateToThisDegree)
	{
		yield return new WaitForSeconds (Delay);
		
		for(int I=0; I<SpawnedObjects.Count; I++)
		{
			StartCoroutine(C_RotateToDegree(0, SpawnedObjects[I], RotateSpeed, RotateToThisDegree, false));
		}
	}
	IEnumerator C_RotateToDegree(float Delay, GameObject AGameObject, float RotateSpeed, float RotateToThisDegree, bool MaintainChildRotation)
	{
		yield return new WaitForSeconds (Delay);
		if(AGameObject == null)
			AGameObject = gameObject;
		
		if(RotateSpeed == 0)
		{
			AGameObject.transform.rotation = Quaternion.Euler(0,0,RotateToThisDegree);
			if(MaintainChildRotation)
			{
				foreach (GameObject child in SpawnedObjects)
				{
					child.transform.rotation = Quaternion.Euler(0,0,0);
				}
			}
		}
		else
		{
			while((Quaternion.Angle (AGameObject.transform.localRotation, Quaternion.AngleAxis (RotateToThisDegree, Vector3.forward))) > 0)
			{
				//				float AngleDifference = (Quaternion.Angle (AGameObject.transform.rotation, Quaternion.AngleAxis (RotateHere, Vector3.forward)));
				//				print (AngleDifference);
				
				if((Quaternion.Angle (AGameObject.transform.localRotation, Quaternion.AngleAxis (RotateToThisDegree, Vector3.forward))) < Mathf.Abs(RotateSpeed))
					AGameObject.transform.localRotation = Quaternion.Euler(0,0,RotateToThisDegree);
				else
					AGameObject.transform.Rotate(0,0, RotateSpeed * Time.deltaTime*90);
				
				if(MaintainChildRotation)
				{
					foreach (GameObject child in SpawnedObjects)
					{
						if(child != null)
							child.transform.rotation = Quaternion.Euler(0,0,0);
					}
				}
				yield return null;
			}
		}
	}

	// Rotate this object twords target object
	// Delay - Delay of start
	// Rotate speed - Speed of lerping - If it is 0 then instant lerp
	// TargetObject - object to look at

	// Makes the children look at the target
	IEnumerator C_RotateTwordsObject_Children(float Delay, Transform TargetObject, float RotateSpeed)
	{
		yield return new WaitForSeconds (Delay);

		for(int I=0; I<SpawnedObjects.Count; I++)
		{
			StartCoroutine(C_RotateTwordsObject(0, SpawnedObjects[I], TargetObject, RotateSpeed, false));
		}
	}
	// Makes an individual object look at a target
	// If no gameobject is passed (null) then it assumes the main object the script is attached to is the gameobject
	IEnumerator C_RotateTwordsObject(float Delay, GameObject AGameObject, Transform TargetObject, float RotateSpeed, bool MaintainChildRotation)
	{
		yield return new WaitForSeconds (Delay);
		RotateSpeed = Mathf.Abs (RotateSpeed);

		// If no object to rotate is sent, make it the main object this script is attached to.
		if(AGameObject == null)
			AGameObject = gameObject;

		// If no object is targeted, it will target the player
		if(TargetObject == null)
		{
			TargetObject = GameController.control.GuyLocation.transform;
			ForeverRotate = true;
		}

		Vector3 pos = AGameObject.transform.position;
		Vector3 dir = TargetObject.position - pos;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		// Angle Differential between the two objects
		float AngleDifference = (Quaternion.Angle (AGameObject.transform.rotation, Quaternion.AngleAxis (angle, Vector3.forward)));

		// If no rotatespeed, then instant snap to the target (no lerp)
		if(RotateSpeed == 0)
		{
			AGameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		else
		{
			while((AngleDifference > 0) || ForeverRotate)
			{
				// Recheck rotational difference between now and where it wants to go
				pos = AGameObject.transform.position;
				dir = TargetObject.position - pos;
				angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				AngleDifference = (Quaternion.Angle (AGameObject.transform.rotation, Quaternion.AngleAxis (angle, Vector3.forward)));

				if(AngleDifference > Mathf.Abs(RotateSpeed/2))
					AGameObject.transform.rotation = Quaternion.Slerp(AGameObject.transform.rotation,  Quaternion.AngleAxis (angle, Vector3.forward), RotateSpeed*Time.deltaTime);
				else
					AGameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

				if(MaintainChildRotation)
				{
					foreach (GameObject child in SpawnedObjects)
					{
						if(child != null)
							child.transform.rotation = Quaternion.Euler(0,0,0);
					}
				}

				yield return null;
			}
		}
	}

	// Rotates the object that script is attached to around another target transform
	// Even if the other object moves, it will continues to rotate around it
	// Can only stop the rotation via the C_StopRotateAroundObject coroutine
	IEnumerator C_RotateAroundObject(float StartDelay, Transform RotateAroundThis, float Radius, float Speed)
	{
		yield return new WaitForSeconds(StartDelay);
		
		if (RotateAroundThis == null)
			RotateAroundThis = GameController.control.GuyLocation.transform;
		
		Vector3 pointPos;
		float i = 0;
		
		AllowRotateAroundObject = true;
		while(gameObject == true && AllowRotateAroundObject)
		{
			float pointNum = (i*1.0f)/360;
			float angle = pointNum*Mathf.PI*2;
			
			float x = Mathf.Sin (angle)*Radius;
			float y = Mathf.Cos (angle)*Radius;
			
			//			pointPos = new Vector3(x, y);
			//			CreateClone(true).transform.localPosition = pointPos;
			
			pointPos = new Vector3(x, y) + RotateAroundThis.position;
			pointPos = Vector3.Lerp (gameObject.transform.position, pointPos, Time.deltaTime*Speed);
			gameObject.transform.position = pointPos;
			
			i = i+Speed;
			if(i>360)
				i = i-360;
			
			yield return null;
		}
	}
	IEnumerator C_StopRotateAroundObject(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);
		
		AllowRotateAroundObject = false;		
	}

////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////
	// Create an object and set the variables
	// Will grab a pooled object if pooling is enabled
	GameObject CreateClone(bool AddToList)
	{
		GameObject newclone;

		if(EnablePooling)
		{
			newclone = GetPooledObject();
		}
		else
		{
			newclone = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
			if(newclone.GetComponent<Rigidbody2D>() == false)
				newclone.AddComponent <Rigidbody2D>();
			newclone.GetComponent<Rigidbody2D>().freezeRotation = true;
			if(KinematicObjects)
				newclone.GetComponent<Rigidbody2D>().isKinematic = true;

			NonPooledObjects.Add (newclone);
		}

		if(RandomColorSpawn == true)
			newclone.GetComponent<SpriteRenderer>().color = new Color (Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f), 1F);
		else
			newclone.GetComponent<SpriteRenderer>().color = SpawnColor;

		if(AddToList)
		{
			newclone.transform.parent = this.transform;
			SpawnedObjects.Add (newclone);

//			newclone.SetActive(false);
		}
		return newclone;
	}

	// Creates an X-Legged object (NumberOfLegs) of length (LengthOfLeg)
	// IsLocal - Tells the object to be created in local / global space
	// A locally created object will be created correctly even if this base object is rotating
	// A globally created object will use global coordinates and thus may be skewed during creation if the base object is rotating
	// globally creating a line for instance during rotation will create a spiral, while creating it locally will create a normal line
	IEnumerator C_CreateMultiLines(float StartDelay, float AppearDelay, int NumberOfLegs, float LengthOfLeg, int NumberOfObjectsPerLeg, bool IsLocal)
	{
		yield return new WaitForSeconds(StartDelay);
		
		Vector3 pointPos;
		
		for(int i = 0; i<NumberOfLegs; i++)
		{
			float pointNum = (i*1.0f)/NumberOfLegs;
			float angle = pointNum*Mathf.PI*2;
			
			float x = Mathf.Sin (angle);
			float y = Mathf.Cos (angle);
			
			pointPos = new Vector3(x, y) + transform.position;
			StartCoroutine (C_CreateLineInDirection(0, AppearDelay, NumberOfObjectsPerLeg, pointPos, LengthOfLeg, IsLocal));
		}
	}

	// Create a line from this point to the target
	IEnumerator C_CreateLineToTarget(float StartDelay, float AppearDelay, int NumberOfPoints, Transform Target, bool IsLocal)
	{
		yield return new WaitForSeconds (StartDelay);

		Vector3 pointPos;
		if(Target == null)
			Target = GameController.control.GuyLocation.transform;

		pointPos = Vector3.Lerp(transform.position, Target.position, 1);
		pointPos -= gameObject.transform.position;
		StartCoroutine (C_CreateLineInDirection(0, AppearDelay, NumberOfPoints, pointPos, Vector3.Distance(gameObject.transform.position, Target.position), IsLocal));

		// Remove below commented area later when I know this works
/*
		for(int i = 0; i<NumberOfPoints; i++)
		{
			float pointNum = (Vector3.Distance(gameObject.transform.position, Target.position))*(i*1.0f)/NumberOfPoints;

			pointPos = Vector3.Lerp(transform.position, Target.position, (i*1.0f)/NumberOfPoints);
//			pointPos = Vector3.MoveTowards(transform.position, Target.position, pointNum);
			GameObject TEMP = CreateClone(true);

			if(IsLocal)
				TEMP.transform.localPosition = pointPos - transform.position;
			else
				TEMP.transform.position = pointPos;					

			TEMP.SetActive (true);

			if(AppearDelay != 0)
				yield return new WaitForSeconds (AppearDelay);
		}
*/
		yield return null;
	}

	// Create a line in a vector 3 direction
	IEnumerator C_CreateLineInDirection(float StartDelay, float AppearDelay, int NumberOfPoints, Vector3 Target, float Length, bool IsLocal)
	{
		yield return new WaitForSeconds (StartDelay);
		Vector3 pointPos;

		for(int i = 0; i<NumberOfPoints; i++)
		{
			float pointNum = Length*(i*1.0f)/NumberOfPoints;

//			pointPos = ((Target).normalized*pointNum);
			pointPos = ((Target - gameObject.transform.position).normalized*pointNum)+transform.position;

			if(IsLocal)
			{
				pointPos = ((Target).normalized*pointNum);
				CreateClone(true).transform.localPosition = pointPos;
			}
			else
				CreateClone(true).transform.position = pointPos;

			if(AppearDelay != 0)
				yield return new WaitForSeconds (AppearDelay);
		}

		yield return null;
	}

	// Create a circle
	IEnumerator C_CreateCircle(float StartDelay, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, float Segmentation, bool IsLocal)
	{
		yield return new WaitForSeconds (StartDelay);
		Vector3 pointPos;

		for(int i = 0; i<NumberOfPoints; i++)
		{
			float pointNum = (i*1.0f)/NumberOfPoints;
			float angle = pointNum*Mathf.PI*2/Segmentation;

			float x = Mathf.Sin (angle)*X_Width;
			float y = Mathf.Cos (angle)*Y_Width;

			if(IsLocal)
			{
				pointPos = new Vector3(x, y);
				CreateClone(true).transform.localPosition = pointPos;
			}
			else
			{
				pointPos = new Vector3(x, y) + transform.position;
				CreateClone(true).transform.position = pointPos;
			}

			if(AppearDelay != 0)
				yield return new WaitForSeconds (AppearDelay);
		}

		yield return null;
	}

	// Create a square
	IEnumerator C_CreateSquare(float StartDelay, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, bool IsLocal)
	{
		yield return new WaitForSeconds (StartDelay);
		Vector3 pointPos;
		float pointNum = 0;

		int EvenObjects = NumberOfPoints / 4 * 4;

		for(int i = 0; i<NumberOfPoints/4; i++)
		{
			pointNum = -1f*X_Width + i*(4/(float)EvenObjects*2*X_Width);
			pointPos = new Vector3(pointNum, 1*Y_Width);

			if(IsLocal)
				CreateClone(true).transform.localPosition = pointPos;
			else
				CreateClone(true).transform.position = pointPos+transform.position;

			if(AppearDelay != 0)
				yield return new WaitForSeconds (AppearDelay);
		}

		for(int i = 0; i<NumberOfPoints/4; i++)
		{
			pointNum = 1f*Y_Width - i*(4/(float)EvenObjects*2*Y_Width);
			pointPos = new Vector3(1*X_Width, pointNum);
			if(IsLocal)
				CreateClone(true).transform.localPosition = pointPos;
			else
				CreateClone(true).transform.position = pointPos+transform.position;

			if(AppearDelay != 0)
				yield return new WaitForSeconds (AppearDelay);
		}

		for(int i = 0; i<NumberOfPoints/4; i++)
		{
			pointNum = 1f*X_Width - i*(4/(float)EvenObjects*2*X_Width);
			pointPos = new Vector3(pointNum, -1*Y_Width);
			if(IsLocal)
				CreateClone(true).transform.localPosition = pointPos;
			else
				CreateClone(true).transform.position = pointPos+transform.position;

			if(AppearDelay != 0)
				yield return new WaitForSeconds (AppearDelay);
		}

		for(int i = 0; i<NumberOfPoints/4; i++)
		{
			pointNum = -1f*Y_Width + i*(4/(float)EvenObjects*2*Y_Width);
			pointPos = new Vector3(-1*X_Width, pointNum);
			if(IsLocal)
				CreateClone(true).transform.localPosition = pointPos;
			else
				CreateClone(true).transform.position = pointPos+transform.position;

			if(AppearDelay != 0)
				yield return new WaitForSeconds (AppearDelay);
		}

		while(SpawnedObjects.Count < NumberOfPoints)
		{
			pointPos = new Vector3(-1*X_Width, pointNum);
			if(IsLocal)
				CreateClone(true).transform.localPosition = pointPos;
			else
				CreateClone(true).transform.position = pointPos+transform.position;

			if(AppearDelay != 0)
				yield return new WaitForSeconds (AppearDelay);
		}

		yield return null;
	}

	// Create a triangle
	IEnumerator C_CreateTriangle(float StartDelay, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, bool IsLocal)
	{
		yield return new WaitForSeconds (StartDelay);

		Vector3 pointPos;
		float pointNum = 0;
		float pointNum2 = 0;
		int EvenObjects = NumberOfPoints / 3 * 3;
		
		for(int i = 0; i<NumberOfPoints/3; i++)
		{
			pointNum = -1f*X_Width + i*(3/(float)EvenObjects*2*X_Width);
			pointPos = new Vector3(pointNum, -1*Y_Width);

			if(IsLocal)
				CreateClone(true).transform.localPosition = pointPos;
			else
				CreateClone(true).transform.position = pointPos+transform.position;

			if(AppearDelay != 0)
				yield return new WaitForSeconds (AppearDelay);
		}
		
		for(int i = 0; i<NumberOfPoints/3; i++)
		{
			pointNum = -1f*Y_Width + i*(3/(float)EvenObjects*2*Y_Width);
			pointNum2 = 1f*X_Width - i*(3/(float)EvenObjects*X_Width);
			pointPos = new Vector3(pointNum2, pointNum);

			if(IsLocal)
				CreateClone(true).transform.localPosition = pointPos;
			else
				CreateClone(true).transform.position = pointPos+transform.position;

			if(AppearDelay != 0)
				yield return new WaitForSeconds (AppearDelay);
		}

		for(int i = 0; i<NumberOfPoints/3; i++)
		{
			pointNum = 1f*Y_Width - i*(3/(float)EvenObjects*2*Y_Width);
			pointNum2 = 0 - i*(3/(float)EvenObjects*X_Width);
			pointPos = new Vector3(pointNum2, pointNum);

			if(IsLocal)
				CreateClone(true).transform.localPosition = pointPos;
			else
				CreateClone(true).transform.position = pointPos+transform.position;

			if(AppearDelay != 0)
				yield return new WaitForSeconds (AppearDelay);
		}

		while(SpawnedObjects.Count < NumberOfPoints)
		{
			pointPos = new Vector3(pointNum2, pointNum);

			if(IsLocal)
				CreateClone(true).transform.localPosition = pointPos;
			else
				CreateClone(true).transform.position = pointPos+transform.position;

			if(AppearDelay != 0)
				yield return new WaitForSeconds (AppearDelay);
		}

		yield return null;
	}
////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////
	// Function calls that call the coroutines
	// This is for simplicity when using this script from another script
	// This will make it so that all coroutines are ran from the object with the script on it and
	// not the script that called the function

	public void CreateCircle(float Delay, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, float Segmentation)
	{
		StartCoroutine (C_CreateCircle(Delay, AppearDelay, NumberOfPoints, X_Width, Y_Width, Segmentation, true));
	}
	public void CreateCircle_World(float Delay, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, float Segmentation)
	{
		StartCoroutine (C_CreateCircle(Delay, AppearDelay, NumberOfPoints, X_Width, Y_Width, Segmentation, false));
	}
	public void CreateSquare(float Delay, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width)
	{
		StartCoroutine (C_CreateSquare(Delay, AppearDelay, NumberOfPoints, X_Width, Y_Width, true));
	}
	public void CreateSquare_World(float Delay, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width)
	{
		StartCoroutine (C_CreateSquare(Delay, AppearDelay, NumberOfPoints, X_Width, Y_Width, false));
	}
	public void CreateTriangle(float Delay, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width)
	{
		StartCoroutine (C_CreateTriangle (Delay, AppearDelay, NumberOfPoints, X_Width, Y_Width, true));
	}
	public void CreateTriangle_World(float Delay, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width)
	{
		StartCoroutine (C_CreateTriangle (Delay, AppearDelay, NumberOfPoints, X_Width, Y_Width, false));
	}

	public void CreateLineInDirection(float Delay, float AppearDelay, int NumberOfPoints, Vector3 Target, float Length)
	{
		StartCoroutine (C_CreateLineInDirection (Delay, AppearDelay, NumberOfPoints, Target, Length, true));
	}
	public void CreateLineInDirection_World(float Delay, float AppearDelay, int NumberOfPoints, Vector3 Target, float Length)
	{
		StartCoroutine (C_CreateLineInDirection (Delay, AppearDelay, NumberOfPoints, Target, Length, false));
	}
	public void CreateMultiLines(float StartDelay, float AppearDelay, int NumberOfLegs, float LengthOfLeg, int NumberOfObjectsPerLeg)
	{
		StartCoroutine (C_CreateMultiLines(StartDelay, AppearDelay, NumberOfLegs, LengthOfLeg, NumberOfObjectsPerLeg, true));
	}
	public void CreateMultiLines_World(float StartDelay, float AppearDelay, int NumberOfLegs, float LengthOfLeg, int NumberOfObjectsPerLeg)
	{
		StartCoroutine (C_CreateMultiLines(StartDelay, AppearDelay, NumberOfLegs, LengthOfLeg, NumberOfObjectsPerLeg, false));
	}

	public void  CreateLineToTarget(float Delay, float AppearDelay, int NumberOfPoints, Transform Target)
	{
		StartCoroutine (C_CreateLineToTarget (Delay, AppearDelay, NumberOfPoints, Target, true));
	}
	public void  CreateLineToTarget_Player(float Delay, float AppearDelay, int NumberOfPoints)
	{
		StartCoroutine (C_CreateLineToTarget (Delay, AppearDelay, NumberOfPoints, null, true));
	}
	public void  CreateLineToTarget_World(float Delay, float AppearDelay, int NumberOfPoints, Transform Target)
	{
		StartCoroutine (C_CreateLineToTarget (Delay, AppearDelay, NumberOfPoints, Target, false));
	}
	public void  CreateLineToTarget_Player_World(float Delay, float AppearDelay, int NumberOfPoints)
	{
		StartCoroutine (C_CreateLineToTarget (Delay, AppearDelay, NumberOfPoints, null, false));
	}

	//

	public void RotateToDegree(float Delay, float RotateSpeed, float RotateToThisDegree, bool MaintainChildRotation)
	{
		StartCoroutine (C_RotateToDegree(Delay, null, RotateSpeed, RotateToThisDegree, MaintainChildRotation));
	}
	public void RotateToDegree(float Delay, GameObject AGameObject, float RotateSpeed, float RotateToThisDegree, bool MaintainChildRotation)
	{
		StartCoroutine (C_RotateToDegree(Delay, AGameObject, RotateSpeed, RotateToThisDegree, MaintainChildRotation));
	}
	public void RotateToDegree_Children(float Delay, float RotateSpeed, float RotateToThisDegree)
	{
		StartCoroutine (C_RotateToDegree_Children(Delay, RotateSpeed, RotateToThisDegree));
	}


	public void RotateTwordsObject(float Delay, Transform TargetObject, float RotateSpeed, bool MaintainChildRotation)
	{
		StartCoroutine (C_RotateTwordsObject(Delay, null, TargetObject, RotateSpeed, MaintainChildRotation));
	}
	public void RotateTwordsObject(float Delay, GameObject MainObject, Transform TargetObject, float RotateSpeed, bool MaintainChildRotation)
	{
		StartCoroutine (C_RotateTwordsObject(Delay, MainObject, TargetObject, RotateSpeed, MaintainChildRotation));
	}

	public void RotateTwordsObject_Player(float Delay, float RotateSpeed, bool MaintainChildRotation)
	{
		StartCoroutine (C_RotateTwordsObject(Delay, null, null, RotateSpeed, MaintainChildRotation));
	}
	public void RotateTwordsObject_Player(float Delay, GameObject MainObject, float RotateSpeed, bool MaintainChildRotation)
	{
		StartCoroutine (C_RotateTwordsObject(Delay, MainObject, null, RotateSpeed, MaintainChildRotation));
	}

	public void RotateTwordsObject_Children(float Delay, Transform TargetObject, float RotateSpeed)
	{
		StartCoroutine (C_RotateTwordsObject_Children(Delay, TargetObject, RotateSpeed));
	}
	public void RotateDegrees(float Delay, float RotateSpeed, float RotateDegrees, bool MaintainChildRotation)
	{
		StartCoroutine (C_RotateDegrees(Delay, null, RotateSpeed, RotateDegrees, MaintainChildRotation));
	}
	public void RotateDegrees(float Delay, GameObject MainObject, float RotateSpeed, float RotateDegrees, bool MaintainChildRotation)
	{
		StartCoroutine (C_RotateDegrees(Delay, MainObject, RotateSpeed, RotateDegrees, MaintainChildRotation));
	}


	public void RotateDegrees_Children(float Delay, float RotateSpeed, float RotateDegrees)
	{
		StartCoroutine (C_RotateDegrees_Children(Delay, RotateSpeed, RotateDegrees));
	}
	public void StopRotation(float Delay)
	{
		StartCoroutine (C_StopRotation(Delay));
	}

	public void RotateAroundObject(float StartDelay, Transform RotateAroundThis, float Radius, float Speed)
	{
		StartCoroutine (C_RotateAroundObject(StartDelay, RotateAroundThis, Radius, Speed));
	}
	public void RotateAroundObject_Player(float StartDelay, float Radius, float Speed)
	{
		StartCoroutine (C_RotateAroundObject(StartDelay, null, Radius, Speed));
	}
	public void StopRotateAroundObject(float StartDelay)
	{
		StartCoroutine (C_StopRotateAroundObject(StartDelay));
	}
	
	//

	public void ChangeScale(float Delay, float ScaleChange, float Speed, bool MaintainChildSize)
	{
		StartCoroutine (C_ChangeScale(Delay, ScaleChange, Speed, MaintainChildSize));
	}



	public void MoveObjectToPoint(float Delay, Vector3 Target, float Speed)
	{
		StartCoroutine (C_MoveObjectToPoint(Delay, null, Target, Speed));
	}
	public void MoveObjectToPoint(float Delay, Transform Target, float Speed)
	{
		StartCoroutine (C_MoveObjectToPoint(Delay, null, Target.position, Speed));
	}
	public void MoveObjectToPoint(float Delay, GameObject ObjectToMove, Vector3 Target, float Speed)
	{
		StartCoroutine (C_MoveObjectToPoint(Delay, ObjectToMove, Target, Speed));
	}
	public void MoveObjectToPoint(float Delay, GameObject ObjectToMove, Transform Target, float Speed)
	{
		StartCoroutine (C_MoveObjectToPoint(Delay, ObjectToMove, Target.position, Speed));
	}


	public void MoveToPosition(float Delay, Transform Target, float Speed)
	{
		StartCoroutine (C_MoveToPosition(Delay, Target, Speed, true, true));
	}
	public void MoveToPosition_X(float Delay, Transform Target, float Speed)
	{
		StartCoroutine (C_MoveToPosition(Delay, Target, Speed, true, false));
	}
	public void MoveToPosition_Y(float Delay, Transform Target, float Speed)
	{
		StartCoroutine (C_MoveToPosition(Delay, Target, Speed, false, true));
	}
	public void MoveToPosition_Player(float Delay, float Speed)
	{
		StartCoroutine (C_MoveToPosition(Delay, null, Speed, true, true));
	}
	public void MoveToPosition_Player_X(float Delay, float Speed)
	{
		StartCoroutine (C_MoveToPosition(Delay, null, Speed, true, false));
	}
	public void MoveToPosition_Player_Y(float Delay, float Speed)
	{
		StartCoroutine (C_MoveToPosition(Delay, null, Speed, false, true));
	}

	public void LaunchInDirection(float StartDelay, Vector3 Target, float Speed)
	{
		StartCoroutine (C_LaunchInDirection(StartDelay, Target, Speed));
	}

	public void LaunchAtTarget(float Delay, Transform Target, float Speed)
	{
		StartCoroutine (C_LaunchAtTarget(Delay, Target, Speed));
	}
	public void LaunchAtTarget_Player(float Delay, float Speed)
	{
		StartCoroutine (C_LaunchAtTarget(Delay, null, Speed));
	}

	public void StopMovement(float Delay)
	{
		StartCoroutine (C_StopMovement(Delay));
	}

	//

	public void WaveEffectObject(float StartDelay, GameObject child, float I, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y)
	{
		StartCoroutine (C_WaveEffectObject(StartDelay, child, I, Amplitude_X, PeriodSpeed_X, Amplitude_Y, PeriodSpeed_Y));	
	}
	public void WaveEffectObject(float StartDelay, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y)
	{
		StartCoroutine (C_WaveEffectObject(StartDelay, gameObject, 1, Amplitude_X, PeriodSpeed_X, Amplitude_Y, PeriodSpeed_Y));	
	}
	
	public void WaveEffect_children(float StartDelay, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y)
	{
		StartCoroutine (C_WaveEffect_Children(StartDelay, Amplitude_X, PeriodSpeed_X, Amplitude_Y, PeriodSpeed_Y));
	}

	public void StopWaveEffect(float StartDelay)
	{
		StartCoroutine (C_StopWaveEffect (StartDelay));
	}

	public void MorphToCircle(float StartDelay, float X_Width, float Y_Width,  float ObjectDelay, float Speed)
	{
		StartCoroutine (C_MorphToCircle(StartDelay, X_Width, Y_Width,  ObjectDelay, Speed));	
	}

	public void MorphToSquare(float StartDelay, float X_Width, float Y_Width, float ObjectDelay, float Speed)
	{
		StartCoroutine (C_MorphToSquare(StartDelay, X_Width, Y_Width, ObjectDelay, Speed));
	}

	//

	public void BreakRandomly(float Delay, float ObjectDelay, float ObjectSpeed)
	{
		StartCoroutine (C_BreakRandomly(Delay, ObjectDelay, ObjectSpeed));
	}
	
	public void BreakToTarget(float Delay, Transform Target, float ObjectDelay, float ObjectSpeed)
	{
		StartCoroutine (C_BreakToTarget(Delay, Target, ObjectDelay, ObjectSpeed));
	}
	public void BreakToTarget_Self(float Delay, float ObjectDelay, float ObjectSpeed)
	{
		StartCoroutine (C_BreakToTarget(Delay, gameObject.transform, ObjectDelay, ObjectSpeed));
	}
	public void BreakToTarget_Player(float Delay, float ObjectDelay, float ObjectSpeed)
	{
		StartCoroutine (C_BreakToTarget(Delay, null, ObjectDelay, ObjectSpeed));
	}
	
	public void BreakCircleBurst(float Delay, int CircleBurstSize, float ObjectDelay, float ObjectSpeed)
	{
		StartCoroutine (C_BreakCircleBurst(Delay, CircleBurstSize, ObjectDelay, ObjectSpeed));
	}

	public void CircleBurst(float StartDelay, Transform TargetLocation, int NumberOfPoints, float ObjectSpeed)
	{
		StartCoroutine (C_CircleBurst (StartDelay, TargetLocation, NumberOfPoints, ObjectSpeed));
	}
	public void CircleBurst(float StartDelay, int NumberOfPoints, float ObjectSpeed)
	{
		StartCoroutine (C_CircleBurst (StartDelay, gameObject.transform, NumberOfPoints, ObjectSpeed));
	}
	


	public void WaveSpray_Horizontal(float StartDelay, int NumberOfObjects, float ObjectDelay, float Amplitude, float PeriodSpeed, float Speed)
	{
		StartCoroutine (C_WaveSpray(StartDelay, NumberOfObjects, ObjectDelay, new Vector2(1,0), 0,0, Amplitude, PeriodSpeed, Speed));
	}
	public void WaveSpray_Vertical(float StartDelay, int NumberOfObjects, float ObjectDelay, float Amplitude, float PeriodSpeed, float Speed)
	{
		StartCoroutine (C_WaveSpray(StartDelay, NumberOfObjects, ObjectDelay, new Vector2(0,1), Amplitude, PeriodSpeed, 0,0, Speed));
	}
	public void ShootDirection(float StartDelay, int ObjectAmount, Vector3 Target, float ObjectDelay, float ObjectSpeed)
	{
		StartCoroutine(C_ShootDirection (StartDelay, ObjectAmount, Target, ObjectDelay, ObjectSpeed));
	}
	public void FollowTarget(float StartDelay, Transform Target, float DeathTimer, float ObjectSpeed)
	{
		StartCoroutine(C_FollowTarget (StartDelay, Target, DeathTimer, ObjectSpeed));		
	}
	public void FollowTarget_Player(float StartDelay, float DeathTimer, float ObjectSpeed)
	{
		StartCoroutine(C_FollowTarget (StartDelay, null, DeathTimer, ObjectSpeed));		
	}
	public void ShootFromTopScreen(float StartDelay, int ObjectAmount, float ObjectDelay, float ObjectSpeed)
	{
		StartCoroutine (C_ShootFromTopScreen(StartDelay, ObjectAmount, ObjectDelay, ObjectSpeed));
	}
	public void ShootFromRightScreen(float StartDelay, int ObjectAmount, float ObjectDelay, float ObjectSpeed)
	{
		StartCoroutine (C_ShootFromRightScreen(StartDelay, ObjectAmount, ObjectDelay, ObjectSpeed));		
	}
	public void SprayInDirection(float StartDelay, int ObjectAmount, Vector3 Target, float ObjectDelay, float ObjectSpeed)
	{
		StartCoroutine(C_SprayInDirection (StartDelay, ObjectAmount, Target, ObjectDelay, ObjectSpeed));	
	}
	public void RandomSprayOutwards(float StartDelay, float AppearDelay, int NumberOfObjects, float ObjectSpeed)
	{
		StartCoroutine (C_RandomSprayOutwards(StartDelay, AppearDelay, NumberOfObjects, ObjectSpeed));
	}
	public void ShootAtTarget(float StartDelay, float ObjectDelay, int ObjectAmount, Transform Target, float ObjectSpeed)
	{
		StartCoroutine(C_ShootAtTarget (StartDelay, ObjectDelay, ObjectAmount, Target, ObjectSpeed));		
	}
	public void ShootAtTarget_Player(float StartDelay, float ObjectDelay, int ObjectAmount, float ObjectSpeed)
	{
		StartCoroutine(C_ShootAtTarget (StartDelay, ObjectDelay, ObjectAmount, null, ObjectSpeed));		
	}
	public void RandomSprayBounce(float StartDelay, float AppearDelay, int NumberOfObjects, float ObjectSpeed)
	{
		StartCoroutine (C_RandomSprayBounce(StartDelay, AppearDelay, NumberOfObjects, ObjectSpeed));
	}


	public void ChangeColor(float StartDelay, Color NewColor, float Speed)
	{
		StartCoroutine (C_ChangeColor(StartDelay, gameObject, NewColor, Speed, false));
	}
	public void ChangeColor(float StartDelay, GameObject gameobject, Color NewColor, float Speed)
	{
		StartCoroutine (C_ChangeColor(StartDelay, gameobject, NewColor, Speed, false));
	}
	public void ChangeColor_Random(float StartDelay, float Speed)
	{
		Color NewColor = new Color (Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f), gameObject.GetComponent<SpriteRenderer>().color.a);
		StartCoroutine (C_ChangeColor (StartDelay, gameObject, NewColor, Speed, true));
	}
	public void ChangeColor_Children(float StartDelay, float Speed)
	{
		StartCoroutine (C_ChangeColor_Children(StartDelay, Speed));
	}
	public void ChangeColor_Children(float StartDelay, Color NewColor, float Speed)
	{
		StartCoroutine (C_ChangeColor_Children(StartDelay, NewColor, Speed));
	}
	public void StopColorChangeEffect(float StartDelay)
	{
		StartCoroutine (C_StopColorChangeEffect (StartDelay));
	}
	public void FadeObject(float StartDelay, GameObject AGameObject, float Speed)
	{
		StartCoroutine (C_FadeObject (StartDelay, AGameObject, Speed));
	}
	public void FadeObject_Children(float StartDelay, float Speed)
	{
		StartCoroutine (C_FadeObject_Children(StartDelay, Speed));
	}
	public void UnfadeObject(float StartDelay, GameObject AGameObject, float Speed)
	{
		StartCoroutine (C_UnfadeObject (StartDelay, AGameObject, Speed));
	}
	public void UnfadeObject_Children(float StartDelay, float Speed)
	{
		StartCoroutine (C_UnfadeObject_Children(StartDelay, Speed));
	}
	public void DestroyChildren(float StartDelay, float ObjectDelay)
	{
		StartCoroutine (C_DestroyChildren(StartDelay, ObjectDelay));
	}
	public void ReattachChildren(float StartDelay)
	{
		StartCoroutine (C_ReattachChildren (StartDelay));
	}


	public void SetToEnablePooling(float StartDelay, bool enablePooling)
	{
		StartCoroutine (C_SetToEnablePooling (StartDelay, enablePooling));
	}
	public void SetDeathTimer(float StartDelay, float DeathTimer)
	{
		StartCoroutine (C_SetDeathTimer (StartDelay, DeathTimer));
	}
	public void SetKinematicObjects(float StartDelay, bool areObjectsKinematic)
	{
		StartCoroutine (C_SetKinematicObjects(StartDelay, areObjectsKinematic));
	}
	public void SetProjectile(float StartDelay, GameObject ProjectileSet)
	{
		StartCoroutine (C_SetProjectile(StartDelay, ProjectileSet));
	}
	public void SetColor (float StartDelay, Color newColor)
	{
		StartCoroutine (C_SetColor (StartDelay, newColor));
	}
	public void SetColor_Random (float StartDelay)
	{
		StartCoroutine (C_SetColor_Random (StartDelay));
	}
	public void EnableObject(float StartDelay, GameObject ObjectToEnable)
	{
		StartCoroutine (C_EnableObject (StartDelay, ObjectToEnable));
	}
	public void DisableObject(float StartDelay, GameObject ObjectToDisable)
	{
		StartCoroutine (C_DisableObject (StartDelay, ObjectToDisable));
	}
	public void PlayClip(float StartDelay, AudioClip ClipToPlay)
	{
		StartCoroutine (C_PlayClip(StartDelay, ClipToPlay));
	}
}