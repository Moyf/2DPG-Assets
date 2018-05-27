// RegalPrime 11-25-14 - CycleThroughText.cs

// Shows a set of chat above the gameobject when the player enters the collider
// The strings of text in the vector Text, will be displayed one after another with a delay
// You can add more lines by increasing the size of the string vector "Text" in the editor

// Text is displayed by creating a GUI prefab that has the text element on it


using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CycleThroughText : MonoBehaviour
{
	public GameObject basicLabelprefab;			// The basic label used when displaying text
	public float ChatChangeDelay = 3f;			// Delay between each string vector index
//	public int FontSize = 9;					// Font of text - this scales with the size of the screen
//	public float labelLength = 125;

	public bool LoopText = false;				// If this is true, the text will start over when reaching the end of the vector
	public string[] Text;						// All of the text to display

	GameObject CloneLabel;

	void Start()
	{
		EventManager.resetObjects += Reset;
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		StopAllCoroutines ();
		if(CloneLabel != null)
			Destroy (CloneLabel);
		gameObject.GetComponent<BoxCollider2D> ().enabled = false;
		gameObject.GetComponent<BoxCollider2D> ().enabled = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			StartCoroutine (C_ChatSwapping());
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			StopAllCoroutines();
			if(CloneLabel != null)
				Destroy (CloneLabel);
		}
	}

	IEnumerator C_ChatSwapping()
	{
		for(int i=0; i<Text.Length; i++)
		{
			MakeLabel();
			CloneLabel.GetComponentInChildren<Text>().text = Text[i];

			yield return new WaitForSeconds (ChatChangeDelay);
		}

		if(LoopText)
			StartCoroutine (C_ChatSwapping());
		else
			CloneLabel.SetActive (false);
	}

	void MakeLabel()
	{
		if(CloneLabel != null)
			Destroy (CloneLabel);
		CloneLabel = Instantiate (basicLabelprefab, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -9), Quaternion.identity) as GameObject;
		CloneLabel.transform.SetParent(gameObject.transform);
	}
}

/*
// OLD VERSION - PRE GUI UPDATE
// RegalPrime 11-25-14 - CycleThroughText.cs

// Shows a set of chat above the gameobject when the player enters the collider
// The strings of text in the vector Text, will be displayed one after another with a delay
// You can add more lines by increasing the size of the string vector "Text" in the editor

// Text is displayed using the BasicLabel.cs script

// public GameObject basicLabelprefab;
// This is an object with a BasicLabel script attached to it.
// This also a styleGuide which is put on by the BasicLabel script
// For the most part, dont change what object is put at this location


using UnityEngine;
using System.Collections;

public class CycleThroughText : MonoBehaviour
{
	public GameObject basicLabelprefab;			// The basic label used when displaying text
	public float ChatChangeDelay = 3f;			// Delay between each string vector index
	public int FontSize = 9;					// Font of text - this scales with the size of the screen
	public float labelLength = 125;

	public bool LoopText = false;				// If this is true, the text will start over when reaching the end of the vector
	public string[] Text;						// All of the text to display

	BasicLabel newLabel;
	GameObject CloneLabel;

	void Start()
	{
		ResetManager.resetObjects += Reset;
	}
	void OnDestroy()
	{
		ResetManager.resetObjects -= Reset;
	}
	void Reset()
	{
		StopAllCoroutines ();
		if (newLabel != null)
			Destroy (newLabel);
		gameObject.GetComponent<BoxCollider2D> ().enabled = false;
		gameObject.GetComponent<BoxCollider2D> ().enabled = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			StartCoroutine (C_ChatSwapping());
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			StopAllCoroutines();
			if(CloneLabel != null)
				Destroy (CloneLabel);
		}
	}

	IEnumerator C_ChatSwapping()
	{
		for(int i=0; i<Text.Length; i++)
		{
			MakeLabel();
			newLabel.TextToDisplay = Text[i];

			yield return new WaitForSeconds (ChatChangeDelay);
		}

		if(LoopText)
			StartCoroutine (C_ChatSwapping());
		else
			CloneLabel.SetActive (false);
	}

	void MakeLabel()
	{
		if(CloneLabel != null)
			Destroy (CloneLabel);
		CloneLabel = Instantiate (basicLabelprefab, gameObject.transform.position, Quaternion.identity) as GameObject;
		CloneLabel.transform.parent = gameObject.transform;
		newLabel = CloneLabel.GetComponent<BasicLabel> ();
		newLabel.lengthOfLabel = labelLength;
		newLabel.styleGuide.fontSize = FontSize;
	}
}
*/