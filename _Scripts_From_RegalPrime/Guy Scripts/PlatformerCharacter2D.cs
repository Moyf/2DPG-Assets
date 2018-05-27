// RegalPrime 1-21-15 - PlatformerCharacter2D.cs


using UnityEngine;
using System.Collections;

public abstract class PlatformerCharacter2D : MonoBehaviour
{	
	public LayerMask whatIsGround;						// What is considered ground
	public AudioClip jump1Sound;						// First Jump sound
	public AudioClip jump2Sound;						// Second Jump sound

	private float moveDirection = 0; 					// The current move direction based on keyboard input
	private bool _isMoving = false;						// Can the move function be called? onkeypressed
	private bool _isJumping = false;					// Can the jump function be called? onkeypressed

	private float _currentHorizontalSpeed = 0;			// Current horizontal move speed
	private float _currentFallSpeed = 0;				// The current fall speed

	private float _gravityMultiplier	= 1;			// Gravity multiplier
	private float _movementMultiplier	= 1;			// Movement multiplier
	private float _jumpforceMultiplier	= 1;			// Jump force multiplier

	private float B_gravityMultiplier 	= 1;			// Backup gravity multiplier
	private float B_maxFallSpeed		= 1;			// Backup of the max fall speed

														// Stats are based on Gamemaker values (next 5 variables)
	private float _jumpSpeed 	= 8.5f;					// Jump speed
	private float _jumpSpeed2 	= 7.0f;					// Jump speed for 2nd jump
	private float _moveSpeed 	= 3.0f;					// Move speed
	private float _maxFallSpeed =-9.0f; 				// Capped fall speed
	private float _gravity 		= 0.4f;					// Gravity on the character

	private int current_MultiJumps = 0;					// Current number of extra jumps the player can make
	private int max_MultiJumps = 1;						// Max number of extra jumps the player can take
	public bool _infinityJump = false;					// Allows for infinite jumping

	private float FramesPerSecond = 50;					// How many fps is this game supposed to run at
	private float PixelsPerUnit = 100;					// How many pixels per unit
	private float GMtoUnityConversion;					// Calculated conversion between gamemaker guy stats and unity
	
	public bool pixelMovement 			{ get; set;}	// should it use pixel movements instead of physics
	public bool disableCharacterMovement{ get; set;}	// Can disable directional inputs
	public bool disableCharacterJumping { get; set;}	// Can disable jumping
	public bool disableCharacterShooting{ get; set;}	// Can disable the ability to fire

	private CalculateChecks calculateChecks;			// Reference to the script that calculates the distance between two colliders
	private Animator anim;								// Reference to the player's animator component.
	private Rigidbody2D _theRigidbody2D;				// Reference to the player's rigidbody2d

	private bool _facingRight = true;					// For determining which way the player is currently facing
	private bool _isUpright = true;						// For determining which way the player is flipped

	private float howCloseIsSolid = 0.02f;				// How close is close enough to be considred near an object?
	public float colliderBuffer { get; set;}			// The added buffer that deals with fatter colliders in the editor settings (set by child)
	
	private float _distanceToGround = 1;				// Calculated distance to the ground
	private float _distanceToWall = 1;					// Calculated distance to the wall
	private float _distanceToCeiling = 1;				// Calculated distance to the ceiling

	private bool _onGround   = false;					// Charcter on ground?
	private bool _hitCeiling = false;					// Character hit ceiling?
	private bool _hitWall    = false;					// Character hit wall?

	private bool _AllowPreCheck_Wall = true;			// Allow prechecks to have an effect on movement
	private bool _AllowPreCheck_Ground = true;			// Allow prechecks to have an effect on movement
	private bool _AllowPreCheck_Ceiling = true;			// Allow prechecks to have an effect on movement


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float currentHorizontalSpeed {get{return _currentHorizontalSpeed;} set{_currentHorizontalSpeed = value; }}
	public float currentFallSpeed		{get{return _currentFallSpeed;} set{_currentFallSpeed = value; }}

	public float gravityMultiplier	{get{return _gravityMultiplier;} set{_gravityMultiplier = value; }}
	public float movementMultiplier	{get{return _movementMultiplier;} set{_movementMultiplier = value; }}
	public float jumpforceMultiplier{get{return _jumpforceMultiplier;} set{_jumpforceMultiplier = value; }}

	public float jumpSpeed		{get{return _jumpSpeed;}}
	public float jumpSpeed2		{get{return _jumpSpeed2;}}
	public float moveSpeed		{get{return _moveSpeed;}}
	public float maxFallSpeed	{get{return _maxFallSpeed;} set{ _maxFallSpeed = value*GMtoUnityConversion; }}
	public float gravity		{get{return _gravity;}}

	public bool infinityJump {get{return _infinityJump;} set{_infinityJump = value; }}

	public bool isMoving	{get{return _isMoving;}}
	public bool isJumping	{get{return _isJumping;}}

	public Rigidbody2D theRigidbody2D {get{return _theRigidbody2D; }}

	public bool facingRight { get{return _facingRight; }}
	public bool isUpright   { get{return _isUpright; }}	

	public float distanceToGround 	{ get{return _distanceToGround ; } }
	public float distanceToWall		{ get{return _distanceToWall ; } }
	public float distanceToCeiling	{ get{return _distanceToCeiling ; } }

	public bool onGround   { get{return _onGround ; } 	set{_onGround = value ; }}
	public bool hitCeiling { get{return _hitCeiling ; } set{_hitCeiling = value ; }}
	public bool hitWall    { get{return _hitWall ; } 	set{_hitWall = value ; }}

	public bool AllowPreCheck_Ground 	{ get{ return _AllowPreCheck_Ground; } 	set{_AllowPreCheck_Ground = value ; }}
	public bool AllowPreCheck_Wall 		{ get{ return _AllowPreCheck_Wall; } 	set{_AllowPreCheck_Wall = value ; }}
	public bool AllowPreCheck_Ceiling	{ get{ return _AllowPreCheck_Ceiling; }	set{_AllowPreCheck_Ceiling = value ; }}
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public void Awake()
	{
		EventManager.resetObjects += Reset;

		EventManager.CreateEventManagerIfNeeded ();
		_theRigidbody2D = GetComponent<Rigidbody2D> ();
		_theRigidbody2D.velocity = Vector2.zero;
		
		GMtoUnityConversion = FramesPerSecond / PixelsPerUnit;
		_jumpSpeed	*= GMtoUnityConversion;
		_jumpSpeed2	*= GMtoUnityConversion;
		_moveSpeed	*= GMtoUnityConversion;
		_gravity 	*= GMtoUnityConversion;
		_maxFallSpeed*= GMtoUnityConversion;
		
		B_maxFallSpeed = _maxFallSpeed;
		
		pixelMovement = false;
		disableCharacterMovement = false;
		disableCharacterJumping = false;
		disableCharacterShooting = false;

		anim = GetComponent<Animator>();
		
		if(GetComponent<CalculateChecks> ())
			calculateChecks = GetComponent<CalculateChecks> ();
		else
			calculateChecks = gameObject.AddComponent<CalculateChecks> ();
		calculateChecks.ChangeLayerMask (whatIsGround);
	}

	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		FlipCharacter_Upright ();
		Refresh_MultiJump ();

		_isMoving = false;
		_isJumping = false;
		_currentFallSpeed = 0;
		_currentHorizontalSpeed = 0;

		_theRigidbody2D.velocity = Vector3.zero;

		pixelMovement = false;
		gameObject.transform.parent = null;
	}
	
	
	void FixedUpdate()
	{
		if(_isMoving)
		{
			Move (moveDirection);
			_isMoving = false;
		}
		
		if(_isJumping)
		{
			Jump ();
			_isJumping = false;
		}
		
		ApplyGravity ();
		SetAnimationVariables ();

		if(currentHorizontalSpeed == 0 && _theRigidbody2D.velocity.x != 0)	// Nullify outside forces on the player, just in case
		{
//			print (_theRigidBody2D.velocity.x);
			_theRigidbody2D.velocity = new Vector2(0, _theRigidbody2D.velocity.y);
		}
	}
	
	void Update()
	{
		if(!_isMoving)
		{
			moveDirection = Input.GetAxis("Horizontal");
			
			if(currentHorizontalSpeed != moveDirection || moveDirection != 0)
			{
				_isMoving = true;
			}
		}
		
		if (Input.GetButtonDown("Jump") && !_isJumping)		// Check if jump button was used
		{
			_isJumping = true;
		}

		if (Input.GetButtonUp ("Jump") && !_isJumping && currentFallSpeed*gravityMultiplier > 0)
//		if (Input.GetButtonUp ("Jump") && !_isJumping && _theRigidbody2D.velocity.y*transform.localScale.y > 0)
//		if (Input.GetButtonUp ("Jump") && !_isJumping && ((currentFallSpeed*gravityMultiplier > 0) || (_theRigidbody2D.velocity.y*transform.localScale.y > 0)))
		{
			ShortJumping ();
		}
	}
	
	public abstract void Move(float moveDirection);
	public abstract void Jump();
	public abstract void ApplyGravity ();
	public abstract void ShortJumping();
	public abstract void Set_MinPenetration ();
	

	
	
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void SetAnimationVariables()
	{
		anim.SetBool("Ground", _onGround);										// Set grounded animation tag
		anim.SetFloat("horizontalSpeed", Mathf.Abs(moveDirection));						// Set animation controller tag
		
		if(isUpright)
			anim.SetFloat("fallSpeed", currentFallSpeed/GMtoUnityConversion);		// Set the vertical animation tag
		else
			anim.SetFloat("fallSpeed", -currentFallSpeed/GMtoUnityConversion);		// Set the vertical animation tag
	}
	
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Area checks
	public void Check_Ground()
	{
		_distanceToGround = calculateChecks.GroundCheck (_isUpright);				// Calculate Ground check

		if((_distanceToGround < (howCloseIsSolid + colliderBuffer)) && (currentFallSpeed*gravityMultiplier <= 0.0))
		{
			_onGround = true;
			Refresh_MultiJump();
		}
		else
			_onGround = false;
	}
	public void Check_Wall()
	{
		_distanceToWall = calculateChecks.WallCheck (_isUpright, facingRight);		// Calculate Wall check
		if(_distanceToWall < (howCloseIsSolid + colliderBuffer))
			_hitWall = true;
		else
			_hitWall = false;
	}
	public void Check_Ceiling()
	{
		_distanceToCeiling = calculateChecks.CeilingCheck (_isUpright);				// Calculate Ceiling check
		if (_distanceToCeiling < (howCloseIsSolid + colliderBuffer))
			_hitCeiling = true;
		else
			_hitCeiling = false;

		if(_distanceToCeiling < 0)
		{
			_distanceToCeiling = 0;
//			print ("Negative "+_distanceToCeiling);
		}
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Flips the charcter horizontally
	public void Flip ()
	{
		_facingRight = !_facingRight;					// Switch the way the player is labelled as facing.
		
		Vector3 theScale = transform.localScale;	// Multiply the player's x local scale by -1.
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	
	// Makes the character flip upright if not already upright
	public bool FlipCharacter_Upright()
	{
		if(!_isUpright)					// If not upright - flip the character
		{
			_isUpright = !_isUpright;
			
			_theRigidbody2D.gravityScale *= -1;
			_gravityMultiplier = 1;
			_jumpforceMultiplier = 1;
			
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
		if(_isUpright)					// If upright - flip the character
		{
			_isUpright = !_isUpright;
			
			_theRigidbody2D.gravityScale *= -1;
			_gravityMultiplier *= -1;
			_jumpforceMultiplier *= -1;
			
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
		_isUpright = !_isUpright;
		
		_theRigidbody2D.gravityScale *= -1;
		_gravityMultiplier *= -1;
		_jumpforceMultiplier *= -1;
		
		Vector3 theScale = transform.localScale;
		theScale.y *= -1;
		transform.localScale = theScale;
	}
	
	
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Jumping methods
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
		if(current_MultiJumps <= max_MultiJumps)
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


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void Save_GravityMultiplier()
	{
		B_gravityMultiplier = _gravityMultiplier;
	}
	public void Load_GravityMultiplier()
	{
		_gravityMultiplier = B_gravityMultiplier;
	}

	public void ResetGravityMultiplier()
	{
		_gravityMultiplier = 1;
	}
	public void ResetMaxFallSpeed()
	{
		_maxFallSpeed = B_maxFallSpeed;
	}
	public void ResetJumpforceMultipler()
	{
		_jumpforceMultiplier = 1;
	}
	public void ResetMovementMultiplier()
	{
		_movementMultiplier = 1;
	}


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void ZeroFallSpeed()
	{
		_currentFallSpeed = 0;
		_theRigidbody2D.velocity = new Vector2 (_theRigidbody2D.velocity.x, 0);
	}
	public void StopHorizontalMovement()
	{
		_currentHorizontalSpeed = 0;
		_theRigidbody2D.velocity = new Vector2 (0, _theRigidbody2D.velocity.y);
	}




}
























