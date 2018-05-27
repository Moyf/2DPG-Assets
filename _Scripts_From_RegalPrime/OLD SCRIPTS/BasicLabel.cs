// RegalPrime 11-25-14 - BasicLabel.cs

// Used before the GUI update - replaced by unity GUI objects

// Displays text at the gameobjects position
// LeftJustified = false - This means the label will be centered at the gameobject location
// LeftJustified = true - This means the label will be left justified at the gameobject location

// This script will resize the font size based on how big the screen is

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicLabel : MonoBehaviour
{
	public string TextToDisplay = "Text To Display";		// Change this attribute in the editor
	public bool LeftJustified = false;

	public float lengthOfLabel = 125;						// Word wrap is enabled, so this will be how much text can be displayed before making a new line
	private float heightOfLabel = 20;

	private Vector2 pixelOffset;
	private Camera mainCamera;

	public GUIStyle styleGuide;

	private GUIContent MyInfo;

//	Matrix4x4 GUImatrix;

	float virtualWidth = 407.0f;
//	float virtualHeight = 305.0f;

	void Start ()
	{
		mainCamera = Camera.main;
//		GUImatrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3((Screen.width/virtualWidth), (Screen.height/virtualHeight), 1.0f));

		styleGuide.fontSize = (int)(styleGuide.fontSize * (Screen.width / virtualWidth));
		lengthOfLabel = lengthOfLabel * (Screen.width / virtualWidth);

		CalculateFixedHeight ();
	}
	
	void OnGUI()
	{
		//GUI.matrix = GUImatrix;
		pixelOffset = mainCamera.WorldToScreenPoint (new Vector2 (transform.position.x, transform.position.y));	// Turn into pixel coordinates

		if(LeftJustified)
			GUI.Label(new Rect(pixelOffset.x, Screen.height-pixelOffset.y, lengthOfLabel, heightOfLabel),TextToDisplay, styleGuide);
		else
			GUI.Label(new Rect(pixelOffset.x-lengthOfLabel/2, Screen.height-pixelOffset.y, lengthOfLabel, heightOfLabel),TextToDisplay, styleGuide);

	}

	public void CalculateFixedHeight()
	{
		styleGuide.fixedHeight = styleGuide.CalcHeight (new GUIContent (TextToDisplay), lengthOfLabel);
	}
}

/*
 * using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicLabel : MonoBehaviour
{
	public string TextToDisplay = "Text To Display";
	public int NumberOfLettersPerLine = 20;

	public float lengthOfLabel = 125;
	public float PixelGap = 15;

	private Vector2 pixelOffset;
	private Camera mainCamera;

	List<string> myList = new List<string>();
	string[] SingleWords;

	public GUIStyle styleGuide;

	void Start ()
	{
		mainCamera = Camera.main;

		SingleWords = TextToDisplay.Split (' ');
		int characterCount = 0;
		string TEMP = "";

		for(int I=0; I<SingleWords.Length; I++)
		{
			characterCount += SingleWords[I].Length;	// Length of the individual word
			characterCount++;

			if(characterCount > NumberOfLettersPerLine)
			{
				myList.Add(TEMP);
				TEMP = "";
				characterCount = SingleWords[I].Length;
			}

			TEMP += SingleWords[I] + ' ';
		}
		myList.Add(TEMP);

	}
	
	void OnGUI()
	{
		pixelOffset = mainCamera.WorldToScreenPoint (new Vector2 (transform.position.x, transform.position.y));	// Turn into pixel coordinates
		GUI.contentColor = Color.black;

		for(int I=0; I<myList.Count; I++)
		{
			GUI.Label(new Rect(pixelOffset.x, Screen.height-pixelOffset.y, lengthOfLabel, 20), myList[I], styleGuide);
			pixelOffset.y -= PixelGap;
		}
	}
}
*/