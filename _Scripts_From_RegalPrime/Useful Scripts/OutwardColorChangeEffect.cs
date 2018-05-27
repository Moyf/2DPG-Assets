// RegalPrime 12-06-14 - OutwardColorChangeEffect.cs

// This script, when triggered, changes the color of all adjacent tiles (of a certain layermask) to a different color (based on the colorarray and the setcolorindex)
// It does this by checking all adjacent tiles and sees if there is a valid hit.
// If so, it will pass this script and its values to those tiles and repeat until there are no valid adjacent tiles.

// After a delay (ColorDecrementDelay), the colors will change backwards through the colorarray

// As long as the checked tile has a SpriteRenderer and the correct LayerMask, it will check and pass to the neighbor
// So putting a SpriteRenderer and disabling it will allow for the color change to continue throughout the tileset while having "holes" in it

// After writing out all of the data, I was able to create very generic functions to calculate the offset checks
// so, the generic functions might seem confusing

using UnityEngine;
using System.Collections;

public class OutwardColorChangeEffect : MonoBehaviour
{
	public LayerMask LayerIndex;						// What layer mask should be checked
	public float ImpactSpeed = 0.1f;					// The delay of moving between objects
	public float ColorDecrementDelay = 0.2f;			// Delay before color decrementation
	public float ColorChangeSpeed = 2f;					// Speed of the color change
	public int SetColorIndex = 5;						// Which position of the vector to set the color at


	private int ColorValue = 0;							// This objects current color index
	public Color[] colorarray= new Color[] {Color.white, Color.black, Color.yellow, Color.red, Color.blue, Color.cyan};

	private Color StartingColor;						// Starting color of object. Used for resetting


	void Awake()
	{
		EventManager.resetObjects += Reset;
		if(GetComponent<SpriteRenderer>())
			StartingColor = GetComponent<SpriteRenderer> ().color;

		// Index checks
		if(SetColorIndex > colorarray.Length-1)
			SetColorIndex = colorarray.Length-1;

		if(SetColorIndex < 0)
			SetColorIndex = 0;
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		StopAllCoroutines ();
		GetComponent<SpriteRenderer> ().color = StartingColor;

		ColorValue = 0;
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")			// If the player triggers this object
		{
			if(SetColorIndex > colorarray.Length-1)
				SetColorIndex = colorarray.Length-1;		

			InitialCheck_Generic(0, 0.32f, 0.32f, 0.32f, -0.32f, 0.32f);		// UP CHECK
			InitialCheck_Generic(0.32f, 0, 0.32f, -0.32f, 0.32f, 0.32f);		// RIGHT CHECK
			InitialCheck_Generic(0, -0.32f, -0.32f, -0.32f, 0.32f, -0.32f);		// DOWN CHECK
			InitialCheck_Generic(-0.32f, 0, -0.32f, 0.32f, -0.32f, -0.32f);		// LEFT CHECK
		}
	}

	// Used to send the gameobjects data to the next object
	public void SetVariables(Color[] parentcolorarray, float parentImpactSpeed, float parentColorDecrementDelay, float parentColorChangeSpeed, int parentSetColorIndex, LayerMask parentLayerIndex)
	{
		colorarray = parentcolorarray;
		ImpactSpeed = parentImpactSpeed;
		ColorDecrementDelay = parentColorDecrementDelay;
		ColorChangeSpeed = parentColorChangeSpeed;
		SetColorIndex = parentSetColorIndex;
		LayerIndex = parentLayerIndex;
	}



	// Initial check to see if this script can continue in that direction
	public void InitialCheck_Generic(float X1, float Y1, float X2, float Y2, float X3, float Y3)
	{
		StartCoroutine (C_InitialCheck_Generic (X1, Y1, X2, Y2, X3, Y3));
	}
	IEnumerator C_InitialCheck_Generic(float X1, float Y1, float X2, float Y2, float X3, float Y3)
	{
		yield return new WaitForSeconds(ImpactSpeed);

		OutwardColorChangeEffect tempEffect = null;
		tempEffect = FindNeighbor (0, 0);
		if(tempEffect != null)
		{
			tempEffect.SetColor ();
		}

		tempEffect = FindNeighbor (X1, Y1);
		if(tempEffect != null)
		{
			tempEffect.SetColor ();
			tempEffect.Check_Generic_Forward (X1, Y1);
		}

		tempEffect = null;
		tempEffect = FindNeighbor (X2, Y2);
		if(tempEffect != null)
		{
			tempEffect.SetColor ();

			if(X1 == 0)
				Y2 = 0;
			else
				X2 = 0;
//			print ("FAKE RIGHT: " + X1 + ", " + Y1 +" !! " + X2 +", "+ Y2);
			tempEffect.Check_Generic_Right (X1, Y1, X2, Y2);
		}

		tempEffect = null;
		tempEffect = FindNeighbor (X3, Y3);
		if(tempEffect != null)
		{
			tempEffect.SetColor ();

			if(X1 == 0)
				Y3 = 0;
			else
				X3 = 0;
//			print ("FAKE LEFT: " + X1 + ", " + Y1 +" !! " + X3 +", "+ Y3);
			tempEffect.Check_Generic_Left (X1, Y1, X3, Y3);
		}

	}

	// Keep passing this scripts information "forward" if the check passes
	public void Check_Generic_Forward(float X1, float Y1)
	{
		StartCoroutine (C_Check_Generic_Forward (X1, Y1));
	}
	IEnumerator C_Check_Generic_Forward(float X1, float Y1)
	{
		OutwardColorChangeEffect tempEffect;
		yield return new WaitForSeconds(ImpactSpeed);
		tempEffect = FindNeighbor (X1, Y1);
		
		if(tempEffect != null)
		{
			tempEffect.SetColor ();
			tempEffect.Check_Generic_Forward (X1, Y1);
		}
	}

	// Keep passing this scripts information "Right" if the check passes
	public void Check_Generic_Right(float X1, float Y1, float X2, float Y2)
	{
		StartCoroutine (C_Check_Generic_Right (X1, Y1, X2, Y2));
	}
	IEnumerator C_Check_Generic_Right(float X1, float Y1, float X2, float Y2)
	{
		OutwardColorChangeEffect tempEffect;
		yield return new WaitForSeconds(ImpactSpeed);
		
		tempEffect = FindNeighbor (X1, Y1);
		if(tempEffect != null)
		{
			tempEffect.SetColor ();
			tempEffect.Check_Generic_Right (X1, Y1, X2, Y2);
		}
		tempEffect = FindNeighbor (X2, Y2);
		if(tempEffect != null)
		{
			tempEffect.SetColor ();
			tempEffect.Check_Generic_Right (X1, Y1, X2, Y2);
		}
	}

	// Keep passing this scripts information "Left" if the check passes
	public void Check_Generic_Left(float X1, float Y1, float X2, float Y2)
	{
		StartCoroutine (C_Check_Generic_Left (X1, Y1, X2, Y2));
	}
	IEnumerator C_Check_Generic_Left(float X1, float Y1, float X2, float Y2)
	{
		OutwardColorChangeEffect tempEffect;
		yield return new WaitForSeconds(ImpactSpeed);
		
		tempEffect = FindNeighbor (X1, Y1);
		if(tempEffect != null)
		{
			tempEffect.SetColor ();
			tempEffect.Check_Generic_Left (X1, Y1, X2, Y2);
		}
		tempEffect = FindNeighbor (X2, Y2);
		if(tempEffect != null)
		{
			tempEffect.SetColor ();
			tempEffect.Check_Generic_Left (X1, Y1, X2, Y2);
		}
	}

	// Checks to see if there is a valid object at a certain offset
	OutwardColorChangeEffect FindNeighbor(float XOffset, float YOffset)
	{
		OutwardColorChangeEffect NewTestEffect;
		Collider2D hit0 = Physics2D.OverlapCircle(gameObject.transform.position + new Vector3(XOffset, YOffset), 0.01f, LayerIndex);
		if(hit0 && hit0.gameObject.GetComponent<SpriteRenderer>())
		{
			if(hit0.gameObject.GetComponent<OutwardColorChangeEffect> ())
			{
				NewTestEffect = hit0.gameObject.GetComponent<OutwardColorChangeEffect> ();
			}
			else
			{
				NewTestEffect = hit0.gameObject.AddComponent<OutwardColorChangeEffect> ();
			}

			if(SetColorIndex == NewTestEffect.ColorValue)
			{
				return null;
			}

			NewTestEffect.SetVariables(colorarray, ImpactSpeed, ColorDecrementDelay, ColorChangeSpeed, SetColorIndex, LayerIndex);
			return NewTestEffect;
		}

		return null;
	}

	// Set the color index and then change the color and start the decrement delay
	public void SetColor()
	{
		ColorValue = SetColorIndex;

		StopCoroutine ("C_ChangeColor");
		StopCoroutine ("DecrementColor");
		
		StartCoroutine ("C_ChangeColor");
		StartCoroutine ("DecrementColor");
	}

	// After a set time, decrease the color index and change the color until index = 0
	IEnumerator DecrementColor()
	{
		yield return new WaitForSeconds(ColorDecrementDelay);
		while(ColorValue > 0)
		{
			StopCoroutine ("C_ChangeColor");
			ColorValue --;
			yield return StartCoroutine ("C_ChangeColor");
			
//			yield return new WaitForSeconds(ImpactSpeed);
		}
	}

	// Coroutine that changes the color of an object
	void ChangeColor()
	{
		StartCoroutine ("C_ChangeColor");
	}
	IEnumerator C_ChangeColor()
	{
		if(GetComponent<SpriteRenderer>())
		{
			float Duration = 0;
			Color OldColor = gameObject.GetComponent<SpriteRenderer>().color;

			while(Duration <= 1)
			{
				Duration += Time.deltaTime*ColorChangeSpeed;
				gameObject.GetComponent<SpriteRenderer>().color  = Color.Lerp(OldColor, colorarray[ColorValue], Duration);
				yield return null;
			}
			yield return null;
		}
	}
}