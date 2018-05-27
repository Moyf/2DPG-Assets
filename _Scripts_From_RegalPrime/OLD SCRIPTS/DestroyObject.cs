using UnityEngine;
using System.Collections;

public class DestroyObject : MonoBehaviour
{

	public float duration;
	void Start()
	{
		Destroy (gameObject, duration);
	}
}