using UnityEngine;
using System.Collections;

public class BACKaRoom : MonoBehaviour
{
	private int currentLevel;

	void Start()
	{
		currentLevel = (Application.loadedLevel);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
			Application.LoadLevel (currentLevel-1);
	}

}
