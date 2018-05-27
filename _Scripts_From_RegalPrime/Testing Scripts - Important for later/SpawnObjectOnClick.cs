// RegalPrime 11-27-14 - SpawnObjectOnClick.cs

// Spawns a certain object at a spawnpoint when this object is clicked
// Was created for testing

using UnityEngine;
using System.Collections;

public class SpawnObjectOnClick : MonoBehaviour 
{
	public GameObject ObjectToSpawn;
	public AudioClip SpawnSound;
	public int NumberOfObjects = 1;
	public float SpawnDelay = 0.5f;

	public GameObject SpawnPoint;

	void Start()
	{

	}

	void OnMouseDown()
	{
		StartCoroutine (C_SpawnObjects());

//			Instantiate(ObjectToSpawn[SpawnIndex], SpawnPoint.transform.position, Quaternion.identity);
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
	}

}
