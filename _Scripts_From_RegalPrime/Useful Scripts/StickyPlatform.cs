// RegalPrime 11-27-14 - StickyPlatform.cs

// Makes the player the child of this object when triggered
// Created to make the player stick to a moving platform


using UnityEngine;
using System.Collections;

public class StickyPlatform : MonoBehaviour
{
	void Start ()
	{

	}
	
	void Update ()
	{
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == ("Player") && other.GetComponent<PlatformerCharacter2D>())
		{
			other.gameObject.transform.parent = this.transform;
			other.GetComponent<PlatformerCharacter2D>().pixelMovement = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.tag == ("Player") && other.transform.parent == this.transform && other.GetComponent<PlatformerCharacter2D>())
		{
			other.gameObject.transform.parent = null;

			other.GetComponent<PlatformerCharacter2D>().Refresh_MultiJump();
			other.GetComponent<PlatformerCharacter2D>().pixelMovement = false;
		}
	}

//	void OnDestroy()	// Not sure I even need this anymore
//	{
//		if (GameController.control.GetPlayerGameObject() != null && GameController.control.GetPlayerGameObject().transform.parent == this.transform)
//			GameController.control.GetPlayerGameObject().transform.parent = null;
//	}
}


/*

using UnityEngine;
using System.Collections;

public class StickyPlatform : MonoBehaviour
{
	void Start ()
	{

	}
	
	void Update ()
	{
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == ("Player"))
		{
			other.gameObject.transform.parent = this.transform;
			GameController.control.GetPlayerGameObject().GetComponent<PlatformerCharacter2D>().set_PixelMovement(true);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.tag == ("Player") && GameController.control.GetPlayerGameObject().transform.parent == this.transform)
		{
			other.gameObject.transform.parent = null;

			GameController.control.GetPlayerGameObject().GetComponent<PlatformerCharacter2D>().Refresh_MultiJump();
			GameController.control.GetPlayerGameObject().GetComponent<PlatformerCharacter2D>().set_PixelMovement(false);
		}
	}

	void OnDestroy()
	{
		if (GameController.control.GetPlayerGameObject() != null && GameController.control.GetPlayerGameObject().transform.parent == this.transform)
			GameController.control.GetPlayerGameObject().transform.parent = null;
	}
}
*/