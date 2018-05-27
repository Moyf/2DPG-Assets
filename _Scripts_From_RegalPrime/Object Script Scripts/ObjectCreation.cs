// RegalPrime 5-22-15 - ObjectCreation.cs

// This script has deals with creating objects of various shapes and sizes
// Below are listed all the base coroutines that are available. There are public functions that call these coroutines for easy of use.

// See TestNewObjectScript.cs for examples on how to call these functions

/*
public void ResetObjectSpawner()

public GameObject CreateClone(bool AddAsChild)
GameObject GetPooledObject()
Destroy_GameObject(GameObject ObjectToDestroy, float ObjectDeathTimer)
C_PrePoolObjects(float StartDelay, int AmountToPrePool)

C_SetEnablePooling(float StartDelay, bool enablePooling)
C_SetProjectile(float StartDelay, GameObject ProjectileSet)
C_SetColor (float StartDelay, Color newColor)
C_SetColor_Random (float StartDelay)
C_SetSpawnScale(float StartDelay, Vector3 NewSpawnScale)
C_SetKinematic(float StartDelay, bool IsKinematic)
C_SetGravityScale(float StartDelay, float newGravityScale)

IEnumerable<Vector3> Calculate_Line(Vector3 StartingPoint, Vector3 EndingPoint, int NumberOfPoints, float BendMultiplier, int EndingPoint_Indent)
IEnumerable<Vector3> Calculate_Circle(int NumberOfPoints, float X_Width, float Y_Width, int PointSkip, float Segmentation)
IEnumerable<Starting_EndingPoint> Calculate_Square(int ObjectsPerLeg, float X_Width, float Y_Width)
IEnumerable<Starting_EndingPoint> Calculate_Polygon(int NumberOfPoints, int LineDrawSkip, float X_Width, float Y_Width)
IEnumerable<Vector3> Calculate_PolarCurve(int NumberOfPoints, float X_Width, float Y_Width, PolarCurveFormula PolarCurve, float Multiplier)

C_CreateLine_ToFrom(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, GameObject EndingObject, Vector3 EndingPoint, float AppearDelay, int NumberOfPoints, float BendMultiplier, int EndingPoint_Indent, bool IsLocal)
C_CreateLine_InDirection(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, Vector3 TargetDirection, float AppearDelay, int NumberOfPoints, float Length, float BendMultiplier, int EndingPoint_Indent, bool IsLocal)
C_CreateLine_MultiLines(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfLegs, float LengthOfLeg, int ObjectsPerLeg, float BendMultiplier, int EndingPoint_Indent, bool IsLocal)

C_CreateShape_Circle(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, float Segmentation, bool IsLocal)
C_CreateShape_Square(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int ObjectsPerLeg, float BendMultiplier, float X_Width, float Y_Width, bool LegDelay, bool IsLocal)
C_CreateShape_Triangle(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int ObjectsPerLeg, float BendMultiplier, float X_Width, float Y_Width, bool LegDelay, bool IsLocal)
C_CreateShape_Polygon(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, int ObjectsPerLeg, int LineDrawSkip, float IndentPercent, float BendMultiplier, float X_Width, float Y_Width, bool LegDelay, bool IsLocal)
C_CreateShape_PolarCurve(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, bool IsLocal, PolarCurveFormula APolarCurve, float Multiplier)
CreateShape_PolarCurve_abcoskt(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, float Value_A, float Value_B, float Value_K_Top, float Value_K_Bottom, float X_Width, float Y_Width, bool IsLocal)
CreateShape_PolarCurve_Heart(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, bool IsLocal)
CreateShape_PolarCurve_Spiral(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, float PiMultiplier, bool IsLocal)

C_MorphTo_Line_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject StartingObject, Vector3 StartingPoint, GameObject EndingObject, Vector3 EndingPoint, float ObjectDelay, float BendMultiplier, int EndingPoint_Indent, float inSeconds)
C_MorphTo_Circle_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float inSeconds)
C_MorphTo_Square_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float BendMultiplier, float X_Width, float Y_Width, float inSeconds)
C_MorphTo_Polygon_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, int NumberOfPoints, int LineDrawSkip, float IndentPercent, float BendMultiplier, float X_Width, float Y_Width, float inSeconds)
C_MorphTo_PolarCurve_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float inSeconds, PolarCurveFormula APolarCurve, float Multiplier)
MorphTo_PolarCurve_abcoskt_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float Value_A, float Value_B, float Value_K_Top, float Value_K_Bottom, float X_Width, float Y_Width, float inSeconds)
MorphTo_PolarCurve_Heart_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float inSeconds)
MorphTo_PolarCurve_Spiral_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float PiMultiplier, float inSeconds)

C_Destroy_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay)
C_Destroy_GameObjectList_CircleBurst(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, int NumberOfPoints, float ObjectSpeed, float ObjectDeathTimer)
C_ChangeAlpha_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float AlphaValue, float inSeconds)
C_ChangeColor_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, Color NewColor, float inSeconds, bool ChangeForever)
C_ChangeScale_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float X_Scale, float Y_Scale, float inSeconds)
C_ChangeKinematicState_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, bool isKinematic)
C_ChangeGravity_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float newGravityScale)

C_WaveEffectObject_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y, bool LerpMovement)
C_BreakToTarget_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float ObjectSpeed)
C_BreakRandomly_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float ObjectSpeed)
C_StopVelocity_GameObjectList(float StartDelay, List<GameObject> GameObjectList)

C_RotateDegrees_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float RotateDegree, float inSeconds)
C_RotateTwordsPoint_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float RotateSpeed, bool ForeverRotateTwords)

C_Shoot_AtTarget(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, GameObject TargetObject, Vector3 TargetPoint,  float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
C_Shoot_InDirection(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, Vector3 TargetDirection, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float SprayVariance, float ObjectDeathTimer)
C_Shoot_FollowTarget (float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, GameObject TargetObject, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
C_Shoot_RandomSprayOutwards(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
C_Shoot_CircularSpray(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, Vector3 TargetDirection, int NumberOfObjectsPerSpawn, float DelayBetweenObjectSpawn, float ObjectSpeed, float DegreeChange, float DegreeCap, int NumberOfObjects, float AppearDelay, float ObjectDeathTimer)
C_Shoot_WaveSpray(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, Vector2 TargetDirection, float AppearDelay, int NumberOfObjects, float Speed, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y, bool LerpMovement, float ObjectDeathTimer)
C_Shoot_FromScreenEdge(float StartDelay, Vector3 Direction, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
C_Shoot_CircleBurst(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, int NumberOfPoints, float ObjectSpeed, float ObjectDeathTimer)

*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Calculates the radius of a polar curve taking t = theta
// Examples in Create_PolarCurve_Heart and Create_PolarCurve_abcoskt
public class ObjectCreation : ObjectManipulation
{
	public delegate float PolarCurveFormula(float t);

	List<GameObject> Projectile;		// The object used when spawning objects through this script


	private Color SpawnColor = Color.white;

	private bool RandomColorSpawn = true;
	private bool UseDefaultColor = true;									// Will use the projectiles color
	private bool UseDefaultGravity = true;

	private Vector3 SpawnScale;
	private bool UseDefaultSpawnScale = true;

	private bool KinematicObjects = true;
	private float ObjectsGravityScale = 1f;


	public bool EnablePooling = true;										// Should we reuse our projectiles?


	private PhysicsMaterial2D BouncyMaterial;


	public List<GameObject> AttachedObjects = new List<GameObject>();		// Objects that are attached to this script

	public List<GameObject> PooledObjects = new List<GameObject>();			// List of pooled objects
	public List<GameObject> NonPooledObjects = new List<GameObject> ();		// List of non-pooled objects (used in resetting the script early)

	private int NonPooledObjectCount = 0;									// This is just to keep tract and name each consecutive clone that spawns

	void Start()
	{
		BouncyMaterial = new PhysicsMaterial2D ();
		BouncyMaterial.bounciness = 1f;
		BouncyMaterial.friction = 0f;
	}
	public void ResetObjectSpawner()
	{
		StopAllCoroutines ();						// Stops all Coroutines
		
		for(int I=0; I<PooledObjects.Count; I++)	// Resetting all pooled objects 
		{
			StartCoroutine (Destroy_GameObject(PooledObjects[I], 0));
		}

		for(int I=0; I<NonPooledObjects.Count; I++)	// Destroying all non-pooled objects
		{
			Destroy (NonPooledObjects[I]);
		}
		
		NonPooledObjects.Clear ();					// Clear the list
		AttachedObjects.Clear ();					// Clear the list
	}

	public struct Starting_EndingPoint
	{
		public Vector3 StartingPoint;
		public Vector3 EndingPoint;
	}










// CLONE CREATION ---------------------------------------------------------------------------------------------------------------------------

	// AddToList = true will add objects created to the AttachedObjects List. These objects can then be manipluated by other scripts
	public GameObject CreateClone(bool AddAsChild)
	{
		GameObject newclone;

		if(EnablePooling)
		{
			newclone = GetPooledObject();
		}
		else
		{
			newclone = Instantiate(GetGameObjectToSpawn(), transform.position, Quaternion.identity) as GameObject;
			NonPooledObjectCount ++;
			newclone.name = gameObject.name + " - clone - "+NonPooledObjectCount;

			if(newclone.GetComponent<Rigidbody2D>() == false)
			{
				newclone.AddComponent <Rigidbody2D>();
//				newclone.GetComponent <Rigidbody2D>().gravityScale = ObjectsGravityScale;
			}

			NonPooledObjects.Add(newclone);
		}

		// Have an object (either new of repooled). Change object settings.
		newclone.GetComponent<Rigidbody2D>().freezeRotation = true;

		if(UseDefaultGravity == false)
			newclone.GetComponent<Rigidbody2D>().gravityScale = ObjectsGravityScale;

		newclone.GetComponent<Rigidbody2D>().isKinematic = KinematicObjects;

		if(UseDefaultColor == false)
		{
			if(RandomColorSpawn == true)
				newclone.GetComponent<SpriteRenderer>().color = new Color (Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f), 1F);
			else
				newclone.GetComponent<SpriteRenderer>().color = SpawnColor;
		}

		if(UseDefaultSpawnScale == false)
			newclone.transform.localScale = SpawnScale;
		
		if(AddAsChild)
		{
			newclone.transform.parent = this.transform;
			newclone.transform.localRotation = Quaternion.identity;
			AttachedObjects.Add (newclone);
		}

		return newclone;
	}
	GameObject GetPooledObject()
	{
		foreach (GameObject child in PooledObjects)			// Look for a pooled object to use
		{
			if(!child.activeInHierarchy)
			{
				child.transform.position = gameObject.transform.position;
				child.SetActive(true);
				return child;
			}
		}
		
		// Couldnt find one, so make a new one and add it to the end of the list
		GameObject newclone;
		
		newclone = Instantiate(GetGameObjectToSpawn(), transform.position, Quaternion.identity) as GameObject;
		newclone.name = gameObject.name + " - clone_pooled - "+PooledObjects.Count;
		if(newclone.GetComponent<Rigidbody2D>() == false)
			newclone.AddComponent <Rigidbody2D>();
	
		PooledObjects.Add (newclone);
		return newclone;
	}

	GameObject GetGameObjectToSpawn()
	{
		int randomPosition = (int)Random.Range (0f, (Projectile.Count));
		return Projectile[randomPosition];
	}

	IEnumerator Destroy_GameObject(GameObject ObjectToDestroy, float ObjectDeathTimer)	// "Destroy" an object after a set time (repooling if enabled)
	{
		if(ObjectDeathTimer > 0)
			yield return new WaitForSeconds(ObjectDeathTimer);

		if(EnablePooling)
		{
//			ObjectToDestroy.layer = LayerMask.NameToLayer ("Default");
			ObjectToDestroy.transform.parent = null;
//			ObjectToDestroy.transform.localScale = new Vector3 (1, 1, 1);
			ObjectToDestroy.transform.localScale = Projectile[0].transform.localScale;
			ObjectToDestroy.transform.rotation = Quaternion.identity;
			ObjectToDestroy.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			
			ObjectToDestroy.SetActive (false);
		}
		else
		{
			NonPooledObjects.Remove (ObjectToDestroy);
			Destroy (ObjectToDestroy);
		}
	}
	public void destroy_GameObject(GameObject ObjectToDestroy, float ObjectDeathTimer)
	{
		StartCoroutine (Destroy_GameObject (ObjectToDestroy, ObjectDeathTimer));
	}

	// Will create a set of pooled objects. Can preload objects if you want.
	IEnumerator C_PrePoolObjects(float StartDelay, int AmountToPrePool)
	{
		yield return new WaitForSeconds(StartDelay);
		int CurrentPooledObjects = PooledObjects.Count;

		if(EnablePooling)
		{
			for(int I=0; I<(AmountToPrePool-CurrentPooledObjects); I++)
			{
				GameObject newclone;
				
				newclone = Instantiate(GetGameObjectToSpawn(), transform.position, Quaternion.identity) as GameObject;
				newclone.name = gameObject.name + " - clone_pooled - "+PooledObjects.Count;
				if(newclone.GetComponent<Rigidbody2D>() == false)
					newclone.AddComponent <Rigidbody2D>();
				
				newclone.GetComponent<Rigidbody2D>().freezeRotation = true;
				newclone.GetComponent<Rigidbody2D>().gravityScale = ObjectsGravityScale;
				newclone.GetComponent<Rigidbody2D>().isKinematic = KinematicObjects;

				PooledObjects.Add (newclone);
				newclone.SetActive (false);
			}
		}
		else
			print ("TRYING TO PREPOOL OBJECTS BUT SCRIPT IS SET TO NOT POOL OBJECTS");
	}
	public void PrePoolObjects(float StartDelay, int AmountToPrePool)
	{
		StartCoroutine (C_PrePoolObjects (StartDelay, AmountToPrePool));
	}


// SET VARIABLES ----------------------------------------------------------------------------------------------------------------------------
	// Sets if pooling is enabled or not
	// Default = true
	// It is also probably wise to not swap this option in the middle of a script (if you need to set it, do so at the setup)
	IEnumerator C_SetEnablePooling(float StartDelay, bool enablePooling)
	{
		yield return new WaitForSeconds (StartDelay);
		EnablePooling = enablePooling;
	}
	public void SetEnablePooling(float StartDelay, bool enablePooling)
	{
		StartCoroutine (C_SetEnablePooling (StartDelay, enablePooling));
	}
	// Sets the projectile used when a clone is created
	IEnumerator C_SetProjectile(float StartDelay, List<GameObject> ProjectileSet)
	{
		yield return new WaitForSeconds(StartDelay);
		Projectile = ProjectileSet;
	}
	public void SetProjectile(float StartDelay, List<GameObject> ProjectileSet)
	{
		StartCoroutine (C_SetProjectile (StartDelay, ProjectileSet));
	}
	public void SetProjectile(float StartDelay, params GameObject[] AProjectile)
	{
		List<GameObject> ProjectileList = new List<GameObject>(AProjectile);
		StartCoroutine (C_SetProjectile (StartDelay, ProjectileList));
	}
	// Set the color of the objects that are spawning
	IEnumerator C_SetColor (float StartDelay, Color newColor)
	{
		yield return new WaitForSeconds(StartDelay);
		UseDefaultColor = false;
		RandomColorSpawn = false;
		SpawnColor = newColor;
	}
	public void SetColor (float StartDelay, Color newColor)
	{
		StartCoroutine (C_SetColor (StartDelay, newColor));
	}
	IEnumerator C_SetColor_Random (float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);
		UseDefaultColor = false;
		RandomColorSpawn = true;
	}
	public void SetColor_Random (float StartDelay)
	{
		StartCoroutine (C_SetColor_Random (StartDelay));
	}
	IEnumerator C_SetSpawnScale(float StartDelay, Vector3 NewSpawnScale)
	{
		yield return new WaitForSeconds (StartDelay);
		UseDefaultSpawnScale = false;
		SpawnScale = NewSpawnScale;
	}
	public void SetSpawnScale(float StartDelay, Vector3 NewSpawnScale)
	{
		StartCoroutine (C_SetSpawnScale(StartDelay, NewSpawnScale));
	}
	IEnumerator C_SetKinematic(float StartDelay, bool IsKinematic)
	{
		yield return new WaitForSeconds (StartDelay);
		KinematicObjects = IsKinematic;
	}
	public void SetKinematic(float StartDelay, bool IsKinematic)
	{
		StartCoroutine (C_SetKinematic(StartDelay, IsKinematic));
	}
	IEnumerator C_SetGravityScale(float StartDelay, float newGravityScale)
	{
		yield return new WaitForSeconds (StartDelay);
		UseDefaultGravity = false;
		ObjectsGravityScale = newGravityScale;
	}
	public void SetGravityScale(float StartDelay, float newGravityScale)
	{
		StartCoroutine (C_SetGravityScale(StartDelay, newGravityScale));
	}

// SHAPE CALCULATIONS ----------------------------------------------------------------------------------------------------------------------
// Other functions will use these in order to calculate the points of a given shape type

	// Returns the positional offset of each object in a line
	public IEnumerable<Vector3> Calculate_Line(Vector3 StartingPoint, Vector3 EndingPoint, int NumberOfPoints, float BendMultiplier, int EndingPoint_Indent)
	{
		Vector3 Starting_Pos, Ending_Pos;
		float ObjectPosition;
		
		Starting_Pos = Vector3.zero;
		Ending_Pos = EndingPoint - StartingPoint;
		int Indent = -1 + EndingPoint_Indent;

		if(NumberOfPoints == 1)
			Indent = 0;
		
		Vector3 BendPoint = Vector3.Lerp (Starting_Pos, Ending_Pos, 0.5f);
		Vector3 IDirection = (new Vector2 (-(Ending_Pos - Starting_Pos).normalized.y, (Ending_Pos - Starting_Pos).normalized.x));
		BendPoint += BendMultiplier * IDirection * (Vector2.Distance (Starting_Pos, Ending_Pos)/2);
		
		//		print (string.Format ("{0} - {1} - {2}", Starting_Pos, BendPoint, Ending_Pos));
		
		for(int i = 0; i< NumberOfPoints; i++)
		{
			ObjectPosition = (float)i / (NumberOfPoints+Indent);
			yield return (((1 - ObjectPosition) * (1 - ObjectPosition)) * Starting_Pos) + (((1 - ObjectPosition) * 2.0f) * ObjectPosition * BendPoint) + ((ObjectPosition * ObjectPosition) * Ending_Pos);
		}
	}

	// This returns the position of certain points around a circle based on the variables
	public IEnumerable<Vector3> Calculate_Circle(int NumberOfPoints, float X_Width, float Y_Width, int PointSkip, float Segmentation)
	{
		float x = 0;
		float y = 0;
		Vector3 pointPos;
		float pointNum, angle;

		for(int i = 0; i<NumberOfPoints*PointSkip; i+=PointSkip)
		{
			pointNum = (i*1.0f)/NumberOfPoints;
			angle = pointNum*Mathf.PI*2*Segmentation;
			
			x = Mathf.Sin (angle)*X_Width;
			y = Mathf.Cos (angle)*Y_Width;
			
			pointPos = new Vector3(x, y);

			yield return pointPos;
		}
	}

	// Returns both the starting point and ending point of each leg of a square based on the variables
	public IEnumerable<Starting_EndingPoint> Calculate_Square(int ObjectsPerLeg, float X_Width, float Y_Width)
	{
		Starting_EndingPoint SE_Point = new Starting_EndingPoint();
		
		SE_Point.StartingPoint = new Vector3(-1f*X_Width, 1*Y_Width);
		SE_Point.EndingPoint = new Vector3(1f*X_Width, 1*Y_Width);
		yield return SE_Point;
		
		SE_Point.StartingPoint = new Vector3(1f*X_Width, 1*Y_Width);
		SE_Point.EndingPoint = new Vector3(1f*X_Width, -1*Y_Width);
		yield return SE_Point;
		
		SE_Point.StartingPoint = new Vector3(1f*X_Width, -1*Y_Width);
		SE_Point.EndingPoint = new Vector3(-1f*X_Width, -1*Y_Width);
		yield return SE_Point;
		
		SE_Point.StartingPoint = new Vector3(-1f*X_Width, -1*Y_Width);
		SE_Point.EndingPoint = new Vector3(-1f*X_Width, 1*Y_Width);
		yield return SE_Point;
	}

	// Returns both the starting point and ending point of each leg of a polygon based on the variables
	public IEnumerable<Starting_EndingPoint> Calculate_Polygon(int NumberOfPoints, int LineDrawSkip, float X_Width, float Y_Width)
	{
		Starting_EndingPoint SE_Point = new Starting_EndingPoint();
		Vector3 Initial_Pos;
		
		IEnumerator<Vector3> CirclePosition = Calculate_Circle (NumberOfPoints, X_Width, Y_Width, LineDrawSkip, 1).GetEnumerator ();
		
		CirclePosition.MoveNext ();
		Initial_Pos = CirclePosition.Current;
		SE_Point.StartingPoint = Initial_Pos;
		SE_Point.EndingPoint   = Initial_Pos;
		
		while(CirclePosition.MoveNext())
		{
			SE_Point.EndingPoint = CirclePosition.Current;
			yield return SE_Point;
			
			SE_Point.StartingPoint = SE_Point.EndingPoint;
		}
		
		SE_Point.EndingPoint = Initial_Pos;
		yield return SE_Point;
	}

	// This calculates the points on a polar curve (formula is given via the delegate PolarCurveFormula)
	// It calculates the x,y coordinates and returns the vector location
	public IEnumerable<Vector3> Calculate_PolarCurve(int NumberOfPoints, float X_Width, float Y_Width, PolarCurveFormula PolarCurve, float Multiplier)
	{
		float radius;
		float X, Y;

		float NumberOfIncrements = (Mathf.PI * Multiplier) / (float)NumberOfPoints;

		for(float t=0; t < 3.14f * Multiplier; t+=NumberOfIncrements)
		{
			radius = PolarCurve(t);
			
			X = (radius * Mathf.Cos (t)) * X_Width;
			Y = (radius * Mathf.Sin (t)) * Y_Width;
			
			yield return new Vector2(X,Y);
		}
	}


// CREATE OBJECTS --------------------------------------------------------------------------------------------------------------------------
// IsLocal - Tells the object to be created in local / global space
// IsLocal = false. A globally created object will use global coordinates and thus may be skewed during creation if the base object is rotating
// IsLocal = true. will be created correctly even if this base object is rotating or moved
// IsLocal = true. Target will be a local offset instead of a global point
// globally creating a line during rotation will create a spiral, while creating it locally will create a normal line

// Objects in this section that consist of multiple lines can be created in two different ways
// LegDelay = true. This will cause each leg to wait until the previous one has been completed before moving on
// LegDelay = false. This will cause all of the legs to build at the same time

// The starting position can be either a gameobject or a vector3 location
// If the gameobject = null, then it will use the vector3 location


	// Creates a line between two points consisting of NumberOfPoints Objects
	public IEnumerator C_CreateLine_ToFrom(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, GameObject EndingObject, Vector3 EndingPoint, float AppearDelay, int NumberOfPoints, float BendMultiplier, int EndingPoint_Indent, bool IsLocal)
	{
		if(StartDelay > 0)
			yield return new WaitForSeconds (StartDelay);
		
		Vector3 Starting_Pos, Ending_Pos;
		
		Starting_Pos = ReturnProperPoint (StartingObject, StartingPoint);
		Ending_Pos   = ReturnProperPoint(EndingObject, EndingPoint);
		
		if (IsLocal)
		{
			Starting_Pos = gameObject.transform.InverseTransformPoint(Starting_Pos);
			Ending_Pos = gameObject.transform.InverseTransformPoint(Ending_Pos);
		}
		
		foreach(Vector3 ReturnValue in Calculate_Line(Starting_Pos, Ending_Pos, NumberOfPoints, BendMultiplier, EndingPoint_Indent))
		{
			if(IsLocal)
				CreateClone(true).transform.localPosition = Starting_Pos + ReturnValue;
			else
				CreateClone(true).transform.position = Starting_Pos + ReturnValue;
			
			if(AppearDelay != 0)
				yield return new WaitForSeconds (AppearDelay);
		}
	}
	public void CreateLine_ToFrom(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, GameObject EndingObject, Vector3 EndingPoint, float AppearDelay, int NumberOfPoints, float BendMultiplier, int EndingPoint_Indent, bool IsLocal)
	{
		StartCoroutine (C_CreateLine_ToFrom (StartDelay, StartingObject, StartingPoint, EndingObject, EndingPoint, AppearDelay, NumberOfPoints, BendMultiplier, EndingPoint_Indent, IsLocal));
	}

	// Create a line in a vector3 direction
	// Calculates certain values then uses "C_CreateLine_InDirection" to create the line
	IEnumerator C_CreateLine_InDirection(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, Vector3 TargetDirection, float AppearDelay, int NumberOfPoints, float Length, float BendMultiplier, int EndingPoint_Indent, bool IsLocal)
	{
		yield return new WaitForSeconds (StartDelay);
		Vector3 pointPos;
		
		pointPos = ReturnProperPoint (StartingObject, StartingPoint) + TargetDirection.normalized*Length;
		StartCoroutine (C_CreateLine_ToFrom (0, StartingObject, StartingPoint, null, pointPos, AppearDelay, NumberOfPoints, BendMultiplier, EndingPoint_Indent, IsLocal));
	}
	public void CreateLine_InDirection(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, Vector3 TargetDirection, float AppearDelay, int NumberOfPoints, float Length, float BendMultiplier, int EndingPoint_Indent, bool IsLocal)
	{
		StartCoroutine (C_CreateLine_InDirection (StartDelay, StartingObject, StartingPoint, TargetDirection, AppearDelay, NumberOfPoints, Length, BendMultiplier, EndingPoint_Indent, IsLocal));
	}
	

	// Create a multi-lined object with "NumberOfLeg" legs
	// Calculates the leg line positions and then uses "C_CreateLine_ToFrom" to create them
	IEnumerator C_CreateLine_MultiLines(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfLegs, float LengthOfLeg, int ObjectsPerLeg, float BendMultiplier, int EndingPoint_Indent, bool IsLocal)
	{
		yield return new WaitForSeconds(StartDelay);
		
		Vector3 SpawnPoint = ReturnProperPoint (StartingObject, StartingPoint);

		Vector3 pointPos;
		Vector3 IndentedSpawnPoint;

		ObjectsPerLeg --;
		CreateClone (true).transform.position = SpawnPoint;

		foreach(Vector3 ReturnValue in Calculate_Circle (NumberOfLegs, LengthOfLeg,LengthOfLeg, 1, 1))
		{
			pointPos = ReturnValue + SpawnPoint;
			IndentedSpawnPoint = Vector3.Lerp (SpawnPoint, pointPos, 1/(float)ObjectsPerLeg);

			StartCoroutine (C_CreateLine_ToFrom (0, null, IndentedSpawnPoint, null, pointPos, AppearDelay, ObjectsPerLeg, BendMultiplier, EndingPoint_Indent, IsLocal));
		}
	}
	public void CreateLine_MultiLines(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfLegs, float LengthOfLeg, int ObjectsPerLeg, float BendMultiplier, int EndingPoint_Indent, bool IsLocal)
	{
		StartCoroutine (C_CreateLine_MultiLines (StartDelay, StartingObject, StartingPoint, AppearDelay, NumberOfLegs, LengthOfLeg, ObjectsPerLeg,  BendMultiplier, EndingPoint_Indent, IsLocal));
	}
	
	
	// Create a circle with NumberOfPoints points
	// X_Width and Y_Width are separate so you can create a squished circle
	// Using negative values for X_Width and Y_Width will change the side and way the circle is created
	// Segmentation is a wierd extra parameter that can create nice effects. It tells how much of the circle to create
	// Segmentation = 1 - This will create the circle as expected (ie 100% of the circle) - should be used as default
	// Segmentation = 0.5f - This will create only 1/2 of a circle
	// Segmentation = 2 - This will create 2x a circle. So half the objects will create one circle and the other half will make another circle on top of the first
	// - For values >1, make sure the value is non-divisible of 360. This will cause each consecutive circle to be offset and create a nice effect (try using 7)
	// Negitive numbers will cause the circle to spawn backwards
	IEnumerator C_CreateShape_Circle(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, float Segmentation, bool IsLocal)
	{
		yield return new WaitForSeconds (StartDelay);

		Vector3 SpawnPoint = ReturnProperPoint (StartingObject, StartingPoint);
		if(IsLocal)
			SpawnPoint = gameObject.transform.InverseTransformPoint(SpawnPoint);

		foreach(Vector3 ReturnValue in Calculate_Circle(NumberOfPoints, X_Width, Y_Width, 1, Segmentation))
		{
			if(IsLocal)
				CreateClone(true).transform.localPosition = ReturnValue + SpawnPoint;
			else
				CreateClone(true).transform.position = ReturnValue + SpawnPoint;
			
			if(AppearDelay > 0)
				yield return new WaitForSeconds(AppearDelay);
		}
	}
	public void CreateShape_Circle(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, float Segmentation, bool IsLocal)
	{
		StartCoroutine (C_CreateShape_Circle(StartDelay, StartingObject, StartingPoint, AppearDelay, NumberOfPoints, X_Width, Y_Width, Segmentation, IsLocal));
	}


	// Create a square with ObjectsPerLeg on each of the 4 legs
	// X_Width and Y_Width are separate so you can create a squished square
	// Using negative values for X_Width and Y_Width will change the side and way the square is created
	IEnumerator C_CreateShape_Square(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int ObjectsPerLeg, float BendMultiplier, float X_Width, float Y_Width, bool LegDelay, bool IsLocal)
	{
		yield return new WaitForSeconds (StartDelay);
		
		Vector3 SpawnPoint = ReturnProperPoint (StartingObject, StartingPoint);
		if(IsLocal)
			SpawnPoint = gameObject.transform.InverseTransformPoint(SpawnPoint);
		
		foreach(Starting_EndingPoint ReturnValue in Calculate_Square(ObjectsPerLeg, X_Width, Y_Width))
		{
			if(LegDelay)
			{
				if(IsLocal)
					yield return StartCoroutine (C_CreateLine_ToFrom(0, null, transform.TransformPoint (ReturnValue.StartingPoint+SpawnPoint), null, transform.TransformPoint (ReturnValue.EndingPoint+SpawnPoint), AppearDelay, ObjectsPerLeg, BendMultiplier, 1, IsLocal));
				else
					yield return StartCoroutine (C_CreateLine_ToFrom(0, null, ReturnValue.StartingPoint+SpawnPoint, null, ReturnValue.EndingPoint+SpawnPoint, AppearDelay, ObjectsPerLeg, BendMultiplier, 1, IsLocal));
			}
			else
			{
				if(IsLocal)
					StartCoroutine (C_CreateLine_ToFrom(0, null, transform.TransformPoint (ReturnValue.StartingPoint+SpawnPoint), null, transform.TransformPoint (ReturnValue.EndingPoint+SpawnPoint), AppearDelay, ObjectsPerLeg, BendMultiplier, 1, IsLocal));
				else
					StartCoroutine (C_CreateLine_ToFrom(0, null, ReturnValue.StartingPoint+SpawnPoint, null, ReturnValue.EndingPoint+SpawnPoint, AppearDelay, ObjectsPerLeg, BendMultiplier, 1, IsLocal));
			}
		}
	}
	public void CreateShape_Square(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int ObjectsPerLeg, float BendMultiplier, float X_Width, float Y_Width, bool LegDelay, bool IsLocal)
	{
		StartCoroutine (C_CreateShape_Square (StartDelay, StartingObject, StartingPoint, AppearDelay, ObjectsPerLeg, BendMultiplier, X_Width, Y_Width, LegDelay, IsLocal));
	}


	// Create a triangle with NumberOfPoints points
	// X_Width and Y_Width are separate so you can create a squished triangle
	// Using negative values for X_Width and Y_Width will change the side and way the square is created
	// The code to create the triangle is pretty longhanded. Logically it works but there has got to be a more simplistic way to do it.
	// To create even spacing of the objects, each leg is NumberOfPoints/3
	// The remainders will not be spawned
	IEnumerator C_CreateShape_Triangle(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int ObjectsPerLeg, float BendMultiplier, float X_Width, float Y_Width, bool LegDelay, bool IsLocal)
	{
		yield return new WaitForSeconds (StartDelay);
		
		Vector3 Starting_Pos, Ending_Pos;
		Vector3 SpawnPoint = ReturnProperPoint (StartingObject, StartingPoint);
		if(IsLocal)	SpawnPoint = transform.InverseTransformPoint (SpawnPoint);

		if(IsLocal)
		{
			Starting_Pos = transform.TransformPoint (new Vector3 (0, 1f * Y_Width)) + SpawnPoint;
			Ending_Pos = transform.TransformPoint (new Vector3(1f*X_Width, -1*Y_Width)) + SpawnPoint;
		}
		else
		{
			Starting_Pos = new Vector3 (0, 1f * Y_Width) + SpawnPoint;
			Ending_Pos = new Vector3(1f*X_Width, -1*Y_Width) + SpawnPoint;
		}

		if (LegDelay)
			yield return StartCoroutine (C_CreateLine_ToFrom (0, null, Starting_Pos, null, Ending_Pos, AppearDelay, ObjectsPerLeg, BendMultiplier,1, IsLocal));
		else
			CreateLine_ToFrom (0, null, Starting_Pos, null, Ending_Pos, AppearDelay, ObjectsPerLeg, BendMultiplier,1, IsLocal);

		if(IsLocal)
		{
			Starting_Pos = transform.TransformPoint (new Vector3(1f*X_Width, -1*Y_Width) + SpawnPoint);
			Ending_Pos = transform.TransformPoint (new Vector3(-1f*X_Width, -1*Y_Width) + SpawnPoint);
		}
		else
		{
			Starting_Pos = new Vector3(1f*X_Width, -1*Y_Width) + SpawnPoint;
			Ending_Pos = new Vector3(-1f*X_Width, -1*Y_Width) + SpawnPoint;
		}

		if (LegDelay)
			yield return StartCoroutine (C_CreateLine_ToFrom (0, null, Starting_Pos, null, Ending_Pos, AppearDelay, ObjectsPerLeg, BendMultiplier,1, IsLocal));
		else
			CreateLine_ToFrom (0, null, Starting_Pos, null, Ending_Pos, AppearDelay, ObjectsPerLeg, BendMultiplier,1, IsLocal);
		
		if(IsLocal)
		{
			Starting_Pos = transform.TransformPoint (new Vector3(-1f*X_Width, -1*Y_Width) + SpawnPoint);
			Ending_Pos = transform.TransformPoint (new Vector3 (0, 1f * Y_Width) + SpawnPoint);
		}
		else
		{
			Starting_Pos = new Vector3(-1f*X_Width, -1*Y_Width) + SpawnPoint;
			Ending_Pos = new Vector3 (0, 1f * Y_Width) + SpawnPoint;
		}

		CreateLine_ToFrom (0, null, Starting_Pos, null, Ending_Pos, AppearDelay, ObjectsPerLeg, BendMultiplier,1, IsLocal);		
		yield return null;
	}
	public void CreateShape_Triangle(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int ObjectsPerLeg, float BendMultiplier, float X_Width, float Y_Width, bool LegDelay, bool IsLocal)
	{
		StartCoroutine (C_CreateShape_Triangle (StartDelay, StartingObject, StartingPoint, AppearDelay, ObjectsPerLeg, BendMultiplier, X_Width, Y_Width, LegDelay, IsLocal));
	}

	// Create a shape with NumberOfPoints sides (polygons and star based designs)
	// LineDrawSkip tells how many ponits to skip when drawing the polygons edges
	// LineDrawSkip = 1. A line will be drawn to the next available point in the polygon, thus making a simple border
	// LineDrawSkip = 2. The line will skip one point and draw a line to the next point.
	// It will continue until all the edges are drawn. Using LineDrawSkip is a way to create star like designs
	// IndentPercent is how indented the middle of the lines become
	// If IndentPercent = 1, there is no indent. 0.5f = 50% indent. 1.5f = 150% outdent
	IEnumerator C_CreateShape_Polygon(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, int ObjectsPerLeg, int LineDrawSkip, float IndentPercent, float BendMultiplier, float X_Width, float Y_Width, bool LegDelay, bool IsLocal)
	{
		yield return new WaitForSeconds (StartDelay);
		
		Vector3 SpawnPoint = ReturnProperPoint (StartingObject, StartingPoint);
		if(IsLocal)
			SpawnPoint = gameObject.transform.InverseTransformPoint(SpawnPoint);
		
		Vector3 Starting_Pos, Ending_Pos;
		Vector3 IndentPosition = Vector3.zero;
		
		float MiddleOfLeg = 0.50f;
		if(IndentPercent == 1)
			MiddleOfLeg = (Mathf.RoundToInt(ObjectsPerLeg/2f+0.1f) / (float)ObjectsPerLeg);
		
		
		int FirstLegSegment = (int)ObjectsPerLeg / 2;
		int SecondLegSegment = Mathf.RoundToInt (ObjectsPerLeg / 2f + 0.1f);
		if(IndentPercent == 1)
			FirstLegSegment =  FirstLegSegment + SecondLegSegment;
		
		//		print (string.Format ("Per Leg - {0},  First Leg - {1}, Second Leg - {2}", ObjectsPerLeg, FirstLegSegment, SecondLegSegment));
		
		foreach(Starting_EndingPoint ReturnValue in Calculate_Polygon (NumberOfPoints, LineDrawSkip, X_Width, Y_Width))
		{
			//			print (string.Format ("{0}  -  {1}", ReturnValue.StartingPoint, ReturnValue.EndingPoint));
			
			Starting_Pos = ReturnValue.StartingPoint;
			Ending_Pos = ReturnValue.EndingPoint;
			IndentPosition = Vector3.Lerp (Starting_Pos, Ending_Pos, MiddleOfLeg)*IndentPercent;
			
			if(IndentPercent == 1)	IndentPosition = Ending_Pos;
			
			if(LegDelay)
			{
				if(IsLocal)
				{
					yield return StartCoroutine (C_CreateLine_ToFrom(0, null, transform.TransformPoint (Starting_Pos+SpawnPoint), null, transform.TransformPoint(IndentPosition+SpawnPoint), AppearDelay, FirstLegSegment, BendMultiplier, 1, IsLocal));
					if(IndentPercent != 1)
						yield return StartCoroutine (C_CreateLine_ToFrom(0, null, transform.TransformPoint (IndentPosition+SpawnPoint), null, transform.TransformPoint(Ending_Pos+SpawnPoint), AppearDelay, SecondLegSegment, BendMultiplier, 1, IsLocal));
				}
				else
				{
					yield return StartCoroutine (C_CreateLine_ToFrom(0, null, Starting_Pos+SpawnPoint, null, IndentPosition+SpawnPoint, AppearDelay, FirstLegSegment, BendMultiplier, 1, IsLocal));
					if(IndentPercent != 1)
						yield return StartCoroutine (C_CreateLine_ToFrom(0, null, IndentPosition+SpawnPoint, null, Ending_Pos+SpawnPoint, AppearDelay, SecondLegSegment, BendMultiplier, 1, IsLocal));
				}
			}
			else
			{
				if(IsLocal)
				{
					StartCoroutine (C_CreateLine_ToFrom(0, null, transform.TransformPoint (Starting_Pos+SpawnPoint), null, transform.TransformPoint(IndentPosition+SpawnPoint), AppearDelay, FirstLegSegment, BendMultiplier, 1, IsLocal));
					if(IndentPercent != 1)
						StartCoroutine (C_CreateLine_ToFrom(0, null, transform.TransformPoint (IndentPosition+SpawnPoint), null, transform.TransformPoint(Ending_Pos+SpawnPoint), AppearDelay, SecondLegSegment, BendMultiplier, 1, IsLocal));
				}
				else
				{
					StartCoroutine (C_CreateLine_ToFrom(0, null, Starting_Pos+SpawnPoint, null, IndentPosition+SpawnPoint, AppearDelay, FirstLegSegment, BendMultiplier, 1, IsLocal));
					if(IndentPercent != 1)
						StartCoroutine (C_CreateLine_ToFrom(0, null, IndentPosition+SpawnPoint, null, Ending_Pos+SpawnPoint, AppearDelay, SecondLegSegment, BendMultiplier, 1, IsLocal));
				}
			}
		}
		
		yield return null;
	}
	public void CreateShape_Polygon(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, int ObjectsPerLeg, int LineDrawSkip, float IndentPercent, float BendMultiplier, float X_Width, float Y_Width, bool LegDelay, bool IsLocal)
	{
		StartCoroutine (C_CreateShape_Polygon(StartDelay, StartingObject, StartingPoint, AppearDelay, NumberOfPoints, ObjectsPerLeg, LineDrawSkip, IndentPercent, BendMultiplier, X_Width, Y_Width, LegDelay, IsLocal));
	}


	// Creates an object using a polar curve formula
	IEnumerator C_CreateShape_PolarCurve(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, bool IsLocal, PolarCurveFormula APolarCurve, float Multiplier)
	{
		yield return new WaitForSeconds (StartDelay);
		
		Vector3 SpawnPoint = ReturnProperPoint (StartingObject, StartingPoint);
		if(IsLocal)
			SpawnPoint = gameObject.transform.InverseTransformPoint(SpawnPoint);
		
		foreach(Vector3 ReturnValue in Calculate_PolarCurve(NumberOfPoints, X_Width, Y_Width, APolarCurve, Multiplier))
		{
			if(IsLocal)
				CreateClone(true).transform.localPosition = ReturnValue + SpawnPoint;
			else
				CreateClone(true).transform.position = ReturnValue + SpawnPoint;
			
			if(AppearDelay > 0)
				yield return new WaitForSeconds(AppearDelay);
		}
	}
	// Insert your own polar curve lambda expression and the multiplier
	// Theta goes from 0->Pi * PiMultiplier
	public void Create_PolarCurve(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, PolarCurveFormula PolarFormula, float PiMultiplier, bool IsLocal)
	{
		StartCoroutine (C_CreateShape_PolarCurve(StartDelay, StartingObject, StartingPoint, AppearDelay, NumberOfPoints, X_Width, Y_Width, IsLocal, PolarFormula, PiMultiplier));
	}
	// Uses the polar formula r = Value_A + Value_B*cos(Value_K_Top/Value_K_Bottom * theta)
	// The multiplier is calculated based on thes to create a enclosed object
	public void CreateShape_PolarCurve_abcoskt(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, float Value_A, float Value_B, float Value_K_Top, float Value_K_Bottom, float X_Width, float Y_Width, bool IsLocal)
	{
		PolarCurveFormula del = t => (Value_A + Value_B * Mathf.Cos ((Value_K_Top / Value_K_Bottom) * t));
		
		float Multiplier;
		if(((Value_K_Top * Value_K_Bottom)%2) == 1)
			Multiplier = 1;
		else
			Multiplier = 2;
		
		if(Value_A > 0)
			Multiplier= 2;
		
		Multiplier *= Value_K_Bottom;
		StartCoroutine (C_CreateShape_PolarCurve(StartDelay, StartingObject, StartingPoint, AppearDelay, NumberOfPoints, X_Width, Y_Width, IsLocal, del, Multiplier));
	}
	// Makes a heart shaped object
	public void CreateShape_PolarCurve_Heart(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, bool IsLocal)
	{
		PolarCurveFormula del = t => (2 - 2*Mathf.Sin (t) + (Mathf.Sin(t) * Mathf.Sqrt(Mathf.Abs(Mathf.Cos (t))) / (Mathf.Sin(t) + 1.4f)));
		StartCoroutine (C_CreateShape_PolarCurve(StartDelay, StartingObject, StartingPoint, AppearDelay, NumberOfPoints, X_Width, Y_Width, IsLocal, del, 2));
	}
	// Makes a spiral outwards (default distance between loops is 1)
	// Goes from theta 0->Pi * PiMultiplier. PiMultiplier = 2, would create "one circle"
	public void CreateShape_PolarCurve_Spiral(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, float AppearDelay, int NumberOfPoints, float X_Width, float Y_Width, float PiMultiplier, bool IsLocal)
	{
		PolarCurveFormula del = t => 0.159f*t;
		StartCoroutine (C_CreateShape_PolarCurve(StartDelay, StartingObject, StartingPoint, AppearDelay, NumberOfPoints, X_Width, Y_Width, IsLocal, del, PiMultiplier));
	}


// MORPHTO SHAPES ---------------------------------------------------------------------------------------------------------------------------
	// Takes all the objects in GameObjectList list and forms them in the shape of a line
	// Makes the line between 2 vector3 points
	IEnumerator C_MorphTo_Line_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject StartingObject, Vector3 StartingPoint, GameObject EndingObject, Vector3 EndingPoint, float ObjectDelay, float BendMultiplier, int EndingPoint_Indent, float inSeconds)
	{
		yield return new WaitForSeconds (StartDelay);
		
		int NumberOfAttachedObjects = AttachedObjects.Count;
		int counter = 0;
		float Delay = 0;
		
		StartingPoint = ReturnProperPoint (StartingObject, StartingPoint);
		EndingPoint = ReturnProperPoint (EndingObject, EndingPoint);

		StartingPoint = gameObject.transform.InverseTransformPoint(StartingPoint);
		EndingPoint = gameObject.transform.InverseTransformPoint(EndingPoint);
		
		foreach(Vector3 ReturnValue in Calculate_Line(StartingPoint, EndingPoint, NumberOfAttachedObjects, BendMultiplier, EndingPoint_Indent))
		{
			MoveObjectToPoint_Local(Delay, GameObjectList[counter], ReturnValue+StartingPoint, inSeconds);
			
			Delay += ObjectDelay;
			counter++;
		}
	}
	public void MorphTo_Line_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject StartingObject, Vector3 StartingPoint, GameObject EndingObject, Vector3 EndingPoint, float ObjectDelay, float BendMultiplier, int EndingPoint_Indent, float inSeconds)
	{
		StartCoroutine (C_MorphTo_Line_GameObjectList(StartDelay, GameObjectList, StartingObject, StartingPoint, EndingObject, EndingPoint, ObjectDelay, BendMultiplier, EndingPoint_Indent, inSeconds));
	}
	public void MorphTo_Line_Children(float StartDelay, GameObject StartingObject, Vector3 StartingPoint, GameObject EndingObject, Vector3 EndingPoint, float ObjectDelay, float BendMultiplier, int EndingPoint_Indent, float inSeconds)
	{
		StartCoroutine (C_MorphTo_Line_GameObjectList(StartDelay, AttachedObjects, StartingObject, StartingPoint, EndingObject, EndingPoint, ObjectDelay, BendMultiplier, EndingPoint_Indent, inSeconds));
	}


	// Takes all the objects in GameObjectList list and forms them in the shape of a circle
	IEnumerator C_MorphTo_Circle_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float inSeconds)
	{
		yield return new WaitForSeconds(StartDelay);
		inSeconds = Mathf.Abs (inSeconds);
		Vector3 CreationPoint = ReturnProperPoint (TargetObject, TargetPoint);
		CreationPoint = gameObject.transform.InverseTransformPoint(CreationPoint);

		int i = 0;
		float Delay = 0;
				
		foreach(Vector3 ReturnValue in Calculate_Circle(GameObjectList.Count, X_Width, Y_Width, 1, 1f))
		{
			MoveObjectToPoint_Local(Delay, GameObjectList[i], ReturnValue+CreationPoint, inSeconds);
			i++;
			Delay += ObjectDelay;
		}
		
		yield return null;
	}
	public void MorphTo_Circle_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float inSeconds)
	{
		StartCoroutine (C_MorphTo_Circle_GameObjectList (StartDelay, GameObjectList, TargetObject, TargetPoint, ObjectDelay, X_Width, Y_Width, inSeconds));
	}
	public void MorphTo_Circle_Children(float StartDelay, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float inSeconds)
	{
		StartCoroutine (C_MorphTo_Circle_GameObjectList (StartDelay, AttachedObjects, TargetObject, TargetPoint, ObjectDelay, X_Width, Y_Width, inSeconds));
	}
	
	
	// Takes all the objects in GameObjectList list and forms them in the shape of a square
	IEnumerator C_MorphTo_Square_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float BendMultiplier, float X_Width, float Y_Width, float inSeconds)
	{
		yield return new WaitForSeconds(StartDelay);
		inSeconds = Mathf.Abs (inSeconds);
		Vector3 Starting_Pos, Ending_Pos = Vector3.zero;
		
		Vector3 CreationPoint = ReturnProperPoint (TargetObject, TargetPoint);
		CreationPoint = gameObject.transform.InverseTransformPoint(CreationPoint);
		
		int ObjectsPerLeg = GameObjectList.Count/4;
		int ObjectRemainder = GameObjectList.Count % 4;
		int counter = 0;
		float Delay = 0;

//		print (string.Format ("Per Leg - {0}, Remainder - {1}", ObjectsPerLeg, ObjectRemainder));
			
		foreach(Starting_EndingPoint ReturnValue in Calculate_Square((int)ObjectsPerLeg+1, X_Width, Y_Width))
		{
			Starting_Pos = ReturnValue.StartingPoint;
			Ending_Pos = ReturnValue.EndingPoint;

			foreach(Vector3 ReturnValueL in Calculate_Line(Starting_Pos, Ending_Pos, ObjectsPerLeg, BendMultiplier, 1))
			{
				MoveObjectToPoint_Local(Delay, GameObjectList[counter], ReturnValueL + Starting_Pos + CreationPoint, inSeconds);
				Delay += ObjectDelay;
				counter++;
			}
		}
		
		for(int I=0; I<ObjectRemainder; I++)
		{
			MoveObjectToPoint_Local(Delay, GameObjectList[counter], Ending_Pos+CreationPoint, inSeconds);
			counter++;
		}
		
		yield return null;
	}
	public void MorphTo_Square_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float BendMultiplier, float X_Width, float Y_Width, float inSeconds)
	{
		StartCoroutine (C_MorphTo_Square_GameObjectList(StartDelay, GameObjectList, TargetObject, TargetPoint, ObjectDelay, BendMultiplier, X_Width, Y_Width, inSeconds));
	}
	public void MorphTo_Square_Children(float StartDelay, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float BendMultiplier, float X_Width, float Y_Width, float inSeconds)
	{
		StartCoroutine (C_MorphTo_Square_GameObjectList(StartDelay, AttachedObjects, TargetObject, TargetPoint, ObjectDelay, BendMultiplier, X_Width, Y_Width, inSeconds));
	}


	// Takes all the objects in GameObjectList list and forms them in the shape of a polygon
	IEnumerator C_MorphTo_Polygon_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, int NumberOfPoints, int LineDrawSkip, float IndentPercent, float BendMultiplier, float X_Width, float Y_Width, float inSeconds)
	{
		yield return new WaitForSeconds (StartDelay);
		
		Vector3 SpawnPoint = ReturnProperPoint (TargetObject, TargetPoint);
		SpawnPoint = gameObject.transform.InverseTransformPoint(SpawnPoint);
		
		Vector3 Starting_Pos, Ending_Pos = Vector3.zero;
		Vector3 IndentPosition = Vector3.zero;
		
		int ObjectsPerLeg = GameObjectList.Count/NumberOfPoints;
		int ObjectRemainder = GameObjectList.Count % NumberOfPoints;
		int counter = 0;
		float Delay = 0;
		
//		print (string.Format ("Per Leg - {0},  First Leg - {2}, Second Leg - {1}, Remainder - {3}", ObjectsPerLeg, Mathf.RoundToInt(ObjectsPerLeg/2f+0.1f), (ObjectsPerLeg/2), ObjectRemainder));
		
		int FirstLegSegment = (int)ObjectsPerLeg / 2;
		int SecondLegSegment = Mathf.RoundToInt (ObjectsPerLeg / 2f + 0.1f);
		
		float MiddleOfLeg = 0.50f;
		if(IndentPercent == 1)	FirstLegSegment =  FirstLegSegment + SecondLegSegment;
		if(IndentPercent == 1)	MiddleOfLeg = (Mathf.RoundToInt(ObjectsPerLeg/2f+0.1f) / (float)ObjectsPerLeg);
		
		foreach(Starting_EndingPoint ReturnValue_P in Calculate_Polygon (NumberOfPoints, LineDrawSkip, X_Width, Y_Width))
		{
			Starting_Pos = ReturnValue_P.StartingPoint;
			Ending_Pos = ReturnValue_P.EndingPoint;
			IndentPosition = Vector3.Lerp (Starting_Pos, Ending_Pos, MiddleOfLeg)*IndentPercent;
			
//			print (string.Format ("{0} - {1} - {2}", Starting_Pos, IndentPosition, Ending_Pos));
			
			if(IndentPercent == 1)	IndentPosition = Ending_Pos;
			
			foreach(Vector3 ReturnValueL in Calculate_Line(Starting_Pos, IndentPosition, FirstLegSegment, BendMultiplier, 1))
			{
				MoveObjectToPoint_Local(Delay, GameObjectList[counter], (ReturnValueL + Starting_Pos + SpawnPoint), inSeconds);
				Delay += ObjectDelay;
				counter++;
			}
			if(IndentPercent != 1)
				foreach(Vector3 ReturnValueL in Calculate_Line(IndentPosition, Ending_Pos, SecondLegSegment, BendMultiplier, 1))
			{
				MoveObjectToPoint_Local(Delay, GameObjectList[counter], ReturnValueL + IndentPosition + SpawnPoint, inSeconds);
				Delay += ObjectDelay;
				counter++;
			}
		}
		for(int I=0; I<ObjectRemainder; I++)	// Remainder objects will be "hidden" under another object
		{
			MoveObjectToPoint_Local(Delay, GameObjectList[counter], Ending_Pos+SpawnPoint, inSeconds);
			counter++;
		}
		
		yield return null;
	}
	public void MorphTo_Polygon_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, int NumberOfPoints, int LineDrawSkip, float IndentPercent, float BendMultiplier, float X_Width, float Y_Width, float inSeconds)
	{
		StartCoroutine (C_MorphTo_Polygon_GameObjectList(StartDelay, GameObjectList, TargetObject, TargetPoint, ObjectDelay, NumberOfPoints, LineDrawSkip, IndentPercent, BendMultiplier, X_Width, Y_Width, inSeconds));
	}
	public void MorphTo_Polygon_Children(float StartDelay, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, int NumberOfPoints, int LineDrawSkip, float IndentPercent, float BendMultiplier, float X_Width, float Y_Width, float inSeconds)
	{
		StartCoroutine (C_MorphTo_Polygon_GameObjectList(StartDelay, AttachedObjects, TargetObject, TargetPoint, ObjectDelay, NumberOfPoints, LineDrawSkip, IndentPercent, BendMultiplier, X_Width, Y_Width, inSeconds));
	}


	// Takes all the objects in GameObjectList list and forms them in the shape of a polar curve
	IEnumerator C_MorphTo_PolarCurve_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float inSeconds, PolarCurveFormula APolarCurve, float Multiplier)
	{
		yield return new WaitForSeconds(StartDelay);
		inSeconds = Mathf.Abs (inSeconds);
		Vector3 CreationPoint = ReturnProperPoint (TargetObject, TargetPoint);
		CreationPoint = gameObject.transform.InverseTransformPoint(CreationPoint);
		
		int i = 0;
		float Delay = 0;
		
		foreach(Vector3 ReturnValue in Calculate_PolarCurve(GameObjectList.Count, X_Width, Y_Width, APolarCurve, Multiplier))
		{
			MoveObjectToPoint_Local(Delay, GameObjectList[i], ReturnValue+CreationPoint, inSeconds);
			i++;
			Delay += ObjectDelay;
		}
		
		yield return null;
	}
	// Insert your own polar curve lambda expression and the multiplier
	// Theta goes from 0->Pi * PiMultiplier
	public void MorphTo_PolarCurve_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float inSeconds, PolarCurveFormula APolarCurve, float Multiplier)
	{
		StartCoroutine (C_MorphTo_PolarCurve_GameObjectList(StartDelay, GameObjectList, TargetObject, TargetPoint, ObjectDelay, X_Width, Y_Width, inSeconds, APolarCurve, Multiplier));
	}
	public void MorphTo_PolarCurve_Children(float StartDelay, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float inSeconds, PolarCurveFormula APolarCurve, float Multiplier)
	{
		StartCoroutine (C_MorphTo_PolarCurve_GameObjectList(StartDelay, AttachedObjects, TargetObject, TargetPoint, ObjectDelay, X_Width, Y_Width, inSeconds, APolarCurve, Multiplier));
	}
	// Uses the polar formula r = Value_A + Value_B*cos(Value_K_Top/Value_K_Bottom * theta)
	// The multiplier is calculated based on thes to create a enclosed object
	public void MorphTo_PolarCurve_abcoskt_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float Value_A, float Value_B, float Value_K_Top, float Value_K_Bottom, float X_Width, float Y_Width, float inSeconds)
	{
		PolarCurveFormula del = t => (Value_A + Value_B * Mathf.Cos ((Value_K_Top / Value_K_Bottom) * t));
		
		float Multiplier;
		if(((Value_K_Top * Value_K_Bottom)%2) == 1)
			Multiplier = 1;
		else
			Multiplier = 2;
		
		if(Value_A > 0)
			Multiplier= 2;
		
		Multiplier *= Value_K_Bottom;
		
		StartCoroutine (C_MorphTo_PolarCurve_GameObjectList(StartDelay, GameObjectList, TargetObject, TargetPoint, ObjectDelay, X_Width, Y_Width, inSeconds, del, Multiplier));
	}
	public void MorphTo_PolarCurve_abcoskt_Children(float StartDelay, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float Value_A, float Value_B, float Value_K_Top, float Value_K_Bottom, float X_Width, float Y_Width, float inSeconds)
	{
		MorphTo_PolarCurve_abcoskt_GameObjectList(StartDelay, AttachedObjects, TargetObject, TargetPoint, ObjectDelay, Value_A, Value_B, Value_K_Top, Value_K_Bottom, X_Width, Y_Width, inSeconds);
	}
	// Makes a heart shaped object
	public void MorphTo_PolarCurve_Heart_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float inSeconds)
	{
		PolarCurveFormula del = t => (2 - 2*Mathf.Sin (t) + (Mathf.Sin(t) * Mathf.Sqrt(Mathf.Abs(Mathf.Cos (t))) / (Mathf.Sin(t) + 1.4f)));
		StartCoroutine (C_MorphTo_PolarCurve_GameObjectList(StartDelay, GameObjectList, TargetObject, TargetPoint, ObjectDelay, X_Width, Y_Width, inSeconds, del, 2));
	}
	public void MorphTo_PolarCurve_Heart_Children(float StartDelay, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float inSeconds)
	{
		PolarCurveFormula del = t => (2 - 2*Mathf.Sin (t) + (Mathf.Sin(t) * Mathf.Sqrt(Mathf.Abs(Mathf.Cos (t))) / (Mathf.Sin(t) + 1.4f)));
		StartCoroutine (C_MorphTo_PolarCurve_GameObjectList(StartDelay, AttachedObjects, TargetObject, TargetPoint, ObjectDelay, X_Width, Y_Width, inSeconds, del, 2));
	}
	// Makes a spiral outwards (default distance between loops is 1)
	// Goes from theta 0->Pi * PiMultiplier. PiMultiplier = 2, would create "one circle"
	public void MorphTo_PolarCurve_Spiral_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float PiMultiplier, float inSeconds)
	{
		PolarCurveFormula del = t => 0.159f*t;
		StartCoroutine (C_MorphTo_PolarCurve_GameObjectList(StartDelay, GameObjectList, TargetObject, TargetPoint, ObjectDelay, X_Width, Y_Width, inSeconds, del, PiMultiplier));
	}
	public void MorphTo_PolarCurve_Spiral_Children(float StartDelay, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float X_Width, float Y_Width, float PiMultiplier, float inSeconds)
	{
		PolarCurveFormula del = t => 0.159f*t;
		StartCoroutine (C_MorphTo_PolarCurve_GameObjectList(StartDelay, AttachedObjects, TargetObject, TargetPoint, ObjectDelay, X_Width, Y_Width, inSeconds, del, PiMultiplier));
	}


// CHILDREN FUNCTIONS -----------------------------------------------------------------------------------------------------------------------
// The below Coroutines will effect a list of gameobjects
// The below functions will to the action on all attached objects (list AttachedObjects)


	// Destroy each object in a list after a certain delay (ObjectDelay)
	IEnumerator C_Destroy_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay)
	{
		yield return new WaitForSeconds (StartDelay);
		float childDeathDelay = 0;
		
		foreach(GameObject child in GameObjectList)
		{
			StartCoroutine (Destroy_GameObject(child, childDeathDelay));
			childDeathDelay += ObjectDelay;
		}
		AttachedObjects.Clear ();
	}
	public void Destroy_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay)
	{
		StartCoroutine (C_Destroy_GameObjectList(StartDelay, GameObjectList, ObjectDelay));
	}
	public void Destroy_Children(float StartDelay, float ObjectDelay)
	{
		StartCoroutine (C_Destroy_GameObjectList(StartDelay, AttachedObjects, ObjectDelay));
	}

	// Destroy each child object and create a circle burst in its place
	IEnumerator C_Destroy_GameObjectList_CircleBurst(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, int NumberOfPoints, float ObjectSpeed, float ObjectDeathTimer)
	{
		yield return new WaitForSeconds (StartDelay);
		if(ObjectDeathTimer <= 0)
			ObjectDeathTimer = 1;

		foreach(GameObject child in GameObjectList)
		{
			Shoot_CircleBurst(0, child, child.transform.position, NumberOfPoints, ObjectSpeed, ObjectDeathTimer);

			StartCoroutine (Destroy_GameObject(child, 0.05f));

			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}
		AttachedObjects.Clear ();
	}
	public void Destroy_GameObjectList_CircleBurst(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, int NumberOfPoints, float ObjectSpeed, float ObjectDeathTimer)
	{
		StartCoroutine (C_Destroy_GameObjectList_CircleBurst(StartDelay, GameObjectList, ObjectDelay, NumberOfPoints, ObjectSpeed, ObjectDeathTimer));
	}
	public void Destroy_Children_CircleBurst(float StartDelay, float ObjectDelay, int NumberOfPoints, float ObjectSpeed, float ObjectDeathTimer)
	{
		StartCoroutine (C_Destroy_GameObjectList_CircleBurst(StartDelay, AttachedObjects, ObjectDelay, NumberOfPoints, ObjectSpeed, ObjectDeathTimer));
	}
	
	// Change the alpha of every child
	IEnumerator C_ChangeAlpha_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float AlphaValue, float inSeconds)
	{
		yield return new WaitForSeconds (StartDelay);
		
		foreach(GameObject child in GameObjectList)
		{
			ChangeAlpha (0, child, AlphaValue, inSeconds);
			
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}
	}
	public void ChangeAlpha_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float AlphaValue, float inSeconds)
	{
		StartCoroutine (C_ChangeAlpha_GameObjectList(StartDelay, GameObjectList, ObjectDelay, AlphaValue, inSeconds));
	}
	public void ChangeAlpha_Children(float StartDelay, float ObjectDelay, float AlphaValue, float inSeconds)
	{
		StartCoroutine (C_ChangeAlpha_GameObjectList(StartDelay, AttachedObjects, ObjectDelay, AlphaValue, inSeconds));
	}

	// Change the color of every child
	IEnumerator C_ChangeColor_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, Color NewColor, float inSeconds, bool ChangeForever)
	{
		yield return new WaitForSeconds (StartDelay);
		
		foreach(GameObject child in GameObjectList)
		{
			ChangeColor (0, child, NewColor, inSeconds, ChangeForever);
			
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}
	}
	public void ChangeColor_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, Color NewColor, float inSeconds, bool ChangeForever)
	{
		StartCoroutine (C_ChangeColor_GameObjectList(StartDelay, GameObjectList, ObjectDelay, NewColor, inSeconds, ChangeForever));
	}
	public void ChangeColor_Children(float StartDelay, float ObjectDelay, Color NewColor, float inSeconds, bool ChangeForever)
	{
		StartCoroutine (C_ChangeColor_GameObjectList(StartDelay, AttachedObjects, ObjectDelay, NewColor, inSeconds, ChangeForever));
	}

	// Apply ChangeScale on all objects of a list
	IEnumerator C_ChangeScale_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float X_Scale, float Y_Scale, float inSeconds)
	{
		yield return new WaitForSeconds (StartDelay);
		
		foreach(GameObject child in GameObjectList)
		{
			ChangeScale (0, child, X_Scale, Y_Scale, inSeconds);
			
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}
	}
	public void ChangeScale_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float X_Scale, float Y_Scale, float inSeconds)
	{
		StartCoroutine (C_ChangeScale_GameObjectList(StartDelay, GameObjectList, ObjectDelay, X_Scale, Y_Scale, inSeconds));
	}
	public void ChangeScale_Children(float StartDelay, float ObjectDelay, float X_Scale, float Y_Scale, float inSeconds)
	{
		StartCoroutine (C_ChangeScale_GameObjectList(StartDelay, AttachedObjects, ObjectDelay, X_Scale, Y_Scale, inSeconds));
	}

	// Change the kinematic state of every child
	IEnumerator C_ChangeKinematicState_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, bool isKinematic)
	{
		yield return new WaitForSeconds (StartDelay);
		
		foreach(GameObject child in GameObjectList)
		{
			child.GetComponent<Rigidbody2D>().isKinematic = isKinematic;
			
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}
	}
	public void ChangeKinematicState_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, bool isKinematic)
	{
		StartCoroutine (C_ChangeKinematicState_GameObjectList(StartDelay, GameObjectList, ObjectDelay, isKinematic));
	}
	public void ChangeKinematicState_Children(float StartDelay, float ObjectDelay, bool isKinematic)
	{
		StartCoroutine (C_ChangeKinematicState_GameObjectList(StartDelay, AttachedObjects, ObjectDelay, isKinematic));
	}

	// Change the kinematic state of every child
	IEnumerator C_ChangeGravity_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float newGravityScale)
	{
		yield return new WaitForSeconds (StartDelay);
		
		foreach(GameObject child in GameObjectList)
		{
			child.GetComponent<Rigidbody2D>().gravityScale = newGravityScale;
			
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}
	}
	public void ChangeGravity_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float newGravityScale)
	{
		StartCoroutine (C_ChangeGravity_GameObjectList(StartDelay, GameObjectList, ObjectDelay, newGravityScale));
	}
	public void ChangeGravity_GameObjectList(float StartDelay, float ObjectDelay, float newGravityScale)
	{
		StartCoroutine (C_ChangeGravity_GameObjectList(StartDelay, AttachedObjects, ObjectDelay, newGravityScale));
	}

	// Create a wave effect on every child
	IEnumerator C_WaveEffectObject_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y, bool LerpMovement)
	{
		yield return new WaitForSeconds (StartDelay);
		int I=0;
		
		foreach(GameObject child in GameObjectList)
		{
			WaveEffectObject (0, child, Amplitude_X, PeriodSpeed_X, Amplitude_Y, PeriodSpeed_Y, I, LerpMovement);
			I++;
		}
	}
	public void WaveEffectObject_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y, bool LerpMovement)
	{
		StartCoroutine (C_WaveEffectObject_GameObjectList(StartDelay, GameObjectList, Amplitude_X, PeriodSpeed_X, Amplitude_Y, PeriodSpeed_Y, LerpMovement));
	}
	public void WaveEffectObject_Children(float StartDelay, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y, bool LerpMovement)
	{
		StartCoroutine (C_WaveEffectObject_GameObjectList(StartDelay, AttachedObjects, Amplitude_X, PeriodSpeed_X, Amplitude_Y, PeriodSpeed_Y, LerpMovement));
	}

	// Make each child move twords a target
	IEnumerator C_BreakToTarget_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float ObjectSpeed)
	{
		yield return new WaitForSeconds (StartDelay);

		foreach(GameObject child in GameObjectList)
		{
			child.GetComponent<Rigidbody2D>().velocity = (ReturnProperPoint (TargetObject, TargetPoint) - child.transform.position).normalized * ObjectSpeed;

			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}
	}
	public void BreakToTarget_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float ObjectSpeed)
	{
		StartCoroutine (C_BreakToTarget_GameObjectList(StartDelay, GameObjectList, TargetObject, TargetPoint, ObjectDelay, ObjectSpeed));
	}
	public void BreakToTarget_Children(float StartDelay, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float ObjectSpeed)
	{
		StartCoroutine (C_BreakToTarget_GameObjectList(StartDelay, AttachedObjects, TargetObject, TargetPoint, ObjectDelay, ObjectSpeed));
	}

	// Make each child move randomly in a direction
	IEnumerator C_BreakRandomly_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float ObjectSpeed)
	{
		yield return new WaitForSeconds (StartDelay);
		float RandomX;
		float RandomY;

		foreach(GameObject child in GameObjectList)
		{
			RandomX = Random.Range (-1f, 1);
			RandomY = Random.Range (-1f, 1);

			child.GetComponent<Rigidbody2D>().velocity = new Vector3 (RandomX, RandomY, 0).normalized * ObjectSpeed;
			
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}
	}
	public void BreakRandomly_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float ObjectSpeed)
	{
		StartCoroutine (C_BreakRandomly_GameObjectList(StartDelay, GameObjectList, ObjectDelay, ObjectSpeed));
	}
	public void BreakRandomly_Children(float StartDelay, float ObjectDelay, float ObjectSpeed)
	{
		StartCoroutine (C_BreakRandomly_GameObjectList(StartDelay, AttachedObjects, ObjectDelay, ObjectSpeed));
	}

	// Stop all velocity of every child
	IEnumerator C_StopVelocity_GameObjectList(float StartDelay, List<GameObject> GameObjectList)
	{
		yield return new WaitForSeconds (StartDelay);

		foreach(GameObject child in GameObjectList)
		{
			child.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		}
	}
	public void StopVelocity_GameObjectList(float StartDelay, List<GameObject> GameObjectList)
	{
		StartCoroutine (C_StopVelocity_GameObjectList(StartDelay, GameObjectList));
	}
	public void StopVelocity_Children(float StartDelay)
	{
		StartCoroutine (C_StopVelocity_GameObjectList(StartDelay, AttachedObjects));
	}

	// Apply RotateDegrees on all objects of a list
	IEnumerator C_RotateDegrees_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float RotateDegree, float inSeconds)
	{
		yield return new WaitForSeconds (StartDelay);
		
		foreach(GameObject child in GameObjectList)
		{
			RotateDegrees(0, child, RotateDegree, inSeconds);
			
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}
	}
	public void RotateDegrees_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float RotateDegree, float inSeconds)
	{
		StartCoroutine (C_RotateDegrees_GameObjectList(StartDelay, GameObjectList, ObjectDelay, RotateDegree, inSeconds));
	}
	public void RotateDegrees_Children(float StartDelay, float ObjectDelay, float RotateDegree, float inSeconds)
	{
		StartCoroutine (C_RotateDegrees_GameObjectList(StartDelay, AttachedObjects, ObjectDelay, RotateDegree, inSeconds));
	}

	// Apply RotateToDegree on all objects of a list
	IEnumerator C_RotateToDegree_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float RotateToThisDegree, float inSeconds, bool Clockwise)
	{
		yield return new WaitForSeconds (StartDelay);
		
		foreach(GameObject child in GameObjectList)
		{
			RotateToDegree(0, child, RotateToThisDegree, inSeconds, Clockwise);
			
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}
	}
	public void RotateToDegree_GameObjectList(float StartDelay, List<GameObject> GameObjectList, float ObjectDelay, float RotateToThisDegree, float inSeconds, bool Clockwise)
	{
		StartCoroutine (C_RotateToDegree_GameObjectList(StartDelay, GameObjectList, ObjectDelay, RotateToThisDegree, inSeconds, Clockwise));
	}
	public void RotateToDegree_Children(float StartDelay, float ObjectDelay, float RotateToThisDegree, float inSeconds, bool Clockwise)
	{
		StartCoroutine (C_RotateToDegree_GameObjectList(StartDelay, AttachedObjects, ObjectDelay, RotateToThisDegree, inSeconds, Clockwise));
	}

	// Apply RotateTwordsPoint on all objects of a list
	IEnumerator C_RotateTwordsPoint_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float RotateSpeed, bool ForeverRotateTwords)
	{
		yield return new WaitForSeconds (StartDelay);
		
		foreach(GameObject child in GameObjectList)
		{
			RotateTwordsPoint(0, child, TargetObject, TargetPoint, RotateSpeed, ForeverRotateTwords);
			
			if(ObjectDelay != 0)
				yield return new WaitForSeconds(ObjectDelay);
		}
	}
	public void RotateTwordsPoint_GameObjectList(float StartDelay, List<GameObject> GameObjectList, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float RotateSpeed, bool ForeverRotateTwords)
	{
		StartCoroutine (C_RotateTwordsPoint_GameObjectList(StartDelay, GameObjectList, TargetObject, TargetPoint, ObjectDelay, RotateSpeed, ForeverRotateTwords));
	}
	public void RotateTwordsPoint_Children(float StartDelay, GameObject TargetObject, Vector3 TargetPoint, float ObjectDelay, float RotateSpeed, bool ForeverRotateTwords)
	{
		StartCoroutine (C_RotateTwordsPoint_GameObjectList(StartDelay, AttachedObjects, TargetObject, TargetPoint, ObjectDelay, RotateSpeed, ForeverRotateTwords));
	}


// ACTION SPAWNS (Shoot)--------------------------------------------------------------------------------------------------------------------
// Calls in this section with an ObjectDeathTimer = 0 will be added to the AttachedObjects list
// One would assume that if you dont want them to die, then you want to have control over there at some future time
// Calls in this section with an NumberOfObjects = 0 will spawn objects infinitly
// Be careful not to use both infinite spawns with no death timers (A warning will be given if you attempt to)


	// Shoots a set of objects at a certain target
	// DegreeVariance is how varied the spray happens in the given direction
	IEnumerator C_Shoot_AtTarget(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, GameObject TargetObject, Vector3 TargetPoint,  float AppearDelay, int NumberOfObjects, float ObjectSpeed, float DegreeVariance, float ObjectDeathTimer)
	{
		yield return new WaitForSeconds(StartDelay);
		if(NumberOfObjects == 0 && ObjectDeathTimer == 0)
			print ("ShootAtTarget - WARNING - Infinite object spawning with no death timer.");

		DegreeVariance /= 2;

		GameObject newclone;
		Vector3 sprayDirection;
		int I = 0;

		while(I<NumberOfObjects || NumberOfObjects == 0)
		{
			if(ObjectDeathTimer <= 0)
				newclone = CreateClone (true);
			else
			{
				newclone = CreateClone (false);
				StartCoroutine (Destroy_GameObject(newclone, ObjectDeathTimer));
			}
			newclone.transform.position = ReturnProperPoint (SpawnTarget, SpawnPoint);

			if(DegreeVariance > 0)
			{
				Vector3 TargetDirection = (ReturnProperPoint(TargetObject, TargetPoint) - newclone.transform.position).normalized;
				float TargetDegree = Mathf.Atan2 (TargetDirection.y, TargetDirection.x);
				float RandomAngle = Random.Range (-DegreeVariance, DegreeVariance);
				RandomAngle = RandomAngle * Mathf.Deg2Rad;
				sprayDirection = new Vector3 (Mathf.Cos(RandomAngle + TargetDegree), Mathf.Sin(RandomAngle + TargetDegree));
				newclone.GetComponent<Rigidbody2D>().velocity = sprayDirection * ObjectSpeed;
			}
			else
			{
				newclone.GetComponent<Rigidbody2D>().velocity = ((ReturnProperPoint(TargetObject, TargetPoint) - newclone.transform.position).normalized) * ObjectSpeed;
			}

			I++;
			yield return new WaitForSeconds(AppearDelay);
		}
		
		yield return null;
	}
	public void Shoot_AtTarget(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, GameObject TargetObject, Vector3 TargetPoint, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float DegreeVariance, float ObjectDeathTimer)
	{
		StartCoroutine (C_Shoot_AtTarget(StartDelay, SpawnTarget, SpawnPoint, TargetObject, TargetPoint, AppearDelay, NumberOfObjects, ObjectSpeed, DegreeVariance, ObjectDeathTimer));
	}


	// Shoots a set of objects in a certain vector direction
	// DegreeVariance is how varied the spray happens in the given direction
	// Code is similar to Shoot_AtTarget, but this version always shoots in a certain vector direction instead of right at a target
	// I am sure there is a way to combine them and only have one function, but I will worry about that later.
	IEnumerator C_Shoot_InDirection(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, Vector3 TargetDirection, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float DegreeVariance, float ObjectDeathTimer)
	{
		yield return new WaitForSeconds(StartDelay);
		if(NumberOfObjects == 0 && ObjectDeathTimer == 0)
			print ("SprayInDirection - WARNING - Infinite object spawning with no death timer.");
		
		TargetDirection = TargetDirection.normalized;
		float TargetDegree = Mathf.Atan2 (TargetDirection.y, TargetDirection.x);
		
		DegreeVariance /= 2;
		
		GameObject newclone;
		Vector3 sprayDirection;
		int I = 0;
		
		while(I<NumberOfObjects || NumberOfObjects==0)
		{
			if(ObjectDeathTimer <= 0)
				newclone = CreateClone (true);
			else
			{
				newclone = CreateClone (false);
				StartCoroutine (Destroy_GameObject(newclone, ObjectDeathTimer));
			}
			newclone.transform.position = ReturnProperPoint (SpawnTarget, SpawnPoint);
			
			// Random spray variance, but objects may be slower / faster depending on that variance
//			Vector3 sprayDirection = (TargetDirection + new Vector3(Random.Range (-SprayVariance_X, SprayVariance_X), Random.Range (-SprayVariance_Y, SprayVariance_Y), 0));
			
			// Finds a random degree offset and calculates the new direction. Uses polar math, so it is always normalized
			float RandomAngle = Random.Range (-DegreeVariance, DegreeVariance);
			RandomAngle = RandomAngle * Mathf.Deg2Rad;
			sprayDirection = new Vector3 (Mathf.Cos(RandomAngle + TargetDegree), Mathf.Sin(RandomAngle + TargetDegree));
			
			newclone.GetComponent<Rigidbody2D>().velocity = sprayDirection * ObjectSpeed;
			
			I++;
			yield return new WaitForSeconds(AppearDelay);
		}
		
		yield return null;
	}
	public void Shoot_InDirection(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, Vector3 TargetDirection, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float DegreeVariance, float ObjectDeathTimer)
	{
		StartCoroutine (C_Shoot_InDirection(StartDelay, SpawnTarget, SpawnPoint, TargetDirection, AppearDelay, NumberOfObjects, ObjectSpeed, DegreeVariance, ObjectDeathTimer));
	}


	// Creates a set of objects that follows a certain target
	// Can stop the movement via Stop_FollowTarget
	// Is forced to be Kinematic
	IEnumerator C_Shoot_FollowTarget (float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, GameObject TargetObject, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
	{
		yield return new WaitForSeconds (StartDelay);
		GameObject newclone;

		for(int I=0; I<NumberOfObjects; I++)
		{
			if(ObjectDeathTimer <= 0)
				newclone = CreateClone (true);
			else
			{
				newclone = CreateClone (false);
				StartCoroutine (Destroy_GameObject(newclone, ObjectDeathTimer));
			}

			newclone.GetComponent<Rigidbody2D>().isKinematic = true;
			newclone.transform.position = ReturnProperPoint (SpawnTarget, SpawnPoint);

			FollowTarget(0, newclone, TargetObject, ObjectSpeed);
			yield return new WaitForSeconds (AppearDelay);
		}
	}
	public void Shoot_FollowTarget (float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, GameObject TargetObject, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
	{
		StartCoroutine (C_Shoot_FollowTarget (StartDelay, SpawnTarget, SpawnPoint, TargetObject, AppearDelay, NumberOfObjects, ObjectSpeed, ObjectDeathTimer));
	}




	// Sprays a set of objects outward from the target SpawnPoint
	// If SpawnPoint = null as used then it will use NewSpawnPoints location
	IEnumerator C_Shoot_RandomSprayOutwards(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
	{
		yield return new WaitForSeconds(StartDelay);
		if(NumberOfObjects == 0 && ObjectDeathTimer == 0)
			print ("RandomSprayOutwards - WARNING - Infinite object spawning with no death timer.");
		
		float RandomX, RandomY;
		GameObject newclone;
		int I = 0;
		
		while(I<NumberOfObjects || NumberOfObjects == 0)
		{
			if(ObjectDeathTimer <= 0)
				newclone = CreateClone (true);
			else
			{
				newclone = CreateClone (false);
				StartCoroutine (Destroy_GameObject(newclone, ObjectDeathTimer));
			}

			newclone.transform.position = ReturnProperPoint (SpawnTarget, SpawnPoint);

			RandomX = Random.Range (-1f, 1);
			RandomY = Random.Range (-1f, 1);			
			
			newclone.GetComponent<Rigidbody2D>().velocity = new Vector3 (RandomX, RandomY, 0).normalized * ObjectSpeed;

			I++;
			yield return new WaitForSeconds(AppearDelay);
		}
	}
	public void Shoot_RandomSprayOutwards(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
	{
		StartCoroutine (C_Shoot_RandomSprayOutwards(StartDelay, SpawnTarget, SpawnPoint, AppearDelay, NumberOfObjects, ObjectSpeed, ObjectDeathTimer));
	}


	// Shoots objects in an outward circular spray
	// Shoot objects in a TargetDirection and then changes its rotation (DegreeChange) after each spawn
	// Can have multiple objects per spawn (NumberOfObjectsPerSpawn), with a delay between them (DelayBetweenObjectSpawn)
	// Can cap how many degrees it will rotate before rotating in the opposite direction (A spray back and forth effect)
	// If DegreeCap = 0, it will constantly spiral
	// NumberOfObjects = The number of spawns that happen (if NumberOfObjectsPerSpawn = 1, then it will also be the number of objects created)
	// NumberOfObjects = 0, then infinite spawns
	// AppearDelay = The delay between each spawn
	IEnumerator C_Shoot_CircularSpray(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, Vector3 TargetDirection, int NumberOfObjectsPerSpawn, float DelayBetweenObjectSpawn, float DegreeChange, float DegreeCap, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
	{
		yield return new WaitForSeconds (StartDelay);
		if(NumberOfObjects == 0 && ObjectDeathTimer == 0)
			print ("C_Shoot_CircularSpray - WARNING - Infinite object spawning with no death timer.");
		
		float DegreeCounter = 0;
		int I = 0;
		
		while(I<NumberOfObjects || NumberOfObjects==0)
		{
			Shoot_InDirection (0, SpawnTarget, SpawnPoint, TargetDirection, DelayBetweenObjectSpawn, NumberOfObjectsPerSpawn, ObjectSpeed, 0, ObjectDeathTimer);
			TargetDirection = Quaternion.Euler(0, 0, DegreeChange) * TargetDirection;
			
			if(DegreeCap > 0)
			{
				DegreeCounter += DegreeChange;
				if(DegreeCounter >= DegreeCap/2)
					DegreeChange = -DegreeChange;
				
				if(DegreeCounter <= -DegreeCap/2)
					DegreeChange = -DegreeChange;
			}
			
			I++;
			yield return new WaitForSeconds(AppearDelay);
		}
		
	}

//	public void Shoot_CircularSpray(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, Vector3 TargetDirection, int NumberOfObjectsPerSpawn, float DelayBetweenObjectSpawn, float ObjectSpeed, float DegreeChange, float DegreeCap, int NumberOfObjects, float AppearDelay, float ObjectDeathTimer)
	public void Shoot_CircularSpray(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, Vector3 TargetDirection, int NumberOfObjectsPerSpawn, float DelayBetweenObjectSpawn, float DegreeChange, float DegreeCap, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
	{
		StartCoroutine (C_Shoot_CircularSpray(StartDelay, SpawnTarget, SpawnPoint, TargetDirection, NumberOfObjectsPerSpawn, DelayBetweenObjectSpawn, DegreeChange, DegreeCap, AppearDelay, NumberOfObjects, ObjectSpeed, ObjectDeathTimer));
	}


	// Creates a wave like line from the scripted object
	// For now this function will work best when using a TargetDirection in either (0,+/-1) or (+/-1,0)
	// Also only use either AmplitudeX and PeriodX or AmplitudeY and PeriodY (keep one set at 0)
	// There is a temporary limitation on trying to move and do a wave effect in the same way
	// Moving in the X direction and waving in the Y direction works, but moving in the X and waving in the X will cause wierd results for now.
	IEnumerator C_Shoot_WaveSpray(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, Vector2 TargetDirection, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y, bool LerpMovement, float ObjectDeathTimer)
	{
		yield return new WaitForSeconds(StartDelay);
		if(NumberOfObjects == 0 && ObjectDeathTimer == 0)
			print ("RandomSprayOutwards - WARNING - Infinite object spawning with no death timer.");
		
		GameObject newclone;
		int I = 0;
		
		while(I<NumberOfObjects || NumberOfObjects==0)
		{
			if(ObjectDeathTimer <= 0)
				newclone = CreateClone (true);
			else
			{
				newclone = CreateClone (false);
				StartCoroutine (Destroy_GameObject(newclone, ObjectDeathTimer));
			}
			newclone.transform.position = ReturnProperPoint (SpawnTarget, SpawnPoint);
			newclone.GetComponent<Rigidbody2D>().velocity = TargetDirection.normalized * ObjectSpeed;
			
			yield return null;
			WaveEffectObject(0, newclone, Amplitude_X, PeriodSpeed_X, Amplitude_Y, PeriodSpeed_Y, I, LerpMovement);
			
			I++;
			yield return new WaitForSeconds(AppearDelay);
		}
	}
	public void Shoot_WaveSpray(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, Vector2 TargetDirection, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y, bool LerpMovement, float ObjectDeathTimer)
	{
		StartCoroutine (C_Shoot_WaveSpray(StartDelay, SpawnTarget, SpawnPoint, TargetDirection, AppearDelay, NumberOfObjects, ObjectSpeed, Amplitude_X, PeriodSpeed_X, Amplitude_Y, PeriodSpeed_Y, LerpMovement, ObjectDeathTimer));
	}

	
	// Spawns objects randomly along the edges of the screen
	// Direction = The vector3 direction you want the objects to travel
	// Based on the direction you want the object to travel, this will set which screen edge it will spawn from
	// - If you want an object to go down, vector3(0,-1), the object will spawn along the top of the screen
	// - vector3(1,-1) would make objects go down and right and thus spawn from the top and left edges of the screen
	IEnumerator C_Shoot_FromScreenEdge(float StartDelay, Vector3 Direction, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
	{
		yield return new WaitForSeconds(StartDelay);
		if(NumberOfObjects == 0 && ObjectDeathTimer == 0)
			print ("ShootFromScreenEdge - WARNING - Infinite object spawning with no death timer.");
		
		List<Vector3> RandomPositions = new List<Vector3>();
		Direction = Direction.normalized;
		int I = 0;
		
		while(I<NumberOfObjects || NumberOfObjects == 0)
		{
			GameObject newclone;
			if(ObjectDeathTimer <= 0)
				newclone = CreateClone (true);
			else
			{
				newclone = CreateClone (false);
				StartCoroutine (Destroy_GameObject(newclone, ObjectDeathTimer));
			}
			
			// Random spawn point
			float RandomNumber = Random.Range (0, 1f);
			
			if(Direction.x > 0)	// Spawn Left Wall
			{
				RandomPositions.Add (Camera.main.ViewportToWorldPoint(new Vector3(0, RandomNumber, 10)));
			}
			if(Direction.x < 0) // Spawn Right Wall
			{
				RandomPositions.Add (Camera.main.ViewportToWorldPoint(new Vector3(1, RandomNumber, 10)));
			}
			if(Direction.y > 0) // Spawn Bottom Wall
			{
				RandomPositions.Add(Camera.main.ViewportToWorldPoint(new Vector3(RandomNumber, 0, 10)));
			}
			if(Direction.y < 0) // Spawn Top Wall
			{
				RandomPositions.Add (Camera.main.ViewportToWorldPoint(new Vector3(RandomNumber, 1, 10)));
			}
			if(Direction.x == 0 && Direction.y == 0)
			{
				RandomPositions.Add (new Vector3(0,0,0));
			}
			
			newclone.transform.position = RandomPositions[Random.Range (0,(RandomPositions.Count))];
			newclone.GetComponent<Rigidbody2D>().velocity = Direction*ObjectSpeed;
			
			RandomPositions.Clear ();
			
			I++;
			yield return new WaitForSeconds(AppearDelay);
		}
		yield return null;
	}
	public void Shoot_FromScreenEdge(float StartDelay, Vector3 Direction, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
	{
		StartCoroutine (C_Shoot_FromScreenEdge(StartDelay, Direction, AppearDelay, NumberOfObjects, ObjectSpeed, ObjectDeathTimer));
	}

	
	// Create a circle burst at a certain location
	// The circle is created at a certain location and expands outwards at a certain speed
	IEnumerator C_Shoot_CircleBurst(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, int NumberOfPoints, float ObjectSpeed, float ObjectDeathTimer)
	{
		yield return new WaitForSeconds(StartDelay);

		GameObject newclone;

		foreach(Vector3 ReturnValue in Calculate_Circle(NumberOfPoints, 1, 1, 1, 1))
		{
			if(ObjectDeathTimer <= 0)
				newclone = CreateClone (true);
			else
			{
				newclone = CreateClone (false);
				StartCoroutine (Destroy_GameObject(newclone, ObjectDeathTimer));
			}
			
			newclone.transform.position = ReturnProperPoint (SpawnTarget, SpawnPoint);
			newclone.GetComponent<Rigidbody2D>().velocity = (gameObject.transform.rotation)*ReturnValue*ObjectSpeed;
		}

		yield return null;
	}
	public void Shoot_CircleBurst(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, int NumberOfPoints, float ObjectSpeed, float ObjectDeathTimer)
	{
		StartCoroutine (C_Shoot_CircleBurst(StartDelay, SpawnTarget, SpawnPoint, NumberOfPoints, ObjectSpeed, ObjectDeathTimer));
	}




}

















/*
	// Creates a set of projectiles in a random direction that bounce off of objects
	// If BouceOffEachOther = true, the objects can collide with each other
	// If BouceOffEachOther = false, the objects will pass through each other
	// This is done by changing the layermask from default to monster
	// Due to the nature of this function, the spawned objects stats are modified and would be a pain to repool (they are just destroyed)
	// The bounce is done by attaching the bouncy material to the spawned objects, so unity is doing all of the calculations
	IEnumerator C_Shoot_RandomSprayBounce(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, float AppearDelay, int NumberOfObjects, float ObjectSpeed, bool BouceOffEachOther, float ObjectDeathTimer)
	{
		yield return new WaitForSeconds(StartDelay);
		
		float RandomX;
		float RandomY;
		int I = 0;
		GameObject newclone;
		
		while(I<NumberOfObjects || NumberOfObjects == 0)
		{
			newclone = Instantiate(GetGameObjectToSpawn(), transform.position, Quaternion.identity) as GameObject;
			NonPooledObjectCount ++;
			newclone.name = gameObject.name + " - clone - "+NonPooledObjectCount;
			
			if(newclone.rigidbody2D == false)
				newclone.AddComponent ("Rigidbody2D");
			newclone.rigidbody2D.isKinematic = false;
			newclone.rigidbody2D.gravityScale = ObjectsGravityScale;
			newclone.rigidbody2D.angularDrag = 0;
			newclone.rigidbody2D.fixedAngle = true;
			
			if(!BouceOffEachOther)
				newclone.layer = LayerMask.NameToLayer ("Monsters");
			
			CircleCollider2D TempCircle = newclone.AddComponent<CircleCollider2D> ();
			TempCircle.radius = 0.1f;
			TempCircle.sharedMaterial = BouncyMaterial;
			TempCircle.enabled = false;
			TempCircle.enabled = true;
			
			NonPooledObjects.Add(newclone);
			Destroy (newclone, ObjectDeathTimer);
			
			RandomX = Random.Range (-1f, 1);
			RandomY = Random.Range (-1f, 1);	
			newclone.transform.position = ReturnProperPoint (SpawnTarget, SpawnPoint);
			newclone.rigidbody2D.velocity = new Vector3 (RandomX, RandomY, 0).normalized * ObjectSpeed;
			
			I++;
			yield return new WaitForSeconds(AppearDelay);
		}
		yield return null;
	}
	public void Shoot_RandomSprayBounce(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, float AppearDelay, int NumberOfObjects, float ObjectSpeed, bool BouceOffEachOther, float ObjectDeathTimer)
	{
		StartCoroutine (C_Shoot_RandomSprayBounce(StartDelay, SpawnTarget, SpawnPoint, AppearDelay, NumberOfObjects, ObjectSpeed, BouceOffEachOther, ObjectDeathTimer));
	}

// Sprays a set of objects in a certain vector direction
	// Objects are non-dinematic and thus gravity will effect them
	// SprayVariance is the variance in which the objects will spawn. If = 0, they will all spawn in a line
	// Be careful of using large values for SprayVariance
	IEnumerator C_Shoot_SprayInDirection(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, Vector3 TargetDirection, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float SprayVariance_X, float SprayVariance_Y, float ObjectDeathTimer)
	{
		yield return new WaitForSeconds(StartDelay);
		if(NumberOfObjects == 0 && ObjectDeathTimer == 0)
			print ("SprayInDirection - WARNING - Infinite object spawning with no death timer.");

		float PercentArea = 90;
		PercentArea /= 2;

		GameObject newclone;
		int I = 0;
		
		while(I<NumberOfObjects || NumberOfObjects==0)
		{
			if(ObjectDeathTimer <= 0)
				newclone = CreateClone (true);
			else
			{
				newclone = CreateClone (false);
				StartCoroutine (Destroy_GameObject(newclone, ObjectDeathTimer));
			}
			newclone.transform.position = ReturnProperPoint (SpawnTarget, SpawnPoint);

//			Vector3 sprayDirection = TargetDirection.normalized + new Vector3(Random.Range (-SprayVariance, SprayVariance), Random.Range (-SprayVariance, SprayVariance));
//			Vector3 sprayDirection = TargetDirection.normalized + new Vector3(Random.Range (-SprayVariance_X, SprayVariance_X), Random.Range (-SprayVariance_Y, SprayVariance_Y), 0);

			// Random spray variance, but objects may be slower / faster depending on that variance
//			Vector3 sprayDirection = (TargetDirection + new Vector3(Random.Range (-SprayVariance_X, SprayVariance_X), Random.Range (-SprayVariance_Y, SprayVariance_Y), 0));

			// Finds a random degree offset and calculates the new direction. Uses polar math, so it is always normalized
			Vector3 sprayDirection = TargetDirection.normalized;
			float RandomAngle = Random.Range (-PercentArea, PercentArea);
			RandomAngle = RandomAngle * 3.14f / 180;
			sprayDirection = new Vector3 (Mathf.Cos(RandomAngle + Mathf.Atan2(TargetDirection.y, TargetDirection.y)), Mathf.Sin(RandomAngle + Mathf.Atan2(TargetDirection.y, TargetDirection.y)));

			newclone.rigidbody2D.velocity = sprayDirection * ObjectSpeed;
			
			I++;
			yield return new WaitForSeconds(AppearDelay);
		}
		
		yield return null;
	}
	public void Shoot_SprayInDirection(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, Vector3 TargetDirection, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float SprayVariance_X, float SprayVariance_Y, float ObjectDeathTimer)
	{
		StartCoroutine (C_Shoot_SprayInDirection(StartDelay, SpawnTarget, SpawnPoint, TargetDirection, AppearDelay, NumberOfObjects, ObjectSpeed, SprayVariance_X, SprayVariance_Y, ObjectDeathTimer));
	}

	// Shoots a set of objects at a certain target
	// Has a bool to try and target the player
	IEnumerator C_Shoot_AtTarget(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, GameObject TargetObject, Vector3 TargetPoint,  float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
	{
		yield return new WaitForSeconds(StartDelay);
		if(NumberOfObjects == 0 && ObjectDeathTimer == 0)
			print ("ShootAtTarget - WARNING - Infinite object spawning with no death timer.");
		
		GameObject newclone;
		int I = 0;

		while(I<NumberOfObjects || NumberOfObjects == 0)
		{
			if(ObjectDeathTimer <= 0)
				newclone = CreateClone (true);
			else
			{
				newclone = CreateClone (false);
				StartCoroutine (Destroy_GameObject(newclone, ObjectDeathTimer));
			}
			
			newclone.transform.position = ReturnProperPoint (SpawnTarget, SpawnPoint);
			newclone.rigidbody2D.velocity = (ReturnProperPoint(TargetObject, TargetPoint) - newclone.transform.position).normalized * ObjectSpeed;
			
			I++;
			yield return new WaitForSeconds(AppearDelay);
		}
		
		yield return null;
	}
	public void Shoot_AtTarget(float StartDelay, GameObject SpawnTarget, Vector3 SpawnPoint, GameObject TargetObject, Vector3 TargetPoint, float AppearDelay, int NumberOfObjects, float ObjectSpeed, float ObjectDeathTimer)
	{
		StartCoroutine (C_Shoot_AtTarget(StartDelay, SpawnTarget, SpawnPoint, TargetObject, TargetPoint, AppearDelay, NumberOfObjects, ObjectSpeed, ObjectDeathTimer));
	}
*/


