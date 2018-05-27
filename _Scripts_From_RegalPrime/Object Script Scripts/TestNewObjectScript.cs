// RegalPrime 5-22-15 - TestNewObjectScript.cs

// A script I wrote so I could easily test the functions of ObjectManipulation.cs / ObjectCreation.cs and its child classes
// This has no inengine use

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestNewObjectScript : MonoBehaviour
{
	private ObjectCreationExtras Test1;


	public List<GameObject> ObjectToSpawn;

	public GameObject AlternateObject1;
	public GameObject AlternateObject2;

	void Start ()
	{
		EventManager.resetObjects += Reset;
		Test1 = gameObject.AddComponent<ObjectCreationExtras> ();

		StartCoroutine (Starting ());
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		Test1.ResetObjectSpawner ();
		StopAllCoroutines ();
		StartCoroutine (Starting ());
	}
	public GameObject PlayersGameObject()
	{
		if(GameController.control.GetPlayerGameObject () != null)
		{
			return GameController.control.GetPlayerGameObject ();
		}
		else
		{
			print ("TRIED TO TARGET THE PLAYER BUT HE DOES NOT EXIST");
			return null;
		}
	}


	IEnumerator Starting()
	{
		yield return new WaitForSeconds(1f);
		Test1.SetEnablePooling (0, true);
		Test1.SetProjectile (0, ObjectToSpawn);
		Test1.SetColor (0, Color.white);
//		Test1.SetColor_Random(0);
		Test1.SetKinematic (0, true);
		Test1.SetGravityScale (0, 0.5f);
		Test1.SetSpawnScale (0, new Vector3 (1, 1, 1));
//		Test1.PrePoolObjects (0, 100);

		yield return new WaitForSeconds(0.02f);


//		Test1.CreateShape_Square (0, null, new Vector2 (1, 1), 0.1f, 10, -0.5f, 1, 1, false, true);
//		Test1.MorphTo_Polygon_Children (2f, null, new Vector2 (0, 0), 0.0f, 5, 1, 1.0f, -1.0f, 1, 1, 0.8f);










//		Test1.Custom_CircleBurstEffect (0, new Vector2 (0, 0), 10 ,1 ,1f, 1.5f, 0.0f, 5, 3);
//		Test1.Custom_SingleExpandEffect (0, new Vector2 ( 0,  0), 50, 50, 3);


//		Test1.MoveObjectToPoint (1, null, new Vector3 (1, 1), 1);
//		Test1.MoveObjectToPoint_Local (1, null, new Vector3 (1, 1), 1);

//		Test1.FollowTarget (0, null, AlternateObject1, 1);
//		Test1.Stop_FollowTarget (5);

//		Test1.WaveEffectObject (0, null, 1, 1, 0, 0, 1, true);
//		Test1.WaveEffectObject_Children (1, 1, 1, 1, 1, true);
//		Test1.Stop_WaveEffect (4);

//		Test1.RotateDegrees (0, null, 0, 15);
//		Test1.RotateDegrees_Children (1, 0.00f, 0, -90);
//		Test1.Stop_ForeverRotate (4);

//		Test1.RotateToDegree (0, null, 180, 1, false);
//		Test1.RotateToDegree_Children (4, 0.0f, 90, 1, true);

//		Test1.RotateTwordsPoint (0, null, PlayersGameObject(), new Vector2(0,2), 2, true);
//		Test1.RotateTwordsPoint_Children (1f, gameObject, new Vector2 (0, 0), 0.0f, 5, false);
//		Test1.Stop_RotateTwords (4);

//		Test1.RotateAroundPoint (0, null, null, new Vector2(0,0), 1f, 1, 1, 180, true);
//		Test1.Stop_RotateAround (4);

//		Test1.ChangeColor (0, AlternateObject1, Color.blue, 1, true);
//		Test1.ChangeColor_Children (2, 0.1f, Color.blue, 1.0f, false);
//		Test1.StopColorChangeEffect (4);

//		Test1.ChangeScale (1, null, 2, 1, 1);
//		Test1.ChangeScale_Children (1, 0.1f, 2, 2, 1);

//		Test1.ChangeAlpha (0, AlternateObject1, 0, 1);
//		Test1.ChangeAlpha_Children (1, 0.0f, 0.5f, 0);


//		Test1.Destroy_Children (2, 0.5f);
//		Test1.Destroy_Children_CircleBurst (2, 0.1f, 5, 1, 0);

//		Test1.ChangeKinematicState_Children (2, 0.01f, false);
//		Test1.ChangeGravity_GameObjectList (2.5f, 0, 0);

//		Test1.BreakToTarget_Children (4, null, new Vector2 (0, 0), 0.0f, 2);
//		Test1.BreakRandomly_Children (4, 0.0f, 1);
//		Test1.StopVelocity_Children (4);


//		Test1.CreateLine_ToFrom (0, null, new Vector2 (-1, 0), null, new Vector2 (0, 1), 0.1f, 10, 0, 0, false);
//		Test1.CreateLine_InDirection (0, null, new Vector3(-2,-2), new Vector3 (1, 1), 0.1f, 20, 2,  0, 0, false);
//		Test1.CreateLine_MultiLines (0, null, new Vector3 (0,2), 0.1f, 5, 2, 7, 0, 0, false);

//		Test1.CreateShape_Circle (0, null, new Vector2(1,1), 0.1f, 30, 1, 1, 1f, true);
//		Test1.CreateShape_Square (0, null, new Vector2 (1, 1), 0.1f, 10, 0f, 1, 1, true, true);
//		Test1.CreateShape_Triangle (0, null, new Vector2(1,1), 0.05f, 10, 0f, 1, 1, true, false);
//		Test1.CreateShape_Polygon (0, null, new Vector2 (0, 1.0f), 0.0f, 10, 10, 3, 1.0f, 0.0f, 1, 1, true, true);

//		Test1.Create_PolarCurve_abcoskt (0, null, new Vector2(0,0), 0.02f, 50, 1,1,1,2, 1,1, true);
//		Test1.Create_PolarCurve_Heart (0, null, new Vector2(0,0), 0.02f, 50, 1,1, true);
//		Test1.Create_PolarCurve_Spiral (0, null, new Vector2(0,0), 0.02f, 50, 1,1, true);
//		Test1.Create_PolarCurve (0, null, new Vector2 (0, 0), 0.05f, 50, 1, 1, t => Mathf.Cos (t), 1, true);


//		Test1.MorphTo_Line_Children (2, null, new Vector2 (-2, 0), null, new Vector2 (2, 0), 0.0f, 0, 0, 1);
//		Test1.MorphTo_Circle_Children (4, null, new Vector2(0,0), 0.0f, 1f, 1f, 1);
//		Test1.MorphTo_Square_Children (2, gameObject, new Vector3(0,0), 0.0f, -0.45f, 1, 1, 1);
//		Test1.MorphTo_Polygon_Children (2f, null, new Vector2 (0, 0), 0.0f, 5, 1, 1.0f, 0.0f, 1, 1, 0.8f);

//		Test1.MorphTo_PolarCurve_abcoskt_Children (2, null, new Vector2 (0, 0), 0.0f, 2,1,3,1, 1,1, 1);
//		Test1.MorphTo_PolarCurve_Heart_Children(2, null, new Vector2 (0,2), 0.0f, 1,1, 1);
//		Test1.MorphTo_PolarCurve_Spiral_Children(2, gameObject, new Vector2(0,0), 0.00f, 1,1, 4, 1);
//		Test1.MorphTo_PolarCurve_Children (2, null, new Vector2 (0, 0), 0.02f, 1, 1, 1f, t => 0.159f*t, 4);


//		Test1.Shoot_AtTarget (0, gameObject, new Vector2 (0, 2), AlternateObject2, new Vector2 (2, 0), 0.05f, 0, 2, 45, 2);
//		Test1.Shoot_InDirection (0, gameObject, new Vector3(0,0), new Vector2 (0, 1), 0.02f, 0, 2, 50, 1);
//		Test1.Shoot_FollowTarget (0, null, new Vector2 (0, 0), PlayersGameObject(), 0.5f, 5, 1, 5);

//		Test1.Shoot_RandomSprayOutwards (0, gameObject, new Vector3 (1, 0), 0.1f, 0, 3, 2);
//		Test1.Shoot_CircularSpray (1, gameObject, new Vector2 (0, 0), new Vector2 (0, -1), 1, 0.2f, 2, 15, 0, 0, 0.2f, 3);
//		Test1.Shoot_WaveSpray (0, null, new Vector3 (2, 0), new Vector2 (-1, 0), 0.1f, 0, 1, 0,0, 1,1, true, 5);

//		Test1.Shoot_FromScreenEdge (0, new Vector3(-1, -1), 0.3f, 0, 2, 5);
//		Test1.Shoot_CircleBurst (0, gameObject, new Vector3 (0, 0), 12, 2, 2);


		yield return new WaitForSeconds(10);
		print ("SECOND HAVE PASSED");

	}
}



















