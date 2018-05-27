// RegalPrime 5-22-15 - ObjectManipulation.cs

// This script has deals with manipulating objects, usually over a period of time (seconds)
// Below are listed all the base coroutines that are available. There are public functions that call these coroutines for easy of use.

// See TestNewObjectScript.cs for examples on how to call these functions

/*
// ALREADY IN THIS SCRIPT

bool IsObjectActive(GameObject AGameObject)
public Vector3 ReturnProperPoint(GameObject Object1, Vector3 Point1)
public GameObject PlayersGameObject()
C_stopallcoroutines (float StartDelay)


C_MoveObjectToPoint(float StartDelay, GameObject ObjectToMove, Vector3 TargetPosition, float inSeconds, bool IsLocal)
C_FollowTarget(float StartDelay, GameObject AGameObject, GameObject TargetObject, float ObjectSpeed)
C_Stop_FollowTarget(float StartDelay)

C_WaveEffectObject(float StartDelay, GameObject AGameObject, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y, float I, bool LerpMovement)
C_Stop_WaveEffect(float StartDelay)

C_RotateDegrees(float StartDelay, GameObject AGameObject, float RotateDegrees, float inSeconds)
C_Stop_ForeverRotate(float StartDelay)

C_RotateToDegree(float StartDelay, GameObject AGameObject, float RotateToThisDegree, float inSeconds, bool Clockwise)

C_RotateTwordsPoint(float StartDelay, GameObject AGameObject, GameObject TargetObject, Vector3 TargetPoint, float RotateSpeed, bool ForeverRotateTwords)
C_Stop_RotateTwords(float StartDelay)

C_RotateAroundPoint(float StartDelay, GameObject AGameObject, GameObject TargetObject, Vector3 TargetPoint, float X_Width, float Y_Width, float RotateSpeed, float StartingPoint, bool LerpMovement)
C_Stop_RotateAround(float StartDelay)

C_ChangeScale(float StartDelay, GameObject AGameObject, float X_Scale, float Y_Scale, float inSeconds)

C_ChangeColor(float StartDelay, GameObject AGameObject, Color NewColor, float inSeconds, bool ChangeForever)
C_StopColorChangeEffect(float StartDelay)

C_ChangeAlpha(float StartDelay, GameObject AGameObject, float AlphaValue, float inSeconds)

*/





using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManipulation: MonoBehaviour
{

	private bool AllowFollowTarget = false;					// Used to tell when to stop a certain action
	private bool AllowWaveEffect = false;					// Used to tell when to stop a certain action
	private bool AllowForeverRotate = false;				// Used to tell when to stop a certain action
	private bool AllowRotateTwords = false;					// Used to tell when to stop a certain action
	private bool AllowRotateAroundPoint = false;			// Used to tell when to stop a certain action
	private bool AllowForeverColorChange = false;			// Used to tell when to stop a certain action

	// Simple function to see if an object is active
	// Can change how it works later if needed
	public bool IsObjectActive(GameObject AGameObject)
	{
		if(AGameObject == null)
			return false;

		if(AGameObject.activeInHierarchy)
			return true;

		return false;
	}

	// Most functions allow for either gameObjects or a vector3 as a starting point (for example)
	// The gameObjects position needs to be updated every time whereas the vector3 point never moves
	// This is  nothing more than a simple function to return the correct version based on the two inputs
	// If gameobject = null, then it will use the vector3 provided
	// If for some reason they forgot both, the Vector3 will be used anyways as its default value is (0,0)
	public Vector3 ReturnProperPoint(GameObject Object1, Vector3 Point1)
	{
		if(Object1 != null)
			return Object1.transform.position;
		else
			return Point1;
	}

	// Simple function to get the players gameobject
	// Can be used in functions for an easy way to reference this object
	public GameObject PlayersGameObject()
	{
		if(GameController.control.GetPlayerGameObject () != null)
		{
			return GameController.control.GetPlayerGameObject ();
		}
		else
		{
			print ("TRIED TO TARGET THE PLAYER BUT HE DOES NOT EXIST");
			return null;
		}
	}

	// Simple function to stop all coroutines after a delay
	// This might not be a useful function, but its here if I need it.
	IEnumerator C_stopallcoroutines (float StartDelay)
	{
		if(StartDelay != 0)
			yield return new WaitForSeconds (StartDelay);
		StopAllCoroutines ();
	}
	public void stopallcoroutines (float StartDelay)
	{
		StartCoroutine (C_stopallcoroutines (StartDelay));
	}

/*


*/
































	// Moves a gameobject to a certain target point over a certain period of time
	// If the ObjectToMove = null, then it will move itself to the target location
	// inSeconds = number of seconds to reach the target. If = 0, then instant move
	// IsLocal = Will the object move relative to the local scale or global scale?
	IEnumerator C_MoveObjectToPoint(float StartDelay, GameObject ObjectToMove, Vector3 TargetPosition, float inSeconds, bool IsLocal)
	{
		yield return new WaitForSeconds(StartDelay);

		if(ObjectToMove == null)
			ObjectToMove = gameObject;

		float Duration = 0;
		Vector3 StartingPostion;

		if(IsLocal)
			StartingPostion = ObjectToMove.transform.localPosition;
		else
			StartingPostion = ObjectToMove.transform.position;
				
		if(inSeconds == 0)
			ObjectToMove.transform.position = TargetPosition;
		else
			while(Duration < 1)
			{
				Duration += Time.deltaTime/Mathf.Abs(inSeconds);
				if(IsLocal)
					ObjectToMove.transform.localPosition = Vector3.Lerp(StartingPostion, TargetPosition, Duration);
				else
					ObjectToMove.transform.position = Vector3.Lerp(StartingPostion, TargetPosition, Duration);

				yield return null;
			}
		yield return null;
	}
	// Possibly make more functions to deal with other object types - gameobject / transform
	public void MoveObjectToPoint(float Delay, GameObject ObjectToMove, Vector3 TargetPosition, float inSeconds)
	{
		StartCoroutine (C_MoveObjectToPoint(Delay, ObjectToMove, TargetPosition, inSeconds, false));
	}
	public void MoveObjectToPoint_Local(float Delay, GameObject ObjectToMove, Vector3 TargetPosition, float inSeconds)
	{
		StartCoroutine (C_MoveObjectToPoint(Delay, ObjectToMove, TargetPosition, inSeconds, true));
	}
	

	// Takes a gameobject and makes it move twords a certain gameobject forever
	// Can stop this via Stop_FollowTarget
	IEnumerator C_FollowTarget(float StartDelay, GameObject AGameObject, GameObject TargetObject, float ObjectSpeed)
	{
		yield return new WaitForSeconds(StartDelay);
		AllowFollowTarget = true;

		if (AGameObject == null)
			AGameObject = gameObject;

		while(AGameObject.activeInHierarchy && AllowFollowTarget)
		{
			AGameObject.transform.position = Vector3.MoveTowards(AGameObject.transform.position, TargetObject.transform.position, Time.deltaTime*ObjectSpeed);
			yield return null;
		}
		yield return null;
	}
	public void FollowTarget(float StartDelay, GameObject AGameObject, GameObject TargetObject, float ObjectSpeed)
	{
		StartCoroutine (C_FollowTarget(StartDelay, AGameObject, TargetObject, ObjectSpeed));
	}
	IEnumerator C_Stop_FollowTarget(float StartDelay)
	{
		yield return new WaitForSeconds (StartDelay);
		AllowFollowTarget = false;
	}
	public void Stop_FollowTarget(float StartDelay)
	{
		StartCoroutine (C_Stop_FollowTarget(StartDelay));
	}


	// Takes a single object and makes them do a wavy like motion based on the variables
	// If gameobject = null then it will become the object the script is attached to
	// Amplitude is how far the object "waves"
	// PeriodSpeed is how fast the object waves
	// I is a variable that represents what position in time the wave starts. Used when creating a wave of objects in a row
	// Can stop this via Stop_WaveEffect
	IEnumerator C_WaveEffectObject(float StartDelay, GameObject AGameObject, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y, float I, bool LerpMovement)
	{
		yield return new WaitForSeconds(StartDelay);
		AllowWaveEffect = true;

		if(AGameObject == null)
			AGameObject = gameObject;
		
		float YLerp = 0;
		float XLerp = 0;
		float X = AGameObject.transform.localPosition.x;
		float Y = AGameObject.transform.localPosition.y;
		
		while(IsObjectActive(AGameObject) && AllowWaveEffect)
		{
			if(Amplitude_X != 0)
			{
				if(LerpMovement)
					XLerp = Mathf.Lerp(AGameObject.transform.localPosition.x, X+Amplitude_X*Mathf.Sin (I+ Time.time*PeriodSpeed_X), Time.deltaTime);
				else
					XLerp = X+Amplitude_X*Mathf.Sin (I+ Time.time*PeriodSpeed_X);

				AGameObject.transform.localPosition = new Vector3(XLerp, AGameObject.transform.localPosition.y, 0);
			}
			if(Amplitude_Y != 0)
			{
				if(LerpMovement)
					YLerp = Mathf.Lerp(AGameObject.transform.localPosition.y, Y+Amplitude_Y*Mathf.Sin (I+ Time.time*PeriodSpeed_Y), Time.deltaTime);
				else
					YLerp = Y+Amplitude_Y*Mathf.Sin (I+ Time.time*PeriodSpeed_Y);

				AGameObject.transform.localPosition = new Vector3(AGameObject.transform.localPosition.x, YLerp, 0);
			}		

			yield return null;
		}
		yield return null;
	}
	public void WaveEffectObject(float StartDelay, GameObject AGameObject, float Amplitude_X, float PeriodSpeed_X, float Amplitude_Y, float PeriodSpeed_Y, float I, bool LerpMovement)
	{
		StartCoroutine (C_WaveEffectObject(StartDelay, AGameObject, Amplitude_X, PeriodSpeed_X, Amplitude_Y, PeriodSpeed_Y, I, LerpMovement));
	}
	IEnumerator C_Stop_WaveEffect(float StartDelay)
	{
		yield return new WaitForSeconds (StartDelay);
		AllowWaveEffect = false;
	}
	public void Stop_WaveEffect(float StartDelay)
	{
		StartCoroutine (C_Stop_WaveEffect(StartDelay));
	}


	// Rotates an object X degrees
	// StartDelay - Start of delay
	// RotateDegrees - The object will rotate X degrees (in +/- direction) before stopping
	// If RotateDegrees = 0, it will rotate forever. In this case inSeconds = #ofDegrees rotated per second
	// inSeconds - Rotate over X seconds (cannot be negative)
	// Can stop via Stop_ForeverRotate
	IEnumerator C_RotateDegrees(float StartDelay, GameObject AGameObject, float RotateDegrees, float inSeconds)
	{
		yield return new WaitForSeconds (StartDelay);
		
		if(AGameObject == null)
			AGameObject = gameObject;
		
		if(RotateDegrees == 0)								// Rotate forever
		{
			AllowForeverRotate = true;
			while(IsObjectActive(AGameObject) && AllowForeverRotate)
			{
				AGameObject.transform.Rotate(0,0, inSeconds * Time.deltaTime);
				yield return null;
			}
		}
		else
		{
			float RotateSpeed = RotateDegrees / Mathf.Abs(inSeconds);
			float RemainingDegrees = Mathf.Abs (RotateDegrees);
			
			if(inSeconds == 0)								// Snap to new rotation
				AGameObject.transform.Rotate(0,0, RotateDegrees);
			else
			{
				while(RemainingDegrees >= 0)				// Rotate remaining degrees
				{
					RemainingDegrees -= Mathf.Abs(RotateSpeed * Time.deltaTime);
					if(RemainingDegrees > 0)
						AGameObject.transform.Rotate(0,0, RotateSpeed * Time.deltaTime);
					else
					{
						if(RotateDegrees < 0)
							AGameObject.transform.Rotate(0,0, (RotateSpeed * Time.deltaTime) - RemainingDegrees);
						else
							AGameObject.transform.Rotate(0,0, (RotateSpeed * Time.deltaTime) + RemainingDegrees);
					}
					yield return null;
				}
			}
		}
		
		yield return null;
	}
	public void RotateDegrees(float StartDelay, GameObject AGameObject, float RotateDegrees, float inSeconds)
	{
		StartCoroutine (C_RotateDegrees (StartDelay, AGameObject, RotateDegrees, inSeconds));
	}
	IEnumerator C_Stop_ForeverRotate(float StartDelay)
	{
		yield return new WaitForSeconds (StartDelay);
		AllowForeverRotate = false;
	}
	public void Stop_ForeverRotate(float StartDelay)
	{
		StartCoroutine (C_Stop_ForeverRotate(StartDelay));
	}


	// Rotates an object to a cetain degree
	// StartDelay - Start of delay
	// RotateToThisDegree - The object will rotate to this degree mark
	// inSeconds - Rotate over X seconds (cannot be negative)
	// Clockwise - will determine which way the object spins to reach the degree mark
	IEnumerator C_RotateToDegree(float StartDelay, GameObject AGameObject, float RotateToThisDegree, float inSeconds, bool Clockwise)
	{
		yield return new WaitForSeconds (StartDelay);
		if(AGameObject == null)
			AGameObject = gameObject;
		
		RotateToThisDegree = Mathf.Abs (RotateToThisDegree);
		float RotateSpeed = 0;
		float RemainingDegrees = 0;
		if(Clockwise)
		{
			RemainingDegrees = Mathf.Abs(AGameObject.transform.rotation.eulerAngles.z - RotateToThisDegree + 360);	// How many degress needed to rotate
			RemainingDegrees = RemainingDegrees - (int)(RemainingDegrees/360)*360;									// Reduce anything over 360 degrees
			RotateSpeed = RemainingDegrees / Mathf.Abs(inSeconds) * -1;												// How fast it needs to rotate

//			print (AGameObject.transform.rotation.eulerAngles.z+360 + "  -  "+ RotateToThisDegree + "  =  " + RemainingDegrees);
		}
		else
		{
			RemainingDegrees = Mathf.Abs(AGameObject.transform.rotation.eulerAngles.z - RotateToThisDegree - 360);	// How many degress needed to rotate
			RemainingDegrees = RemainingDegrees - (int)(RemainingDegrees/360)*360;									// Reduce anything over 360 degrees
			RotateSpeed = RemainingDegrees / Mathf.Abs(inSeconds) * 1;												// How fast it needs to rotate

//			print (AGameObject.transform.rotation.eulerAngles.z-360 + "  -  "+ RotateToThisDegree + "  =  " + RemainingDegrees);
		}

		if(inSeconds == 0)
		{
			AGameObject.transform.rotation = Quaternion.Euler(0,0,RotateToThisDegree);
		}
		else
		{
			while(RemainingDegrees > 0)
			{
				RemainingDegrees -= Mathf.Abs(RotateSpeed * Time.deltaTime);
				
				if(RemainingDegrees < 0)
					AGameObject.transform.localRotation = Quaternion.Euler(0,0,RotateToThisDegree);
				else
					AGameObject.transform.Rotate(0,0, RotateSpeed * Time.deltaTime);
				
				yield return null;
			}
		}
	}
	public void RotateToDegree(float StartDelay, GameObject AGameObject, float RotateToThisDegree, float inSeconds, bool Clockwise)
	{
		StartCoroutine (C_RotateToDegree(StartDelay, AGameObject, RotateToThisDegree, inSeconds, Clockwise));
	}

	// Rotates a gameobject object to look at another target object (via lerping)
	// AGameObject = the object that is rotated. If = null, then it uses this object
	// TargetObject = the object that is rotated twords. If = null, then it will target the position TargetPoint
	// RotateSpeed = Speed in which the rotate Lerps
	// If RotateSpeed is negitive than it will rotate away from the target
	// ForeverRotate. If true, then the object will constantly look twords the targeted object. If false, it will look at the target and stop
	// Can stop via Stop_RotateTwords
	IEnumerator C_RotateTwordsPoint(float StartDelay, GameObject AGameObject, GameObject TargetObject, Vector3 TargetPoint, float RotateSpeed, bool ForeverRotateTwords)
	{
		yield return new WaitForSeconds (StartDelay);
		if(ForeverRotateTwords)
			AllowRotateTwords = true;

		int RotateDirection = 1;

		if(RotateSpeed != 0)
			RotateDirection = (int)(Mathf.Abs(RotateSpeed) / RotateSpeed);
		RotateSpeed = Mathf.Abs (RotateSpeed);
		
		if(AGameObject == null)
			AGameObject = gameObject;
		
		if(TargetObject == null)
		{
			AllowRotateTwords = false;
		}

		// Calculate the Angle Differential between the two objects
		Vector3 pos = AGameObject.transform.position;
		Vector3 dir = (ReturnProperPoint (TargetObject, TargetPoint) - pos) * RotateDirection;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		float AngleDifference = (Quaternion.Angle (AGameObject.transform.rotation, Quaternion.AngleAxis (angle, Vector3.forward)));


		// If no rotatespeed, then instant snap to the target (no lerp)
		if(RotateSpeed == 0)
		{
			AGameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		else
		{
			while((AngleDifference > 0) || (ForeverRotateTwords && AllowRotateTwords))
			{
				// Recheck rotational difference between now and where it wants to go
				pos = AGameObject.transform.position;
				dir = (ReturnProperPoint (TargetObject, TargetPoint) - pos) * RotateDirection;
				angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				AngleDifference = (Quaternion.Angle (AGameObject.transform.rotation, Quaternion.AngleAxis (angle, Vector3.forward)));
				
				if(AngleDifference > 1)
					AGameObject.transform.rotation = Quaternion.Slerp(AGameObject.transform.rotation,  Quaternion.AngleAxis (angle, Vector3.forward), RotateSpeed*Time.deltaTime);
				else
					AGameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
				
				yield return null;
			}
		}
	}
	public void RotateTwordsPoint(float StartDelay, GameObject AGameObject, GameObject TargetObject, Vector3 TargetPoint, float RotateSpeed, bool ForeverRotateTwords)
	{
		StartCoroutine (C_RotateTwordsPoint(StartDelay, AGameObject, TargetObject, TargetPoint, RotateSpeed, ForeverRotateTwords));
	}
	IEnumerator C_Stop_RotateTwords(float StartDelay)
	{
		yield return new WaitForSeconds (StartDelay);
		AllowRotateTwords = false;
	}
	public void Stop_RotateTwords(float StartDelay)
	{
		StartCoroutine (C_Stop_RotateTwords(StartDelay));
	}


	// Rotates a gameobject around another gameobject
	// AGameObject = the object that will rotate around an object. If null it will pick this object
	// TargetObject = Object to rotate around. If null, it will use the position TargetPoint
	// X_Width / Y_Width - The X and Y radius of the circle
	// Speed = Speed of rotation
	// StartingPoint = Where the rotation will focus on first (0 = Up, 90 = Right, 180 = Down, 270 = Left)
	// LerpMovement = If true, the object will smoothly goto the position and then rotate (will be a bendy radius if the TargetObject is moving)
	// If false, the object will snap to the starting location and then spin around the object (will be a constant radius)
	// Can stop via Stop_RotateAround
	IEnumerator C_RotateAroundPoint(float StartDelay, GameObject AGameObject, GameObject TargetObject, Vector3 TargetPoint, float X_Width, float Y_Width, float RotateSpeed, float StartingPoint, bool LerpMovement)
	{
		yield return new WaitForSeconds(StartDelay);
		AllowRotateAroundPoint = true;

		if (AGameObject == null)
			AGameObject = gameObject;
		
		Vector3 pointPos;
		float i = StartingPoint;

		while(IsObjectActive(AGameObject) && AllowRotateAroundPoint)
		{
			float pointNum = (i*1.0f)/360;
			float angle = pointNum*Mathf.PI*2;
			
			float x = Mathf.Sin (angle)*X_Width;
			float y = Mathf.Cos (angle)*Y_Width;
			
			pointPos = new Vector3(x, y) + ReturnProperPoint(TargetObject, TargetPoint);
			if(LerpMovement)
				pointPos = Vector3.Lerp (AGameObject.transform.position, pointPos, Time.deltaTime*Mathf.Abs (RotateSpeed));
			AGameObject.transform.position = pointPos;
			
			i = i+RotateSpeed;
			if(i>360)
				i = i-360;
			
			if(i<0)
				i = 360+i;
			
			yield return null;
		}
	}
	public void  RotateAroundPoint(float StartDelay, GameObject AGameObject, GameObject TargetObject, Vector3 TargetPoint, float X_Width, float Y_Width, float RotateSpeed, float StartingPoint, bool LerpMovement)
	{
		StartCoroutine(C_RotateAroundPoint(StartDelay, AGameObject, TargetObject, TargetPoint, X_Width, Y_Width, RotateSpeed, StartingPoint,LerpMovement));
	}
	IEnumerator C_Stop_RotateAround(float StartDelay)
	{
		yield return new WaitForSeconds (StartDelay);
		AllowRotateAroundPoint = false;
	}
	public void Stop_RotateAround(float StartDelay)
	{
		StartCoroutine (C_Stop_RotateAround(StartDelay));
	}


	// Scales an object over inSeconds time
	// StartDelay - Start of delay
	// AGameObject - The object that is scaled. If null, then it scales this object
	// NewScaleValue - 1 = Normal size
	// inSeconds - how fast this change takes place
	IEnumerator C_ChangeScale(float StartDelay, GameObject AGameObject, float X_Scale, float Y_Scale, float inSeconds)
	{
		yield return new WaitForSeconds (StartDelay);
		
		if(AGameObject == null)
			AGameObject = gameObject;
		
		float Duration = 0;
		Vector3 StartingScale = AGameObject.transform.localScale;
		Vector3 EndingScale = new Vector3(X_Scale, Y_Scale, AGameObject.transform.localScale.z);
		
		if(inSeconds == 0)
		{
			AGameObject.transform.localScale = EndingScale;
		}
		else
		{
			while(Duration <= 1 && IsObjectActive(AGameObject))
			{
				AGameObject.transform.localScale = Vector3.Lerp (StartingScale, EndingScale, Duration);
				Duration += Time.deltaTime/Mathf.Abs(inSeconds);
				yield return null;
			}
		}
	}
	public void ChangeScale(float StartDelay, GameObject AGameObject, float X_Scale, float Y_Scale, float inSeconds)
	{
		StartCoroutine (C_ChangeScale(StartDelay, AGameObject, X_Scale, Y_Scale, inSeconds));
	}


	// Changes the color of AGameObject over time (will exit if AGameObject has no spriterenderer)
	// If ChangeForever = true, then it will randomly change colors forever
	// Can stop via C_StopColorChangeEffect (I noticed it will finish the current color changing effect before stopping though)
	IEnumerator C_ChangeColor(float StartDelay, GameObject AGameObject, Color NewColor, float inSeconds, bool ChangeForever)
	{
		if(StartDelay != 0)
			yield return new WaitForSeconds(StartDelay);

		if(AGameObject == null)
			AGameObject = gameObject;


		SpriteRenderer SRenderer;
		if(AGameObject.GetComponent<SpriteRenderer>() == null)
			yield break;
		else
			SRenderer = AGameObject.GetComponent<SpriteRenderer>();
		
		AllowForeverColorChange = ChangeForever;
		if(ChangeForever)
			NewColor = new Color (Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f), SRenderer.color.a);

		float Duration = 0;
		Color OldColor;
		do{
			Duration = 0;
			OldColor = SRenderer.color;

			if(inSeconds == 0)
			{
				SRenderer.color = NewColor;
			}
			else
			{
				while(Duration < 1 && IsObjectActive(AGameObject))
				{
					Duration += Time.deltaTime/Mathf.Abs (inSeconds);
					SRenderer.color  = Color.Lerp(OldColor, NewColor, Duration);
					yield return null;
				}
				SRenderer.color = NewColor;
			}
			NewColor = new Color (Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f), SRenderer.color.a);
			
			yield return null;
		}while(AllowForeverColorChange);
	}
	public void ChangeColor(float StartDelay, GameObject AGameObject, Color NewColor, float inSeconds, bool ChangeForever)
	{
		StartCoroutine(C_ChangeColor(StartDelay, AGameObject, NewColor, inSeconds, ChangeForever));
	}
	IEnumerator C_StopColorChangeEffect(float StartDelay)
	{
		yield return new WaitForSeconds(StartDelay);
		AllowForeverColorChange = false;
	}
	public void StopColorChangeEffect(float StartDelay)
	{
		StartCoroutine (C_StopColorChangeEffect(StartDelay));
	}


	// Changes the Alpha of AGameObject over time (will exit if AGameObject has no spriterenderer)
	// Alpha is how visible the sprite is and you can use this function to create a fade / unfade effects
	// Alpha values range from 0->1 and will be clamped (ie anything above or below will be changed)
	// AlphaValue = 0 - Makes the object fully invisible
	// AlphaValue = 1 - Makee the object fully visible
	IEnumerator C_ChangeAlpha(float StartDelay, GameObject AGameObject, float AlphaValue, float inSeconds)
	{
		yield return new WaitForSeconds(StartDelay);
		AlphaValue = Mathf.Clamp (AlphaValue, 0, 1);

		if(AGameObject == null)
			AGameObject = gameObject;

		SpriteRenderer SRenderer;
		if(AGameObject.GetComponent<SpriteRenderer>() == null)
			yield break;
		else
			SRenderer = AGameObject.GetComponent<SpriteRenderer>();

		float Duration = 0;
		float StartingAlpha = 0;
		float NewAlpha = 0;
		StartingAlpha = SRenderer.color.a;

		if(inSeconds == 0)
		{
			SRenderer.color = new Color(SRenderer.color.r,SRenderer.color.g,SRenderer.color.b, AlphaValue);
		}
		else
		{
			while (Duration < 1)
			{
				NewAlpha = Mathf.Lerp(StartingAlpha, AlphaValue, Duration);
				SRenderer.color = new Color(SRenderer.color.r,SRenderer.color.g,SRenderer.color.b, NewAlpha);
				Duration += Time.deltaTime/Mathf.Abs (inSeconds);
				yield return null;
			}
			SRenderer.color = new Color(SRenderer.color.r,SRenderer.color.g,SRenderer.color.b, AlphaValue);
		}
	}

	public void ChangeAlpha(float StartDelay, GameObject AGameObject, float AlphaValue, float inSeconds)
	{
		StartCoroutine (C_ChangeAlpha(StartDelay, AGameObject, AlphaValue, inSeconds));
	}






}


/*

	// Rotates a gameobject object to look at another target object (via lerping)
	// AGameObject = the object that is rotated. If = null, then it uses this object
	// TargetObject = the object that is rotated twords. If = null, then it will target the position TargetPoint
	// RotateSpeed = Speed in which the rotate Lerps
	// ForeverRotate. If true, then the object will constantly look twords the targeted object. If false, it will look at the target and stop
	// Can stop via Stop_RotateTwords
	IEnumerator C_RotateTwordsPoint(float StartDelay, GameObject AGameObject, GameObject TargetObject, Vector3 TargetPoint, float RotateSpeed, bool ForeverRotateTwords)
	{
		yield return new WaitForSeconds (StartDelay);
		if(ForeverRotateTwords)
			AllowRotateTwords = true;

		RotateSpeed = Mathf.Abs (RotateSpeed);
		
		if(AGameObject == null)
			AGameObject = gameObject;
		
		if(TargetObject == null)
		{
			AllowRotateTwords = false;
		}

		// Calculate the Angle Differential between the two objects
		Vector3 pos = AGameObject.transform.position;
		Vector3 dir = ReturnProperPoint (TargetObject, TargetPoint) - pos;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		float AngleDifference = (Quaternion.Angle (AGameObject.transform.rotation, Quaternion.AngleAxis (angle, Vector3.forward)));


		// If no rotatespeed, then instant snap to the target (no lerp)
		if(RotateSpeed == 0)
		{
			AGameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		else
		{
			while((AngleDifference > 0) || (ForeverRotateTwords && AllowRotateTwords))
			{
				// Recheck rotational difference between now and where it wants to go
				pos = AGameObject.transform.position;
				dir = ReturnProperPoint (TargetObject, TargetPoint) - pos;
				angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				AngleDifference = (Quaternion.Angle (AGameObject.transform.rotation, Quaternion.AngleAxis (angle, Vector3.forward)));
				
				if(AngleDifference > 1)
					AGameObject.transform.rotation = Quaternion.Slerp(AGameObject.transform.rotation,  Quaternion.AngleAxis (angle, Vector3.forward), RotateSpeed*Time.deltaTime);
				else
					AGameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
				
				yield return null;
			}
		}
	}
	public void RotateTwordsPoint(float StartDelay, GameObject AGameObject, GameObject TargetObject, Vector3 TargetPoint, float RotateSpeed, bool ForeverRotateTwords)
	{
		StartCoroutine (C_RotateTwordsPoint(StartDelay, AGameObject, TargetObject, TargetPoint, RotateSpeed, ForeverRotateTwords));
	}
	IEnumerator C_Stop_RotateTwords(float StartDelay)
	{
		yield return new WaitForSeconds (StartDelay);
		AllowRotateTwords = false;
	}
	public void Stop_RotateTwords(float StartDelay)
	{
		StartCoroutine (C_Stop_RotateTwords(StartDelay));
	}


*/