using UnityEngine;
using System.Collections;

public class TestCombo : MonoBehaviour
{
	public GameObject Projectile1;

	public GameObject Exit;
	public AudioClip ExitClip;

	public Transform UpperLeftCorner;
	public Transform UpperRightCorner;
	public Transform BottomLeftCorner;
	public Transform BottomRightCorner;
	public Transform CenterOfScreen;

	private int ObjectCount = 0;

	NewObjectSpawner CreateNewObjectSpawner()
	{
		ObjectCount++;
		Rigidbody2D TEMPBODY;
		
		GameObject clone = new GameObject();
		clone.transform.position = gameObject.transform.position;
		clone.name = "ObjectSpawner - " + ObjectCount;
		TEMPBODY = clone.AddComponent<Rigidbody2D> ();
		TEMPBODY.isKinematic = true;
		
		return clone.AddComponent<NewObjectSpawner> ();
	}

	void Start ()
	{
		NewObjectSpawner Object1 = CreateNewObjectSpawner ();
		Object1.SetProjectile (0, Projectile1);
		Object1.SetColor (0, Color.red);

		NewObjectSpawner Object2 = CreateNewObjectSpawner ();
		Object2.SetProjectile (0, Projectile1);
		Object2.SetColor (0, Color.blue);

		NewObjectSpawner Object3 = CreateNewObjectSpawner ();
		Object3.SetProjectile (0, Projectile1);
		Object3.SetColor (0, Color.green);

		NewObjectSpawner Object4 = CreateNewObjectSpawner ();
		Object4.SetProjectile (0, Projectile1);
		Object4.SetColor (0, Color.yellow);

		NewObjectSpawner Object5 = CreateNewObjectSpawner ();
		Object5.SetProjectile (0, Projectile1);
		Object5.SetColor (0, Color.gray);



		Object1.CreateCircle (0, 0.1f, 12, 1, 1, 1);
		Object1.MoveToPosition_Player_X (2, 1);
		Object1.MoveToPosition_Player_Y (2, 1);




	}

	// Testing multiple objects doing stuff
	void Action3()
	{
		NewObjectSpawner Object1 = CreateNewObjectSpawner ();
		Object1.SetProjectile (0, Projectile1);
		Object1.SetColor (0, Color.blue);

		NewObjectSpawner Object2 = CreateNewObjectSpawner ();
		Object2.SetProjectile (0, Projectile1);
		Object2.SetColor (0, Color.red);

		NewObjectSpawner Object3 = CreateNewObjectSpawner ();
		Object3.SetProjectile (0, Projectile1);
		Object3.SetColor (0, Color.green);


		Object1.CreateCircle (1, 0.1f, 25, 1, 1, 2f);
		Object2.RotateToDegree (0, null, 0, 180, true);
		Object2.CreateCircle (3.7f, 0.1f, 25, 1, 1, 2f);

		Object1.RotateDegrees (7, null, 2, 180, true);
		Object2.RotateDegrees (7, null, 2, 180, true);

		Object1.LaunchAtTarget (9, UpperLeftCorner, 2);
		Object1.StopMovement (10);
		Object2.LaunchAtTarget (9, UpperRightCorner, 2);
		Object2.StopMovement (10);

		Object1.MorphToSquare (9, 1, 1, 0f, 2.5f);
		Object2.MorphToCircle (9, 1, 1, 0f, 2.5f);

		Object1.ChangeColor_Children (10, Color.red, 1);
		Object2.ChangeColor_Children (10, Color.blue, 1);

		Object1.MorphToCircle (12, 1, 1, 0f, 2);
		Object2.MorphToSquare (12, 1, 1, 0f, 2);

		Object2.MorphToCircle (14, 1, 1, 0f, 2);
		Object1.MoveToPosition (14, CenterOfScreen, 2);
		Object2.MoveToPosition (14, CenterOfScreen, 2);

		Object1.ChangeScale (16, 2, 1, false);

		Object1.RotateDegrees (18, null, 1, 0, true);
		Object2.RotateDegrees (18, null, -1, 0, true);

		Object1.StopRotation (21);
		Object2.StopRotation (21);

		Object3.MoveToPosition (0, BottomLeftCorner, 0);
		Object3.CreateLineToTarget (21, 0.002f, 30, UpperRightCorner);

		Object1.ChangeColor_Children (21, 1);
		Object2.ChangeColor_Children (21, 1);

		Object3.BreakRandomly (22, 0.02f, 4);

		Object1.ChangeScale (24, 0.5f, 3, false);
		Object2.ChangeScale (24, 0.5f, 1, false);

		Object1.RotateDegrees (25, null, 1, 0, true);
		Object2.RotateDegrees (25, null, -1, 0, true);
		Object1.BreakRandomly (26, 0.1f, 2);
		Object2.BreakRandomly (26, 0.1f, 2);
		Object1.ChangeScale (26f, 3f, 1f, false);
		Object2.ChangeScale (26f, 3f, 1f, false);

		Object1.StopColorChangeEffect (28);
		Object2.StopColorChangeEffect (28);

		Object1.SprayInDirection (30, 40, new Vector3 (-0.25f, 1), 0.1f, 3);
		Object2.SprayInDirection (30, 40, new Vector3 (0.25f, 1), 0.1f, 3);
		Object3.SprayInDirection (30, 80, new Vector3 (0.5f, 1f), 0.05f, 7);

		Object1.EnableObject (33, Exit);
		Object1.PlayClip (33, ExitClip);
	}

	// This creates a pinwheel type object
	void Action1()
	{
		NewObjectSpawner Object1 = CreateNewObjectSpawner ();
		Object1.SetProjectile (0, Projectile1);

//		Object1.MoveObjectToPoint (0, Object1.gameObject, UpperLeftCorner.position, 0);
		Object1.CreateMultiLines_World (1, 0.1f, 5, 2, 10);
		Object1.RotateDegrees (1, null, 1, 360, true);
	}

	// Outward circular spiral - enlarge as it is built
	void Action2()
	{
		NewObjectSpawner Object1 = CreateNewObjectSpawner ();
		Object1.SetProjectile (0, Projectile1);
		
		Object1.CreateCircle_World (1, 0.1f, 25, 0.5f, 0.5f, 1f);
		Object1.RotateToDegree (1, null, 0.75f, 180, true);
		Object1.ChangeScale (1, 3.5f, 1f, false);

		Object1.ChangeColor_Children (5, 1f);
	}
}
