// RegalPrime 12-01-14 - CatBoss.cs

// I wont go too much in detail about this boss, but it uses the NewObjectSpawner.cs to do much of it
// A good attempt at my very first boss. It really should be rewritten to be more clean and commented, but to tell you the truth
// it seems like a lot of work. I would rather put that time twords making a different boss.

// Has a left hand / right hand / head
// Randoms between 5 attacks (C_PawStomp / C_PawShoot / C_PawCombine / C_SpinnyHands / C_PawSlam)
// When the hands die, the head does a final attack


using UnityEngine;
using System.Collections;

public class CatBoss : MonoBehaviour
{
	public AudioClip BossMusic;
	private AudioClip StartingMusic;

	public GameObject LeftArmObject;
	public GameObject RightArmObject;
	public GameObject LeftEar;
	public GameObject RightEar;
	public GameObject LeftEyeObject;
	public GameObject RightEyeObject;
	public GameObject Mouth;

	public GameObject ExitWarp;
	public GameObject BackgroundSpin;

	public GameObject BottomLeft;
	public GameObject BottomRight;

	public GameObject Projectile;
	public GameObject KittenProjectile;
	public GameObject EyeballPrefab;

	public AudioClip IntroRumble;
	public AudioClip Pound1;
	public AudioClip ArmMove;
	public AudioClip Death1;
	public AudioClip Predeath;
	public AudioClip Bullet1;
	public AudioClip Bullet2;

	NewObjectSpawner CenterHead;
	NewObjectSpawner LeftArm;
	NewObjectSpawner RightArm;
	NewObjectSpawner LeftEye;
	NewObjectSpawner RightEye;
	NewObjectSpawner BottomLeftSpawn;
	NewObjectSpawner BottomRightSpawn;

	Vector3 CenterStartingPosition;
	Vector3 LeftArmStartingPosition;
	Vector3 RightArmStartingPosition;

	private bool StopColorCheck_Left = false;
	private bool StopColorCheck_Right = false;

	int LeftArmCurrentHP = 0;
	int RightArmCurrentHP = 0;

	void LateUpdate()		// Checks the current hp and changes the color of the hands
	{
		if(LeftArmObject.activeInHierarchy && !StopColorCheck_Left)
		{
			if(LeftArmCurrentHP != (LeftArmObject.GetComponent<HPModule> ().currentHP))
			{
				LeftArmCurrentHP = (LeftArmObject.GetComponent<HPModule> ().currentHP);
				switch(LeftArmCurrentHP)
				{
					case 1:
						{
							StopColorCheck_Left = true;
							LeftArm.ChangeColor_Children(0, 1);						
						}
						break;
					case 2:
						LeftArm.ChangeColor_Children(0, Color.red, 1);
						break;
					case 3:
						LeftArm.ChangeColor_Children(0, new Color32(255,128,0,255), 1);
						break;
					case 4:
						LeftArm.ChangeColor_Children(0, Color.yellow, 1);
						break;
				}
			}
		}

		if(RightArmObject.activeInHierarchy && !StopColorCheck_Right)
		{
			if(RightArmCurrentHP != (RightArmObject.GetComponent<HPModule> ().currentHP))
			{
				RightArmCurrentHP = (RightArmObject.GetComponent<HPModule> ().currentHP);
				switch(RightArmCurrentHP)
				{
					case 1:
						{
							StopColorCheck_Right = true;
							RightArm.ChangeColor_Children(0, 1);						
						}
						break;
					case 2:
						RightArm.ChangeColor_Children(0, Color.red, 1);
						break;
					case 3:
						RightArm.ChangeColor_Children(0, new Color32(255,128,0,255), 1);
						break;
					case 4:
						RightArm.ChangeColor_Children(0, Color.yellow, 1);
						break;
				}
			}
		}
	}

	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		BackgroundSpin.gameObject.SetActive(false);
		StopAllCoroutines ();

		MusicController.control.PlayNewClip (StartingMusic);

		CenterHead.ResetObjectSpawner();

		LeftArm.ResetObjectSpawner();
		RightArm.ResetObjectSpawner();
		StopColorCheck_Left = false;
		StopColorCheck_Right = false;
		LeftArmObject.SetActive (true);
		RightArmObject.SetActive (true);

		LeftEye.ResetObjectSpawner();
		RightEye.ResetObjectSpawner();
		BottomLeftSpawn.ResetObjectSpawner();
		BottomRightSpawn.ResetObjectSpawner();
		
		LeftEar.GetComponent<NewObjectSpawner> ().ResetObjectSpawner();
		RightEar.GetComponent<NewObjectSpawner> ().ResetObjectSpawner();
		Mouth.GetComponent<NewObjectSpawner> ().ResetObjectSpawner();

		RightArmObject.transform.position = RightArmStartingPosition;
		RightArmObject.transform.rotation = Quaternion.identity;
		LeftArmObject.transform.position = LeftArmStartingPosition;
		LeftArmObject.transform.rotation = Quaternion.identity;
		gameObject.transform.position = CenterStartingPosition;
//		LeftEyeObject.transform.rotation = Quaternion.identity;
//		RightEyeObject.transform.rotation = Quaternion.identity;

		ExitWarp.SetActive (true);
	}

	void Awake()		// Setup all of the objects
	{
		EventManager.resetObjects += Reset;
		LeftArmStartingPosition = LeftArmObject.transform.localPosition;
		RightArmStartingPosition = RightArmObject.transform.localPosition;
		CenterStartingPosition = gameObject.transform.localPosition;

		StartingMusic = MusicController.control.musicclip;

		Rigidbody2D TEMPBODY;
		NewObjectSpawner TEMP;

		BottomLeftSpawn = BottomLeft.AddComponent<NewObjectSpawner> ();
		BottomLeftSpawn.SetProjectile (0, Projectile);
		BottomLeftSpawn.SetColor_Random (0);
		BottomLeftSpawn.SetDeathTimer (0, 10);
		BottomRightSpawn = BottomRight.AddComponent<NewObjectSpawner> ();
		BottomRightSpawn.SetProjectile (0, Projectile);
		BottomRightSpawn.SetColor_Random (0);
		BottomRightSpawn.SetDeathTimer (0, 10);

		if(!BackgroundSpin.GetComponent<NewObjectSpawner>())
			BackgroundSpin.AddComponent<NewObjectSpawner> ();
		CenterHead = gameObject.AddComponent<NewObjectSpawner> ();
		CenterHead.SetProjectile (0, Projectile);
		CenterHead.SetColor (0, Color.white);
		TEMPBODY = gameObject.AddComponent<Rigidbody2D> ();
		TEMPBODY.isKinematic = true;

		TEMP = LeftEar.AddComponent<NewObjectSpawner> ();
		TEMP.SetProjectile (0, Projectile);
		TEMP.SetColor (0, Color.white);
		TEMP.RotateDegrees (0, 0, 45, true);

		TEMP = RightEar.AddComponent<NewObjectSpawner> ();
		TEMP.SetProjectile (0, Projectile);
		TEMP.SetColor (0, Color.white);
		TEMP.RotateDegrees (0, 0, 315, true);

		LeftEye = LeftEyeObject.AddComponent<NewObjectSpawner> ();
		LeftEye.SetProjectile (0, EyeballPrefab);
		LeftEye.SetColor (0, Color.white);

		RightEye = RightEyeObject.AddComponent<NewObjectSpawner> ();
		RightEye.SetProjectile (0, EyeballPrefab);
		RightEye.SetColor (0, Color.white);

		TEMP = Mouth.AddComponent<NewObjectSpawner> ();
		TEMP.SetProjectile (0, Projectile);
		TEMP.SetColor (0, Color.white);
		TEMP.RotateDegrees (0, 0, 260, true);

		LeftArm = LeftArmObject.AddComponent<NewObjectSpawner> ();
		LeftArm.SetProjectile (0, Projectile);
		LeftArm.SetColor (0, Color.white);
		TEMPBODY = LeftArmObject.AddComponent<Rigidbody2D> ();
		TEMPBODY.isKinematic = true;

		RightArm = RightArmObject.AddComponent<NewObjectSpawner> ();
		RightArm.SetProjectile (0, Projectile);
		RightArm.SetColor (0, Color.white);
		TEMPBODY = RightArmObject.AddComponent<Rigidbody2D> ();
		TEMPBODY.isKinematic = true;

	}

	void OnEnable()		// Create all of the objects
	{
//		Instantiate (BossMusic_Controller);
//		MusicController.control.Fade_Play_Unfade (BossMusic);
		MusicController.control.PlayNewClip (BossMusic);

		NewObjectSpawner TEMP;

		CenterHead.SetColor (0, Color.white);
		BackgroundSpin.SetActive (true);
		CenterHead.UnfadeObject (0, BackgroundSpin, 1f);

		TEMP = BackgroundSpin.GetComponent<NewObjectSpawner> ();
		TEMP.RotateDegrees (0, 0.1f, 0, false);

		TEMP = LeftEar.GetComponent<NewObjectSpawner> ();
		TEMP.CreateTriangle (0, 0.1f, 9, 0.25f, 0.25f);
		CenterHead.TESTCOMBINE (3, TEMP);

		TEMP = RightEar.GetComponent<NewObjectSpawner> ();
		TEMP.CreateTriangle (0, 0.1f, 9, 0.25f, 0.25f);
		CenterHead.TESTCOMBINE (3, TEMP);

		LeftEye = LeftEyeObject.GetComponent<NewObjectSpawner> ();
		LeftEye.CreateLineInDirection (0, 0, 1, new Vector2 (0, 0), 0);
		LeftEye.RotateDegrees (0, 0, 315, true);
//		LeftEye.RotateTwordsObject_Player (0, 5, false);

		RightEye = RightEyeObject.GetComponent<NewObjectSpawner> ();
		RightEye.CreateLineInDirection (0, 0, 1, new Vector2 (0, 0), 0);
		RightEye.RotateDegrees (0, 0, 315, true);
//		RightEye.RotateTwordsObject_Player (0, 5, false);

		TEMP = Mouth.GetComponent<NewObjectSpawner> ();
		TEMP.CreateCircle (0, 0.1f, 7, 0.5f, 0.5f, 2);
		TEMP.CreateCircle (0, 0.1f, 5, 0.05f, 0.05f, 1);
		TEMP.CreateLineInDirection_World (0, 0.1f, 2, new Vector2 (0, -1), 0.5f);

		CenterHead.TESTCOMBINE (3, TEMP);
		LeftArm.SetColor (0, Color.white);
		RightArm.SetColor (0, Color.white);

		CenterHead.CreateCircle (0, 0.05f, 30, 1, 1, 1);
		LeftArm.CreateSquare (0, 0.1f, 16, 0.5f, 0.4f);
		RightArm.CreateSquare (0, 0.1f, 16, 0.5f, 0.4f);
		LeftArm.SetColor_Random (3);
		RightArm.SetColor_Random (3);

		StartCoroutine (C_StartBattle());
	}


	IEnumerator C_StartBattle()		// Start battle and pick a random attack until the hands are disabled
	{
		CenterHead.PlayClip (0, IntroRumble);

		CenterHead.WaveEffectObject (0, 0.1f, 10, 0, 0);
		CenterHead.MoveObjectToPoint(3, CenterStartingPosition, 1);
		CenterHead.StopWaveEffect (3);

		LeftArm.WaveEffectObject (0, 0.5f, 5, 0, 0);
		LeftArm.StopWaveEffect (2);
		LeftArm.MoveObjectToPoint(3, LeftArmStartingPosition, 1);
		
		RightArm.WaveEffectObject (0, 0.5f, 5, 0, 0);
		RightArm.StopWaveEffect (2);
		RightArm.MoveObjectToPoint(3, RightArmStartingPosition, 1);

		yield return new WaitForSeconds(3f);
		LeftArm.SetProjectile (0, KittenProjectile);
		RightArm.SetProjectile (0, KittenProjectile);
//		CenterHead.SetProjectile (0, KittenProjectile);
		yield return new WaitForSeconds(1f);

//		while((LeftArmObject != null) || (RightArmObject != null))
		while((LeftArmObject.activeInHierarchy) || (RightArmObject.activeInHierarchy))
		{
 			int RandomNumber = Random.Range (0,5);
//			print(RandomNumber);

			switch(RandomNumber)
			{
				case 0:
					yield return StartCoroutine (C_PawStomp (0));
					break;
				case 1:
					yield return StartCoroutine (C_PawShoot (0));
					break;
				case 2:
					yield return StartCoroutine (C_PawCombine (0));
					break;
				case 3:
					yield return StartCoroutine (C_SpinnyHands (0));
					break;
				case 4:
					yield return StartCoroutine (C_PawSlam (0));
					break;
			}
		}

		yield return StartCoroutine (C_HeadAttack (0));


		
		yield return null;
	}

	// Final attack - head
	IEnumerator C_HeadAttack(float StartDelay)
	{
		CenterHead.WaveEffect_children (0, 0.8f, 30, 0, 0);
		CenterHead.ChangeColor_Children (0, Color.red, 0.3f);
		CenterHead.StopWaveEffect (3f);
		CenterHead.BreakRandomly (3f, 0, 2);

		CenterHead.SetColor (3, Color.red);
		CenterHead.RandomSprayOutwards (3.3f, 0.05f, 50, 2);
		CenterHead.SprayInDirection (3.3f, 30, new Vector2 (-1, 4), 0.1f, 4);
		CenterHead.SprayInDirection (3.3f, 30, new Vector2 (1, 4), 0.1f, 4);
		CenterHead.SprayInDirection (3.3f, 20, new Vector2 (0, 1), 0.2f, 1);
		CenterHead.DestroyChildren (5, 0.08f);

//		LeftEye.SetColor_Random (8);
		LeftEye.CircleBurst (8, 20, 1);
		LeftEye.BreakCircleBurst (8.5f, 20, 0, 1);

//		RightEye.SetColor_Random (8);
		RightEye.CircleBurst (8, 20, 1.2f);
		RightEye.BreakCircleBurst (9, 20, 0, 1);

		for (float I=3.1f; I<6.1f; I=I+0.1f)
			CenterHead.PlayClip (I, Bullet1);

		CenterHead.PlayClip (1, Predeath);
		RightEye.PlayClip (8, Death1);

		yield return new WaitForSeconds(13);
//		CenterHead.EnableObject (0, ExitWarp);
//		BackgroundSpin.SetActive (false);
		CenterHead.DisableObject (0, ExitWarp);
		BackgroundSpin.GetComponent<NewObjectSpawner> ().FadeObject (0, BackgroundSpin, 1);
		MusicController.control.FadeMusic ();
	}

	// Wiggle then stomp the ground with both paws
	IEnumerator C_PawStomp(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);

		CenterHead.WaveEffectObject (0, 0.1f, 10, 0, 0);
		CenterHead.MoveObjectToPoint(4, CenterStartingPosition, 1);
		CenterHead.StopWaveEffect (4);

		CenterHead.PlayClip (2.8f, ArmMove);
		CenterHead.PlayClip (3.5f, Pound1);

		if(LeftArm.gameObject.activeInHierarchy)
		{
			LeftArm.WaveEffectObject (0, 1, Random.Range (-2f, 2f), 0, 0);
			LeftArm.StopWaveEffect (2);
			LeftArm.MoveToPosition_Y (2, gameObject.transform, 1);
			LeftArm.MoveToPosition_Y (3, BottomLeft.transform, 2);
			LeftArm.MoveObjectToPoint(4, LeftArmStartingPosition, 1);
		}

		if(RightArm.gameObject.activeInHierarchy)
		{
			RightArm.WaveEffectObject (0, 1, Random.Range (-2f, 2f), 0, 0);
			RightArm.StopWaveEffect (2);
			RightArm.MoveToPosition_Y (2, gameObject.transform, 1);
			RightArm.MoveToPosition_Y (3, BottomLeft.transform, 2);
			RightArm.MoveObjectToPoint(4, RightArmStartingPosition, 1);
		}

		yield return new WaitForSeconds(7);
	}

	// Slam the ground with both paws and create a wave of objects
	IEnumerator C_PawSlam(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);

		CenterHead.PlayClip (0.9f, ArmMove);
		CenterHead.PlayClip (1.5f, Pound1);

		if(LeftArm.gameObject.activeInHierarchy)
		{
			LeftArm.MoveToPosition_Y (0, gameObject.transform, 2);
			LeftArm.MoveToPosition_Y (1, BottomLeft.transform, 4);
			BottomLeftSpawn.WaveSpray_Horizontal (1.2f, 8, 0.3f, 0.8f, 1.5f, 1f);
			LeftArm.MoveObjectToPoint(2f, LeftArmStartingPosition, 1);
		}
		
		if(RightArm.gameObject.activeInHierarchy)
		{
			RightArm.MoveToPosition_Y (0, gameObject.transform, 2);
			RightArm.MoveToPosition_Y (1, BottomLeft.transform, 4);
			BottomRightSpawn.WaveSpray_Horizontal (1.2f, 8, 0.3f, -0.8f, 1.5f, -1f);
			RightArm.MoveObjectToPoint(2f, RightArmStartingPosition, 1);
		}

		yield return new WaitForSeconds(6.5f);
	}

	// Shoot a set of objects at the player from each paw
	IEnumerator C_PawShoot(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);

		if(LeftArm.gameObject.activeInHierarchy)
		{
			LeftArm.SetDeathTimer (0, 5);
			LeftArm.MoveToPosition_Y (0, gameObject.transform, 1);
			LeftArm.ShootAtTarget_Player (2, 0.2f, 15, 2);
			LeftArm.MoveObjectToPoint(6, LeftArmStartingPosition, 1);
		}

		if(RightArm.gameObject.activeInHierarchy)
		{
			RightArm.SetDeathTimer (0, 5);
			RightArm.MoveToPosition_Y (0, gameObject.transform, 1);
			RightArm.ShootAtTarget_Player (2, 0.2f, 15, 2);
			RightArm.MoveObjectToPoint(6, RightArmStartingPosition, 1);
		}

		for (float I=2; I<5; I=I+0.2f)
			CenterHead.PlayClip (I, Bullet2);

		yield return new WaitForSeconds(7);
	}

	// Both paws go together and create random spray of objects
	IEnumerator C_PawCombine(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);

		if (LeftArm.gameObject.activeInHierarchy)
		{
			LeftArm.SetDeathTimer(0, 5);
			LeftArm.MoveToPosition_X (0, gameObject.transform, 1);
			LeftArm.RandomSprayOutwards (2, 0.1f, 50, 1f);
			LeftArm.MoveObjectToPoint(8, LeftArmStartingPosition, 1);
		}

		if(RightArm.gameObject.activeInHierarchy)
		{
			RightArm.SetDeathTimer(0, 5);
			RightArm.MoveToPosition_X (0, gameObject.transform, 1);
			RightArm.RandomSprayOutwards (2, 0.1f, 50, 1.1f);
			RightArm.MoveObjectToPoint(8, RightArmStartingPosition, 1);
		}

		for (float I=2; I<7; I=I+0.1f)
			CenterHead.PlayClip (I, Bullet1);

		yield return new WaitForSeconds(10);
	}

	// Spin and create a circle burst of objects from each paw
	IEnumerator C_SpinnyHands(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);

//		float RNG = Random.Range(0.50f, 0.55f);
		float RNG = 0.50f;

		if (LeftArm.gameObject.activeInHierarchy)
		{
			LeftArm.SetDeathTimer(0, 7);
			LeftArm.RotateDegrees(0, 1, 0, true);
			for(float I=0; I<5; I=I+RNG)
				LeftArm.CircleBurst(I, 10, 1);
			LeftArm.StopRotation(6);
			LeftArm.RotateToDegree (6, 1, 0, true);
		}

		if(RightArm.gameObject.activeInHierarchy)
		{
			RightArm.SetDeathTimer(0, 7);
			RightArm.RotateDegrees(0, 1, 0, true);
			for(float I=0; I<5; I=I+RNG)
				RightArm.CircleBurst(I, 10, 1);
			RightArm.StopRotation(6);
			RightArm.RotateToDegree (6, 1, 0, true);
		}

		for(float I=0; I<5; I=I+0.51f)
		{
			CenterHead.PlayClip (I, Bullet1);
		}


		yield return new WaitForSeconds(10);
	}
}





/*
 * 
// BACKUP OF CAT BOSS BEFORE I MESSED WITH EVERYTHING - DELETE THIS LATER IF EVERYTHING IS OK
using UnityEngine;
using System.Collections;

public class CatBoss : MonoBehaviour
{
	public GameObject BossMusic_Controller;

	public GameObject LeftArmObject;
	public GameObject RightArmObject;
	public GameObject LeftEar;
	public GameObject RightEar;
	public GameObject LeftEyeObject;
	public GameObject RightEyeObject;
	public GameObject Mouth;

	public GameObject ExitWarp;
	public GameObject BackgroundSpin;

	public GameObject BottomLeft;
	public GameObject BottomRight;

	public GameObject Projectile;
	public GameObject KittenProjectile;
	public GameObject EyeballPrefab;

	public AudioClip IntroRumble;
	public AudioClip Pound1;
	public AudioClip ArmMove;
	public AudioClip Death1;
	public AudioClip Predeath;
	public AudioClip Bullet1;
	public AudioClip Bullet2;

	NewObjectSpawner CenterHead;
	NewObjectSpawner LeftArm;
	NewObjectSpawner RightArm;
	NewObjectSpawner LeftEye;
	NewObjectSpawner RightEye;
	NewObjectSpawner BottomLeftSpawn;
	NewObjectSpawner BottomRightSpawn;

	Vector3 CenterStartingPosition;
	Vector3 LeftArmStartingPosition;
	Vector3 RightArmStartingPosition;

	private bool StopColorCheck_Left = false;
	private bool StopColorCheck_Right = false;

	void LateUpdate()
	{
		if(LeftArmObject != null && !StopColorCheck_Left)
		{
			int LeftArmCurrentHP = (LeftArmObject.GetComponent<HPModule> ().currentHP);
			switch(LeftArmCurrentHP)
			{
				case 2:
					{
						LeftArm.ChangeColor_Children(0, 1);
						StopColorCheck_Left = true;
					}
					break;
				case 4:
					LeftArm.ChangeColor_Children(0, Color.red, 1);
					break;
				case 6:
					LeftArm.ChangeColor_Children(0, new Color32(255,128,0,255), 1);
					break;
				case 8:
					LeftArm.ChangeColor_Children(0, Color.yellow, 1);
					break;
			}
		}

		if(RightArmObject != null && !StopColorCheck_Right)
		{
			int RightArmCurrentHP = (RightArmObject.GetComponent<HPModule> ().currentHP);
			switch(RightArmCurrentHP)
			{
				case 2:
					{
						RightArm.ChangeColor_Children(0, 1);
						StopColorCheck_Right = true;
					}
					break;
				case 4:
					RightArm.ChangeColor_Children(0, Color.red, 1);
					break;
				case 6:
					RightArm.ChangeColor_Children(0, new Color32(255,128,0,255), 1);
					break;
				case 8:
					RightArm.ChangeColor_Children(0, Color.yellow, 1);
					break;
			}
		}
	}

	void OnDestroy()
	{
		ResetManager.resetObjects -= Reset;
	}
	void Reset()
	{
		BackgroundSpin.SetActive(false);
		StopAllCoroutines ();
	}

	void Start()
	{
		ResetManager.resetObjects += Reset;
	}

	void OnEnable()
	{
		Instantiate (BossMusic_Controller);

		LeftArmStartingPosition = LeftArmObject.transform.localPosition;
		RightArmStartingPosition = RightArmObject.transform.localPosition;
		CenterStartingPosition = gameObject.transform.localPosition;

		BottomLeftSpawn = BottomLeft.AddComponent<NewObjectSpawner> ();
		BottomLeftSpawn.SetProjectile (0, Projectile);
		BottomLeftSpawn.SetColor_Random (0);
		BottomLeftSpawn.SetDeathTimer (0, 10);
		BottomRightSpawn = BottomRight.AddComponent<NewObjectSpawner> ();
		BottomRightSpawn.SetProjectile (0, Projectile);
		BottomRightSpawn.SetColor_Random (0);
		BottomRightSpawn.SetDeathTimer (0, 10);

		Rigidbody2D TEMPBODY;
		NewObjectSpawner TEMP;

		BackgroundSpin.SetActive(true);
		TEMP = BackgroundSpin.AddComponent<NewObjectSpawner> ();
		TEMP.RotateDegrees (0, 0.1f, 0, false);
		TEMP.AddSelfToList (0);
//		TEMP.UnfadeObject (0, 1);

		CenterHead = gameObject.AddComponent<NewObjectSpawner> ();
		TEMPBODY = gameObject.AddComponent<Rigidbody2D> ();
		TEMPBODY.isKinematic = true;
		CenterHead.SetProjectile (0, Projectile);
		CenterHead.SetColor (0, Color.white);

		TEMP = LeftEar.AddComponent<NewObjectSpawner> ();
		TEMP.SetProjectile (0, Projectile);
		TEMP.SetColor (0, Color.white);
		TEMP.RotateDegrees (0, 0, 45, true);
		TEMP.CreateTriangle (0, 0.1f, 9, 0.25f, 0.25f);
		CenterHead.TESTCOMBINE (3, TEMP);

		TEMP = RightEar.AddComponent<NewObjectSpawner> ();
		TEMP.SetProjectile (0, Projectile);
		TEMP.SetColor (0, Color.white);
		TEMP.RotateDegrees (0, 0, 315, true);
		TEMP.CreateTriangle (0, 0.1f, 9, 0.25f, 0.25f);
		CenterHead.TESTCOMBINE (3, TEMP);

		LeftEye = LeftEyeObject.AddComponent<NewObjectSpawner> ();
		LeftEye.SetProjectile (0, EyeballPrefab);
		LeftEye.SetColor (0, Color.white);
		LeftEye.RotateDegrees (0, 0, 315, true);
		LeftEye.CreateLineInDirection (0, 0, 1, new Vector2 (0, 0), 0);
		LeftEye.RotateTwordsObject_Player (0, 5, false);


		RightEye = RightEyeObject.AddComponent<NewObjectSpawner> ();
		RightEye.SetProjectile (0, EyeballPrefab);
		RightEye.SetColor (0, Color.white);
		RightEye.RotateDegrees (0, 0, 315, true);
		RightEye.CreateLineInDirection (0, 0, 1, new Vector2 (0, 0), 0);
		RightEye.RotateTwordsObject_Player (0, 5, false);


		TEMP = Mouth.AddComponent<NewObjectSpawner> ();
		TEMP.SetProjectile (0, Projectile);
		TEMP.SetColor (0, Color.white);
		TEMP.RotateDegrees (0, 0, 260, true);
		TEMP.CreateCircle (0, 0.1f, 7, 0.5f, 0.5f, 2);
		TEMP.CreateCircle (0, 0.1f, 5, 0.05f, 0.05f, 1);
		TEMP.CreateLineInDirection_World (0, 0.1f, 2, new Vector2 (0, -1), 0.5f);

		CenterHead.TESTCOMBINE (3, TEMP);

		LeftArm = LeftArmObject.AddComponent<NewObjectSpawner> ();
		TEMPBODY = LeftArmObject.AddComponent<Rigidbody2D> ();
		TEMPBODY.isKinematic = true;
		LeftArm.SetProjectile (0, Projectile);
		LeftArm.SetColor (0, Color.white);

		RightArm = RightArmObject.AddComponent<NewObjectSpawner> ();
		TEMPBODY = RightArmObject.AddComponent<Rigidbody2D> ();
		TEMPBODY.isKinematic = true;
		RightArm.SetProjectile (0, Projectile);
		RightArm.SetColor (0, Color.white);


		CenterHead.CreateCircle (0, 0.05f, 30, 1, 1, 1);
		LeftArm.CreateSquare (0, 0.1f, 16, 0.5f, 0.4f);
		RightArm.CreateSquare (0, 0.1f, 16, 0.5f, 0.4f);
		LeftArm.SetColor_Random (3);
		RightArm.SetColor_Random (3);


//		StartCoroutine (C_StartBattle());
	}


	IEnumerator C_StartBattle()
	{
		CenterHead.PlayClip (0, IntroRumble);

		CenterHead.WaveEffectObject (0, 0.1f, 10, 0, 0);
		CenterHead.MoveObjectToPoint(3, CenterStartingPosition, 1);
		CenterHead.StopWaveEffect (3);

		LeftArm.WaveEffectObject (0, 0.5f, 5, 0, 0);
		LeftArm.StopWaveEffect (2);
		LeftArm.MoveObjectToPoint(3, LeftArmStartingPosition, 1);
		
		RightArm.WaveEffectObject (0, 0.5f, 5, 0, 0);
		RightArm.StopWaveEffect (2);
		RightArm.MoveObjectToPoint(3, RightArmStartingPosition, 1);

		yield return new WaitForSeconds(3f);
		LeftArm.SetProjectile (0, KittenProjectile);
		RightArm.SetProjectile (0, KittenProjectile);
//		CenterHead.SetProjectile (0, KittenProjectile);
		yield return new WaitForSeconds(1f);

		while((LeftArmObject != null) || (RightArmObject != null))
		{
 			int RandomNumber = Random.Range (0,5);
//			print(RandomNumber);

			switch(RandomNumber)
			{
				case 0:
					yield return StartCoroutine (C_PawStomp (0));
					break;
				case 1:
					yield return StartCoroutine (C_PawShoot (0));
					break;
				case 2:
					yield return StartCoroutine (C_PawCombine (0));
					break;
				case 3:
					yield return StartCoroutine (C_SpinnyHands (0));
					break;
				case 4:
					yield return StartCoroutine (C_PawSlam (0));
					break;
			}
		}

		yield return StartCoroutine (C_HeadAttack (0));


		
		yield return null;
	}

	IEnumerator C_HeadAttack(float StartDelay)
	{
		CenterHead.WaveEffect_children (0, 0.8f, 30, 0, 0);
		CenterHead.ChangeColor_Children (0, Color.red, 0.3f);
		CenterHead.BreakRandomly (3f, 0, 2);
		CenterHead.StopWaveEffect (3f);

		CenterHead.SetColor (3, Color.red);
		CenterHead.RandomSprayOutwards (3.1f, 0.05f, 50, 2);
		CenterHead.SprayInDirection (3.1f, 30, new Vector2 (-1, 4), 0.1f, 4);
		CenterHead.SprayInDirection (3.1f, 30, new Vector2 (1, 4), 0.1f, 4);
		CenterHead.SprayInDirection (3.1f, 20, new Vector2 (0, 1), 0.2f, 1);
		CenterHead.DestroyChildren (5, 0.1f);

//		LeftEye.SetColor_Random (8);
		LeftEye.CircleBurst (8, 20, 1);
		LeftEye.BreakCircleBurst (8.5f, 20, 0, 1);

//		RightEye.SetColor_Random (8);
		RightEye.CircleBurst (8, 20, 1.2f);
		RightEye.BreakCircleBurst (9, 20, 0, 1);

		for (float I=3.1f; I<6.1f; I=I+0.1f)
			CenterHead.PlayClip (I, Bullet1);

		CenterHead.PlayClip (1, Predeath);
		RightEye.PlayClip (8, Death1);

		yield return new WaitForSeconds(13);
//		CenterHead.EnableObject (0, ExitWarp);
		CenterHead.DisableObject (0, ExitWarp);
		BackgroundSpin.GetComponent<NewObjectSpawner> ().FadeObjects (0, 1);
		MusicController.control.FadeMusic ();
	}


	IEnumerator C_PawStomp(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);

		CenterHead.WaveEffectObject (0, 0.1f, 10, 0, 0);
		CenterHead.MoveObjectToPoint(4, CenterStartingPosition, 1);
		CenterHead.StopWaveEffect (4);

		CenterHead.PlayClip (2.8f, ArmMove);
		CenterHead.PlayClip (3.5f, Pound1);

		if(LeftArm != null)
		{
			LeftArm.WaveEffectObject (0, 1, Random.Range (-2f, 2f), 0, 0);
			LeftArm.StopWaveEffect (2);
			LeftArm.MoveToPosition_Y (2, gameObject.transform, 1);
			LeftArm.MoveToPosition_Y (3, BottomLeft.transform, 2);
			LeftArm.MoveObjectToPoint(4, LeftArmStartingPosition, 1);
		}

		if(RightArm != null)
		{
			RightArm.WaveEffectObject (0, 1, Random.Range (-2f, 2f), 0, 0);
			RightArm.StopWaveEffect (2);
			RightArm.MoveToPosition_Y (2, gameObject.transform, 1);
			RightArm.MoveToPosition_Y (3, BottomLeft.transform, 2);
			RightArm.MoveObjectToPoint(4, RightArmStartingPosition, 1);
		}

		yield return new WaitForSeconds(7);
	}

	IEnumerator C_PawSlam(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);

		CenterHead.PlayClip (0.9f, ArmMove);
		CenterHead.PlayClip (1.5f, Pound1);

		if(LeftArm != null)
		{
			LeftArm.MoveToPosition_Y (0, gameObject.transform, 2);
			LeftArm.MoveToPosition_Y (1, BottomLeft.transform, 4);
			BottomLeftSpawn.WaveSpray_Horizontal (1.2f, 8, 0.3f, 0.8f, 1.5f, 1f);
			LeftArm.MoveObjectToPoint(2f, LeftArmStartingPosition, 1);
		}
		
		if(RightArm != null)
		{
			RightArm.MoveToPosition_Y (0, gameObject.transform, 2);
			RightArm.MoveToPosition_Y (1, BottomLeft.transform, 4);
			BottomRightSpawn.WaveSpray_Horizontal (1.2f, 8, 0.3f, -0.8f, 1.5f, -1f);
			RightArm.MoveObjectToPoint(2f, RightArmStartingPosition, 1);
		}

		yield return new WaitForSeconds(6.5f);
	}

	IEnumerator C_PawShoot(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);

		if(LeftArm != null)
		{
			LeftArm.SetDeathTimer (0, 5);
			LeftArm.MoveToPosition_Y (0, gameObject.transform, 1);
			LeftArm.ShootAtTarget_Player (2, 0.2f, 15, 2);
			LeftArm.MoveObjectToPoint(6, LeftArmStartingPosition, 1);
		}

		if(RightArm != null)
		{
			RightArm.SetDeathTimer (0, 5);
			RightArm.MoveToPosition_Y (0, gameObject.transform, 1);
			RightArm.ShootAtTarget_Player (2, 0.2f, 15, 2);
			RightArm.MoveObjectToPoint(6, RightArmStartingPosition, 1);
		}

		for (float I=2; I<5; I=I+0.2f)
			CenterHead.PlayClip (I, Bullet2);

		yield return new WaitForSeconds(7);
	}

	IEnumerator C_PawCombine(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);

		if (LeftArm != null)
		{
			LeftArm.SetDeathTimer(0, 5);
			LeftArm.MoveToPosition_X (0, gameObject.transform, 1);
			LeftArm.RandomSprayOutwards (2, 0.1f, 50, 1f);
			LeftArm.MoveObjectToPoint(8, LeftArmStartingPosition, 1);
		}

		if(RightArm != null)
		{
			RightArm.SetDeathTimer(0, 5);
			RightArm.MoveToPosition_X (0, gameObject.transform, 1);
			RightArm.RandomSprayOutwards (2, 0.1f, 50, 1.1f);
			RightArm.MoveObjectToPoint(8, RightArmStartingPosition, 1);
		}

		for (float I=2; I<7; I=I+0.1f)
			CenterHead.PlayClip (I, Bullet1);

		yield return new WaitForSeconds(10);
	}


	IEnumerator C_SpinnyHands(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);

//		float RNG = Random.Range(0.50f, 0.55f);
		float RNG = 0.50f;

		if (LeftArm != null)
		{
			LeftArm.SetDeathTimer(0, 7);
			LeftArm.RotateDegrees(0, 1, 0, true);
			for(float I=0; I<5; I=I+RNG)
				LeftArm.CircleBurst(I, 10, 1);
			LeftArm.StopRotation(6);
			LeftArm.RotateToDegree (6, 1, 0, true);
		}

		if(RightArm != null)
		{
			RightArm.SetDeathTimer(0, 7);
			RightArm.RotateDegrees(0, 1, 0, true);
			for(float I=0; I<5; I=I+RNG)
				RightArm.CircleBurst(I, 10, 1);
			RightArm.StopRotation(6);
			RightArm.RotateToDegree (6, 1, 0, true);
		}

		for(float I=0; I<5; I=I+0.51f)
		{
			CenterHead.PlayClip (I, Bullet1);
		}


		yield return new WaitForSeconds(10);
	}
}


*/


