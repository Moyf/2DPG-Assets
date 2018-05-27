using UnityEngine;
using System.Collections;

public class PlatformerCharacter2D_Physics : PlatformerCharacter2D
{
	private float Physics_ColliderBuffer   = 0.011f;	// Closest distance you can get between two colliders
	private float MinPenetrationForPenalty = 0.004f;	// Will set the editor to this value
	
	new void Awake()
	{
		base.Awake ();
		
		colliderBuffer = Physics_ColliderBuffer;
		Physics2D.defaultContactOffset = MinPenetrationForPenalty;
	}
	
	public override void Move (float MoveDirection)
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
				MoveVelocity(MoveDirection);
			}
		}
	}
	void MoveVelocity(float MoveDirection)
	{
		if(MoveDirection != 0)
		{
			theRigidbody2D.velocity = new Vector2(MoveDirection * moveSpeed * movementMultiplier, theRigidbody2D.velocity.y);
		}
		else
		{
			theRigidbody2D.velocity = new Vector2(0, theRigidbody2D.velocity.y);
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
					
//					currentFallSpeed = jumpSpeed*jumpforceMultiplier;
					theRigidbody2D.velocity = new Vector2(theRigidbody2D.velocity.x, jumpSpeed*jumpforceMultiplier);

					if(MusicController.control != null)
						MusicController.control.OneShotAudio(jump1Sound);
				}
				else													// Jumping via double jump / multi jumpeffect
				{
//					currentFallSpeed = jumpSpeed2*jumpforceMultiplier;
					theRigidbody2D.velocity = new Vector2(theRigidbody2D.velocity.x, jumpSpeed2*jumpforceMultiplier);

					if(MusicController.control != null)
						MusicController.control.OneShotAudio(jump2Sound);
					
					MultiJumpUsed();
				}
			}
		}
	}
	
	public override void ApplyGravity ()
	{
		Check_Ground ();
		Check_Ceiling ();

		float maxFallSpeed_gravity = maxFallSpeed * gravityMultiplier;
		currentFallSpeed = theRigidbody2D.velocity.y;

		// Cap fall speed if needed
		if(currentFallSpeed < maxFallSpeed_gravity && isUpright)
		{
			currentFallSpeed = maxFallSpeed_gravity;
			theRigidbody2D.velocity = new Vector2(theRigidbody2D.velocity.x, maxFallSpeed_gravity);
		}
		if(currentFallSpeed > maxFallSpeed_gravity && !isUpright)
		{
			currentFallSpeed = maxFallSpeed_gravity;
			theRigidbody2D.velocity = new Vector2(theRigidbody2D.velocity.x, maxFallSpeed_gravity);
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

















