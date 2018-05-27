// RegalPrime 5-22-15 - ObjectCreationExtras.cs

// This is a place to put custom combination actions

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ObjectCreationExtras : ObjectCreation
{










// CUSTOM FUNCTIONS -------------------------------------------------------------------------------------------------------------------------	
// The below do multiple actions in one generic function to create a specialized effect

	// Creates a single object at a target location and then changes scale, color randomly, and decreases alpha to 0. Objects are then destroyed
	IEnumerator C_Custom_SingleExpandEffect(float StartDelay, Vector3 SpawnPoint, float X_Scale, float Y_Scale, float inSeconds)
	{
		yield return new WaitForSeconds (StartDelay);
		
		Color newColor = new Color (Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f), 1F);
		
		GameObject newClone = CreateClone (false);
		newClone.transform.position = SpawnPoint;
		
		ChangeScale (0, newClone, X_Scale, Y_Scale, inSeconds);
		ChangeColor (0, newClone, newColor, inSeconds, false);
		ChangeAlpha (0, newClone, 0, inSeconds);
		
		destroy_GameObject(newClone, inSeconds);
	}
	public void Custom_SingleExpandEffect(float StartDelay, Vector3 SpawnPoint, float X_Scale, float Y_Scale, float inSeconds)
	{
		StartCoroutine (C_Custom_SingleExpandEffect(StartDelay, SpawnPoint, X_Scale, Y_Scale, inSeconds));
	}
	public void Custom_SingleExpandEffect_Random(float StartDelay, float X_Scale, float Y_Scale, float inSeconds)
	{
		float RandomNumber1 = Random.Range (0, 1f);
		float RandomNumber2 = Random.Range (0, 1f);
		Vector3 SpawnPoint;
		SpawnPoint = Camera.main.ViewportToWorldPoint(new Vector3(RandomNumber1, RandomNumber2, 0));
		SpawnPoint.z = gameObject.transform.position.z;
		StartCoroutine (C_Custom_SingleExpandEffect(StartDelay, SpawnPoint, X_Scale, Y_Scale, inSeconds));
	}


	// Creates an initial circle burst, stops the movement, then creates circle burst at each objects location. Objects are then destroyed after a set time.
	IEnumerator C_Custom_CircleBurstEffect(float StartDelay, Vector3 SpawnPoint, int Starting_NumberOfPoints, float ObjectSpeed, float StopDelay, float BurstDelay, float ObjectDelay, int CircleBurst_NumberOfPoints, float ObjectDeathTimer)
	{
		yield return new WaitForSeconds (StartDelay);

		List<GameObject> TempList = new List<GameObject>();
		GameObject newclone;

		foreach(Vector3 ReturnValue in Calculate_Circle(Starting_NumberOfPoints, 1, 1, 1, 1))
		{
			newclone = CreateClone (false);
			TempList.Add (newclone);

			newclone.transform.position = SpawnPoint;
			newclone.GetComponent<Rigidbody2D>().velocity = ReturnValue*ObjectSpeed;
		}

		yield return null;
		
		StopVelocity_GameObjectList (StopDelay, TempList);
		Destroy_GameObjectList_CircleBurst (BurstDelay, TempList, ObjectDelay, CircleBurst_NumberOfPoints, ObjectSpeed, ObjectDeathTimer);
		
	}
	public void Custom_CircleBurstEffect(float StartDelay, Vector3 SpawnPoint, int Starting_NumberOfPoints, float ObjectSpeed, float StopDelay, float BurstDelay, float ObjectDelay, int CircleBurst_NumberOfPoints, float ObjectDeathTimer)
	{
		StartCoroutine (C_Custom_CircleBurstEffect(StartDelay, SpawnPoint, Starting_NumberOfPoints, ObjectSpeed, StopDelay, BurstDelay, ObjectDelay, CircleBurst_NumberOfPoints, ObjectDeathTimer));
	}	

}




/*


	IEnumerator C_TEST1_Polar(float StartDelay)
	{
		yield return new WaitForSeconds (StartDelay);


		float theta, radius;
		float X, Y;
		Vector3 Temp;
		int Multiplier = 0;


		float X_Width = 01.0f;
		float Y_Width = 01.0f;


		float Value_A			= 2.0f;
		float Value_B			= 1.0f;

		float Value_K_Top		= 3;
		float Value_K_Bottom	= 1;

		int NumberOfObjects = 100;

		if(((Value_K_Top * Value_K_Bottom)%2) == 1)
			Multiplier = 1;
		else
			Multiplier = 2;

		if(Value_A > 0)
			Multiplier= 2;

		float NumberOfIncrements = Mathf.PI * Value_K_Bottom * Multiplier / NumberOfObjects;


		for(float t=0; t < Mathf.PI * Value_K_Bottom * Multiplier; t+=NumberOfIncrements)
		{

//			radius = 2 + 3 * Mathf.Cos (t);				// Limacon with inner loop:
//			radius = 2 * Mathf.Sin (3*t);				// Rose - Odd petal (first number = X number of petals) Pi*1
//			radius = 2 * Mathf.Cos (2*t);				// Rose - Even petal (first number = X * 2 number of petals) Pi*2


			radius = Value_A + Value_B * Mathf.Cos ((Value_K_Top / Value_K_Bottom) * t);


//			radius = 2 - 2*Mathf.Sin (t) + (Mathf.Sin(t) * Mathf.Sqrt(Mathf.Abs(Mathf.Cos (t))) / (Mathf.Sin(t) + 1.4f));		// Heart - (0,0) is the top of the heart
//
//			X = 16 * Mathf.Pow (Mathf.Sin (t), 3);																				// Huge heart but its center is (0,0)
//			Y = 13*Mathf.Cos (t) - 5 * Mathf.Cos (2*t) - 2*Mathf.Cos(3*t) - Mathf.Cos (4*t);
//			X /= 16;																											// Around 1x1 Heart with center at (0,0)
//			Y /= 16;

			theta = t;
			X = (radius * Mathf.Cos (theta)) * X_Width;
			Y = (radius * Mathf.Sin (theta)) * Y_Width;


			Temp = new Vector2(X,Y);
			CreateClone(true).transform.position = Temp;


			yield return new WaitForSeconds (0.01f);
		}
	}
	public void TEST1_Polar(float StartDelay)
	{
		StartCoroutine (C_TEST1_Polar (StartDelay));
	}

*/