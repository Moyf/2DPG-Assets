// RegalPrime 11-29-14 - GenericMonsterAI_Flying.cs

// A somewhat simplistic AI for air based units
// Depending on certain radiuses from the player, this unit will go into different states
// This is fairly similar to that of the GenericMonsterAI, but with a few changes
// - Monster is never grounded and is flying
// - Jump attack is now the swooping attack

// Attack Radius - If the player is within this attack radius, it will attack (either jump / shoot)
// Default attack is the swooping attack
// ShootWhenAggro = true - the unit will shoot a projectile instead - selecting ShootAtPlayer will cause it to shoot at the player
// ShotConstraint calculates if the player is in a vertical range from the center point of the unit, when it will shoot (if ShootAtPlayer = false)
// in another words, this causes the unit to only shoot when the player is in front of the unit.

// Passive Radius - If the player is outside of this radius, it will become passive and start patrolling

// The distance between Attack and Passive Radius, the unit will try to move twords the player

// Changing move speed / patrol speed / jump height to zero will cause the unit to idle (when they are in the correct state)
// A patrol area of 0 will cause the unit to only turn around when they come in contact with a wall

// Check out the monster prefabs to test how they work
// Using an Animator Override Controller on the GenericAnimationController will allow you to plug in your own Animations for your own units

// This uses calculate checks script to detect its surroundings


using UnityEngine;
using System.Collections;

public class GenericMonsterAI_Flying : MonoBehaviour
{	
	public LayerMask whatIsGround;
	public Transform wallCheck;
	public GameObject MonsterProjectile;
	
	public bool DefaultFacingRight = false;		// if the sprite being used is facing the wrong way, this toggle will flip the objects
	
	public float AttackRadius = 1f;				// If the player enters this radius, the monster will attack the player
	public float PassiveRadius = 2f;			// If the player is outside of this radius, the monster will become passive and patrol
												// Any gaps inbetween will cause the monster to move twords the player
	
	public float MoveSpeed = 1f;				// The speed at which the monster will move when trying to reach the player
	
	public float PatrolArea = 1f;				// The distance on either side of the monster it will patrol
	public float PatrolSpeed = 1f;				// The speed of the patroling monster
	
	public float JumpSpeed = 1f;				// Speed of the jumping monster
//	public float JumpPower = 1f;				// Jump power of the monster
	public float DelayBetweenJumps = 0.5f;		// The time between jumps
	
	public bool ShootWhenAggro = false;			// Selecting this will cause the monster to shoot forward when in attack range
	public bool ShootAtPlayer = false;			// Setting this to true, will shoot the projectiles at the player
	public float ShotSpeed = 1;					// Speed of the shot
	public int NumberOfShots = 5;				// Number of shots per set
	public float ShotDelay = 0.1f;				// Delay between each shot
	public float ReloadDelay = 1;				// Delay between each set of shots
	public float ShotConstraint = 0.50f;		// The width in which the monster will shoot when aggro
	
	private CalculateChecks calculateChecks;	// The script that calculates the distance between two colliders
	private bool wallcheck;
	private float DistanceToWall = 0;

	Animator anim;
	
	bool facingRight = false;
	bool AbleToJump = true;
//	float ScaleModifier;

	float DistanceToPlayer = 0;
	public int AI_Action = 0;
	
	private NewObjectSpawner MonsterManipulation;
	private Vector3 startingPosition;
	
	void Start()
	{
		EventManager.resetObjects += Reset;
		startingPosition = gameObject.transform.position;

		anim = GetComponent<Animator>();
		if(DefaultFacingRight)
		{
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
		
		MonsterManipulation = gameObject.AddComponent<NewObjectSpawner> ();
		MonsterManipulation.SetProjectile (0, MonsterProjectile);
		MonsterManipulation.SetToEnablePooling (0, false);

//		MonsterManipulation.AddSelfToList (0);
//		MonsterManipulation.ChangeColor_Children (0, 1f);
//		MonsterManipulation.SetColor_Random (0);
//		MonsterManipulation.SetKinematicObjects (0, false);
//		ScaleModifier = gameObject.transform.localScale.x;

		if(GetComponent<CalculateChecks> ())
			calculateChecks = GetComponent<CalculateChecks> ();
		else
			calculateChecks = gameObject.AddComponent<CalculateChecks> ();
		calculateChecks.ChangeLayerMask (whatIsGround);

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
		MonsterManipulation.ResetObjectSpawner ();
		gameObject.SetActive (true);
		anim.SetBool ("IsMoving", false);
		anim.SetBool ("IsJumping", false);
		anim.SetBool ("IsShooting", false);
		anim.SetBool ("IsDead", false);
		AbleToJump = true;
		
		StartCoroutine (C_StartAI ());
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

	void FixedUpdate()
	{
		DistanceToPlayer = Vector2.Distance (GameController.control.GuyLocation.transform.position, gameObject.transform.position);
		
		AI_Action = 3;
		
		// Move Twords
		if(DistanceToPlayer > 0)
			AI_Action = 2;
		
		// Patrol
//		if(DistanceToPlayer > PassiveRadius && PassiveRadius >= AttackRadius)
		if(DistanceToPlayer > PassiveRadius)
			AI_Action = 0;
		
		// Attack
		if(DistanceToPlayer < AttackRadius)
		{
			if(gameObject.transform.position.x-ShotConstraint < GameController.control.GuyLocation.transform.position.x && GameController.control.GuyLocation.transform.position.x < gameObject.transform.position.x+ShotConstraint)
				AI_Action = 1;
			if(gameObject.transform.position.y-ShotConstraint < GameController.control.GuyLocation.transform.position.y && GameController.control.GuyLocation.transform.position.y < gameObject.transform.position.y+ShotConstraint)
				AI_Action = 1;
			if(ShootAtPlayer)
				AI_Action = 1;
			if(!ShootWhenAggro)
				AI_Action = 1;
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
				//		case 4:
				//			yield return StartCoroutine (C_PawSlam (0));
				//			break;
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
			if(GameController.control.GuyLocation.transform.position.x > gameObject.transform.position.x && !facingRight)
				Flip ();
			if(GameController.control.GuyLocation.transform.position.x < gameObject.transform.position.x && facingRight)
				Flip ();

			GetComponent<Rigidbody2D>().velocity = ((GameController.control.GuyLocation.transform.position - transform.position).normalized * MoveSpeed);

			yield return null;
		}

		GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
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
				if(gameObject.transform.position.x-ShotConstraint < GameController.control.GuyLocation.transform.position.x && GameController.control.GuyLocation.transform.position.x < gameObject.transform.position.x+ShotConstraint)
				{
					if(gameObject.transform.position.y < GameController.control.GuyLocation.transform.position.y)
						MonsterManipulation.ShootDirection(0, NumberOfShots, new Vector2(0,1), ShotDelay, ShotSpeed);
					else
						MonsterManipulation.ShootDirection(0, NumberOfShots, new Vector2(0,-1), ShotDelay, ShotSpeed);
				}
				else
				if(gameObject.transform.position.y-ShotConstraint < GameController.control.GuyLocation.transform.position.y && GameController.control.GuyLocation.transform.position.y < gameObject.transform.position.y+ShotConstraint)
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
		}
		
		print ("I AM DONE TRYING TO SHOOT");
		anim.SetBool ("IsShooting", false);
	}
	
	IEnumerator C_JumpAttack()
	{
		print ("I AM TRYING TO JUMP ATTACK");

		if(JumpSpeed != 0)
			anim.SetBool ("IsJumping", true);

		while(AI_Action == 1)
		{
			if(DistanceToPlayer < AttackRadius && AbleToJump && JumpSpeed > 0)
			{
				yield return StartCoroutine (C_Leap ());
			}
			
			yield return null;
		}

		print ("I AM DONE TRYING TO JUMP ATTACK");
		anim.SetBool ("IsJumping", false);
	}
	
	IEnumerator C_Leap()
	{
		AbleToJump = false;
		
		if(GameController.control.GuyLocation.transform.position.x > gameObject.transform.position.x && !facingRight)
			Flip ();
		if(GameController.control.GuyLocation.transform.position.x < gameObject.transform.position.x && facingRight)
			Flip ();


		Vector3 pointPos;
		float CenterPointOffset = GameController.control.GuyLocation.transform.position.x - gameObject.transform.position.x;
		Vector3 RotateAroundThis = new Vector3(gameObject.transform.position.x+CenterPointOffset, gameObject.transform.position.y, 0);

//		Vector3 RotateAroundThis = new Vector3(GameController.control.GuyLocation.transform.position.x, gameObject.transform.position.y, 0);

		float X_Width = Mathf.Abs (RotateAroundThis.x - gameObject.transform.position.x);
		float Y_Width = (gameObject.transform.position.y - GameController.control.GuyLocation.transform.position.y);

		if(GameController.control.GuyLocation.transform.position.x > gameObject.transform.position.x)
		for(float I=270; I>=90; I=I-1*JumpSpeed)
		{
			float pointNum = (I*1.0f)/360;
			float angle = pointNum*Mathf.PI*2;
			
			float x = Mathf.Sin (angle)*X_Width;
			float y = Mathf.Cos (angle)*Y_Width;

			pointPos = new Vector3(x, y, gameObject.transform.position.z) + RotateAroundThis;
//			pointPos = Vector3.Lerp(gameObject.transform.position, pointPos, Time.deltaTime*JumpSpeed);
			gameObject.transform.position = pointPos;

/*
			if(!Physics2D.OverlapCircle(pointPos, renderer.bounds.extents.x, whatIsGround))
				gameObject.transform.position = pointPos;
			else
			{
				print ("BUMP");
				I=0;
			}
*/			
			yield return null;
		}
		else
		for(float I=90; I<=270; I=I+1*JumpSpeed)
		{
			float pointNum = (I*1.0f)/360;
			float angle = pointNum*Mathf.PI*2;
			
			float x = Mathf.Sin (angle)*X_Width;
			float y = Mathf.Cos (angle)*Y_Width;
				
			pointPos = new Vector3(x, y, gameObject.transform.position.z) + RotateAroundThis;
//			pointPos = Vector3.Lerp(gameObject.transform.position, pointPos, Time.deltaTime*JumpSpeed);
			gameObject.transform.position = pointPos;

/*
			if(!Physics2D.OverlapCircle(pointPos, renderer.bounds.extents.x, whatIsGround))
				gameObject.transform.position = pointPos;
			else
			{
				print ("BUMP");
				I=271;
			}
*/
			yield return null;
		}

		GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		yield return new WaitForSeconds (DelayBetweenJumps);

		AbleToJump = true;
	}
	
	IEnumerator C_Idle()
	{
		while(AI_Action == 3)
		{
			yield return null;
		}
		yield return null;
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

//Bézier Curve Leap
	IEnumerator C_Leap()
	{
		AbleToJump = false;
		
		if(GameController.control.GuyLocation.transform.position.x > gameObject.transform.position.x && !facingRight)
			Flip ();
		if(GameController.control.GuyLocation.transform.position.x < gameObject.transform.position.x && facingRight)
			Flip ();

		Vector3 m_StartPosition = gameObject.transform.position;

//		Vector3 m_CurvePosition = GameController.control.GuyLocation.transform.position;
		Vector3 m_CurvePosition;
		if(GameController.control.GuyLocation.transform.position.y < gameObject.transform.position.y)
			m_CurvePosition = new Vector3(GameController.control.GuyLocation.transform.position.x, GameController.control.GuyLocation.transform.position.y-2, GameController.control.GuyLocation.transform.position.z);
		else
			m_CurvePosition = new Vector3(GameController.control.GuyLocation.transform.position.x, GameController.control.GuyLocation.transform.position.y+2, GameController.control.GuyLocation.transform.position.z);


		float DistanceToPlayer = Mathf.Abs (GameController.control.GuyLocation.transform.position.x - gameObject.transform.position.x);
		Vector3 m_TargetPosition;
		if(facingRight)
			m_TargetPosition = new Vector3 (m_StartPosition.x+DistanceToPlayer, m_StartPosition.y, m_StartPosition.z);
		else
			m_TargetPosition = new Vector3 (m_StartPosition.x-DistanceToPlayer, m_StartPosition.y, m_StartPosition.z);

		for(float I=0; I<1; I+=Time.deltaTime)
		{
			gameObject.transform.position = (((1 - I) * (1 - I)) * m_StartPosition) + (((1 - I) * 2.0f) * I * m_CurvePosition) + ((I * I) * m_TargetPosition);

			yield return null;
		}

		AbleToJump = true;
	}



// OLD LEAP
 	IEnumerator C_Leap()
	{
		AbleToJump = false;
		
		if(GameController.control.GuyLocation.transform.position.x > gameObject.transform.position.x && !facingRight)
			Flip ();
		if(GameController.control.GuyLocation.transform.position.x < gameObject.transform.position.x && facingRight)
			Flip ();

		float YDirection = 0;

		JumpPower = Mathf.Abs (GameController.control.GuyLocation.transform.position.y - gameObject.transform.position.y);

		if(GameController.control.GuyLocation.transform.position.y > gameObject.transform.position.y)
			YDirection = Time.deltaTime*JumpSpeed;
		else
			YDirection = -Time.deltaTime*JumpSpeed;

		for(float I=0; I<JumpPower; I+=Time.deltaTime*JumpSpeed)
		{
			if(facingRight)
				gameObject.transform.position = new Vector2(gameObject.transform.position.x+Time.deltaTime*JumpSpeed, gameObject.transform.position.y+YDirection);
			else
				gameObject.transform.position = new Vector2(gameObject.transform.position.x-Time.deltaTime*JumpSpeed, gameObject.transform.position.y+YDirection);

			yield return null;
		}

		for(float I=0; I<JumpPower; I+=Time.deltaTime*JumpSpeed)
		{
			if(facingRight)
				gameObject.transform.position = new Vector2(gameObject.transform.position.x+Time.deltaTime*JumpSpeed, gameObject.transform.position.y-YDirection);
			else
				gameObject.transform.position = new Vector2(gameObject.transform.position.x-Time.deltaTime*JumpSpeed, gameObject.transform.position.y-YDirection);
			
			yield return null;
		}

		AbleToJump = true;
	}

*/