// RegalPrime 12-01-14 - Climbing.cs

// Allows the player to climb objects that are tagged with the "Climbable" tag
// It disables the normal player movement and move then via transforms when on the climbable object

// Uses preventive checks to prevent the player from studdering when hitting the solid objects while climbing

using UnityEngine;
using System.Collections;

public class Climbing : MonoBehaviour
{
	public float HclimbingSpeed = 1;			// Speed at which the character climbs horizontally
	public float VclimbingSpeed = 1.5f;			// Speed at which the character climbs vertically

	private float vineVerticalSpeed = 0;		// Speed the character is moving horizontally
	private float vineHorizontalSpeed = 0;		// Speed the character is moving vertically

//	private float localScale_X = 1;
//	private float localScale_Y = 1;

	private bool isClimbing = false;			// Is the character climbing an object?
	private int collisionCount = 0;				// Used in checking multi collider based objects

	private GameObject climbableObject;				// Reference to the object the character is climbing
	private PlatformerCharacter2D PlayerScript;		// Reference to the player script

	Animator anim;

	void Start()
	{
		EventManager.resetObjects += Reset;
		anim = GetComponent<Animator>();	// Get animator so we can tell it when we are climbing

		PlayerScript = GetComponent<PlatformerCharacter2D> ();
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		DetachFromObject ();
	}
	void OnEnable()
	{

	}

	void OnTriggerEnter2D(Collider2D other)
	{

	}

	void FixedUpdate()
	{
//		print (transform.localPosition.x+ ", " +transform.localPosition.y);

		if(isClimbing)	// If climbing, move based on inputs and via local scale of the object you are connected
		{
			// This transform does it based on global direction (thus moving "up" will cause the player to move upwards on the scrren wile climbing the object)
			if(vineHorizontalSpeed > 0 && !PlayerScript.facingRight)							// Flip the characters sprite if needed
				PlayerScript.Flip();
			else if(vineHorizontalSpeed < 0 && PlayerScript.facingRight)
				PlayerScript.Flip();


			if(Mathf.Abs (vineHorizontalSpeed) > 0)	// If climbing horizontally
			{
				PlayerScript.Check_Wall();
				if(!PlayerScript.hitWall)
				{
					if(HclimbingSpeed*Time.deltaTime > PlayerScript.distanceToWall)
						transform.position += new Vector3(vineHorizontalSpeed*PlayerScript.distanceToWall, 0);
					else
						transform.position += new Vector3(vineHorizontalSpeed*HclimbingSpeed*Time.deltaTime, 0);
				}
			}

			if(vineVerticalSpeed > 0)		// If climbing up
			{
				PlayerScript.Check_Ceiling();

				if(!PlayerScript.hitCeiling)
				{
					if(VclimbingSpeed*Time.deltaTime > PlayerScript.distanceToCeiling)
						transform.position += new Vector3(0, PlayerScript.distanceToCeiling);
					else
						transform.position += new Vector3(0, vineVerticalSpeed*VclimbingSpeed*Time.deltaTime);
				}
			}
			else 							// If climbing down
//			if(vineVerticalSpeed < 0)
			{
				PlayerScript.Check_Ground();

				if(!PlayerScript.onGround)
				{
					if(VclimbingSpeed*Time.deltaTime > PlayerScript.distanceToGround)
						transform.position -= new Vector3(0, PlayerScript.distanceToGround);
					else
						transform.position += new Vector3(0, vineVerticalSpeed*VclimbingSpeed*Time.deltaTime);
				}
			}
					
			anim.SetFloat("verticalSpeed", Mathf.Abs(vineVerticalSpeed));
			anim.SetFloat ("horizontalSpeed", Mathf.Abs (vineHorizontalSpeed));
		}

		if(anim != null)
		anim.SetBool("Climbing", isClimbing);
	}

	void Update()
	{
		if (Input.GetButtonDown ("Jump") && isClimbing)					// If trying to jump from an object
		{
			DetachFromObject();
		}

		vineVerticalSpeed = Input.GetAxis("Vertical");					// Get vertical input
		vineHorizontalSpeed = Input.GetAxis("Horizontal");				// Get horizontal input
	}

	// If in contact with the collider
	void OnTriggerStay2D(Collider2D other)
	{
		if(Mathf.Abs(vineVerticalSpeed)>0 && other.gameObject.tag == ("Climbable") && !isClimbing && collisionCount<1 && other.gameObject != climbableObject)	// If up / down input and not climbing yet
		{
//			print (rigidbody2D.velocity.x + " " + rigidbody2D.velocity.y);
//			print ("I FOUND A NEW OBJECT TO CLIMB");

			climbableObject = other.gameObject;											// get the object you are climbing

			collisionCount++;

			transform.parent = other.transform;											// Set as child of object

			PlayerScript.Save_GravityMultiplier();
			PlayerScript.gravityMultiplier = 0;					// No more gravity
			PlayerScript.ZeroFallSpeed();

			PlayerScript.StopHorizontalMovement ();				// Stops current velocity
			PlayerScript.disableCharacterMovement = true;		// No more default character movement
			PlayerScript.Refresh_MultiJump();					// Refresh double jump

			isClimbing = true;														// Tell script character is now climbing
			anim.SetBool("Climbing", isClimbing);									// Tell animator we are now climbing
			anim.SetTrigger ("ClimbingTrigger");
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject == climbableObject)										// Detach from object if conditions are met
		{
			DetachFromObject();
		}
	}

	void DetachFromObject()
	{
		collisionCount--;
		
		if(collisionCount <0)
			collisionCount = 0;

		transform.parent = null;													// Detach from parent object

		transform.rotation = Quaternion.identity;									// Reset rotation

		PlayerScript.disableCharacterMovement = false;								// Enable horizontal controls
		PlayerScript.Load_GravityMultiplier();

		isClimbing = false;	

		if(gameObject.activeInHierarchy)
			anim.SetBool("Climbing", isClimbing);

		climbableObject = null;

//		print ("I AM DETACHED FROM THIS OBJECT");
	}

}