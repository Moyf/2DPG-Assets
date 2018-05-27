// RegalPrime 5-22-15 - GroundAI_Shooting.cs

// This deals with ground based units and how they shoot

/*
IEnumerator C_RunAndGun(int moveDirection)
	IEnumerator C_RunAndGun_Away()
	IEnumerator C_RunAndGun_Towards()
IEnumerator C_ShootAttack_SecondaryAction()
IEnumerator C_ShootAttack_Forward()
IEnumerator C_ShootAttack_AtPlayer()
*/

using UnityEngine;
using System.Collections;

public class GroundAI_Shooting : GroundAI
{
	public GameObject BulletProjectile;			// Object this unit will shoot with
	public GameObject ShotOffset;				// Where the bullet is fired from

	public float ShotSpeed = 2;					// Speed of the shot
	public int NumberOfShots = 5;				// Number of shots per set
	public float ShotDelay = 0.5f;				// Delay between each shot
	public float ReloadDelay = 1;				// Delay between each set of shots
	public float DeathTimer = 3;				// How long the bullets will last

	private ObjectCreation_CustomShots objectCreationScript;
	private float ShotConstraint = 0.32f;
	private bool ableToShoot = true;


	public override void Start()
	{
		base.Start ();

		if(!gameObject.GetComponent<ObjectCreation_CustomShots>())
			objectCreationScript = gameObject.AddComponent <ObjectCreation_CustomShots>();
		else
			objectCreationScript = gameObject.GetComponent<ObjectCreation_CustomShots>();

		if(ShotOffset == null)	ShotOffset = gameObject;
		objectCreationScript.SetProjectile (0, BulletProjectile);
	}
	public override void Reset()
	{
		base.Reset ();
		objectCreationScript.ResetObjectSpawner ();
	}
	public override void OnMonsterDeath()
	{
		base.OnMonsterDeath ();
		objectCreationScript.ResetObjectSpawner ();
	}


	// Shoot while running at or away from the player
	// Unit will only stand still while firing, but not during the reload time
	public IEnumerator C_RunAndGun(int moveDirection)
	{
		bool isRunning = false;

		while(!ChangeInAction)
		{
			if(ableToShoot && gameObject.transform.position.y-ShotConstraint < PlayersPosition.transform.position.y && PlayersPosition.transform.position.y < gameObject.transform.position.y+ShotConstraint)
			{
				StopCoroutine ("C_MoveUnit_X");
				isRunning = false;

				yield return StartCoroutine (C_Shoot_Forward());
				ableToShoot = false;
				StartCoroutine (ReloadTime ());
			}

			if(!isRunning)
			{
				StartCoroutine ("C_MoveUnit_X", moveDirection);
				isRunning = true;
			}

			yield return new WaitForSeconds(0.1f);
		}

		yield return null;
	}
	[AIAction]
	public IEnumerator C_RunAndGun_Away()
	{
		yield return StartCoroutine (C_RunAndGun (-1));
	}
	[AIAction]
	public IEnumerator C_RunAndGun_Towards()
	{
		yield return StartCoroutine (C_RunAndGun (1));
	}
	IEnumerator ReloadTime()
	{
		yield return new WaitForSeconds (ReloadDelay);
		ableToShoot = true;
	}


	// Shoots a set of objects in front of this unit, but only if the player is within a certain Yaxis range of this unit.
	// If the player is in the AIrange, but not in the Yaxis buffer, then a secondary action is done.
	// in this case, the unit will patrol
	[AIAction]
	public IEnumerator C_ShootAttack_SecondaryAction()
	{
		bool secondaryAction = false;	// Is a secondary action going on?

		while(!ChangeInAction)
		{
			if(gameObject.transform.position.y-ShotConstraint < PlayersPosition.transform.position.y && PlayersPosition.transform.position.y < gameObject.transform.position.y+ShotConstraint)
			{
				StopCoroutine ("C_Patrol");
				secondaryAction = false;
				AnimatorBoolUpdate("IsMoving", false);

				yield return StartCoroutine (C_Shoot_Forward());
				yield return new WaitForSeconds(ReloadDelay);
			}
			else
			{
				if(!secondaryAction)
				{
					AIprint ("NOT IN RANGE TO SHOOT, GOING TO DO A SECONDARY ACTION");
					secondaryAction = true;
					StartCoroutine ("C_Patrol");

					yield return null;
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
		AIprint ("I AM DONE TRYING TO SHOOT");
	}

	// Shoots a set of objects forward of this unit
	[AIAction]
	public IEnumerator C_ShootAttack_Forward()
	{
		AIprint ("I AM SHOOTING FORWARD AT THE PLAYER");
		
		while(!ChangeInAction)
		{
			yield return StartCoroutine (C_Shoot_Forward());
			yield return new WaitForSeconds (ReloadDelay);
			yield return null;
		}
		
		AIprint ("I AM DONE TRYING TO SHOOT");
	}

	// Shoots a set of objects at the player
	[AIAction]
	public IEnumerator C_ShootAttack_AtPlayer()
	{
		AIprint ("I AM SHOOTING AT THE PLAYER");
		
		while(!ChangeInAction)
		{
			yield return StartCoroutine (C_Shoot_AtPlayer());
			yield return new WaitForSeconds (ReloadDelay);			
			yield return null;
		}
		
		AIprint ("I AM DONE SHOOTING AT THE PLAYER");
	}





	// One time action to shoot a set of objects forward
	// To be used in other coroutines for simplicity
	IEnumerator C_Shoot_Forward()
	{
		AnimatorBoolUpdate ("IsShooting", true);
		FlipTowardsPlayer ();
		if (grounded) GetComponent<Rigidbody2D>().velocity = Vector3.zero;

		if(facingRight)
			objectCreationScript.Shoot_InDirection(0, ShotOffset, Vector2.zero, new Vector2(1,0), ShotDelay, NumberOfShots, ShotSpeed, 0, DeathTimer);
		else
			objectCreationScript.Shoot_InDirection(0, ShotOffset, Vector2.zero, new Vector2(-1,0), ShotDelay, NumberOfShots, ShotSpeed, 0, DeathTimer);
		
		yield return new WaitForSeconds (ShotDelay*NumberOfShots);
		AnimatorBoolUpdate ("IsShooting", false);
	}
	// One time action to shoot a set of objects forward
	// To be used in other coroutines for simplicity
	IEnumerator C_Shoot_Vertical()
	{
		AnimatorBoolUpdate ("IsShooting", true);
		FlipTowardsPlayer ();
		if (grounded)	GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		
		if(PlayersPosition.transform.position.y > gameObject.transform.position.y)
			objectCreationScript.Shoot_InDirection(0, ShotOffset, Vector2.zero, new Vector2(0,1), ShotDelay, NumberOfShots, ShotSpeed, 0, DeathTimer);
		else
			objectCreationScript.Shoot_InDirection(0, ShotOffset, Vector2.zero, new Vector2(0,-1), ShotDelay, NumberOfShots, ShotSpeed, 0, DeathTimer);
		
		yield return new WaitForSeconds (ShotDelay*NumberOfShots);
		AnimatorBoolUpdate ("IsShooting", false);
	}
	// One time action to shoot a set of objects at the player
	// To be used in other coroutines for simplicity
	IEnumerator C_Shoot_AtPlayer()
	{
		AnimatorBoolUpdate ("IsShooting", true);
		FlipTowardsPlayer ();
		if (grounded)	GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		
		objectCreationScript.Shoot_AtTarget(0, ShotOffset, Vector2.zero, PlayersPosition, Vector2.zero, ShotDelay, NumberOfShots, ShotSpeed, 0, DeathTimer);
		
		yield return new WaitForSeconds (ShotDelay*NumberOfShots);
		AnimatorBoolUpdate ("IsShooting", false);
	}


}
