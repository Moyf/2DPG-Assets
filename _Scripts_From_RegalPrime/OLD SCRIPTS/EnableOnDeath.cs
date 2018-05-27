using UnityEngine;
using System.Collections;

public class EnableOnDeath : MonoBehaviour
{
	public GameObject objectToEnable;

	void OnDestroy()
	{
		if(objectToEnable != null)
			objectToEnable.SetActive(true);
	}
}
