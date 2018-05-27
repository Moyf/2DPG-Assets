using UnityEngine;
using System.Collections;

public class LightLookAt : MonoBehaviour
{
	public Transform target;

	void Start()
	{

	}
	
	void Update ()
	{
		transform.LookAt(target);
	}
}