// RegalPrime 5-22-15 - GroundAI.cs

// This deals with ground based units and how they patrol and jump

/*
public override IEnumerator C_Patrol()
public IEnumerator C_Jump(int TowardsPlayer)
	public IEnumerator C_JumpTowardsPlayer()
	public IEnumerator C_JumpAwayFromPlayer()
*/

using UnityEngine;
using System.Collections;

public class GroundAI : MovingAI
{
	public float JumpSpeed = 1f;				// Speed of the jumping unit
	public float JumpPower = 1f;				// Jump power of the unit
	public float DelayBetweenJumps = 0.5f;		// The time between each jump
	
	private bool AbleToJump = true;				// If the unit is able to currently jump
	
	
	public override void Reset()
	{
		base.Reset ();
		AbleToJump = true;
	}
	void FixedUpdate()
	{
		GroundCheck ();
		AnimatorBoolUpdate ("Grounded", grounded);
	}

	// The unit will patrol back and forth
	// The unit will turn around when it encounters a wall / pit / hits it patrol area limit
	// If the patrol area is 0, then there is no patrol area limit
	[AIAction]
	public override IEnumerator C_Patrol()
	{
		AIprint (string.Format("I AM NOW PATROLING AT {0} SPEED. {1} PATROL AREA", PatrolSpeed, PatrolArea));
		if(PatrolSpeed != 0)
			AnimatorBoolUpdate ("IsMoving", true);
		
		float PatrolDistance = 0;
		
		while(!ChangeInAction)
		{
			if(PatrolSpeed != 0)
			{
				if(facingRight)
				{
					gameObject.transform.position += new Vector3(PatrolSpeed*Time.deltaTime, 0);
					PatrolDistance += PatrolSpeed*Time.deltaTime;
				}
				else
				{
					gameObject.transform.position += new Vector3(-PatrolSpeed*Time.deltaTime, 0, 0);
					PatrolDistance -= PatrolSpeed*Time.deltaTime;
				}
				
				// Turn around if wall hit / a pit is ahead / outside of patrol bounds
				if(WallCheck() || (!PitCheck()&&grounded) || (Mathf.Abs (PatrolDistance) > PatrolArea && PatrolArea != 0))
				{
					PatrolDistance = 0;
					Flip ();
					yield return null;
				}
			}
			yield return null;
		}
		
		yield return null;
		
		AIprint ("I AM DONE PATROLING");
		AnimatorBoolUpdate ("IsMoving", false);
	}
	
	
	
	// The unit will jump at a certain vertical power and horizontal speed
	public IEnumerator C_Jump(int TowardsPlayer)
	{
		AIprint (string.Format("I AM TRYING TO JUMP AT THE PLAYER WITH {0} POWER AND {1} SPEED", JumpPower, TowardsPlayer*JumpSpeed));
		
		while(!ChangeInAction)
		{
			if(AbleToJump && grounded && JumpPower > 0)
			{
				AbleToJump = false;
				AnimatorBoolUpdate ("IsJumping", true);
				
				yield return null;
				
				if(TowardsPlayer == 1)
					FlipTowardsPlayer();
				else
					FlipAwayFromPlayer();
				
				float Y_Flip = gameObject.transform.localScale.y / Mathf.Abs (gameObject.transform.localScale.y);
				float XDirection = (PlayersPosition.transform.position.x - gameObject.transform.position.x);
				XDirection = XDirection / Mathf.Abs (XDirection);
				
				gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(XDirection*TowardsPlayer*JumpSpeed, JumpPower*Y_Flip);
				
				yield return null;
				yield return null;
				
				// If the object is moving upwards and is not grounded, wait until it is grounded before returning
				while(gameObject.GetComponent<Rigidbody2D>().velocity.y > 0 || !grounded)
				{
					yield return null;
				}
				
				GetComponent<Rigidbody2D>().velocity = Vector3.zero;
				
				if(DelayBetweenJumps != 0)
				{
					AnimatorBoolUpdate ("IsJumping", false);
					yield return new WaitForSeconds (DelayBetweenJumps);
				}
				
				AbleToJump = true;
			}
			yield return new WaitForSeconds(0.1f);
		}
		
		AIprint ("I AM DONE TRYING TO JUMP");
		AnimatorBoolUpdate ("IsJumping", false);
	}
	[AIAction]
	public IEnumerator C_JumpTowardsPlayer()
	{
		yield return StartCoroutine (C_Jump (1));
	}
	[AIAction]
	public IEnumerator C_JumpAwayFromPlayer()
	{
		yield return StartCoroutine (C_Jump (-1));
	}
	
	
}
