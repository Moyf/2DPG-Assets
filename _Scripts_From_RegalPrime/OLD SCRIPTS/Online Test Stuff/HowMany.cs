using UnityEngine;
using System.Collections;

public class HowMany : MonoBehaviour
{

	
	void Start ()
	{


	}


	void FixedUpdate()
	{
			GameObject[] players;
			players = GameObject.FindGameObjectsWithTag ("Player");
			Debug.Log ("How many players =  " + players.Length);
	}
}

