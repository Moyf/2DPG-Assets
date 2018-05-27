// RegalPrime 11-29-14 - GenericMonsterAI.cs

// A somewhat simplistic AI for ground based units
// Depending on certain radiuses from the player, this unit will go into different states

// Attack Radius - If the player is within this attack radius, it will attack (either jump / shoot)
// Default attack is the jump attack
// ShootWhenAggro = true - the unit will shoot a projectile instead - selecting ShootAtPlayer will cause it to shoot at the player
// ShotConstraint calculates if the player is in a vertical range from the center point of the unit, when it will shoot (if ShootAtPlayer = false)
// in another words, this causes the unit to only shoot when the player is in front of the unit.

// Passive Radius - If the player is outside of this radius, it will become passive and start patrolling

// The distance between Attack and Passive Radius, the unit will try to move twords the player

// Changing move speed / patrol speed / jump height to zero will cause the unit to idle (when they are in the correct state)
// A patrol area of 0 will cause the unit to only turn around when they come in contact with a wall

// Check out the monster prefabs to test how they work
// Using an Animator Override Controller on the GenericAnimationController will allow you to plug in your own Animations for your own units

// I have print() calls pretty much at every change in AI state for testing to make sure everything is working right (logically)
// Will comment those out later.


// INFORMATION !!
// The ground / wall / pit checks are done via alternate objects connected to this gameobject and by also setting the size of the checks in script
// I havnt gotten around to making the checks dynamic based on the size of the objects collider

// THIS IS NO LONGER TRUE
// THE OLD CODE IS COMMENTED AT THE END OF THE SCRIPT AS A BACKUP
// THIS SCRIPT USES THE CALCULATE CHECKS SCRIPT TO CALCULATE GROUND / WALL / PIT HITS (CalculateChecks.cs)
// HOPEFULLY THIS WORKS AND I CAN CLEAN UP ALL THE COMMENTS AND THE CODE

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GenericMonsterAI : MonoBehaviour
{	
	public LayerMask whatIsGround;
	public GameObject MonsterProjectile;

	public bool DefaultFacingRight = false;		// if the sprite being used is facing the wrong way, this toggle will flip the objects

	public float AttackRadius = 1f;				// If the player enters this radius, the monster will attack the player
	public float PassiveRadius = 2f;			// If the player is outside of this radius, the monster will become passive and patrol
												// Any gaps inbetween will cause the monster to move twords the player

	public bool JumpUpBlocks = true;

	public float MoveSpeed = 1f;				// The speed at which the monster will move when trying to reach the player

	public float PatrolArea = 1f;				// The distance on either side of the monster it will patrol
	public float PatrolSpeed = 1f;				// The speed of the patroling monster

	public float JumpSpeed = 1f;				// Speed of the jumping monster
	public float JumpPower = 1f;				// Jump power of the monster
	public float DelayBetweenJumps = 0.5f;		// The time between jumps

	public bool ShootWhenAggro = false;			// Selecting this will cause the monster to shoot forward when in attack range
	public bool ShootAtPlayer = false;			// Setting this to true, will shoot the projectiles at the player
	public float ShotSpeed = 1;					// Speed of the shot
	public int NumberOfShots = 5;				// Number of shots per set
	public float ShotDelay = 0.1f;				// Delay between each shot
	public float ReloadDelay = 1;				// Delay between each set of shots
	public float ShotConstraint = 0.50f;		// The width in which the monster will shoot when aggro

	private bool grounded;
	private bool wallcheck;
	private bool pitcheck;
	private CalculateChecks calculateChecks;			// The script that calculates the distance between two colliders
	private float ColliderIndent = 0.01f;				// The slight indent of AoE checks to prevent false positives (if needed)
	private float DistanceToGround = 0;
	private float DistanceToWall = 0;
	private float DistanceToPit = 0;
	private bool facingRight = false;

	bool AbleToJump = true;

	Animator anim;

	float DistanceToPlayer = 0;
	public int AI_Action = 0;

	private NewObjectSpawner MonsterManipulation;
	private Vector3 startingPosition;
//	private Vector3 BackupScale;					// Not used right now

	void MonsterDeath()								// this function is called when the HP module tells this script there is 0 hp
	{
		print ("GenericMonsterAI - I WAS TOLD THAT MY HP IS GONE");
	}


	void Start()
	{
		if(GetComponent<HPModule>())
		{
			HPModule TEMP = GetComponent<HPModule>();
			TEMP.On_HPModule_Death += MonsterDeath;
		}

		EventManager.resetObjects += Reset;
		startingPosition = gameObject.transform.position;

		if(JumpPower == 0)
			JumpSpeed = 0;

		anim = GetComponent<Animator>();
		if(DefaultFacingRight)
		{
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
//			BackupScale = theScale;

			Canvas[] childCanvases = GetComponentsInChildren<Canvas> ();
			foreach(Canvas child in childCanvases)
			{
				theScale = child.transform.localScale;
				theScale.x *= -1;
				child.transform.localScale = theScale;
			}
		}

		MonsterManipulation = gameObject.AddComponent<NewObjectSpawner> ();
		MonsterManipulation.SetProjectile (0, MonsterProjectile);
		MonsterManipulation.SetToEnablePooling (0, false);

		if(GetComponent<CalculateChecks> ())
			calculateChecks = GetComponent<CalculateChecks> ();
		else
			calculateChecks = gameObject.AddComponent<CalculateChecks> ();
		calculateChecks.ChangeLayerMask (whatIsGround);
		calculateChecks.Set_ColliderIndent (ColliderIndent);

		StartCoroutine (C_StartAI ());
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		StopAllCoroutines ();
		gameObject.transform.position = startingPosition;
		gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
//		transform.localScale = BackupScale;
		AbleToJump = true;

		MonsterManipulation.ResetObjectSpawner ();
		gameObject.SetActive (true);
		anim.SetBool ("IsMoving", false);
		anim.SetBool ("IsJumping", false);
		anim.SetBool ("IsShooting", false);
		anim.SetBool ("IsDead", false);

		StartCoroutine (C_StartAI ());
	}


	void GroundCheck()
	{
		DistanceToGround = calculateChecks.GroundCheck (true);					// Calculate Ground check
//		grounded = (DistanceToGround < 0.02f) ? true : false;
		if(DistanceToGround < 0.02f)
			grounded = true;
		else
			grounded = false;
	}

	bool WallCheck()
	{
		DistanceToWall = calculateChecks.WallCheck (true, facingRight);		// Calculate Wall check
		if(DistanceToWall < 0.02f)
			wallcheck = true;
		else
			wallcheck = false;

		return wallcheck;
	}

	bool PitCheck()
	{
		DistanceToPit = calculateChecks.PitCheck (true, facingRight);		// Calculate Wall check

		if(DistanceToPit < 0.02f)
			pitcheck = true;
		else
			pitcheck = false;

		return pitcheck;
	}



	void FixedUpdate()
	{
		GroundCheck ();

		anim.SetBool ("Grounded", grounded);

		DistanceToPlayer = Vector2.Distance (GameController.control.GuyLocation.transform.position, gameObject.transform.position);

		// Default Idle
		AI_Action = 3;

		// Move Twords
		if(DistanceToPlayer >= AttackRadius)
			AI_Action = 2;

		// Patrol
//		if(DistanceToPlayer > PassiveRadius && PassiveRadius >= AttackRadius)
		if(DistanceToPlayer > PassiveRadius)
			AI_Action = 0;

		// Attack
		if(DistanceToPlayer < AttackRadius)
		{
			// if the character is withing the shooting bounds
			if((gameObject.transform.position.y-ShotConstraint < GameController.control.GuyLocation.transform.position.y) && (GameController.control.GuyLocation.transform.position.y < gameObject.transform.position.y+ShotConstraint) && ShootWhenAggro)
				AI_Action = 1;
			else
			{
				if(gameObject.transform.position.y-ShotConstraint*2 < GameController.control.GuyLocation.transform.position.y && GameController.control.GuyLocation.transform.position.y < gameObject.transform.position.y+ShotConstraint*2)
					AI_Action = 2;
				else
					AI_Action = 0;
			}

			// if leaping
			if(!ShootWhenAggro)
				AI_Action = 1;
			// if shooting directly at the player
			if(ShootAtPlayer)
				AI_Action = 1;


//			if(!ShootWhenAggro || gameObject.transform.position.y-ShotConstraint < GameController.control.GuyLocation.transform.position.y && GameController.control.GuyLocation.transform.position.y < gameObject.transform.position.y+ShotConstraint || ShootAtPlayer)
//				AI_Action = 1;
//			else
//				print ("TRYING TO SHOOT BUT CANNOT");
		}

		// Idle
		if(anim.GetBool("IsDead"))
		{
			AI_Action = 3;
		}

	}


	IEnumerator C_StartAI()
	{
		yield return new WaitForSeconds(1f);

		while(gameObject != null)
		{
			switch(AI_Action)
			{
			case 0:
				yield return StartCoroutine (C_Patrol ());
				break;
			case 1:
				yield return StartCoroutine (C_Attack ());
				break;
			case 2:
				yield return StartCoroutine (C_MoveTwordsPlayer ());
				break;
			case 3:
				yield return StartCoroutine (C_Idle ());
				break;
			}
		}
		print ("EXIT");
		yield return null;
	}


	// If PatrolArea = 0, then there are no bounds
	IEnumerator C_Patrol()
	{
		print ("I AM STARTING PATROLING");
		if(PatrolSpeed != 0)
			anim.SetBool ("IsMoving", true);

		float PatrolDistance = 0;

		while(AI_Action == 0)
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

		print ("I AM DONE PATROLING");
		anim.SetBool ("IsMoving", false);
	}

	IEnumerator C_MoveTwordsPlayer()
	{
		print ("I AM MOVING TO THE PLAYER");

		if(MoveSpeed != 0)
			anim.SetBool ("IsMoving", true);

		while(AI_Action == 2)
		{
			if(MoveSpeed !=0)
			{
				if(MoveSpeed < 0)
				{
					if(GameController.control.GuyLocation.transform.position.x < gameObject.transform.position.x && !facingRight && grounded)
						Flip();
					if(GameController.control.GuyLocation.transform.position.x > gameObject.transform.position.x && facingRight && grounded)
						Flip();
				}
				else
				{
					if(GameController.control.GuyLocation.transform.position.x > gameObject.transform.position.x && !facingRight && grounded)
						Flip ();
					if(GameController.control.GuyLocation.transform.position.x < gameObject.transform.position.x && facingRight && grounded)
						Flip ();
				}

				if(WallCheck () && JumpUpBlocks)
				{
					yield return StartCoroutine (C_LeapBlock ());
				}
				else
				{
	//			if(grounded)
					GetComponent<Rigidbody2D>().velocity = new Vector2(((GameController.control.GuyLocation.transform.position - transform.position).normalized * MoveSpeed).x, GetComponent<Rigidbody2D>().velocity.y); 

				}
			}
			yield return null;
		}

		while(!grounded)
			yield return null;

//		if(grounded)
			GetComponent<Rigidbody2D>().velocity = Vector3.zero;

		yield return null;
		print ("I AM DONE TRYING TO MOVE TO THE PLAYER");
		anim.SetBool ("IsMoving", false);
	}

	IEnumerator	C_Attack()
	{
		if(ShootWhenAggro && MonsterProjectile != null)
			yield return StartCoroutine (C_ShootAttack ());
		else
			yield return StartCoroutine (C_JumpAttack ());

		yield return null;
	}

	IEnumerator C_ShootAttack()
	{
		print ("I AM TRYING TO SHOOT");
		
		while(AI_Action == 1)
		{
			anim.SetBool ("IsShooting", true);

			if(GameController.control.GuyLocation.transform.position.x > gameObject.transform.position.x && !facingRight)
				Flip ();
			if(GameController.control.GuyLocation.transform.position.x < gameObject.transform.position.x && facingRight)
				Flip ();
			
			//			gameObject.rigidbody2D.velocity = new Vector2(0, 0);
			
			if(ShootAtPlayer)
			{
				MonsterManipulation.ShootAtTarget_Player(0, ShotDelay, NumberOfShots, ShotSpeed);
			}
			else
			{
				if(gameObject.transform.position.y-0.50f < GameController.control.GuyLocation.transform.position.y && GameController.control.GuyLocation.transform.position.y < gameObject.transform.position.y+0.50f)
				{
					if(facingRight)
						MonsterManipulation.ShootDirection(0, NumberOfShots, new Vector2(1,0), ShotDelay, ShotSpeed);
					else
						MonsterManipulation.ShootDirection(0, NumberOfShots, new Vector2(-1,0), ShotDelay, ShotSpeed);
				}
			}

			yield return new WaitForSeconds (ShotDelay*NumberOfShots);
			anim.SetBool ("IsShooting", false);
			yield return new WaitForSeconds (ReloadDelay);


			yield return null;
		}
		
		print ("I AM DONE TRYING TO SHOOT");
		anim.SetBool ("IsShooting", false);
	}

	IEnumerator C_JumpAttack()
	{
		print ("I AM TRYING TO JUMP ATTACK");

		while(AI_Action == 1)
		{
			if(JumpPower != 0)
				anim.SetBool ("IsJumping", true);
			yield return null;

			if(DistanceToPlayer < AttackRadius && AbleToJump && grounded)
			{
				yield return StartCoroutine (C_Leap ());

				GetComponent<Rigidbody2D>().velocity = Vector3.zero;

				if(DelayBetweenJumps != 0)
				{
					anim.SetBool ("IsJumping", false);
					yield return new WaitForSeconds (DelayBetweenJumps);
				}
			}
		}

		print ("I AM DONE TRYING TO JUMP ATTACK");
		anim.SetBool ("IsJumping", false);
	}

	IEnumerator C_Leap()
	{
		AbleToJump = false;
		
		if(grounded)
		{
			if(MoveSpeed < 0)
			{
				if(GameController.control.GuyLocation.transform.position.x < gameObject.transform.position.x && !facingRight && grounded)
					Flip();
				if(GameController.control.GuyLocation.transform.position.x > gameObject.transform.position.x && facingRight && grounded)
					Flip();
			}
			else
			{
				if(GameController.control.GuyLocation.transform.position.x > gameObject.transform.position.x && !facingRight && grounded)
					Flip ();
				if(GameController.control.GuyLocation.transform.position.x < gameObject.transform.position.x && facingRight && grounded)
					Flip ();
			}

			float Y_Flip = gameObject.transform.localScale.y / Mathf.Abs (gameObject.transform.localScale.y);

			if(facingRight && JumpSpeed > 0)
				gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(JumpSpeed, JumpPower*Y_Flip);
			else
				gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-JumpSpeed, JumpPower*Y_Flip);
		}
		yield return null;
		yield return null;

		// If the object is moving upwards and is not grounded, wait until it is grounded before returning
		while(gameObject.GetComponent<Rigidbody2D>().velocity.y > 0 || !grounded)
		{
			yield return null;
		}

		AbleToJump = true;
		yield return null;
	}

	IEnumerator C_LeapBlock()
	{
		if(grounded)
		{
			print ("TRYING TO LEAP UP A BLOCK");
			AbleToJump = false;

			gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 2);
			yield return new WaitForSeconds (0.3f);

			if(facingRight)
				gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(1, 0);
			else
				gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-1, 0);

			yield return new WaitForSeconds (0.2f);

			AbleToJump = true;
		}

		yield return null;
	}

	IEnumerator C_Idle()
	{
		GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		while(AI_Action == 3)
		{
			yield return null;
		}
	}

	void Flip ()
	{
		facingRight = !facingRight;
		
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;

		Canvas[] childCanvases = GetComponentsInChildren<Canvas> ();
		foreach(Canvas child in childCanvases)
		{
			theScale = child.transform.localScale;
			theScale.x *= -1;
			child.transform.localScale = theScale;
		}
	}
}




/*

*/