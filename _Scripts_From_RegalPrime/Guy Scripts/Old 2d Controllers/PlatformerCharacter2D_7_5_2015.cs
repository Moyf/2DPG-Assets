/*

// RegalPrime 1-21-15 - PlatformerCharacter2D.cs

// This is the parent class of both the physics and pixel based movement script (PlatformerCharacter2D_Physics / PlatformerCharacter2D_Pixel)
// This allows other gameobjects to effect the children without having to change other scripts
// Most of the variables are private and only accessable through functions

// Should I be making this an abstract class and force the children to override the abstract methods instead of hiding them?


using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class PlatformerCharacter2D : MonoBehaviour 
{
	public LayerMask whatIsGround;						// What is considered ground
	public AudioClip jump1Sound;						// First Jump sound
	public AudioClip jump2Sound;						// Second Jump sound
	
	public float gravityMultiplier = 1;					// Gravity multiplier
	public float movementMultiplier = 1;				// Movement multiplier
	public float jumpforceMultiplier = 1;				// Jump force multiplier
	public bool infinityJump = false;					// Allows for infinite jumping
	
	// EVERYTHING ABOVE HAS TO BE PUBLIC AND CLICKABLE IN EDITOR
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// WHAT TO DO WITH THESE VARIABLES ??
	
	
	public float currentFallSpeed = 0;					// The current fall speed
	public float MoveDirection;							// The directional horizontal key input 
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// // EVERYTHING BELOW HAS TO BE PRIVATE AND HAS FUNCTIONS TO ACCESS / CHANGE THEIR VARIABLES
	
	// Stats are gamemaker standard (pixels / frame) and are changed in code so that unity can more easily use them (unity likes pixels (or units) / sec when using velocity)
	private float jumpSpeed = 8.5f;						// Jump speed
	private float jumpSpeed2 = 7.0f;					// Jump speed for 2nd jump
	private float moveSpeed = 3.0f;						// Move speed
	private float maxFallSpeed = -9.0f;					// Capped fall speed
	private float gravity = 0.4f;						// Gravity on the character
	
	private float B_gravityMultiplier = 1;				// Backup gravity multiplier
	private float B_maxFallSpeed = 1;					// A backup of the fall speed
	
	private int current_MultiJumps = 0;					// Current number of extra jumps the player can make
	private int max_MultiJumps = 1;						// Max number of extra jumps the player can take
	private bool PixelMovement = false;					// should it use pixel movements instead of physics
	private bool disableCharacterMovement = false;		// Can disable directional inputs
	private bool disableCharacterJumping = false;		// Can disable jumping
	private bool disableCharacterShooting = false;		// Can disable the ability to fire
	
	private Animator anim;								// Reference to the player's animator component.
	private CalculateChecks calculateChecks;			// The script that calculates the distance between two colliders
	
	private float FramesPerSecond = 50;					// How many fps is this game supposed to run at
	private float PixelsPerUnit = 100;					// How many pixels per unit
	private float GMtoUnityConversion;					// Calculated conversion between gamemaker guy stats and unity
	
	private bool grounded = false;						// Charcter on ground?
	private bool hitCeiling = false;					// Character hit ceiling?
	private bool hitWall = false;						// Character hit wall?
	
	private bool facingRight = true;					// For determining which way the player is currently facing
	private bool isUpright = true;						// For determining which way the player is flipped
	
	private float colliderBuffer = 0;					// 
	private bool AllowPreCheck_Wall = true;				// Allow checks to have an effect on movement
	private bool AllowPreCheck_Ground = true;			// Allow checks to have an effect on movement
	private bool AllowPreCheck_Ceiling = true;			// Allow checks to have an effect on movement
	
	private float DistanceToGround = 1;					// Calculated distance to the ground
	private float DistanceToWall = 1;					// Calculated distance to the wall
	private float DistanceToCeiling = 1;				// Calculated distance to the ceiling
	
	public void set_ColliderBuffer(float aFloat)
	{
		colliderBuffer = aFloat;
	}
	
	
	void Awake()
	{
		GetComponent<Rigidbody2D>().velocity = new Vector2 (0, 0);
		
		GMtoUnityConversion = FramesPerSecond / PixelsPerUnit;
		jumpSpeed	*= GMtoUnityConversion;
		jumpSpeed2	*= GMtoUnityConversion;
		moveSpeed	*= GMtoUnityConversion;
		gravity 	*= GMtoUnityConversion;
		maxFallSpeed*= GMtoUnityConversion;
		
		B_maxFallSpeed = maxFallSpeed;
		
		anim = GetComponent<Animator>();		// Reference to the animator
		
		if(GetComponent<CalculateChecks> ())
			calculateChecks = GetComponent<CalculateChecks> ();
		else
			calculateChecks = gameObject.AddComponent<CalculateChecks> ();
		calculateChecks.ChangeLayerMask (whatIsGround);
	}
	
	public void Start()
	{
		EventManager.resetObjects += Reset;
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		FlipCharacter_Upright ();
		gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		currentFallSpeed = 0;
		Refresh_MultiJump ();
		PixelMovement = false;
		gameObject.transform.parent = null;
		
		
		//		ResetGravityMultiplier ();
		//		ResetJumpforceMultipler ();
		//		ResetMaxFallSpeed ();
		//		ResetMovementMultiplier ();
	}
	
	
	void FixedUpdate()
	{
	}
	
	void Update()
	{
	}
	
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void SetAnimationVariables()
	{
		anim.SetBool("Ground", grounded);										// Set grounded animation tag
		anim.SetFloat("horizontalSpeed", Mathf.Abs(MoveDirection));						// Set animation controller tag
		
		if(isUpright)
			anim.SetFloat("fallSpeed", currentFallSpeed/GMtoUnityConversion);		// Set the vertical animation tag
		else
			anim.SetFloat("fallSpeed", -currentFallSpeed/GMtoUnityConversion);		// Set the vertical animation tag
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void Check_Ground()
	{
		DistanceToGround = calculateChecks.GroundCheck (isUpright);					// Calculate Ground check
		if(DistanceToGround < 0.02f+colliderBuffer && currentFallSpeed*gravityMultiplier <= 0)
		{
			if(current_MultiJumps <= max_MultiJumps)
				current_MultiJumps = max_MultiJumps;
			grounded = true;
		}
		else
			grounded = false;
	}
	public void Check_Wall()
	{
		DistanceToWall = calculateChecks.WallCheck (isUpright, facingRight);		// Calculate Wall check
		if(DistanceToWall < 0.02f+colliderBuffer)
			hitWall = true;
		else
			hitWall = false;
	}
	public void Check_Ceiling()
	{
		DistanceToCeiling = calculateChecks.CeilingCheck (isUpright);				// Calculate Ceiling check
		if (DistanceToCeiling < 0.02f+colliderBuffer)
			hitCeiling = true;
		else
			hitCeiling = false;
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Move the player based on current MoveDirection variable (either -1 or +1 or 0)
	public IEnumerator Move()
	{
		yield return null;
	}
	
	// Move left and right via velocity
	public void MoveVelocity()
	{
		
	}
	// Move left and right via pixels instead of velocity (used in platforms)
	// This does not have wall protection built into it.
	public void MovePixel_X()
	{
		
	}
	
	public IEnumerator Jump()
	{	
		yield return null;
	}
	
	public void ApplyGravity()
	{
		
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void ZeroFallSpeed()
	{
		currentFallSpeed = 0;
		GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, 0);
	}
	public void StopHorizontalMovement()
	{
		GetComponent<Rigidbody2D>().velocity = new Vector2 (0, GetComponent<Rigidbody2D>().velocity.y);
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
			
			gravityMultiplier = 1;
			jumpforceMultiplier = 1;
			
			Vector3 theScale = transform.localScale;
			theScale.y = 1;
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
	
	// Filps the character vertically
	public void FlipCharacter()
	{
		isUpright = !isUpright;
		
		gravityMultiplier *= -1;
		jumpforceMultiplier *= -1;
		
		Vector3 theScale = transform.localScale;
		theScale.y *= -1;
		transform.localScale = theScale;
	}
	
	public void SetMaxFallSpeed(float value)
	{
		maxFallSpeed = value*GMtoUnityConversion;
	}
	public void ResetMaxFallSpeed()
	{
		maxFallSpeed = B_maxFallSpeed;
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
	public void Save_GravityMultiplier()
	{
		B_gravityMultiplier = gravityMultiplier;
	}
	public void Load_GravityMultiplier()
	{
		gravityMultiplier = B_gravityMultiplier;
	}
	public void ResetGravityMultiplier()
	{
		gravityMultiplier = 1;
	}
	
	
	public bool Can_MultiJump()		// Can the player use an extra jump?
	{
		if(current_MultiJumps > 0)
			return true;
		else
			return false;
	}
	public void MultiJumpUsed()		// Player has used an extra jump
	{
		current_MultiJumps --;
	}
	public void Refresh_MultiJump()	// Refresh the number of extra jumps
	{
		current_MultiJumps = max_MultiJumps;
	}
	public void AddToMultiJump(int NumberOfJumps, bool GoOverCap)
	{
		current_MultiJumps += NumberOfJumps;
		if(current_MultiJumps > max_MultiJumps && !GoOverCap)
			current_MultiJumps = max_MultiJumps;
		
	}
	public void Set_MultiJumps(int NumberOfExtraJumps)	// Change the number of extra jumps
	{
		max_MultiJumps = NumberOfExtraJumps;
	}
	public void Reset_MultiJump()						// Changes the number of extra jumps back down to 1
	{
		max_MultiJumps = 1;
	}
	
	
	public bool get_facingRight()
	{
		return facingRight;
	}
	public bool get_isUpright()
	{
		return isUpright;
	}
	
	public bool get_PixelMovement()
	{
		return PixelMovement;
	}
	public void set_PixelMovement(bool aBool)
	{
		PixelMovement = aBool;
	}
	
	public bool get_grounded()
	{
		return grounded;
	}
	public void set_grounded (bool abool)
	{
		grounded = abool;
	}	
	public bool get_hitCeiling()
	{
		return hitCeiling;
	}
	public void set_hitCeiling(bool abool)
	{
		hitCeiling = abool;
	}
	public bool get_hitWall()
	{
		return hitWall;
	}
	
	
	public bool get_AllowPreCheck_Wall()
	{
		return AllowPreCheck_Wall;
	}
	public void set_AllowPreCheck_Wall(bool abool)
	{
		AllowPreCheck_Wall = abool;
	}
	public bool get_AllowPreCheck_Ground()
	{
		return AllowPreCheck_Ground;
	}
	public void set_AllowPreCheck_Ground(bool abool)
	{
		AllowPreCheck_Ground = abool;
	}
	public bool get_AllowPreCheck_Ceiling()
	{
		return AllowPreCheck_Ceiling;
	}
	public void set_AllowPreCheck_Ceiling(bool abool)
	{
		AllowPreCheck_Ceiling = abool;
	}
	
	
	public float get_DistanceToGround()
	{
		return DistanceToGround;
	}
	public float get_DistanceToWall()
	{
		return DistanceToWall;
	}
	public float get_DistanceToCeiling()
	{
		return DistanceToCeiling;
	}
	
	public float get_jumpSpeed()
	{
		return jumpSpeed;
	}
	public float get_jumpSpeed2()
	{
		return jumpSpeed2;
	}
	public float get_moveSpeed()
	{
		return moveSpeed;
	}
	public float get_maxFallSpeed()
	{
		return maxFallSpeed;
	}
	public float get_gravity()
	{
		return gravity;
	}
	
	public bool get_DisableCharacterMovement()
	{
		return disableCharacterMovement;
	}	
	public void set_CharacterMovement(bool aBool)
	{
		disableCharacterMovement = aBool;
	}
	public bool get_DisableCharacterJumping()
	{
		return disableCharacterJumping;
	}
	public void set_CharacterJumping(bool aBool)
	{
		disableCharacterJumping = aBool;
	}
	public bool get_DisableCharacterShooting()
	{
		return disableCharacterShooting;
	}
	public void set_CharacterShooting(bool aBool)
	{
		disableCharacterShooting = aBool;
	}
	
	
	
	
}
*/