// RegalPrime 11-27-14 - SpawnObjectOnHit.cs

// Spawns a certain object at a spawnpoint when this object is triggered by the player
// Was created for testing

using UnityEngine;
using System.Collections;

public class SpawnObjectOnHit : MonoBehaviour
{
	public GameObject ObjectToSpawn;
	public AudioClip SpawnSound;
	public int NumberOfObjects = 1;
	public float SpawnDelay = 0.5f;

	public bool OneTimeActivate = true;
	
	public GameObject SpawnPoint;
	
	void Start()
	{
		
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == ("Player"))
			StartCoroutine (C_SpawnObjects());
	}
	
	IEnumerator C_SpawnObjects()
	{
		yield return null;
		
		if(SpawnSound != null)
			MusicController.control.OneShotAudio(SpawnSound);
		
		for(int I=0; I<NumberOfObjects; I++)
		{
			Instantiate(ObjectToSpawn, SpawnPoint.transform.position, Quaternion.identity);
			yield return new WaitForSeconds(SpawnDelay);
		}

		if(OneTimeActivate)
			Destroy (gameObject);
	}
}
