// RegalPrime 11-25-14 - SpinnyRoom.cs

// Castlevania 4 - Spinny room script
// Rotates the room, spawns many sets of medusas, and then spins the room again
// This uses the NewObjectSpawner.cs script to do all of the actions

using UnityEngine;
using System.Collections;

public class SpinnyRoom : MonoBehaviour
{
	public GameObject EntireRoom;
	public GameObject LeftSpawner;
	public GameObject RightSpawner;
	public GameObject EndPathSpawner;

	public GameObject MedusaHead_Left;
	public GameObject MedusaHead_Right;

	public Sprite BlockSpawner_Sprite;
	private GameObject BlockToSpawn;

	public AudioClip BlockAppearing;
	public AudioClip SpinEffect1;
	public AudioClip SpinEffect2;
	public AudioClip SpinEffect3;

	NewObjectSpawner O_LeftSpawner;
	NewObjectSpawner O_RightSpawner;
	NewObjectSpawner O_EndPathSpawner;

	private NewObjectSpawner Manipluation;
	private Vector3 RoomPosition;
	private Vector3 LeftSpawnPosition;
	private Vector3 RightSpawnPosition;

	void Awake()
	{
		EventManager.resetObjects += Reset;

		RoomPosition = EntireRoom.transform.position;
		LeftSpawnPosition = LeftSpawner.transform.position;
		RightSpawnPosition = RightSpawner.transform.position;

		BlockToSpawn = new GameObject ("CustomBlock");
		BlockToSpawn.AddComponent<SpriteRenderer> ().sprite = BlockSpawner_Sprite;
		BlockToSpawn.tag = "Ground";
		BlockToSpawn.transform.position = new Vector2 (-8, -8);
		BlockToSpawn.layer = LayerMask.NameToLayer ("SolidObjects");
		BlockToSpawn.AddComponent<BoxCollider2D> ();
		BlockToSpawn.transform.localScale *= 2;
		BlockToSpawn.GetComponent<SpriteRenderer> ().material = EntireRoom.GetComponent<SpriteRenderer> ().material;

		Manipluation = GetComponent<NewObjectSpawner> ();

		O_LeftSpawner = LeftSpawner.AddComponent<NewObjectSpawner> ();
		O_LeftSpawner.EnablePooling = false;
		O_LeftSpawner.SetProjectile (0, MedusaHead_Right);
		O_LeftSpawner.SetDeathTimer (0, 10);
		O_LeftSpawner.SetColor (0, Color.white);

		O_RightSpawner = RightSpawner.AddComponent<NewObjectSpawner> ();
		O_RightSpawner.EnablePooling = false;
		O_RightSpawner.SetProjectile (0, MedusaHead_Left);
		O_RightSpawner.SetDeathTimer (0, 10);
		O_RightSpawner.SetColor (0, Color.white);

		O_EndPathSpawner = EndPathSpawner.AddComponent<NewObjectSpawner> ();
		O_EndPathSpawner.SetProjectile (0, BlockToSpawn);
		O_EndPathSpawner.SetColor (0, Color.white);

//		StartCoroutine (C_StartRoomActions (0));
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void OnEnable()
	{
		EntireRoom.transform.parent = this.transform;
		O_LeftSpawner.WaveEffectObject (0, 0, 0, 2, 2);
		O_RightSpawner.WaveEffectObject (0, 0, 0, 2, 2);
		StartCoroutine (C_StartRoomActions (0));
	}
	void Reset()
	{
		StopAllCoroutines ();
		EntireRoom.transform.rotation = Quaternion.identity;
		EntireRoom.transform.position = RoomPosition;
		LeftSpawner.transform.position = LeftSpawnPosition;
		RightSpawner.transform.position = RightSpawnPosition;

		O_LeftSpawner.ResetObjectSpawner ();
		O_RightSpawner.ResetObjectSpawner ();
		O_EndPathSpawner.ResetObjectSpawner ();
		EntireRoom.transform.parent = null;
	}

	IEnumerator C_StartRoomActions(float StartDelay)
	{
		Manipluation.RotateDegrees (3, 0.1f, 90, false);

		for(float I=3; I<13; I=I+1f)
		{
		    Manipluation.PlayClip (I, SpinEffect1);
			Manipluation.PlayClip (I, SpinEffect2);
			Manipluation.PlayClip (I, SpinEffect3);
		}

		yield return (StartCoroutine (C_RandomMedusaSpam (14)));

		Manipluation.RotateDegrees (3, 0.1f, 90, false);
		for(float I=3; I<13; I=I+1f)
		{
			Manipluation.PlayClip (I, SpinEffect1);
			Manipluation.PlayClip (I, SpinEffect2);
			Manipluation.PlayClip (I, SpinEffect3);
		}

		O_EndPathSpawner.CreateLineInDirection (14, 0.7f, 9, new Vector2 (1, 0), 5);
		for(float I=14; I<14+9*0.7f; I=I+0.7f)
			Manipluation.PlayClip (I,BlockAppearing);

	}

	IEnumerator C_RandomMedusaSpam(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);

		for(int I=0; I<6; I++)
		{
			O_LeftSpawner.WaveSpray_Horizontal(0, 3+I, 0.5f, 1,1,1);
			O_RightSpawner.WaveSpray_Horizontal(0, 3+I, 0.5f, 1,-1,-1);

			yield return new WaitForSeconds(3+I*0.3f);
		}

		yield return new WaitForSeconds(3);
	}
}
