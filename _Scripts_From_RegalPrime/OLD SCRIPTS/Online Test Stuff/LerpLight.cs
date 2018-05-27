using UnityEngine;
using System.Collections;

public class LerpLight : MonoBehaviour
{

	public float smooth;
	private Vector3 newPosition;

	void Awake ()
	{
		newPosition = transform.position;
		smooth = 2f;

	}

	void Start ()
	{
	
	}
	

	void Update ()
	{
		Vector3 positionA = new Vector3(-5, 3, -1);
		Vector3 positionB = new Vector3(5, 3, -1);
		Vector3 positionC = new Vector3 (0, 3, -1);
		
		if(Input.GetKeyDown(KeyCode.Q))
			newPosition = positionA;
		if(Input.GetKeyDown(KeyCode.W))
			newPosition = positionC;
		if(Input.GetKeyDown(KeyCode.E))
			newPosition = positionB;
		
		transform.position = Vector3.Lerp(transform.position, newPosition, smooth * Time.deltaTime);
	}



}



