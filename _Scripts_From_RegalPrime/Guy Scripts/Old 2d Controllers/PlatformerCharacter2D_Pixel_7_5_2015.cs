// RegalPrime 1-21-15 - PlatformerCharacter2D_Pixel.cs

// This is an alternate version of the PlatformerCharacter2D where the character fully moves via transforms and NOT by physics
// Overall, this seems to work better than the physics in terms of accuracy, but it cannot climb slopes (as of yet)

// ColliderBuffer is the distance that the player will stay above a ground tagged object
// It helps with the Min Penetration for penalty settings in the Physics2D (in the unity editor)
// Setting it to 2x "Min Penetration for penalty" + 0.01f seems to work best for accurate collider detection
// If a collider is side by side it will trigger and this helps alleviate it

/*
using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class PlatformerCharacter2D_Pixel : PlatformerCharacter2D 
{
	private float ColliderBuffer = 0.0003f;				// Closest distance you can get between two colliders
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
		ApplyGravity ();
		
		if(IsMoving)
			StartCoroutine (Move ());
		
		if(IsJumping)
			StartCoroutine (Jump ());
		
		SetAnimationVariables ();
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
		
		if (Input.GetButtonDown("Jump") && !IsJumping)
		{
			IsJumping = true;
		}
		
		if (Input.GetButtonUp ("Jump") && !IsJumping && currentFallSpeed*gravityMultiplier > 0)
		{
			currentFallSpeed = currentFallSpeed*0.45f;
		}
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Move the player based on current MoveDirection variable (either -1 or +1 or 0)
	// Move the player based on current MoveDirection variable (either -1 or +1 or 0)
	new IEnumerator Move()
	{
		//		yield return new WaitForEndOfFrame();
		//		print ("Moving - " + MoveDirection + "  -  " + Time.deltaTime);
		
		Check_Wall ();
		if(MoveDirection > 0 && !get_facingRight())							// Flip the characters sprite if needed
			Flip();
		else if(MoveDirection < 0 && get_facingRight())
			Flip();
		else
			if(!get_DisableCharacterMovement())								// If able to move character
		{
			MovePixel_X();
		}
		
		//		yield return new WaitForFixedUpdate();
		//		yield return new WaitForEndOfFrame();
		
		IsMoving = false;
		
		yield return null;
	}
	
	new public void MovePixel_X()
	{
		currentHorizontalSpeed = MoveDirection * get_moveSpeed () * movementMultiplier * Time.deltaTime;
		
		float moveSpeed = get_moveSpeed();
		float HowFarTillStop = get_DistanceToWall() - ColliderBuffer;
		float CurentMoveSpeed = moveSpeed * movementMultiplier * Time.deltaTime;
		
		if(HowFarTillStop > 0.005f)
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
			if ((get_grounded () || Can_MultiJump() || infinityJump))				// Jump if able
			{
				float jumpSpeed = get_jumpSpeed();
				float jumpSpeed2 = get_jumpSpeed2();
				
				if(get_grounded ())											// Jumping from ground
				{
					currentFallSpeed = jumpSpeed*jumpforceMultiplier;
					MusicController.control.OneShotAudio(jump1Sound);
				}
				else													// Jumping via double jump / multi jumpeffect
				{
					currentFallSpeed = jumpSpeed2*jumpforceMultiplier;
					MusicController.control.OneShotAudio(jump2Sound);
					MultiJumpUsed();
				}
				
				//				anim.SetBool("Ground", false);
				set_grounded (false);
			}
		}
		
		IsJumping = false;
		yield return null;
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	new public void ApplyGravity()
	{
		float gravity = get_gravity();
		float maxFallSpeed = get_maxFallSpeed() * gravityMultiplier;
		float NewFallSpeed = currentFallSpeed - gravity*gravityMultiplier;
		
		Check_Ground ();
		Check_Ceiling ();
		
		// Cap fall speed if needed
		if(NewFallSpeed < maxFallSpeed && get_isUpright())
			NewFallSpeed = maxFallSpeed;
		if(NewFallSpeed > maxFallSpeed && !get_isUpright())
			NewFallSpeed = maxFallSpeed;
		
		float FlipMultiplier = 1;
		if(!get_isUpright())
			FlipMultiplier = -1;
		
		if(NewFallSpeed * FlipMultiplier > 0)			// If velocity is upwards (jumping), then apply gravity / ceiling check
		{
			float HowFarTillStop_Ceiling = get_DistanceToCeiling() - ColliderBuffer;
			float NewFallSpeed_Pixels = NewFallSpeed * Time.deltaTime;
			
			if(Mathf.Abs(NewFallSpeed_Pixels) > HowFarTillStop_Ceiling)
			{
				gameObject.transform.position += new Vector3(0, HowFarTillStop_Ceiling*FlipMultiplier, 0);
				currentFallSpeed = 0;
				set_hitCeiling(true);
				//				print ("** BONK HEAD ** " + HowFarTillStop_Ceiling);
			}
			else
			{
				gameObject.transform.position += new Vector3(0, NewFallSpeed_Pixels, 0);
				currentFallSpeed = NewFallSpeed;
				//				print ("*FALL AS NORMAL* " + NewFallSpeed_Pixels);
			}
		} 								// If velocity is downwards (falling), apply gravity / ground checks
		else
		{
			float HowFarTillStop_Ground = get_DistanceToGround() - ColliderBuffer;
			float NewFallSpeed_Pixels = NewFallSpeed * Time.deltaTime;
			
			if(HowFarTillStop_Ground > 0.005f)
			{
				if(Mathf.Abs(NewFallSpeed_Pixels) > HowFarTillStop_Ground)
				{
					gameObject.transform.position -= new Vector3(0, HowFarTillStop_Ground*FlipMultiplier, 0);
					set_grounded (true);
					currentFallSpeed = 0;
					//					print ("SHORT FALL " + HowFarTillStop_Ground);
				}
				else
				{
					gameObject.transform.position += new Vector3(0, NewFallSpeed_Pixels, 0);
					currentFallSpeed = NewFallSpeed;
					//					print ("FALL AS NORMAL " + NewFallSpeed_Pixels);
				}
			}
			else
			{
				set_grounded (true);
				currentFallSpeed = 0;
				//			print ("GROUNDED");
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
		currentHorizontalSpeed = 0;
	}
}

*/