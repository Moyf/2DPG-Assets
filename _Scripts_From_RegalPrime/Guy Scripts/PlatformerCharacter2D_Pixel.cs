using UnityEngine;
using System.Collections;

public class PlatformerCharacter2D_Pixel : PlatformerCharacter2D
{
	private float ColliderBuffer		   = 0.0004f;	// Closest distance you can get between two colliders
	private float MinPenetrationForPenalty = 0.0001f;	// Will set the editor to this value
	
	new void Awake()
	{
		base.Awake ();
		
		colliderBuffer = ColliderBuffer;

		if (Physics2D.defaultContactOffset != MinPenetrationForPenalty)
			Debug.LogWarning ("Incorrect 'Min Penetration For Penalty' for this type of player spawn. For better hit detection change Edit->ProjectSettings->Physics2D->Min PenetrationForPenalty to value "+MinPenetrationForPenalty);

		Physics2D.defaultContactOffset = MinPenetrationForPenalty;
	}
	
	
	
	public override void Move (float MoveDirection)
	{
		Check_Wall ();
		if(MoveDirection > 0 && !facingRight)							// Flip the characters sprite if needed
			Flip();
		else if(MoveDirection < 0 && facingRight)
			Flip();
		else
		if(!disableCharacterMovement)								// If able to move character
		{
			MovePixel_X(MoveDirection);
		}
	}
	public void MovePixel_X(float MoveDirection)
	{
		currentHorizontalSpeed = MoveDirection * moveSpeed * movementMultiplier * Time.deltaTime;
		
		float HowFarTillStop = distanceToWall - ColliderBuffer;
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


	public override void Jump()
	{	
		if(!disableCharacterJumping)
		{
			if ((onGround || Can_MultiJump() || infinityJump))			// Jump if able
			{
				if(onGround)											// Jumping from ground
				{
					currentFallSpeed = jumpSpeed*jumpforceMultiplier;

					if(MusicController.control != null)
						MusicController.control.OneShotAudio(jump1Sound);
				}
				else													// Jumping via double jump / multi jumpeffect
				{
					currentFallSpeed = jumpSpeed2*jumpforceMultiplier;

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
		float NewFallSpeed = currentFallSpeed - gravity*gravityMultiplier;
		
		Check_Ground ();
		Check_Ceiling ();
		
		// Cap fall speed if needed
		if(NewFallSpeed < maxFallSpeed_gravity && isUpright)
			NewFallSpeed = maxFallSpeed_gravity;
		if(NewFallSpeed > maxFallSpeed_gravity && !isUpright)
			NewFallSpeed = maxFallSpeed_gravity;
		
		float FlipMultiplier = 1;
		if(!isUpright)
			FlipMultiplier = -1;
		
		if(NewFallSpeed * FlipMultiplier > 0)			// If velocity is upwards (jumping), then apply gravity / ceiling check
		{
			float HowFarTillStop_Ceiling = distanceToCeiling - ColliderBuffer;
			float NewFallSpeed_Pixels = NewFallSpeed * Time.deltaTime;
			
			if(Mathf.Abs(NewFallSpeed_Pixels) > HowFarTillStop_Ceiling)
			{
				gameObject.transform.position += new Vector3(0, HowFarTillStop_Ceiling*FlipMultiplier, 0);
				currentFallSpeed = 0;
				hitCeiling = true;
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
			float HowFarTillStop_Ground = distanceToGround - ColliderBuffer;
			float NewFallSpeed_Pixels = NewFallSpeed * Time.deltaTime;
			
			if(HowFarTillStop_Ground > 0.005f)
			{
				if(Mathf.Abs(NewFallSpeed_Pixels) > HowFarTillStop_Ground)
				{
					gameObject.transform.position -= new Vector3(0, HowFarTillStop_Ground*FlipMultiplier, 0);
					onGround = true;
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
				onGround = true;
				currentFallSpeed = 0;
//				print ("GROUNDED");
			}
		}
	}

	public override void ShortJumping ()
	{
		currentFallSpeed = currentFallSpeed*0.45f;
	}
	public override void Set_MinPenetration ()
	{
		Physics2D.defaultContactOffset = MinPenetrationForPenalty;
	}

}

















