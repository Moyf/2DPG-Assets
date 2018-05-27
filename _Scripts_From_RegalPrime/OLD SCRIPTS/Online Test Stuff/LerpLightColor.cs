using UnityEngine;
using System.Collections;

public class LerpLightColor : MonoBehaviour {

	private Color newColour;
	public float smooth;

	void Awake ()
	{
		smooth = 2f;
		newColour = GetComponent<Light>().color;

	}

	void Start ()
	{
	
	}
	

	void Update ()
	{
		Color colourA = Color.red;
		Color colourB = Color.green;
		Color colourC = Color.white;
		
		if(Input.GetKeyDown(KeyCode.Z))
			newColour = colourA;
		if(Input.GetKeyDown(KeyCode.X))
			newColour = colourC;
		if(Input.GetKeyDown(KeyCode.C))
			newColour = colourB;
		
		GetComponent<Light>().color = Color.Lerp(GetComponent<Light>().color, newColour, smooth * Time.deltaTime);
	}
}
