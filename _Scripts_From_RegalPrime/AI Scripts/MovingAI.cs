// RegalPrime 5-22-15 - MovingAI.cs

// Abstract class that contains coroutines that deal with movement
// The following coroutines can be used in the 3 abstract functions

/*
public IEnumerator C_IdleAction()

public IEnumerator C_MoveUnit_X(int TowardsPlayer)
	public IEnumerator C_MoveTowardsPlayer_X()
	public IEnumerator C_MoveAwayFromPlayer_X()	

public IEnumerator C_MoveUnit_Direct(int TowardsPlayer)
	public IEnumerator C_MoveTowardsPlayer_Direct()
	public IEnumerator C_MoveAwayFromPlayer_Direct()
*/

using UnityEngine;
using System.Collections;

public abstract class MovingAI : BaseAI
{
	public float MoveSpeed = 1f;				// The speed at which the unit will move when trying to reach the player
	public float PatrolArea = 1f;				// The distance on either side of the unit it will patrol
	public float PatrolSpeed = 1f;				// The speed of the patroling unit

	public abstract IEnumerator C_Patrol ();	// Ground / AI patrols are defined in their classes

	// Moves a unit to / from the player, but only on the X-axis
	// if TowardsPlayer is negative, this object will move away from the player
	public IEnumerator C_MoveUnit_X(int TowardsPlayer)
	{
		AIprint (string.Format("I AM TRYING TO MOVE ON XAXIS AT {0} SPEED", TowardsPlayer*MoveSpeed));
		
		if(MoveSpeed != 0)
			AnimatorBoolUpdate ("IsMoving", true);
		
		while(!ChangeInAction)
		{
			if(MoveSpeed > 0)
			{
				if(TowardsPlayer == 1)
					FlipTowardsPlayer ();
				else
					FlipAwayFromPlayer();
				
				float XDirection = (PlayersPosition.transform.position.x - gameObject.transform.position.x);
				XDirection = XDirection / Mathf.Abs (XDirection);
				GetComponent<Rigidbody2D>().velocity = new Vector2(TowardsPlayer*XDirection*MoveSpeed, GetComponent<Rigidbody2D>().velocity.y);
			}

			yield return new WaitForSeconds(0.1f);
		}

		if(grounded)
		GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		
		yield return null;
		AIprint ("I AM DONE TRYING TO MOVE");
		AnimatorBoolUpdate ("IsMoving", false);
	}
	[AIAction]
	public IEnumerator C_MoveTowardsPlayer_X()
	{
		yield return StartCoroutine (C_MoveUnit_X (1));
	}
	[AIAction]
	public IEnumerator C_MoveAwayFromPlayer_X()
	{
		yield return StartCoroutine (C_MoveUnit_X (-1));
	}

	// Move the unit DIRECTALLY towards / away from the player
	// if TowardsPlayer is negative, this object will move away from the player
	// Not for use with ground based units
	public IEnumerator C_MoveUnit_Direct(int TowardsPlayer)
	{
		AIprint (string.Format("I AM TRYING TO MOVE DIRECT AT {0} SPEED", MoveSpeed));
		
		if(MoveSpeed != 0)
			AnimatorBoolUpdate ("IsMoving", true);
		
		while(!ChangeInAction)
		{
			if(MoveSpeed > 0)
			{
				if(TowardsPlayer == 1)
					FlipTowardsPlayer();
				else
					FlipAwayFromPlayer();

				GetComponent<Rigidbody2D>().velocity = ((PlayersPosition.transform.position - transform.position).normalized * MoveSpeed * TowardsPlayer);			
			}
			yield return new WaitForSeconds(0.1f);
		}

		GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);

		yield return null;
		AIprint ("I AM DONE TRYING TO MOVE TO THE PLAYER");
		AnimatorBoolUpdate ("IsMoving", false);
	}
	[AIAction]
	public IEnumerator C_MoveTowardsPlayer_Direct()
	{
		yield return StartCoroutine (C_MoveUnit_Direct (1));
	}
	[AIAction]
	public IEnumerator C_MoveAwayFromPlayer_Direct()
	{
		yield return StartCoroutine (C_MoveUnit_Direct (-1));
	}


}



/*

*/