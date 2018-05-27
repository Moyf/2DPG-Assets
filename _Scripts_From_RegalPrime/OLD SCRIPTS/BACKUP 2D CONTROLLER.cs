using UnityEngine;
using System.Collections;

public class BACKUP_2D_CONTROLLER : MonoBehaviour 
{
	public bool facingRight = true;						// For determining which way the player is currently facing.
	
	public float maxSpeed = 2f;							// The fastest the player can travel in the x axis.
	public float jumpForce = 5f;						// Amount of force added when the player jumps.	
	public bool airControl = false;						// Whether or not a player can steer while jumping;
	
	
	public LayerMask whatIsGround;						// A mask determining what is ground to the character
	
	Transform groundCheck;								// A position marking where to check if the player is grounded.
	float groundedRadius = .01f;						// Radius of the overlap circle to determine if grounded - was 0.2f
	public bool grounded = false;						// Whether or not the player is grounded.
	
	Transform ceilingCheck;								// A position marking where to check for ceilings
	float ceilingRadius = .01f;							// Radius of the overlap circle to determine if the player can stand up
	public bool hitCeiling = false;
	
	Transform wallCheck;								// A position marking where to check for ceilings
	float frontRadius = .01f;							// Radius of the overlap circle to determine if the player can stand up
	public bool hitFront = false;
	
	
	
	Animator anim;										// Reference to the player's animator component.
	
	private bool jump;									// Used in determining when the button for jump is pressed
	public bool infinityJump = false;					// Allows for infinite jumping
	public float doubleJumpModifier = 1;				// How much power to double jump with
	
	public GameObject bulletPrefab;						// Attach the bullet you wish to fire (possibly add multi bullet later via array)
	public Transform  bulletOffset;						// Where the bullet fires from
	public float fireRate = .05f;						// Delay inbetween firing
	public float bulletSpeed = 400f;					// Speed of the bullet
	private bool ableToFire = true;						// Used in determining how often player can fire
	
	public AudioClip bulletSound;						// Default bullet sound
	public AudioClip jump1Sound;						// First Jump sound
	public AudioClip jump2Sound;						// Second Jump sound
	
	public bool disableCharacterMovement = false;		// Can disable directional inputs
	public bool disableCharacterShooting = false;		// Can disable the ability to fire
	
	
	public float currentVSpeed = 0;
	
	public float jumpSpeed = 8.5f;
	public float jumpSpeed2 = 7.0f;
	public bool doubleJump = true;						// Allows for double jump if true
	public float moveSpeed = 3.0f;
	public float gravity = 0.4f;
	public float maxFallSpeed = -9.0f;						// Capped fall speed;
	// 50 fps
	
	private float setTimer;
	
	void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("GroundCheck");
		ceilingCheck = transform.Find("CeilingCheck");
		wallCheck = transform.Find("WallCheck");
		
		
		anim = GetComponent<Animator>();
	}
	
	void Start()
	{
		StartCoroutine("C_DoActions");
	}
	
	void FixedUpdate()
	{
		//		rigidbody2D.velocity = Vector3.zero;
		if(currentVSpeed < maxFallSpeed)
			currentVSpeed = maxFallSpeed;
		
		if (hitCeiling)
			currentVSpeed = -gravity;
	}
	
	void Update()
	{
		
	}
	
	void Do_Action()
	{
		// Check if the character is grounded
		grounded = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);
		hitCeiling = Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround);
		hitFront = Physics2D.OverlapCircle(wallCheck.position, frontRadius, whatIsGround);
		
		anim.SetBool("Ground", grounded);							// Set grounded animation tag
		anim.SetFloat("vSpeed", currentVSpeed);						// Set the vertical animation tag
		
		// Gravity Check every frame
		if(grounded && currentVSpeed < 0)
			currentVSpeed = 0;
		else
		{
			gameObject.transform.position = new Vector2 (transform.position.x, transform.position.y + currentVSpeed / 100);
			currentVSpeed -= gravity;
		}
		GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
		
		
		if(!disableCharacterMovement)								// If able to move character
		{
			float inputDirection = Input.GetAxis("Horizontal");
			Move(inputDirection);
			Jump(jump);
		}
		jump = false;
	}
	
	
	public IEnumerator C_DoActions()
	{
		while(true)
		{
			Do_Action ();
			
			
			if (Input.GetButtonDown ("Fire1")&&ableToFire && !disableCharacterShooting)	// Check if fire button was used
			{
				ableToFire = false;
				Invoke("ResetFire", fireRate);
				Shoot ();
			}
			
			if (Input.GetButtonDown("Jump"))											// Check if jump button was used
				jump = true;
			
			if (Input.GetButtonUp ("Jump") && currentVSpeed > 0)
				currentVSpeed *= 0.45f;
			
			float TEMP = Time.time - setTimer;
			setTimer = Time.time;
			
			yield return new WaitForSeconds(0.02f-TEMP);
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
				currentVSpeed = jumpSpeed;
				AudioSource.PlayClipAtPoint(jump1Sound, transform.position);
			}
			else													// Jumping via double jump / multi jumpeffect
			{
				currentVSpeed = jumpSpeed2;
				AudioSource.PlayClipAtPoint(jump2Sound, transform.position);
			}
			
			doubleJump = false;										// Reset double jump
		}
	}
	
	
	public void Move(float MoveDirection)
	{
		if(grounded || airControl)									// Move character if able
		{
			anim.SetFloat("Speed", Mathf.Abs(MoveDirection));		// Set animation controller tag
			
			if(!disableCharacterMovement && !hitFront)				// Move the character
			{
				float TestNumber = Mathf.Lerp(0,moveSpeed,((Time.time - Time.fixedTime) / Time.deltaTime)*50);
				GetComponent<Rigidbody2D>().position = new Vector2(GetComponent<Rigidbody2D>().position.x + MoveDirection*TestNumber/100, GetComponent<Rigidbody2D>().position.y);
			}
			
			if(MoveDirection > 0 && !facingRight)					// Flip the characters sprite if needed
				Flip();
			else if(MoveDirection < 0 && facingRight)
				Flip();
		}
	}
	
	public void Shoot()												// Create a bullet and make it move
	{
		GameObject clone;
		clone = Instantiate (bulletPrefab, bulletOffset.transform.position, bulletOffset.transform.rotation) as GameObject;
		if(facingRight)
			clone.GetComponent<Rigidbody2D>().AddForce(bulletOffset.transform.right *bulletSpeed);
		else
			clone.GetComponent<Rigidbody2D>().AddForce(bulletOffset.transform.right *-bulletSpeed);
		
		AudioSource.PlayClipAtPoint(bulletSound, transform.position);
	}
	
	void ResetFire()
	{
		ableToFire = true;
	}
	
	void Flip ()
	{
		facingRight = !facingRight;					// Switch the way the player is labelled as facing.
		
		Vector3 theScale = transform.localScale;	// Multiply the player's x local scale by -1.
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
