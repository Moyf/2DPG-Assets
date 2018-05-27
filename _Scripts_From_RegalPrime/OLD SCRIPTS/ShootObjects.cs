using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootObjects : MonoBehaviour
{
	public Transform transform1;
	public Transform transform2;
	public Transform transform3;

	public GameObject warpObject;
	public AudioClip deathSound;

	public GameObject projectile;

	public float ObjectSpeed = 3;
	public float ObjectDeath = 5f;

	private Camera mainCamera;
	
	void Start ()
	{
		mainCamera = Camera.main;

		StartCoroutine(C_ShootFromTopScreen(0, transform1, 10, .1f));

		StartCoroutine(C_ShootPlayer(1, transform2, 10, .1f));
		StartCoroutine(C_ShootPlayer(1, transform3, 10, .1f));

		StartCoroutine(C_ShootLeft(3,transform2, 10, .1f));
		StartCoroutine(C_ShootLeft(3,transform3, 10, .1f));

		StartCoroutine(C_ShootFromOffScreen(5, transform1, 10, .5f));

		StartCoroutine(C_ShootPlayer(9, transform2, 20, 0.5f));
		StartCoroutine(C_ShootPlayer(9, transform3, 10, 1.0f));

		StartCoroutine(C_FollowPlayer(20, transform2, 0, 0));
		StartCoroutine(C_FollowPlayer(21, transform3, 0, 0));

		StartCoroutine(C_ShootFromOffScreen(27, transform1, 100, .1f));

		Invoke ("ActivateWarp", 27);
	}

	void Update()
	{

	}

	void ActivateWarp()
	{
		warpObject.SetActive(true);
		if(deathSound != null)
			MusicController.control.OneShotAudio(deathSound);
//			AudioSource.PlayClipAtPoint(deathSound, transform.position);
	}

	public IEnumerator C_FollowPlayer(float StartDelay, Transform spawnPoint, int ObjectAmount, float ObjectDelay)
	{
		yield return new WaitForSeconds(StartDelay);

		GameObject playerPosition;
		playerPosition = GameController.control.GuyLocation;
		
		GameObject newclone;
		newclone = Instantiate(projectile, spawnPoint.position, transform.rotation) as GameObject;
		newclone.AddComponent <Rigidbody2D>();
		newclone.GetComponent<Rigidbody2D>().freezeRotation = true;
		newclone.GetComponent<Rigidbody2D>().isKinematic = true;
		Destroy(newclone, ObjectDeath);

		while(newclone != null)
		{
			newclone.GetComponent<Rigidbody2D>().velocity = (playerPosition.transform.position - newclone.transform.position).normalized * ObjectSpeed; 
			yield return null;
		}

		yield return null;
	}


	// Shoots at the player
	public IEnumerator C_ShootPlayer(float StartDelay, Transform spawnPoint, int ObjectAmount, float ObjectDelay)
	{
		yield return new WaitForSeconds(StartDelay);

		GameObject playerPosition;
		playerPosition = GameController.control.GuyLocation;
		
		for(int I=0; I<ObjectAmount; I++)
		{
			GameObject newclone;
			newclone = Instantiate(projectile, spawnPoint.position, transform.rotation) as GameObject;
			newclone.AddComponent <Rigidbody2D>();
			newclone.GetComponent<Rigidbody2D>().freezeRotation = true;
			newclone.GetComponent<Rigidbody2D>().isKinematic = true;
			
			newclone.GetComponent<Rigidbody2D>().velocity = (playerPosition.transform.position - newclone.transform.position).normalized * ObjectSpeed; 
			Destroy(newclone, ObjectDeath);

			yield return new WaitForSeconds(ObjectDelay);
		}

		yield return null;
	}


	// Shoots from right side of screen randomly
	public IEnumerator C_ShootFromOffScreen(float StartDelay, Transform spawnPoint, int ObjectAmount, float ObjectDelay)
	{
		yield return new WaitForSeconds(StartDelay);


		for(int I=0; I<ObjectAmount; I++)
		{
			float RandomNumber = Random.Range (0, 1f);

			Vector3 p = mainCamera.ViewportToWorldPoint(new Vector3(1, RandomNumber, 1));

			GameObject newclone;
			newclone = Instantiate(projectile, p, transform.rotation) as GameObject;
			newclone.AddComponent <Rigidbody2D>();
			newclone.GetComponent<Rigidbody2D>().freezeRotation = true;
			newclone.GetComponent<Rigidbody2D>().isKinematic = true;
			
			newclone.GetComponent<Rigidbody2D>().velocity = new Vector2(-1, 0 )*ObjectSpeed;
			Destroy(newclone, ObjectDeath);

			yield return new WaitForSeconds(ObjectDelay);
		}

		yield return null;
	}

	// Shoots from top side of screen randomly
	public IEnumerator C_ShootFromTopScreen(float StartDelay, Transform spawnPoint, int ObjectAmount, float ObjectDelay)
	{
		yield return new WaitForSeconds(StartDelay);
		
		
		for(int I=0; I<ObjectAmount; I++)
		{
			float RandomNumber = Random.Range (0, 1f);
			
			Vector3 p = mainCamera.ViewportToWorldPoint(new Vector3(RandomNumber, 1, 1));
			
			GameObject newclone;
			newclone = Instantiate(projectile, p, transform.rotation) as GameObject;
			newclone.AddComponent <Rigidbody2D>();
			newclone.GetComponent<Rigidbody2D>().freezeRotation = true;
			newclone.GetComponent<Rigidbody2D>().isKinematic = true;
			
			newclone.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -1)*ObjectSpeed;
			Destroy(newclone, ObjectDeath);
			
			yield return new WaitForSeconds(ObjectDelay);
		}
		
		yield return null;
	}
	// Shoots left
	public IEnumerator C_ShootLeft(float StartDelay, Transform spawnPoint, int ObjectAmount, float ObjectDelay)
	{
		yield return new WaitForSeconds(StartDelay);

		for(int I=0; I<ObjectAmount; I++)
		{
			GameObject newclone;
			newclone = Instantiate(projectile, spawnPoint.position, transform.rotation) as GameObject;
			newclone.AddComponent <Rigidbody2D>();
			newclone.GetComponent<Rigidbody2D>().freezeRotation = true;
			newclone.GetComponent<Rigidbody2D>().isKinematic = true;

			newclone.GetComponent<Rigidbody2D>().velocity = new Vector2(-1, 0 )*ObjectSpeed;
			Destroy(newclone, ObjectDeath);

			yield return new WaitForSeconds(ObjectDelay);
		}
		yield return null;
	}
}


//if(atPlayer)
//	TEMP.velocity = (GameController.control.GuyLocation.transform.position - TEMP.transform.position).normalized * trapSpeed; 
//else
//	TEMP.velocity = new Vector2(Xvelocity,Yvelocity)*trapSpeed;


/*
public IEnumerator triggerObjects()
{
	yield return new WaitForSeconds(delayStartTimer);
	
	Rigidbody2D TEMP;
	
	AudioSource.PlayClipAtPoint(triggerSound, transform.position);
	
	for(int I=1; I<traps.Length; I++)
	{
		TEMP = traps[I].gameObject.AddComponent("Rigidbody2D") as Rigidbody2D;
		TEMP.fixedAngle = true;
		TEMP.isKinematic = true;
		
		if(atPlayer)
			TEMP.velocity = (GameController.control.GuyLocation.transform.position - TEMP.transform.position).normalized * trapSpeed; 
		else
			TEMP.velocity = new Vector2(Xvelocity,Yvelocity)*trapSpeed;
		
		Destroy (TEMP.gameObject, objectDeathTimer);
		
		if(objectDelays > 0.02f)
			yield return new WaitForSeconds(objectDelays);
		
	}
	
	Destroy (this.gameObject, objectDeathTimer);

*/