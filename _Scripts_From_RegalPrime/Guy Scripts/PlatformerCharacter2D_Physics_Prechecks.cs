using UnityEngine;
using System.Collections;

public class PlatformerCharacter2D_Physics_Prechecks : PlatformerCharacter2D
{
	private float ColliderBuffer			= 0.011f;	// Closest distance you can get between two colliders
	private float MinPenetrationForPenalty	= 0.004f;	// Will set the editor to this value
	
	new void Awake()
	{
		base.Awake ();
		
		colliderBuffer = ColliderBuffer;

		if (Physics2D.defaultContactOffset != MinPenetrationForPenalty)
			Debug.LogWarning ("Incorrect 'Min Penetration For Penalty' for this type of player spawn. For better hit detection change Edit->ProjectSettings->Physics2D->Min PenetrationForPenalty to value "+MinPenetrationForPenalty);

		Physics2D.defaultContactOffset = MinPenetrationForPenalty;
	}



	public override void Move(float MoveDirection)
	{
		if(!disableCharacterMovement)										// If able to move character
		{
			Check_Wall ();
			if(MoveDirection > 0 && !facingRight)							// Flip the characters sprite if needed
				Flip();
			else if(MoveDirection < 0 && facingRight)
				Flip();
			else
			{
				currentHorizontalSpeed = MoveDirection*moveSpeed*movementMultiplier*Time.deltaTime;
				if(!pixelMovement)
					MoveVelocity(MoveDirection);
				else
					MovePixel_X(MoveDirection);
			}
		}
	}

	// Move left and right via velocity
	public void MoveVelocity(float MoveDirection)
	{
		float HowFarTillStop = distanceToWall - ColliderBuffer;
		
		if(MoveDirection != 0 && HowFarTillStop > 0.005f)		// Move character if able
		{
			if(HowFarTillStop < 2f*moveSpeed*movementMultiplier*Time.deltaTime && AllowPreCheck_Wall)	// Wall is close and allow of prechecks	
			{
				theRigidbody2D.velocity = new Vector2(0, theRigidbody2D.velocity.y);						// Stop velocity (will be using transforms now)
				
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
				theRigidbody2D.velocity = new Vector2(MoveDirection * moveSpeed*movementMultiplier, theRigidbody2D.velocity.y);		// Move normal
//				print ("NORMAL MOVE " + MoveDirection*moveSpeed*Time.deltaTime);
			}
		}
		else
		{
			if(!AllowPreCheck_Wall)
				theRigidbody2D.velocity = new Vector2(MoveDirection * moveSpeed*movementMultiplier, theRigidbody2D.velocity.y);
			else
				theRigidbody2D.velocity = new Vector2(0, theRigidbody2D.velocity.y);						// If no inputs or hitting a wall, then dont move
//			print ("DONT MOVE");
		}
	}
	// Move left and right via pixels instead of velocity (used in platforms)
	// This does not have wall protection built into it.
	public void MovePixel_X(float MoveDirection)
	{
		float HowFarTillStop = distanceToWall - ColliderBuffer;
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





	public override void Jump()
	{	
		if(!disableCharacterJumping)
		{
			Check_Ground ();
			
			if ((onGround || Can_MultiJump() || infinityJump))			// Jump if able
			{
				if(onGround)											// Jumping from ground
				{
					Refresh_MultiJump ();
					
					currentFallSpeed = jumpSpeed*jumpforceMultiplier;
					theRigidbody2D.velocity = new Vector2(theRigidbody2D.velocity.x, jumpSpeed*jumpforceMultiplier);

					if(MusicController.control != null)
						MusicController.control.OneShotAudio(jump1Sound);
				}
				else													// Jumping via double jump / multi jumpeffect
				{
					currentFallSpeed = jumpSpeed2*jumpforceMultiplier;
					theRigidbody2D.velocity = new Vector2(theRigidbody2D.velocity.x, jumpSpeed2*jumpforceMultiplier);

					if(MusicController.control != null)
						MusicController.control.OneShotAudio(jump2Sound);
					
					MultiJumpUsed();
				}

				onGround = false;
			}
		}
	}









	public override void ApplyGravity ()
	{
		float maxFallSpeed_gravity = maxFallSpeed * gravityMultiplier;
		float NewFallSpeed = currentFallSpeed - gravity*gravityMultiplier;		// Calculate the new gravity
		
		Check_Ground ();														// Check the ground
		Check_Ceiling ();														// Check the ceiling
		
		// Cap fall speed if needed
		if(NewFallSpeed < maxFallSpeed_gravity && isUpright)
		{
			NewFallSpeed = maxFallSpeed_gravity;
		}
		if(NewFallSpeed > maxFallSpeed_gravity && !isUpright)
		{
			NewFallSpeed = maxFallSpeed_gravity;
		}
		
		// Used to check if the character is flipped. Using gravity multiplier or the transform multiplier might make things wonkey, so Im using this
		float FlipMultiplier = 1;
		if(!isUpright)
			FlipMultiplier = -1;
		
		if(NewFallSpeed * FlipMultiplier > 0)			// If velocity is upwards (jumping), then apply gravity / ceiling check
		{
			float HowFarTillStop_Ceiling = distanceToCeiling - ColliderBuffer;
			float NewFallSpeed_Pixels = NewFallSpeed * Time.deltaTime;
			
			if(Mathf.Abs(NewFallSpeed_Pixels) > HowFarTillStop_Ceiling && AllowPreCheck_Ceiling)	// Ceiling is very near and allow of prechecks (Add is_AllowPreCheck_Ceiling() instead of false for ceiling checks)
			{
				theRigidbody2D.velocity = new Vector2 (theRigidbody2D.velocity.x, 0);
				gameObject.transform.position += new Vector3(0, HowFarTillStop_Ceiling*FlipMultiplier, 0);
				currentFallSpeed = 0;
				hitCeiling = true;
//				print ("** BONK HEAD ** " + HowFarTillStop_Ceiling);
			}
			else 																	// Nothing above character, do the new velocity
			{
				currentFallSpeed = NewFallSpeed;
				theRigidbody2D.velocity = new Vector2 (theRigidbody2D.velocity.x, NewFallSpeed);
//				print ("*FALL AS NORMAL* " + NewFallSpeed_Pixels);
			}
			
			if(hitCeiling)															// Character hit the ceiling, zero out fall speed
			{
				currentFallSpeed = 0;
				theRigidbody2D.velocity = new Vector2 (theRigidbody2D.velocity.x, 0);
//				print ("** BONK HEAD ** - PHYSICS");
			}
		}
		else 								// If velocity is downwards (falling), apply gravity / ground checks
		{
			float HowFarTillStop_Ground = distanceToGround - ColliderBuffer;
			float NewFallSpeed_Pixels = NewFallSpeed * Time.deltaTime;
			
			if(HowFarTillStop_Ground > 0.005f)										// If not on the ground
			{
				if(Mathf.Abs(2*NewFallSpeed_Pixels) > HowFarTillStop_Ground)		// Ground is close and allow of prechecks //  Add  && is_AllowPreCheck_Ground() to disable edge collider short checks
				{
					if(Mathf.Abs(NewFallSpeed_Pixels) > HowFarTillStop_Ground)		// Ground is very near, move the remaining distance and zero out velocity
					{
						theRigidbody2D.velocity = new Vector2 (theRigidbody2D.velocity.x, 0);
						gameObject.transform.position -= new Vector3(0, HowFarTillStop_Ground*FlipMultiplier, 0);
						onGround = true;
						currentFallSpeed = 0;
//						print ("SHORT FALL " + HowFarTillStop_Ground);
					}
					else 															// Fall via transform at full speed instead of velocity
					{
						theRigidbody2D.velocity = new Vector2 (theRigidbody2D.velocity.x, 0);
						gameObject.transform.position += new Vector3(0, NewFallSpeed*Time.deltaTime, 0);
						currentFallSpeed = NewFallSpeed;
//						print ("SHORT FALL_+ " + HowFarTillStop_Ground);
					}
				}
				else 															// Fall as normal
				{
					currentFallSpeed = NewFallSpeed;
					theRigidbody2D.velocity = new Vector2 (theRigidbody2D.velocity.x, NewFallSpeed);
//					print ("FALL AS NORMAL " + NewFallSpeed_Pixels);
				}
			}
			else 																// Grounded - no need to apply gravity
			{
				onGround = true;
				currentFallSpeed = 0;
				theRigidbody2D.velocity = new Vector2 (theRigidbody2D.velocity.x, 0);
//				print ("GROUNDED");
			}
		}
	}



	public override void ShortJumping ()
	{
		currentFallSpeed = currentFallSpeed*0.45f;
		theRigidbody2D.velocity = new Vector2(theRigidbody2D.velocity.x, currentFallSpeed);
	}
	public override void Set_MinPenetration ()
	{
		Physics2D.defaultContactOffset = MinPenetrationForPenalty;
	}

}

















