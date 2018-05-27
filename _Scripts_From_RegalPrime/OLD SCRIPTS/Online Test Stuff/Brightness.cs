using UnityEngine;
using System.Collections;

public class Brightness : MonoBehaviour {

	public float smooth;
	private float newIntensity;

	void Awake ()
	{
		smooth = 2f;
		newIntensity = GetComponent<Light>().intensity;
	}


	void Start ()
	{
	
	}
	

	void Update ()
	{
		float intensityA = 0.5f;
		float intensityB = 5f;
		float intensityC = 1.5f;

		if(Input.GetKeyDown(KeyCode.A))
			newIntensity = intensityA;
		if(Input.GetKeyDown(KeyCode.D))
			newIntensity = intensityB;
		if(Input.GetKeyDown(KeyCode.S))
			newIntensity = intensityC;
		
		GetComponent<Light>().intensity = Mathf.Lerp(GetComponent<Light>().intensity, newIntensity, smooth * Time.deltaTime);

	}
}
