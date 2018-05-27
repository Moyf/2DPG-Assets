// RegalPrime 11-20-14 - PulsatingLight.cs

// Pulsates the intensity of the light over time.
// This was created for the torch prefab to give a flickering like flame effect
// PulsePercent - 10 = 10% variance in the origional intensity (100% -> 90% -> 100% -> etc)

// PulseSpeed = Speed change 
// 1 = over 1 second
// 2 = over 0.5s second
// 0.5f = over 2 second
// Etc etc

using UnityEngine;
using System.Collections;

public class PulsatingLight : MonoBehaviour
{
	public float PulsePercent = 10;		// Percent power that it will pulsate.
	public float PulseSpeed = 1;		// Speed at which the light will pulsate

	private Light LightPower;			// Light component
	private float StartingIntensity;	// Starting intensity
	private float EndingIntensity;		// Calculated final intensity

	void Start ()
	{
		LightPower = GetComponent<Light> ();
		StartingIntensity = LightPower.intensity;
		EndingIntensity = StartingIntensity - (StartingIntensity * PulsePercent / 100);

//		print (StartingIntensity + ", " + EndingIntensity);

		StartCoroutine (C_Pulsate ());
	}
	
	IEnumerator C_Pulsate()
	{
		float Duration = 0;

		while(gameObject != null)
		{
			while(Duration < 1)
			{
				Duration += Time.deltaTime*PulseSpeed;

				LightPower.intensity = Mathf.Lerp (StartingIntensity, EndingIntensity, Duration);

				yield return null;
			}
			Duration = 0;

			while(Duration < 1)
			{
				Duration += Time.deltaTime*PulseSpeed;
				
				LightPower.intensity = Mathf.Lerp (EndingIntensity, StartingIntensity, Duration);

				yield return null;
			}
			Duration = 0;

			yield return null;
		}
	}
}


/*
 * 
 * // RegalPrime 11-20-14 - PulsatingLight.cs

// Pulsates the intensity of the light over time.
// This was created for the torch prefab to give a flickering like flame effect
// PulsePercent - 10 = 10% variance in the origional intensity (100% -> 90% -> 100% -> etc)

// PulseSpeed = Speed change 
// 1 = over 1 second
// 2 = over 0.5s second
// 0.5f = over 2 second
// Etc etc

using UnityEngine;
using System.Collections;

public class PulsatingLight : MonoBehaviour
{
	public float PulsePercent = 10;		// Percent power that it will pulsate.
	public float PulseSpeed = 1;		// Speed at which the light will pulsate

	private Light LightPower;			// Light component
	private float StartingIntensity;	// Starting intensity
	private float EndingIntensity;		// Calculated final intensity

	void Start ()
	{
		LightPower = GetComponent<Light> ();
		StartingIntensity = LightPower.intensity;
		EndingIntensity = StartingIntensity - (StartingIntensity * PulsePercent / 100);

//		print (StartingIntensity + ", " + EndingIntensity);

		StartCoroutine (C_Pulsate ());
	}
	
	IEnumerator C_Pulsate()
	{
		float Duration = 0;

		while(gameObject != null)
		{
			while(Duration < 1)
			{
				Duration += Time.deltaTime*PulseSpeed;

				LightPower.intensity = Mathf.Lerp (StartingIntensity, EndingIntensity, Duration);

				yield return null;
			}
			Duration = 0;

			while(Duration < 1)
			{
				Duration += Time.deltaTime*PulseSpeed;
				
				LightPower.intensity = Mathf.Lerp (EndingIntensity, StartingIntensity, Duration);

				yield return null;
			}
			Duration = 0;

			yield return null;
		}
	}
}


*/