using UnityEngine;
using System.Collections;

public class AvoidanceTest : MonoBehaviour
{
	public AudioClip BossMusic;						// The boss music

	public GameObject SavePoint;
	public GameObject Entrance;
	public GameObject Exit;

	public GameObject ObjectToSpawn;				// The object that is used when spawning projectiles
	public GameObject BackGroundEffect1;

	public GameObject TriggerStart;
	public GameObject Pikachu_Left;
	public GameObject Pikachu_Right;

	private Vector3 Pikachu_Left_Starting;
	private Vector3 Pikachu_Right_Starting;

	private int NumberOfSpawners = 0;
	private ObjectCreationExtras Spawner_Main;			// The main objects script (not really used much but its there)
	private ObjectCreationExtras Spawner1;				// Reference to the object spawner
	private ObjectCreationExtras Spawner2;				// Reference to the object spawner
	private ObjectCreationExtras Spawner3;				// Reference to the object spawner
	private ObjectCreationExtras Spawner4;				// Reference to the object spawner

	private AudioClip BackupMusicClip;					// Backup of the song that was playing before the boss started


	void PlayerDied(GameObject TEMP)
	{
		print (string.Format("Death By {0} at location {1}. Stop the boss", TEMP.name, TEMP.transform.position));
		StopAllCoroutines ();
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Awake()
	{
		// PUT PRECALCULATION AND SETUP STUFF HERE
		Color Color_1 = Color.blue;
		Color_1.a = 0.90f;

		Color Color_2 = Color.red;
		Color_2.a = 0.90f;

		Color Color_3 = Color.yellow;
		Color_3.a = 0.90f;

		Color Color_4 = Color.white;
		Color_4.a = 0.90f;

		Pikachu_Left_Starting = Pikachu_Left.transform.position;
		Pikachu_Right_Starting = Pikachu_Right.transform.position;


		Spawner_Main = gameObject.AddComponent<ObjectCreationExtras> ();
		Spawner_Main.SetProjectile (0, BackGroundEffect1);
		Spawner_Main.SetColor (0, Color.white);

		Spawner1 = CreateNewObjectSpawner (ObjectToSpawn);
		Spawner1.SetColor (0, Color_1);

		Spawner2 = CreateNewObjectSpawner (ObjectToSpawn);
		Spawner2.SetColor (0, Color_2);

		Spawner3 = CreateNewObjectSpawner (ObjectToSpawn);
		Spawner3.SetColor (0, Color_3);

		Spawner4 = CreateNewObjectSpawner (ObjectToSpawn);
		Spawner4.SetColor (0, Color_4);


	}
	void Start()
	{
		BackupMusicClip = MusicController.control.musicclip;
	}
	void Reset()
	{
		// PUT HOW THE OBJECT RESETS ITSELF
		print ("BOSS IS RESETTING");
		StopAllCoroutines ();

		TriggerStart.SetActive (true);
		SavePoint.SetActive (true);
//		Entrance.SetActive (true);

		MusicController.control.PlayNewClip (BackupMusicClip);
		gameObject.GetComponent<BoxCollider2D> ().enabled = true;
		MusicController.control.Set_LoopSong (true);

		Pikachu_Left.transform.position = Pikachu_Left_Starting;
		Pikachu_Right.transform.position = Pikachu_Right_Starting;

		ResetObjectSpawner (Spawner_Main);
		ResetObjectSpawner (Spawner1);
		ResetObjectSpawner (Spawner2);
		ResetObjectSpawner (Spawner3);
		ResetObjectSpawner (Spawner4);

	}
	void OnEnable()
	{
		EventManager.resetObjects += Reset;
		EventManager.PlayerIsDead += PlayerDied;
	}
	void OnDisable()
	{
		EventManager.resetObjects -= Reset;
		EventManager.PlayerIsDead -= PlayerDied;
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Bullet")
		{
			BackupMusicClip = MusicController.control.musicclip;
			gameObject.GetComponent<BoxCollider2D>().enabled = false;
			StartCoroutine (StartBoss ());
		}
	}
	ObjectCreationExtras CreateNewObjectSpawner(GameObject ObjectToSpawn)
	{
		NumberOfSpawners++;
		GameObject TempGameObject = new GameObject ("ObjectCreator - " + NumberOfSpawners);
		ObjectCreationExtras TempScript = TempGameObject.AddComponent<ObjectCreationExtras> ();
		TempScript.SetProjectile (0, ObjectToSpawn);

		return TempScript;
	}
	void ResetObjectSpawner(ObjectCreation TEMP)
	{
		TEMP.gameObject.transform.position = new Vector3 (0, 0, 0);
		TEMP.gameObject.transform.localScale = new Vector3 (1, 1, 1);
		TEMP.gameObject.transform.rotation = Quaternion.identity;
		TEMP.ResetObjectSpawner ();
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





	IEnumerator StartBoss()
	{
		print ("GOGO BOSS TIME");

		TriggerStart.SetActive (false);
		SavePoint.SetActive (false);
//		Entrance.SetActive (false);

		MusicController.control.PlayNewClip (BossMusic);
		MusicController.control.Set_LoopSong (false);

		yield return StartCoroutine (Phase1 ());
		yield return StartCoroutine (Phase2 ());
		yield return StartCoroutine (Phase3 ());
		yield return StartCoroutine (Phase4 ());

		yield return StartCoroutine (Phase5 ());
		yield return StartCoroutine (Phase6 ());
		yield return StartCoroutine (Phase7 ());
		yield return StartCoroutine (Phase8 ());

		yield return StartCoroutine (Phase9 ());
		yield return StartCoroutine (Phase10 ());

		yield return StartCoroutine (Phase_Complete ());


		yield return null;
	}




	IEnumerator Phase1()
	{
		print ("PHASE 1");

		MusicController.control.SkipToPoint (0);
		Spawner1.Shoot_CircleBurst (1.25f, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (2.00f, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (3.50f, null, new Vector3( 0, 0), 15, 1.5f, 3);
		Spawner2.Shoot_CircleBurst (3.50f, null, new Vector3( 0, 0), 15, 2, 3);
		
		Spawner1.Shoot_CircleBurst (4.10f, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (4.95f, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (5.80f, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (6.65f, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (7.50f, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (8.30f, null, new Vector3( 1, 1), 15, 2, 3);
		
		Spawner1.Shoot_CircleBurst (10.25f, null, new Vector3(0, 0), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (10.25f, null, new Vector3(0, 0), 15, 1.5f, 3);

		yield return new WaitForSeconds(10.85f);
	}
	IEnumerator Phase2()
	{
		print ("PHASE 2");
		float SV = 10.85f;
		MusicController.control.SkipToPoint (SV);

		Spawner1.CreateLine_MultiLines (10.85f - SV, null, new Vector3 (0, 0), 0.4f, 5, 4, 8, 0,0, true);
		Spawner1.MorphTo_Circle_Children (14.40f-SV, null, new Vector3 (-2, 0), 0, 1, 1, 3.1f);

		Spawner2.CreateLine_MultiLines (17.75f - SV, null, new Vector3 (0, 0), 0.4f, 5, 4, 8, 0,0, true);
		Spawner2.MorphTo_Circle_Children (21.10f-SV, null, new Vector3 ( 2, 0), 0, 2, 2, 3.1f);

		Spawner1.RotateDegrees (24.40f-SV, null, 360, 4.95f);
		Spawner2.RotateDegrees (24.40f-SV, null, -360, 4.95f);

		Spawner1.BreakToTarget_Children (30.23f-SV, PlayersGameObject (), new Vector3 (0, 0), 0.00f, 4);
		Spawner2.BreakToTarget_Children (30.23f-SV, PlayersGameObject (), new Vector3 (0, 0), 0.00f, 4);

		Spawner1.ChangeAlpha_Children (30.23f-SV, 0.0f, 0, 2);
		Spawner2.ChangeAlpha_Children (30.23f-SV, 0.0f, 0, 2);

		Spawner1.Destroy_Children (32.23f-SV, 0);
		Spawner2.Destroy_Children (32.23f-SV, 0);

		yield return new WaitForSeconds(31.03f-SV);
		yield return null;
	}
	IEnumerator Phase3()
	{
		print ("PHASE 3");
		float SV = 31.03f;
		MusicController.control.SkipToPoint (SV);
		Spawner3.MoveObjectToPoint (0, null, new Vector2 (-2, 0), 0);
		Spawner4.MoveObjectToPoint (0, null, new Vector2 (2, 0), 0);

		Spawner3.CreateShape_Circle (31.03f-SV, null, new Vector3 (-2, 0), 0.38f, 9, 0.5f, 0.5f, 1, true);
		Spawner3.ChangeScale (34.73f-SV, null, 4, 4, 3.1f);

		Spawner4.CreateShape_Circle (37.74f-SV, null, new Vector3 (2, 0), 0.38f, 9, 0.5f, 0.5f, 1, true);
		Spawner4.ChangeScale (41.35f-SV, null, 4, 4, 3.1f);


		Spawner1.CreateLine_ToFrom (44.36f-SV, null, new Vector2 (-2, -2.5f), null, new Vector2 (2, -2.5f), 0.38f, 5, 0,0, true);
		Spawner1.CreateLine_ToFrom (44.36f-SV, null, new Vector2 (-2, 2.5f), null, new Vector2 (2, 2.5f), 0.38f, 5, 0,0, true);
		Spawner1.BreakRandomly_Children (46.32f-SV, 0, 2);
		Spawner1.ChangeAlpha_Children (46.32f-SV, 0, 0, 4);
		Spawner1.Destroy_Children (50.32f-SV, 0);


		Spawner3.RotateDegrees (46.32f-SV, null, 360, 1.45f);
		Spawner4.RotateDegrees (46.32f-SV, null, -360, 1.45f);


		Spawner2.CreateLine_ToFrom (47.87f-SV, null, new Vector2 (-2.5f, -2.5f), null, new Vector2 (2.5f, -2.5f), 0.38f, 5, 0,0, true);
		Spawner2.CreateLine_ToFrom (47.87f-SV, null, new Vector2 (-2.5f, 2.5f), null, new Vector2 (2.5f, 2.5f), 0.38f, 5, 0,0, true);
		Spawner2.BreakRandomly_Children (49.56f-SV, 0, 2);
		Spawner2.ChangeAlpha_Children (49.56f - SV, 0, 0, 4);
		Spawner2.Destroy_Children (53.56f - SV, 0);


		Spawner3.RotateDegrees (49.73f-SV, null, -360, 1.35f);
		Spawner4.RotateDegrees (49.73f-SV, null, 360, 1.35f);


		Spawner1.CreateLine_ToFrom (51.21f-SV, null, new Vector2 (-2.5f, -2.5f), null, new Vector2 (2.5f, -2.5f), 0.38f, 5, 0,0, true);
		Spawner1.CreateLine_ToFrom (51.21f-SV, null, new Vector2 (-2.5f, 2.5f), null, new Vector2 (2.5f, 2.5f), 0.38f, 5, 0,0, true);
		Spawner1.BreakToTarget_Children (52.89f - SV, null, new Vector2 (0, 0), 0, 4);
		Spawner1.Destroy_Children (56.89f - SV, 0);


		Spawner1.Shoot_CircleBurst (54.90f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (55.75f-SV, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (57.25f-SV, null, new Vector3( 0, 0), 15, 1.5f, 3);
		Spawner2.Shoot_CircleBurst (57.25f-SV, null, new Vector3( 0, 0), 15, 2, 3);

		yield return new WaitForSeconds (57.88f - SV);
		yield return null;
	}
	IEnumerator Phase4()
	{
		print ("PHASE 4");
		float SV = 57.88f;
		MusicController.control.SkipToPoint (SV);

		Spawner3.RotateAroundPoint (57.88f-SV, null, null, new Vector2(0,0), 3.5f, 2.5f, 0.75f, 270, true);
		Spawner3.RotateDegrees (57.88f - SV, null, 0, 30);
		Spawner4.RotateAroundPoint (57.88f-SV, null, null, new Vector2(0,0), 3.5f, 2.5f, 0.75f, 90, true);
		Spawner4.RotateDegrees (57.88f - SV, null, 0, 30);


		Spawner3.Shoot_AtTarget (63.78f-SV, null, new Vector2 (-4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (63.78f-SV, null, new Vector2 (-4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (63.78f-SV, null, new Vector2 ( 4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (63.78f-SV, null, new Vector2 ( 4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		
		Spawner3.Shoot_AtTarget (70.67f-SV, null, new Vector2 (-4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (70.67f-SV, null, new Vector2 (-4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (70.67f-SV, null, new Vector2 ( 4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (70.67f-SV, null, new Vector2 ( 4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		
		Spawner3.Shoot_AtTarget (77.19f-SV, null, new Vector2 (-4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (77.19f-SV, null, new Vector2 (-4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (77.19f-SV, null, new Vector2 ( 4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (77.19f-SV, null, new Vector2 ( 4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		
		Spawner3.Shoot_AtTarget (84.14f-SV, null, new Vector2 (-4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 6, 0, 4);
		Spawner3.Shoot_AtTarget (84.14f-SV, null, new Vector2 (-4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 6, 0, 4);
		Spawner3.Shoot_AtTarget (84.14f-SV, null, new Vector2 ( 4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 6, 0, 4);
		Spawner3.Shoot_AtTarget (84.14f-SV, null, new Vector2 ( 4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 6, 0, 4);


		Spawner3.Stop_RotateAround (82.95f-SV);
		Spawner4.Stop_RotateAround (82.95f-SV);
		Spawner3.Stop_ForeverRotate (82.92f-SV);
		Spawner4.Stop_ForeverRotate (82.92f-SV);

		Spawner3.BreakRandomly_Children (82.95f-SV, 0, 1);
		Spawner4.BreakRandomly_Children (82.95f-SV, 0, 1);
		
		Spawner3.ChangeAlpha_Children (82.95f-SV, 0.0f, 0, 1);
		Spawner4.ChangeAlpha_Children (82.95f-SV, 0.0f, 0, 1);
		
		Spawner3.Destroy_Children (83.95f-SV, 0);
		Spawner4.Destroy_Children (83.95f-SV, 0);


		Spawner1.Shoot_CircleBurst (85.13f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (86.00f-SV, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (87.42f-SV, null, new Vector3( 0, 0), 15, 1.5f, 3);
		Spawner2.Shoot_CircleBurst (87.42f-SV, null, new Vector3( 0, 0), 15, 2, 3);


		yield return new WaitForSeconds(88.1f-SV);

		ResetObjectSpawner (Spawner3);
		ResetObjectSpawner (Spawner4);
		yield return null;
	}
	IEnumerator Phase5()
	{
		print ("PHASE 5");
		float SV = 88.1f;
		MusicController.control.SkipToPoint (SV);

		Spawner1.Shoot_CircleBurst (88.10f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (88.90f-SV, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (89.75f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (90.60f-SV, null, new Vector3( 1, 1), 15, 2, 3);

		Spawner1.Shoot_CircleBurst (91.45f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (92.26f-SV, null, new Vector3( 1, 1), 15, 2, 3);

		Spawner1.Shoot_CircleBurst (94.15f-SV, null, new Vector3( 0, 0), 15, 1.5f, 3);
		Spawner2.Shoot_CircleBurst (94.15f-SV, null, new Vector3( 0, 0), 15, 2, 3);

		yield return new WaitForSeconds (94.78f-SV);
		yield return null;
	}
	IEnumerator Phase6()
	{
		print ("PHASE 6");
		float SV = 94.78f;
		MusicController.control.SkipToPoint (SV);


		Spawner1.CreateLine_MultiLines (94.78f - SV, null, new Vector3 (0, 0), 0.8f, 5, 4, 4, 0,0, true);
		Spawner1.MorphTo_Square_Children (98.39f - SV, null, new Vector3 (-2, 0), 0, 0, 1.85f, 1.85f, 3.1f);
		
		Spawner2.CreateLine_MultiLines (101.53f - SV, null, new Vector3 (0, 0), 0.8f, 5, 4, 4, 0,0, true);
		Spawner2.MorphTo_Square_Children (105.05f - SV, null, new Vector3 (2, 0), 0, 0, 1.85f, 1.85f, 3.1f);
		
		Spawner1.RotateDegrees (108.22f-SV, null, 180, 4.95f);
		Spawner2.RotateDegrees (108.22f-SV, null, 180, 4.95f);

		Spawner1.BreakToTarget_Children (113.25f-SV, PlayersGameObject (), new Vector3 (0, 0), 0.00f, 2);
		Spawner2.BreakToTarget_Children (113.25f-SV, PlayersGameObject (), new Vector3 (0, 0), 0.00f, 2);

		Spawner1.BreakToTarget_Children (113.75f-SV, PlayersGameObject (), new Vector3 (0, 0), 0.00f, -4);
		Spawner2.BreakToTarget_Children (113.75f-SV, PlayersGameObject (), new Vector3 (0, 0), 0.00f, -4);
		
		Spawner1.ChangeAlpha_Children (113.75f-SV, 0.0f, 0, 2);
		Spawner2.ChangeAlpha_Children (113.75f-SV, 0.0f, 0, 2);
		
		Spawner1.Destroy_Children (115.25f-SV, 0);
		Spawner2.Destroy_Children (115.25f-SV, 0);


		yield return new WaitForSeconds(114.94f-SV);
		ResetObjectSpawner (Spawner1);
		ResetObjectSpawner (Spawner2);


		yield return null;
	}
	IEnumerator Phase7()
	{
		print ("PHASE 7");
		float SV = 114.94f;
		MusicController.control.SkipToPoint (SV);

		Spawner3.MoveObjectToPoint (0, null, new Vector2 (-2, 0), 0);
		Spawner4.MoveObjectToPoint (0, null, new Vector2 (2, 0), 0);
		
		Spawner3.CreateShape_Circle (114.94f-SV, null, new Vector3 (-2, 0), 0.38f, 9, 0.5f, 0.5f, 1, true);
		Spawner3.ChangeScale (118.50f-SV, null, 4, 4, 3.1f);
		
		Spawner4.CreateShape_Circle (121.65f-SV, null, new Vector3 (2, 0), 0.38f, 9, 0.5f, 0.5f, 1, true);
		Spawner4.ChangeScale (125.22f-SV, null, 4, 4, 3.1f);
		
		
		Spawner1.CreateLine_ToFrom (128.38f-SV, null, new Vector2 (-2, -2.5f), null, new Vector2 (2, -2.5f), 0.38f, 5, 0,0, true);
		Spawner1.CreateLine_ToFrom (128.38f-SV, null, new Vector2 (-2, 2.5f), null, new Vector2 (2, 2.5f), 0.38f, 5, 0,0, true);
		Spawner1.BreakRandomly_Children (130.25f-SV, 0, 2);
		Spawner1.ChangeAlpha_Children (130.25f - SV, 0, 0, 4);
		Spawner1.Destroy_Children (134.25f - SV, 0);
		
		
		Spawner3.RotateDegrees (130.25f-SV, null, 360, 1.45f);
		Spawner4.RotateDegrees (130.25f-SV, null, -360, 1.45f);
		
		
		Spawner2.CreateLine_ToFrom (131.73f-SV, null, new Vector2 (-2.5f, -2.5f), null, new Vector2 (2.5f, -2.5f), 0.38f, 5, 0,0, true);
		Spawner2.CreateLine_ToFrom (131.73f-SV, null, new Vector2 (-2.5f, 2.5f), null, new Vector2 (2.5f, 2.5f), 0.38f, 5, 0,0, true);
		Spawner2.BreakRandomly_Children (133.57f-SV, 0, 2);
		Spawner2.ChangeAlpha_Children (133.57f - SV, 0, 0, 4);
		Spawner2.Destroy_Children (137.57f - SV, 0);
		
		
		Spawner3.RotateDegrees (133.57f-SV, null, -360, 1.35f);
		Spawner4.RotateDegrees (133.57f-SV, null, 360, 1.35f);
		
		
		Spawner1.CreateLine_ToFrom (135.1f-SV, null, new Vector2 (-2.5f, -2.5f), null, new Vector2 (2.5f, -2.5f), 0.38f, 5, 0,0, true);
		Spawner1.CreateLine_ToFrom (135.1f-SV, null, new Vector2 (-2.5f, 2.5f), null, new Vector2 (2.5f, 2.5f), 0.38f, 5, 0,0, true);
		Spawner1.BreakToTarget_Children (136.81f-SV, null, new Vector2 (0, 0), 0, 4);
		Spawner1.Destroy_Children (138.81f - SV, 0);
		


		Spawner1.Shoot_CircleBurst (138.80f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (139.66f-SV, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (141.18f-SV, null, new Vector3( 0, 0), 15, 1.5f, 3);
		Spawner2.Shoot_CircleBurst (141.18f-SV, null, new Vector3( 0, 0), 15, 2, 3);
		
		yield return new WaitForSeconds (141.78f - SV);
		yield return null;
	}
	IEnumerator Phase8()
	{
		print ("PHASE 8");
		float SV = 141.78f;
		MusicController.control.SkipToPoint (SV);

		Spawner3.RotateAroundPoint (141.78f-SV, null, null, new Vector2(0,0), 3.5f, 2.5f, 0.75f, 270, true);
		Spawner4.RotateAroundPoint (141.78f-SV, null, null, new Vector2(0,0), 3.5f, 2.5f, 0.75f, 90, true);

		Spawner3.RotateDegrees (141.78f-SV - SV, null, 0, 30);
		Spawner4.RotateDegrees (141.78f-SV, null, 0, 30);

		Spawner_Main.MoveObjectToPoint (147.3f-SV, Pikachu_Left, new Vector2 (-3, -1.5f), 0.5f);
		Spawner_Main.MoveObjectToPoint (147.3f-SV, Pikachu_Right, new Vector2 (3, -1.5f), 0.5f);

		Spawner_Main.MoveObjectToPoint (148.5f-SV, Pikachu_Left, Pikachu_Left_Starting, 1f);
		Spawner_Main.MoveObjectToPoint (148.5f-SV, Pikachu_Right, Pikachu_Right_Starting, 1f);


		Spawner3.Shoot_AtTarget (147.7f-SV, null, new Vector2 (-4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (147.7f-SV, null, new Vector2 (-4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (147.7f-SV, null, new Vector2 ( 4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (147.7f-SV, null, new Vector2 ( 4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		
		Spawner3.Shoot_AtTarget (154.58f-SV, null, new Vector2 (-4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (154.58f-SV, null, new Vector2 (-4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (154.58f-SV, null, new Vector2 ( 4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (154.58f-SV, null, new Vector2 ( 4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		
		Spawner3.Shoot_AtTarget (161.09f-SV, null, new Vector2 (-4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (161.09f-SV, null, new Vector2 (-4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (161.09f-SV, null, new Vector2 ( 4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		Spawner3.Shoot_AtTarget (161.09f-SV, null, new Vector2 ( 4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 4, 0, 4);
		
		Spawner3.Shoot_AtTarget (168.01f-SV, null, new Vector2 (-4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 6, 0, 4);
		Spawner3.Shoot_AtTarget (168.01f-SV, null, new Vector2 (-4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 6, 0, 4);
		Spawner3.Shoot_AtTarget (168.01f-SV, null, new Vector2 ( 4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 6, 0, 4);
		Spawner3.Shoot_AtTarget (168.01f-SV, null, new Vector2 ( 4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 6, 0, 4);
		
		
		
		Spawner3.Stop_RotateAround (167.05f-SV);
		Spawner4.Stop_RotateAround (167.05f-SV);
		Spawner3.Stop_ForeverRotate (167.05f-SV);
		Spawner4.Stop_ForeverRotate (167.05f-SV);
		
		Spawner3.BreakRandomly_Children (167.05f-SV, 0, 1);
		Spawner4.BreakRandomly_Children (167.05f-SV, 0, 1);
		
		Spawner3.ChangeAlpha_Children (167.05f-SV, 0.0f, 0, 1);
		Spawner4.ChangeAlpha_Children (167.05f-SV, 0.0f, 0, 1);
		
		Spawner3.Destroy_Children (168.05f-SV, 0);
		Spawner4.Destroy_Children (168.05f-SV, 0);
		
		

		Spawner1.Shoot_CircleBurst (169.07f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (169.87f-SV, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (171.37f-SV, null, new Vector3( 0, 0), 15, 1.5f, 3);
		Spawner2.Shoot_CircleBurst (171.37f-SV, null, new Vector3( 0, 0), 15, 2, 3);
		
		yield return new WaitForSeconds(172.14f-SV);
		ResetObjectSpawner (Spawner3);
		ResetObjectSpawner (Spawner4);
		yield return null;
	}

	IEnumerator Phase9()
	{
		print ("PHASE 9");
		float SV = 172.14f;
		MusicController.control.SkipToPoint (SV);

		Spawner1.Custom_CircleBurstEffect (172.15f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);
		Spawner2.Custom_CircleBurstEffect (172.84f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);
		Spawner1.Custom_CircleBurstEffect (173.76f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);
		Spawner2.Custom_CircleBurstEffect (174.58f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);
		Spawner1.Custom_CircleBurstEffect (175.46f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);
		Spawner2.Custom_CircleBurstEffect (176.23f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);
		Spawner1.Custom_CircleBurstEffect (177.08f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);
		Spawner2.Custom_CircleBurstEffect (177.96f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);
		Spawner1.Custom_CircleBurstEffect (178.90f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);
		Spawner2.Custom_CircleBurstEffect (179.63f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);
		Spawner1.Custom_CircleBurstEffect (180.45f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);
		Spawner2.Custom_CircleBurstEffect (181.27f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);
		Spawner1.Custom_CircleBurstEffect (182.17f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);
		Spawner2.Custom_CircleBurstEffect (182.97f-SV, new Vector2 (0, 0), 5 ,2 ,0.6f, 1.5f, 0.0f, 5, 2);


		yield return new WaitForSeconds (185.45f-SV);
		yield return null;
	}


	IEnumerator Phase10()
	{
		print ("PHASE 10");
		float SV = 185.45f;
		MusicController.control.SkipToPoint (SV);

		Spawner_Main.Custom_SingleExpandEffect (185.45f-SV, new Vector2 (-1, 1), 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect (186.25f-SV, new Vector2 ( 1, 1), 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect (187.13f-SV, new Vector2 (-1, 1), 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect (188.00f-SV, new Vector2 ( 1, 1), 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect (188.81f-SV, new Vector2 (-1, 1), 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect (189.67f-SV, new Vector2 ( 1, 1), 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect (190.52f-SV, new Vector2 (-1, 1), 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect (191.30f-SV, new Vector2 ( 1, 1), 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect (192.15f-SV, new Vector2 (-1, 1), 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect (192.99f-SV, new Vector2 ( 1, 1), 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect (193.86f-SV, new Vector2 (-1, 1), 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect (194.68f-SV, new Vector2 ( 1, 1), 50, 50, 3);
		
		Spawner_Main.Custom_SingleExpandEffect_Random (195.89f-SV, 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect_Random (196.76f-SV, 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect_Random (198.23f-SV, 50, 50, 3);
		Spawner_Main.Custom_SingleExpandEffect_Random (198.23f-SV, 50, 50, 3);

		Spawner1.Shoot_CircleBurst (185.45f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (186.25f-SV, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (187.13f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (188.00f-SV, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (188.81f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (189.67f-SV, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (190.52f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (191.30f-SV, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (192.15f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (192.99f-SV, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (193.86f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (194.68f-SV, null, new Vector3( 1, 1), 15, 2, 3);
		

		Spawner1.CreateShape_Circle (195.89f-SV, null, new Vector2 ( 0,  2f), 0, 30, 0.4f, 0.4f, 1, true);
		Spawner1.CreateShape_Circle (196.76f-SV, null, new Vector2 ( 0, -2f), 0, 30, 0.4f, 0.4f, 1, true);
		Spawner2.CreateShape_Circle (198.13f-SV, null, new Vector2 (-2,  0), 0, 30, 1.5f, 1.5f, 1, true);
		Spawner2.CreateShape_Circle (198.23f-SV, null, new Vector2 ( 2,  0), 0, 30, 1.5f, 1.5f, 1, true);

		Spawner1.RotateDegrees(198.9f-SV, null, 0, -30);
		Spawner2.RotateDegrees(198.9f-SV, null, 0,  30);

		Spawner3.Shoot_InDirection(198.9f-SV, null, new Vector2(-5,-3), new Vector2( 4,1), 0.2f, 110, 2, 30, 1.9f);	// was 0.085f, but not uses degrees
		Spawner3.Shoot_InDirection(198.9f-SV, null, new Vector2( 5,-3), new Vector2(-4,1), 0.2f, 110, 2, 30, 1.9f);	// was 0.085f, but not uses degrees

		Spawner4.Shoot_AtTarget (204.74f-SV, null, new Vector2 (-4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 2, 0, 4);
		Spawner4.Shoot_AtTarget (204.74f-SV, null, new Vector2 (-4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 2, 0, 4);
		Spawner4.Shoot_AtTarget (204.74f-SV, null, new Vector2 ( 4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 2, 0, 4);
		Spawner4.Shoot_AtTarget (204.74f-SV, null, new Vector2 ( 4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 2, 0, 4);
		
		Spawner4.Shoot_AtTarget (211.69f-SV, null, new Vector2 (-4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 2, 0, 4);
		Spawner4.Shoot_AtTarget (211.69f-SV, null, new Vector2 (-4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 2, 0, 4);
		Spawner4.Shoot_AtTarget (211.69f-SV, null, new Vector2 ( 4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 2, 0, 4);
		Spawner4.Shoot_AtTarget (211.69f-SV, null, new Vector2 ( 4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 2, 0, 4);

		Spawner4.Shoot_AtTarget (218.17f-SV, null, new Vector2 (-4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 2, 0, 4);
		Spawner4.Shoot_AtTarget (218.17f-SV, null, new Vector2 (-4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 2, 0, 4);
		Spawner4.Shoot_AtTarget (218.17f-SV, null, new Vector2 ( 4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 2, 0, 4);
		Spawner4.Shoot_AtTarget (218.17f-SV, null, new Vector2 ( 4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 2, 0, 4);
		
		Spawner4.Shoot_AtTarget (225.11f-SV, null, new Vector2 (-4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 6, 0, 4);
		Spawner4.Shoot_AtTarget (225.11f-SV, null, new Vector2 (-4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 6, 0, 4);
		Spawner4.Shoot_AtTarget (225.11f-SV, null, new Vector2 ( 4,  3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 6, 0, 4);
		Spawner4.Shoot_AtTarget (225.11f-SV, null, new Vector2 ( 4, -3), PlayersGameObject (), new Vector2 (0, 0), 0, 1, 6, 0, 4);


		Spawner1.Stop_ForeverRotate (224.04f-SV);
		Spawner2.Stop_ForeverRotate (224.04f-SV);


		Spawner1.BreakToTarget_Children (224.07f-SV, PlayersGameObject (), new Vector2 (0, 0), 0, 1);
		Spawner2.BreakToTarget_Children (224.07f-SV, PlayersGameObject (), new Vector2 (0, 0), 0, 1);

		Spawner1.StopVelocity_Children (224.17f-SV);
		Spawner2.StopVelocity_Children (224.17f-SV);

		Spawner1.BreakToTarget_Children (224.46f-SV, PlayersGameObject (), new Vector2 (0, 0), 0, 1);
		Spawner2.BreakToTarget_Children (224.46f-SV, PlayersGameObject (), new Vector2 (0, 0), 0, 1);

		Spawner1.StopVelocity_Children (224.57f-SV);
		Spawner2.StopVelocity_Children (224.57f-SV);

		Spawner1.BreakToTarget_Children (224.85f-SV, PlayersGameObject (), new Vector2 (0, 0), 0, -4);
		Spawner2.BreakToTarget_Children (224.85f-SV, PlayersGameObject (), new Vector2 (0, 0), 0, -4);

		Spawner1.Destroy_Children (226.86f-SV, 0);
		Spawner2.Destroy_Children (226.86f-SV, 0);


		Spawner1.Shoot_CircleBurst (226.12f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (226.95f-SV, null, new Vector3( 1, 1), 15, 2, 3);
		Spawner1.Shoot_CircleBurst (228.42f-SV, null, new Vector3(-1, 1), 15, 2, 3);
		Spawner2.Shoot_CircleBurst (228.42f-SV, null, new Vector3( 1, 1), 15, 2, 3);

		Spawner_Main.MoveObjectToPoint (228.22f-SV, Pikachu_Left, new Vector2 (-3, -1.5f), 0.5f);
		Spawner_Main.MoveObjectToPoint (228.22f-SV, Pikachu_Right, new Vector2 (3, -1.5f), 0.5f);
		
		Spawner_Main.MoveObjectToPoint (229.42f-SV, Pikachu_Left, Pikachu_Left_Starting, 1f);
		Spawner_Main.MoveObjectToPoint (229.42f-SV, Pikachu_Right, Pikachu_Right_Starting, 1f);

		yield return new WaitForSeconds (230.00f-SV);
		yield return null;
	}

	IEnumerator Phase_Complete()
	{
		print ("BOSS COMPLETE");

		Exit.SetActive (true);
		yield return null;
	}



}



/*

*/