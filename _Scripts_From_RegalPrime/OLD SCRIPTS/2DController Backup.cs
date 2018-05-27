/*
// RegalPrime 12-02-14 - PlatformerCharacter2D.cs
// BACKUP OF PlatformerCharacter2D.cs
// THIS WAS CREATED BEFORE I TORE APART AND REDID THE SCRIPT TO USE AREA CHECKS
// REMOVE THIS LATER

using UnityEngine;
using System.Collections;

public class PlatformerCharacter2D : MonoBehaviour 
{
	Animator anim;										// Reference to the player's animator component.
	
	public LayerMask whatIsGround;						// A mask determining what is ground to the character
	
	Transform groundCheck;								// A position marking where to check if the player is grounded.
	Transform ceilingCheck;								// A position marking where to check for ceilings
	Transform wallCheck;								// A position marking where to check for ceilings
	
	//	float groundedRadius = .05f;						// Radius of the overlap circle to determine if grounded - was 0.2f
	float ceilingRadius = .05f;							// Radius of the overlap circle to determine if the player can stand up
	float frontRadius = .01f;							// Radius of the overlap circle to determine if the player can stand up
	
	public bool grounded = false;						// Whether or not the player is grounded.
	public bool hitCeiling = false;
	public bool hitFront = false;
	
	
	private float FramesPerSecond = 50;
	private float PixelsPerUnit = 100;
	private float GMtoUnityConversion;
	
	
	private bool jump;									// Used in determining when the button for jump is pressed
	public bool facingRight = true;						// For determining which way the player is currently facing
	public bool isUpright = true;
	public bool doubleJump = true;						// Allows for double jump if true
	
	private float jumpSpeed = 8.5f;
	private float jumpSpeed2 = 7.0f;
	private float moveSpeed = 3.0f;
	public float maxFallSpeed = -9.0f;					// Capped fall speed;
	private float maxFallSpeedBackup;
	private float gravity = 0.4f;
	
	public float gravityMultiplier = 1;
	public float movementMultiplier = 1;
	public float jumpforceMultiplier = 1;
	
	
	public AudioClip jump1Sound;						// First Jump sound
	public AudioClip jump2Sound;						// Second Jump sound
	
	
	public bool disableCharacterMovement = false;		// Can disable directional inputs
	public bool disableCharacterJumping = false;		// Can disable jumping
	public bool disableCharacterShooting = false;		// Can disable the ability to fire
	public bool infinityJump = false;					// Allows for infinite jumping
	public bool airControl = false;						// Whether or not a player can steer while jumping;
	
	private float MoveDirection;
	public bool PixelMovement = false;
	
	void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("GroundCheck");
		ceilingCheck = transform.Find("CeilingCheck");
		wallCheck = transform.Find("WallCheck");
		
		GMtoUnityConversion = FramesPerSecond / PixelsPerUnit;
		jumpSpeed	*= GMtoUnityConversion;
		jumpSpeed2	*= GMtoUnityConversion;
		moveSpeed	*= GMtoUnityConversion;
		gravity 	*= GMtoUnityConversion;
		maxFallSpeed*= GMtoUnityConversion;
		
		maxFallSpeedBackup = maxFallSpeed;
		
		anim = GetComponent<Animator>();
	}
	
	void FixedUpdate()
	{
		// Check if the character is grounded
		Vector2 TopLeft     = new Vector2 (groundCheck.position.x-0.04f, groundCheck.position.y);
		Vector2 BottomRight = new Vector2 (groundCheck.position.x+0.04f, groundCheck.position.y-0.06f);
		grounded = Physics2D.OverlapArea (TopLeft, BottomRight, whatIsGround);
		
		//		grounded = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);
		hitCeiling = Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround);
		hitFront = Physics2D.OverlapCircle(wallCheck.position, frontRadius, whatIsGround);
		
		anim.SetBool("Ground", grounded);												// Set grounded animation tag
		
		//		if(transform.localScale.y == 1)
		if(isUpright)
			anim.SetFloat("vSpeed", rigidbody2D.velocity.y/GMtoUnityConversion);		// Set the vertical animation tag
		else
			anim.SetFloat("vSpeed", -rigidbody2D.velocity.y/GMtoUnityConversion);		// Set the vertical animation tag
		
		
		MoveDirection = Input.GetAxis("Horizontal");
		if(!disableCharacterMovement)													// If able to move character
		{
			if(!PixelMovement)
				Move(MoveDirection);
			else
				MovePixel_X();
		}
		
		if(!disableCharacterJumping)
		{
			Jump (jump);
		}
		
		jump = false;
		
		
		// Apply Gravity
		//		rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, rigidbody2D.velocity.y - gravity*gravityMultiplier);
		rigidbody2D.velocity += new Vector2 (0, -gravity*gravityMultiplier);
		
		
		// If ceiling hit, then force downward velocity
		if(hitCeiling)
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, -2*gravity*gravityMultiplier);
		
		// cap the speed based on yscale (flips the check if the player is upside down)
		if(rigidbody2D.velocity.y*transform.localScale.y < maxFallSpeed*gravityMultiplier && isUpright)
		{
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, maxFallSpeed*gravityMultiplier);
		}
		
		if(rigidbody2D.velocity.y*transform.localScale.y < -maxFallSpeed*gravityMultiplier && !isUpright)
		{
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, maxFallSpeed*gravityMultiplier);
		}
	}
	
	
	
	void Update()
	{
		if (Input.GetButtonDown("Jump"))							// Check if jump button was used
			jump = true;
		
		if (Input.GetButtonUp ("Jump") && rigidbody2D.velocity.y*transform.localScale.y > 0)
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, rigidbody2D.velocity.y*0.45f);
	}
	
	public void Move(float move)
	{
		if(grounded || airControl)									// Move character if able
		{
			anim.SetFloat("Speed", Mathf.Abs(move));				// Set animation controller tag
			
			if(!disableCharacterMovement)							// Move the character
				rigidbody2D.velocity = new Vector2(move * moveSpeed*movementMultiplier, rigidbody2D.velocity.y);
			
			if(move > 0 && !facingRight)							// Flip the characters sprite if needed
				Flip();
			else if(move < 0 && facingRight)
				Flip();
		}
	}
	
	// Move left and right via pixels instead of velocity (used in platforms)
	public void MovePixel_X()
	{
		if(grounded || airControl)									// Move character if able
		{
			anim.SetFloat("Speed", Mathf.Abs(MoveDirection));		// Set animation controller tag
			
			gameObject.transform.localPosition += new Vector3(MoveDirection * moveSpeed*movementMultiplier * Time.deltaTime *Mathf.Abs(transform.localScale.x), 0, 0);
			
			if(MoveDirection > 0 && !facingRight)							// Flip the characters sprite if needed
				Flip();
			else if(MoveDirection < 0 && facingRight)
				Flip();
		}
	}
	
	public void Jump(bool jump)
	{		
		// If the character is on the ground - enable a double jump
		if (grounded)
			doubleJump = true;
		
		if ((grounded || doubleJump || infinityJump) && jump)		// Jump if able
		{
			anim.SetBool("Ground", false);
			
			if(grounded)											// Jumping from ground
			{
				rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpSpeed*jumpforceMultiplier);
				MusicController.control.OneShotAudio(jump1Sound);
			}
			else													// Jumping via double jump / multi jumpeffect
			{
				rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpSpeed2*jumpforceMultiplier);
				MusicController.control.OneShotAudio(jump2Sound);
				
				doubleJump = false;										// Reset double jump
			}
		}
	}
	
	public void SetMaxFallSpeed(float value)
	{
		maxFallSpeed = value*GMtoUnityConversion;
	}
	
	public void ResetMaxFallSpeed()
	{
		maxFallSpeed = maxFallSpeedBackup;
	}
	
	public void SetJumpforceMultiplier(float value)
	{
		jumpforceMultiplier = value;
	}
	
	public void ResetJumpforceMultipler()
	{
		jumpforceMultiplier = 1;
	}
	
	public void SetMovementMultiplier(float value)
	{
		movementMultiplier = value;
	}
	
	public void ResetMovementMultiplier()
	{
		movementMultiplier = 1;
	}
	
	public void SetGravityMultiplier(float value)
	{
		gravityMultiplier = value;
	}
	
	public void ResetGravityMultiplier()
	{
		gravityMultiplier = 1;
	}
	
	// Flips the charcter horizontally
	public void Flip ()
	{
		facingRight = !facingRight;					// Switch the way the player is labelled as facing.
		
		Vector3 theScale = transform.localScale;	// Multiply the player's x local scale by -1.
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	
	// Makes the character flip upright if not already upright
	public bool FlipCharacter_Upright()
	{
		if(!isUpright)					// If not upright - flip the character
		{
			isUpright = !isUpright;
			
			gravityMultiplier *= -1;
			jumpforceMultiplier *= -1;
			
			Vector3 theScale = transform.localScale;
			theScale.y *= -1;
			transform.localScale = theScale;
			
			return true;
		}
		else
			return false;
	}
	
	// Makes the character flip upside down if not already upsidedown
	public bool FlipCharacter_UpsideDown()
	{
		if(isUpright)					// If upright - flip the character
		{
			isUpright = !isUpright;
			
			gravityMultiplier *= -1;
			jumpforceMultiplier *= -1;
			
			Vector3 theScale = transform.localScale;
			theScale.y *= -1;
			transform.localScale = theScale;
			
			return true;
		}
		else
			return false;
	}
	
	// Filps the character
	public void FlipCharacter()
	{
		isUpright = !isUpright;
		
		gravityMultiplier *= -1;
		jumpforceMultiplier *= -1;
		
		Vector3 theScale = transform.localScale;
		theScale.y *= -1;
		transform.localScale = theScale;
	}
}

*/