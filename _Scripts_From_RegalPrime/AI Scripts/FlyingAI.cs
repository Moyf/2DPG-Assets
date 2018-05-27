// RegalPrime 5-22-15 - FlyingAI.cs

// This deals with flying based units and how they patrol and swoop attack

/*
public override IEnumerator C_Patrol()
public IEnumerator C_ReturnToStartingPositionAndIdle()
public IEnumerator C_SwoopAttack()
*/

using UnityEngine;
using System.Collections;

public class FlyingAI : MovingAI
{
	public float SwoopSpeed = 1f;				// Speed of the jumping unit
	public float DelayBetweenJumps = 0.5f;		// The time between each jump
	
	private bool AbleToJump = true;				// If the unit is able to currently jump


	public override void Reset()
	{
		base.Reset ();
		AbleToJump = true;
	}


	// If PatrolArea = 0, then there are no bounds
	[AIAction]
	public override IEnumerator C_Patrol()
	{
		AIprint (string.Format("(F) I AM NOW PATROLING AT {0} SPEED. {1} PATROL AREA", PatrolSpeed, PatrolArea));
		if(PatrolSpeed != 0)
			AnimatorBoolUpdate ("IsMoving", true);
		
		float PatrolDistance = 0;
		
		while(!ChangeInAction)
		{
			if(PatrolSpeed != 0)
			{
				if(facingRight)
				{
					gameObject.transform.position += new Vector3(PatrolSpeed*Time.deltaTime, 0, 0);
					PatrolDistance += PatrolSpeed*Time.deltaTime;
				}
				else
				{
					gameObject.transform.position += new Vector3(-PatrolSpeed*Time.deltaTime, 0, 0);
					PatrolDistance -= PatrolSpeed*Time.deltaTime;
				}

				// Turn around if outside of patrol bounds / wall hit
				if((WallCheck() && PatrolArea == 0) || ((Mathf.Abs (PatrolDistance) > PatrolArea) && (PatrolArea != 0)))
				{
					Flip ();
				}
			}
			yield return null;
		}
		
		
		yield return null;
		
		AIprint ("I AM DONE PATROLING");
		AnimatorBoolUpdate ("IsMoving", false);
	}

	// Will move to the starting point and idle when it gets close to the starting point
	[AIAction]
	public IEnumerator C_ReturnToStartingPositionAndIdle()
	{
		while(!ChangeInAction)
		{
			float distanceToStartingPoint = Vector2.Distance (gameObject.transform.position, startingPosition);
			if(distanceToStartingPoint > 0.1f)
			{
				if(startingPosition.x > gameObject.transform.position.x && !facingRight)
					Flip ();
				if(startingPosition.x < gameObject.transform.position.x && facingRight)
					Flip ();

				AnimatorBoolUpdate("IsMoving", true);
				gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, startingPosition, MoveSpeed*Time.deltaTime);
				yield return null;
				AnimatorBoolUpdate("IsMoving", false);
			}
			else
				yield return StartCoroutine (C_IdleAction());
		}
	}


	// Will swoop at the players character in a semi-circle
	[AIAction]
	public IEnumerator C_SwoopAttack()
	{
		AIprint ("I AM TRYING TO JUMP ATTACK");
		
		while(!ChangeInAction)
		{
			if(AbleToJump && SwoopSpeed > 0)
			{
				AbleToJump = false;
				AnimatorBoolUpdate ("IsJumping", true);

				FlipTowardsPlayer ();
				
				Vector3 pointPos;
				float CenterPointOffset = PlayersPosition.transform.position.x - gameObject.transform.position.x;
				Vector3 RotateAroundThis = new Vector3(gameObject.transform.position.x+CenterPointOffset, gameObject.transform.position.y, 0);
				
				float X_Width = Mathf.Abs (RotateAroundThis.x - gameObject.transform.position.x);
				float Y_Width = (gameObject.transform.position.y - PlayersPosition.transform.position.y);
				
				if(PlayersPosition.transform.position.x > gameObject.transform.position.x)
				for(float I=270; I>=90; I=I-1*SwoopSpeed)
				{
					float pointNum = (I*1.0f)/360;
					float angle = pointNum*Mathf.PI*2;
					
					float x = Mathf.Sin (angle)*X_Width;
					float y = Mathf.Cos (angle)*Y_Width;
					
					pointPos = new Vector3(x, y, gameObject.transform.position.z) + RotateAroundThis;
					gameObject.transform.position = pointPos;
					yield return null;
				}
				else
				for(float I=90; I<=270; I=I+1*SwoopSpeed)
				{
					float pointNum = (I*1.0f)/360;
					float angle = pointNum*Mathf.PI*2;
					
					float x = Mathf.Sin (angle)*X_Width;
					float y = Mathf.Cos (angle)*Y_Width;
					
					pointPos = new Vector3(x, y, gameObject.transform.position.z) + RotateAroundThis;
					gameObject.transform.position = pointPos;
					
					yield return null;
				}
				
				if(DelayBetweenJumps != 0)
				{
					AnimatorBoolUpdate ("IsJumping", false);
					yield return new WaitForSeconds (DelayBetweenJumps);
				}

				AbleToJump = true;
			}
			
			yield return new WaitForSeconds(0.1f);
		}

		GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		AIprint ("I AM DONE TRYING TO JUMP ATTACK");
		AnimatorBoolUpdate ("IsJumping", false);
	}

}
