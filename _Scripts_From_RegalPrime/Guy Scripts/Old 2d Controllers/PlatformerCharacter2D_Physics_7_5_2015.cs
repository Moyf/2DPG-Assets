// RegalPrime 12-01-14 - PlatformerCharacter2D.cs

// This is the main means of movement for the player via physics based forces
// Prechecks allow the player to move a lesser distance to counteract the wonky physics clipping you in the side of objects (this is done via transforms)

// ColliderBuffer is the distance that the player will stay above a ground tagged object
// It helps with the Min Penetration for penalty settings in the Physics2D (in the unity editor)
// Setting it to 2x "Min Penetration for penalty" + 0.01f seems to work best for accurate collider detection
// If a collider is side by side it will trigger and this helps alleviate it

/*
using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class PlatformerCharacter2D_Physics : PlatformerCharacter2D
{
	private float ColliderBuffer = 0.011f;				// Closest distance you can get between two colliders
	private bool IsJumping = false;						// Can the jump function be called?
	private bool IsMoving = false;						// Can the move function be called?
	private float currentHorizontalSpeed = 0;			// Current horizontal move speed
	
	new void Start()
	{
		set_ColliderBuffer (ColliderBuffer);
		base.Start ();
	}
	
	void FixedUpdate()
	{
		if(IsMoving)
			StartCoroutine (Move ());
		
		if(IsJumping)
			StartCoroutine (Jump ());
		
		ApplyGravity ();				// Apply gravity every frame
		SetAnimationVariables ();		// Update the animator variables so it will show the correct animations
	}
	
	void Update()
	{
		if(!IsMoving)
		{
			MoveDirection = Input.GetAxis("Horizontal");
			
			if(currentHorizontalSpeed != MoveDirection || MoveDirection != 0)
			{
				IsMoving = true;
			}
		}
		
		if (Input.GetButtonDown("Jump") && !IsJumping)		// Check if jump button was used
		{
			IsJumping = true;
		}
		
		if (Input.GetButtonUp ("Jump") && !IsJumping && GetComponent<Rigidbody2D>().velocity.y*transform.localScale.y > 0)
		{
			currentFallSpeed = currentFallSpeed*0.45f;
		}
		
		
		
		if(currentHorizontalSpeed == 0 && GetComponent<Rigidbody2D>().velocity.x != 0)	// Nullify outside forces on the player, just in case
		{
			//			print (rigidbody2D.velocity.x);
			GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
		}
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Move the player based on current MoveDirection variable (either -1 or +1 or 0)
	new public IEnumerator Move()
	{
		if(!get_DisableCharacterMovement())										// If able to move character
		{
			Check_Wall ();
			if(MoveDirection > 0 && !get_facingRight())							// Flip the characters sprite if needed
				Flip();
			else if(MoveDirection < 0 && get_facingRight())
				Flip();
			else
			{
				currentHorizontalSpeed = MoveDirection;
				if(!get_PixelMovement())
					MoveVelocity();
				else
					MovePixel_X();
			}
		}
		
		IsMoving = false;
		yield return null;
	}
	
	// Move left and right via velocity
	new public void MoveVelocity()
	{
		float HowFarTillStop = get_DistanceToWall() - ColliderBuffer;
		
		float moveSpeed = get_moveSpeed ();
		
		if(MoveDirection != 0 && HowFarTillStop > 0.005f)		// Move character if able
		{
			if(HowFarTillStop < 2f*moveSpeed*movementMultiplier*Time.deltaTime && get_AllowPreCheck_Wall())	// Wall is close and allow of prechecks	
			{
				GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);								// Stop velocity (will be using transforms now)
				
				if(HowFarTillStop < (moveSpeed*movementMultiplier*Time.deltaTime))							// If there is less than 1x Movespeed distance, then just move the remaining distance
				{
					gameObject.transform.position += new Vector3(MoveDirection*HowFarTillStop,0,0);
					//					print ("SHORT MOVE " + MoveDirection*HowFarTillStop);
				}
				else 																						// If there is more than 1x Movespeed distance, move via transform
				{
					gameObject.transform.position += new Vector3(MoveDirection*moveSpeed*movementMultiplier*Time.deltaTime, 0, 0);
					//					print ("SHORT MOVE_+ " + MoveDirection*HowFarTillStop);
				}
			}
			else
			{
				GetComponent<Rigidbody2D>().velocity = new Vector2(MoveDirection * moveSpeed*movementMultiplier, GetComponent<Rigidbody2D>().velocity.y);		// Move normal
				//				print ("NORMAL MOVE " + MoveDirection*moveSpeed*Time.deltaTime);
			}
		}
		else
		{
			if(!get_AllowPreCheck_Wall ())
				GetComponent<Rigidbody2D>().velocity = new Vector2(MoveDirection * moveSpeed*movementMultiplier, GetComponent<Rigidbody2D>().velocity.y);
			else
				GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);						// If no inputs or hitting a wall, then dont move
			//			print ("DONT MOVE");
		}
	}
	// Move left and right via pixels instead of velocity (used in platforms)
	// This does not have wall protection built into it.
	new public void MovePixel_X()
	{
		float moveSpeed = get_moveSpeed();
		float HowFarTillStop = get_DistanceToWall() - ColliderBuffer;
		float CurentMoveSpeed = moveSpeed * movementMultiplier * Time.deltaTime;
		
		if(HowFarTillStop > 0.005f && MoveDirection != 0)
		{
			if(CurentMoveSpeed > HowFarTillStop)		// Move distance is greater than remaining distance (do a shorter move)
			{
				gameObject.transform.position += new Vector3 (MoveDirection*HowFarTillStop, 0, 0);
				//				print ("SHORT MOVE " + MoveDirection*HowFarTillStop);
			}
			else 										// Have enough room, move at full speed
			{
				gameObject.transform.position += new Vector3 (MoveDirection*CurentMoveSpeed, 0, 0);
				//				print ("NORMAL MOVE " + MoveDirection*moveSpeed*Time.deltaTime);
			}
		}
		else
		{
			//			print ("DONT MOVE");
		}
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	new public IEnumerator Jump()
	{	
		if(!get_DisableCharacterJumping())
		{
			if ((get_grounded() || Can_MultiJump() || infinityJump))				// Jump if able
			{
				//				anim.SetBool("Ground", false);
				float jumpSpeed = get_jumpSpeed();
				float jumpSpeed2 = get_jumpSpeed2();
				
				if(get_grounded())											// Jumping from ground
				{
					currentFallSpeed = jumpSpeed*jumpforceMultiplier;
					//					rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpSpeed*jumpforceMultiplier);
					MusicController.control.OneShotAudio(jump1Sound);
				}
				else													// Jumping via double jump / multi jumpeffect
				{
					currentFallSpeed = jumpSpeed2*jumpforceMultiplier;
					//					rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpSpeed2*jumpforceMultiplier);
					MusicController.control.OneShotAudio(jump2Sound);
					
					MultiJumpUsed();									// Reset double jump
				}
			}
		}
		
		IsJumping = false;
		yield return null;
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	new public void ApplyGravity()
	{
		float gravity = get_gravity();
		float maxFallSpeed = get_maxFallSpeed() * gravityMultiplier;
		float NewFallSpeed = currentFallSpeed - gravity*gravityMultiplier;		// Calculate the new gravity
		
		Check_Ground ();														// Check the ground
		Check_Ceiling ();														// Check the ceiling
		
		// Cap fall speed if needed
		if(NewFallSpeed < maxFallSpeed && get_isUpright())
		{
			NewFallSpeed = maxFallSpeed;
		}
		if(NewFallSpeed > maxFallSpeed && !get_isUpright())
		{
			NewFallSpeed = maxFallSpeed;
		}
		
		// Used to check if the character is flipped. Using gravity multiplier or the transform multiplier might make things wonkey, so Im using this
		float FlipMultiplier = 1;
		if(!get_isUpright())
			FlipMultiplier = -1;
		
		if(NewFallSpeed * FlipMultiplier > 0)			// If velocity is upwards (jumping), then apply gravity / ceiling check
		{
			float HowFarTillStop_Ceiling = get_DistanceToCeiling() - ColliderBuffer;
			float NewFallSpeed_Pixels = NewFallSpeed * Time.deltaTime;
			
			if(Mathf.Abs(NewFallSpeed_Pixels) > HowFarTillStop_Ceiling && get_AllowPreCheck_Ceiling())	// Ceiling is very near and allow of prechecks (Add is_AllowPreCheck_Ceiling() instead of false for ceiling checks)
			{
				GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, 0);
				gameObject.transform.position += new Vector3(0, HowFarTillStop_Ceiling*FlipMultiplier, 0);
				currentFallSpeed = 0;
				set_hitCeiling(true);
				//				print ("** BONK HEAD ** " + HowFarTillStop_Ceiling);
			}
			else 																	// Nothing above character, do the new velocity
			{
				currentFallSpeed = NewFallSpeed;
				GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, NewFallSpeed);
				//				print ("*FALL AS NORMAL* " + NewFallSpeed_Pixels);
			}
			
			if(get_hitCeiling())															// Character hit the ceiling, zero out fall speed
			{
				currentFallSpeed = 0;
				GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, 0);
				//				print ("** BONK HEAD ** - PHYSICS");
			}
		}
		else 								// If velocity is downwards (falling), apply gravity / ground checks
		{
			float HowFarTillStop_Ground = get_DistanceToGround() - ColliderBuffer;
			float NewFallSpeed_Pixels = NewFallSpeed * Time.deltaTime;
			
			if(HowFarTillStop_Ground > 0.005f)									// If not on the ground
			{
				if(Mathf.Abs(2*NewFallSpeed_Pixels) > HowFarTillStop_Ground)		// Ground is close and allow of prechecks //  Add  && is_AllowPreCheck_Ground() to disable edge collider short checks
				{
					if(Mathf.Abs(NewFallSpeed_Pixels) > HowFarTillStop_Ground)		// Ground is very near, move the remaining distance and zero out velocity
					{
						GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, 0);
						gameObject.transform.position -= new Vector3(0, HowFarTillStop_Ground*FlipMultiplier, 0);
						set_grounded(true);
						currentFallSpeed = 0;
						//						print ("SHORT FALL " + HowFarTillStop_Ground);
					}
					else 															// Fall via transform at full speed instead of velocity
					{
						GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, 0);
						gameObject.transform.position += new Vector3(0, NewFallSpeed*Time.deltaTime, 0);
						currentFallSpeed = NewFallSpeed;
						//						print ("SHORT FALL_+ " + HowFarTillStop_Ground);
					}
				}
				else 															// Fall as normal
				{
					currentFallSpeed = NewFallSpeed;
					GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, NewFallSpeed);
					//					print ("FALL AS NORMAL " + NewFallSpeed_Pixels);
				}
			}
			else 																// Grounded - no need to apply gravity
			{
				set_grounded(true);
				currentFallSpeed = 0;
				GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, 0);
				//				print ("GROUNDED");
			}
		}
	}
	
	new public void ZeroFallSpeed()
	{
		currentFallSpeed = 0;
		GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, 0);
	}
	new public void StopHorizontalMovement()
	{
		GetComponent<Rigidbody2D>().velocity = new Vector2 (0, GetComponent<Rigidbody2D>().velocity.y);
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}

*/