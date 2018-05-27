// RegalPrime 11-25-14 - DisablePlatformCollision.cs

// This was written for JumpThroughPlatforms
// It detects when the player is below / above a platform and will change the layer mask accordingly
// This is done via collider calculations

// When the player is above the platform - the platform layer mask will become = Ground (and thus solid)
// When the player is below the platform - the platform layer mask will become = IgnorePlayerLayerIndex (and thus the player will be able to pass through it)
// The mask is changed instead of disabling the collider because this will only effect the player (else monsters would also fall through when effected).

// PixelMovement is used on the player when inside the collider to deal with a pulling problem when platforms are moving and the player is using velocity based movement
// This was put in and it seems to really help with the horizontal pulling problem.

using UnityEngine;
using System.Collections;

public class DisablePlatformCollision : MonoBehaviour
{
	public GameObject platform;					// Object to jump through
	public bool allowForDropThrough = false;	// Pressing down will allow for falling through this object

	private bool canDropThrough = false;		// used in buttong detection

	private int GroundLayerIndex;
	private int IgnorePlayerLayerIndex;

//	private float SpriteWidth;
	public float SpriteHeight = 0.16f;			// The height of the sprite - Used to calculate when the player is above this object (and thus enable a solid platform)

//	private float GuySpriteWidth;
	private float GuySpriteHeight;
	private float GuyOffsetX;
	private float GuyOffsetY;

	private 


	void Start()
	{
		EventManager.resetObjects += Reset;
		StartCoroutine (C_Start ());
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		gameObject.GetComponent<BoxCollider2D> ().enabled = false;
		gameObject.GetComponent<BoxCollider2D> ().enabled = true;
		platform.layer = GroundLayerIndex;
	}

	IEnumerator C_Start()		// Get all necessary collider information and calculate needed information
	{
		yield return new WaitForEndOfFrame();

		GroundLayerIndex = LayerMask.NameToLayer("SolidObjects");
		IgnorePlayerLayerIndex = LayerMask.NameToLayer("IgnorePlayer");

		// Get players collider stats
		if(GameController.control.GuyLocation.GetComponent<BoxCollider2D> () != null)
		{
			//			BoxCollider2D Poly = GameController.control.GuyLocation.GetComponent<BoxCollider2D> ();
			//			GuySpriteWidth = Poly.bounds.size.x - Poly.center.x*2;
			//			GuySpriteHeight = Poly.bounds.size.y - Poly.center.y*2;
			
			BoxCollider2D Poly = GameController.control.GuyLocation.GetComponent<BoxCollider2D> ();
			//			GuySpriteWidth = Poly.bounds.size.x;
			GuySpriteHeight = Poly.bounds.size.y;
			//			GuyOffsetX = Poly.center.x;
			GuyOffsetY = Poly.offset.y;
			
			
			//			print (GuySpriteWidth + ", " + GuySpriteHeight);
		}
	}

	void OnTriggerEnter2D()
	{

	}

	void FixedUpdate()
	{
		if(Input.GetKey(KeyCode.DownArrow))
			canDropThrough = true;

		if(Input.GetKeyUp(KeyCode.DownArrow))
			canDropThrough = false;
	}

	void OnTriggerStay2D(Collider2D other)			// while in the collider, check position and change layermask if accordingly 
	{
		if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlatformerCharacter2D>() != null)
		{
			PlatformerCharacter2D GuysScript = other.gameObject.GetComponent<PlatformerCharacter2D>();
			BoxCollider2D Poly = other.gameObject.GetComponent<BoxCollider2D> ();
			GuySpriteHeight = Poly.bounds.size.y;
			GuyOffsetY = Poly.offset.y;

			if((GuysScript.transform.position.y-GuySpriteHeight/2+GuyOffsetY+0.00f) > platform.transform.position.y+SpriteHeight/2) // print (" I AM ABOVE");
			{
				platform.layer = GroundLayerIndex;
				GuysScript.pixelMovement = true;
				GuysScript.StopHorizontalMovement();
	//			print (" I AM ABOVE");
			}

			if((GuysScript.transform.position.y-GuySpriteHeight/2+GuyOffsetY <= platform.transform.position.y+SpriteHeight/2) || (canDropThrough && allowForDropThrough)) //print (" I AM BELOW");
			{
	//			print (" I AM BELOW");
				platform.layer = IgnorePlayerLayerIndex;
				GuysScript.Refresh_MultiJump();

				canDropThrough = false;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlatformerCharacter2D>() != null)
		{
			other.gameObject.GetComponent<PlatformerCharacter2D>().pixelMovement = false;
			platform.layer = GroundLayerIndex;
		}
	}
}


/*
using UnityEngine;
using System.Collections;

public class DisablePlatformCollision : MonoBehaviour
{
	public GameObject platform;					// Object to jump through
	public bool allowForDropThrough = false;	// Pressing down will allow for falling through this object

	private bool canDropThrough = false;		// used in buttong detection

	private int GroundLayerIndex;
	private int IgnorePlayerLayerIndex;

//	private float SpriteWidth;
	public float SpriteHeight = 0.16f;			// The height of the sprite - Used to calculate when the player is above this object (and thus enable a solid platform)

//	private float GuySpriteWidth;
	private float GuySpriteHeight;
	private float GuyOffsetX;
	private float GuyOffsetY;


	void Start()
	{
		ResetManager.resetObjects += Reset;
		StartCoroutine (C_Start ());
	}
	void OnDestroy()
	{
		ResetManager.resetObjects -= Reset;
	}
	void Reset()
	{
		GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D>().set_PixelMovement(false);
		gameObject.GetComponent<BoxCollider2D> ().enabled = false;
		gameObject.GetComponent<BoxCollider2D> ().enabled = true;
		platform.layer = GroundLayerIndex;
	}

	IEnumerator C_Start()		// Get all necessary collider information and calculate needed information
	{
		yield return new WaitForEndOfFrame();

		GroundLayerIndex = LayerMask.NameToLayer("Ground");
		IgnorePlayerLayerIndex = LayerMask.NameToLayer("Ignore Player");
		
		// Get gameobjects collider stats
//		if(gameObject.GetComponent<BoxCollider2D> () != null)
//		{
//			BoxCollider2D Poly = gameObject.GetComponent<BoxCollider2D> ();
//			SpriteWidth = Poly.bounds.size.x - Poly.center.x;
//			SpriteHeight = Poly.bounds.size.y - Poly.center.y;
			
//			print (SpriteWidth + ", " + SpriteHeight);
//		}
		
		// Get players collider stats
		if(GameController.control.GuyLocation.GetComponent<BoxCollider2D> () != null)
		{
			//			BoxCollider2D Poly = GameController.control.GuyLocation.GetComponent<BoxCollider2D> ();
			//			GuySpriteWidth = Poly.bounds.size.x - Poly.center.x*2;
			//			GuySpriteHeight = Poly.bounds.size.y - Poly.center.y*2;
			
			BoxCollider2D Poly = GameController.control.GuyLocation.GetComponent<BoxCollider2D> ();
			//			GuySpriteWidth = Poly.bounds.size.x;
			GuySpriteHeight = Poly.bounds.size.y;
			//			GuyOffsetX = Poly.center.x;
			GuyOffsetY = Poly.center.y;
			
			
			//			print (GuySpriteWidth + ", " + GuySpriteHeight);
		}
	}

	void OnTriggerEnter2D()
	{

	}

	void FixedUpdate()
	{
		if(Input.GetKey(KeyCode.DownArrow))
			canDropThrough = true;

		if(Input.GetKeyUp(KeyCode.DownArrow))
			canDropThrough = false;
	}

	void OnTriggerStay2D(Collider2D other)			// while in the collider, check position and change layermask if accordingly 
	{
		if (other.gameObject.tag == "Player")
			{
			if((GameController.control.GuyLocation.transform.position.y-GuySpriteHeight/2+GuyOffsetY+0.00f) > platform.transform.position.y+SpriteHeight/2) // print (" I AM ABOVE");
			{
				platform.layer = GroundLayerIndex;
				GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D>().set_PixelMovement(true);
				GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D> ().StopHorizontalMovement();
//				print (" I AM ABOVE");
			}

			if((GameController.control.GuyLocation.transform.position.y-GuySpriteHeight/2+GuyOffsetY <= platform.transform.position.y+SpriteHeight/2) || (canDropThrough && allowForDropThrough)) //print (" I AM BELOW");
			{
//				print (" I AM BELOW");
				platform.layer = IgnorePlayerLayerIndex;
				GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D> ().Refresh_MultiJump();

				canDropThrough = false;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			GameController.control.GuyLocation.GetComponent<PlatformerCharacter2D>().set_PixelMovement(false);
			platform.layer = GroundLayerIndex;
		}
	}
}
*/
