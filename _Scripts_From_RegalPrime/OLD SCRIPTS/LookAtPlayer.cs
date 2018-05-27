using UnityEngine;
using System.Collections;

public class LookAtPlayer : MonoBehaviour {

	public Transform target;
	
	void Start()
	{
		target = GameController.control.GuyLocation.transform;
	}
	
	void Update ()
	{
		transform.LookAt(target);
	}
}
