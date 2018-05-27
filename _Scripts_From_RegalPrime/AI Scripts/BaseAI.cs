// RegalPrime 6-02-15 - BaseAI.cs

// Base AI class
// Abstract class that deals with when this object should enter into different AI action states depending on distance to the player
// The child classes create AIactions based on their needs. These actions can be tagged with an attribute [AIAction]
// Any AIaction that is tagged with that attribute will be noticed by both the custom editor and this script.
// A list will be created of all the actions and allows a custom editor to show those to the user.
// The user can then select an action for each AI state
// Previous script version you would of have to create a separate child script and change the abstrat functions to achieve this 

// I would also like to note that doing this was a bit advanced for me and whereas it was a good learning opportunity, there might be some logic errors / bugs

// If available, this script attaches to the HPmodule and is aware when its HP reaches 0

// if ShowAIDialogue = true, messages will be displayed to the console on what the AI is / attempting to do
// This is nothing but feedback for testing and I wanted to have a way to toggle this on and off

// Current hierarchy tree (parent -> child)
// BaseAI(abstract) -> MovingAI(abstract) -> GroundAI -> GroundAI_Shooting
// BaseAI(abstract) -> MovingAI(abstract) -> FlyingAI -> FlyingAI_Shooting

// To create a custom AI unit only attach the NON-Abstract classes to the gameobject.
// Like mentioned above, depending on the script attached, it will auto-populate the available AIActions in the editor.


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
public class AIActionAttribute : Attribute{}

public abstract class BaseAI : MonoBehaviour
{
	public LayerMask whatIsSolid;				// What layer should this object consider solid
	public bool DefaultFacingLeft = false;

	public float AIUpdateDelay = 0.1f;			// How often the AI checks its surroundings

	public float AttackRadius = 1f;				// If the player enters this radius, the monster will attack the player
	public float PassiveRadius = 2f;			// If the player is outside of this radius, the monster will become passive and patrol
												// Any gaps inbetween will cause the monster to move twords the player
	private float DistanceToPlayer = 0;			// Distance to the nearest player
	private int AI_Action = 0;					// Current action
	private int AI_Action_Previous = 0;			// Previous AI action
	private bool _ChangeInAction = false;		// Tells if there is a change in AI behavior
	private bool ObjectIsDead = false;			// Tells if this object is dead



	private CalculateChecks calculateChecks;	// The script that calculates the distance between two colliders
	private float ColliderIndent   = 0.01f;		// The slight indent of AoE checks to prevent false positives (if needed)
	private float ClosenessToSolid = 0.05f;		// How close to an object and still be considered hitting that object

	public bool grounded  {get; set;}
	public bool wallcheck {get; set;}
	public bool pitcheck  {get; set;}
	
	public bool facingRight { get; set; }
	
	public GameObject PlayersPosition {get; set;}
	public Vector3 startingPosition {get; set;}		// Where this object was initially spawned (used in Reset)
	private Animator anim;							// Reference to the animator
	private HPModule HPModuleScript;				// References to the HP module if it exists

	private bool ShowAIDialogue = false;			// Show information about current AI actions in the console



	public delegate IEnumerator CustomAIAction();
	public CustomAIAction PassiveAIAction;
	public CustomAIAction AlertAIAction;
	public CustomAIAction AggressiveAIAction;


	[HideInInspector] public int PassiveAINumber = 0;
	[HideInInspector] public int AlertAINumber = 0;
	[HideInInspector] public int AggressiveAINumber = 0;



	public IEnumerator C_PassiveAction()
	{
		yield return StartCoroutine (PassiveAIAction());
	}
	public IEnumerator C_AlertAction ()
	{
		yield return StartCoroutine (AlertAIAction());
	}
	public IEnumerator C_AggressiveAction ()
	{
		yield return StartCoroutine (AggressiveAIAction());
	}

	public IEnumerator C_IdleAction()
	{
		yield return StartCoroutine (C_Idle ());
	}


	public virtual void Start()
	{
		EventManager.resetObjects += Reset;
		HPModuleScript = GetComponent<HPModule> ();
		if(HPModuleScript != null)
			HPModuleScript.On_HPModule_Death += OnMonsterDeath;

		if(GetComponent<CalculateChecks> ())
			calculateChecks = GetComponent<CalculateChecks> ();
		else
			calculateChecks = gameObject.AddComponent<CalculateChecks> ();
		calculateChecks.ChangeLayerMask (whatIsSolid);
		calculateChecks.Set_ColliderIndent (ColliderIndent);
		calculateChecks.Set_BackwardsSprite_X (DefaultFacingLeft);

		anim = GetComponent<Animator>();
		startingPosition = gameObject.transform.position;

		if(DefaultFacingLeft)
			facingRight = false;
		else
			facingRight = true;

		List<MethodInfo> allTheFunctions = GetFunctionList ();
		PassiveAIAction 	= (CustomAIAction)Delegate.CreateDelegate(typeof(CustomAIAction), this, allTheFunctions[PassiveAINumber]);
		AlertAIAction 		= (CustomAIAction)Delegate.CreateDelegate(typeof(CustomAIAction), this, allTheFunctions[AlertAINumber]);
		AggressiveAIAction 	= (CustomAIAction)Delegate.CreateDelegate(typeof(CustomAIAction), this, allTheFunctions[AggressiveAINumber]);

		StartCoroutine (C_StartAI ());
		StartCoroutine (C_CalculateSurroundings ());
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
		if(HPModuleScript != null)
			HPModuleScript.On_HPModule_Death -= OnMonsterDeath;
	}


	public virtual void Reset()
	{
		StopAllCoroutines ();

		gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		gameObject.transform.position = startingPosition;

		ObjectIsDead = false;
		gameObject.SetActive (true);

		AnimatorBoolUpdate ("IsMoving", false);
		AnimatorBoolUpdate ("IsJumping", false);
		AnimatorBoolUpdate ("IsShooting", false);
		AnimatorBoolUpdate ("IsDead", false);
		AnimatorBoolUpdate ("IsAttacking", false);

		StartCoroutine (C_StartAI ());
		StartCoroutine (C_CalculateSurroundings ());
	}
	public virtual void OnMonsterDeath()
	{
		// What to do when the monster dies
		AIprint ("GenericMonsterAI - I WAS TOLD THAT MY HP IS GONE");
		StopAllCoroutines ();
		GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		ObjectIsDead = true;
	}


	public bool GroundCheck()
	{
		grounded = (calculateChecks.GroundCheck () < ClosenessToSolid) ? true : false;
		return grounded;
	}
	public bool WallCheck()
	{
		wallcheck = (calculateChecks.WallCheck () < ClosenessToSolid) ? true : false;
		return wallcheck;
	}
	public bool PitCheck()
	{
		pitcheck = (calculateChecks.PitCheck () < ClosenessToSolid) ? true : false;
		return pitcheck;
	}

	public void AIprint(string Dialogue)
	{
		if(ShowAIDialogue)
		{
			print (Dialogue);
		}
	}
	public void AnimatorBoolUpdate(string Name, bool aBool)
	{
		anim.SetBool (Name, aBool);
	}
	public bool ChangeInAction
	{
		get{return _ChangeInAction;}
		set{ _ChangeInAction = value;}
	}


	IEnumerator C_CalculateSurroundings()
	{
		while(gameObject.activeInHierarchy)
		{
			yield return new WaitForSeconds(AIUpdateDelay);

			PlayersPosition = GameController.control.GetPlayerGameObject();
			DistanceToPlayer = Vector2.Distance (PlayersPosition.transform.position, gameObject.transform.position);
			
			// Default Idle
			AI_Action = 0;

			if(!ObjectIsDead)			
			{
				// Passive / Patrol
				if(DistanceToPlayer >= PassiveRadius)
					AI_Action = 1;

				// Alert / Move Twords
				if(DistanceToPlayer > AttackRadius && DistanceToPlayer < PassiveRadius)
					AI_Action = 2;
				
				// Aggressive / Attack
				if(DistanceToPlayer <= AttackRadius)
					AI_Action = 3;
			}

			if(AI_Action != AI_Action_Previous)
			{
				ChangeInAction = true;
			}

			AI_Action_Previous = AI_Action;
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
				yield return StartCoroutine (C_IdleAction());
				break;
			case 1:
				yield return StartCoroutine (C_PassiveAction());
				break;
			case 2:
				yield return StartCoroutine (C_AlertAction ());
				break;
			case 3:
				yield return StartCoroutine (AggressiveAIAction());
				break;
			}

			ChangeInAction = false;
		}
		print ("EXIT");
		yield return null;
	}

	[AIAction]
	public IEnumerator C_Idle()
	{
		AIprint ("I AM JUST GOING TO IDLE");
		
		while(!ChangeInAction)
		{
			yield return new WaitForSeconds(0.5f);
		}
		
		AIprint ("I AM DONE BEING IDLE");
	}



	public void Flip ()
	{
		facingRight = !facingRight;
		
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;

//		if(HPModuleScript != null) HPModuleScript.FlipHPBar ();
		
		Canvas[] childCanvases = GetComponentsInChildren<Canvas> ();
		foreach(Canvas child in childCanvases)
		{
			theScale = child.transform.localScale;
			theScale.x *= -1;
			child.transform.localScale = theScale;
		}
	}

	public void FlipTowardsPlayer()
	{
		if(PlayersPosition.transform.position.x > gameObject.transform.position.x && !facingRight)
			Flip ();
		if(PlayersPosition.transform.position.x < gameObject.transform.position.x && facingRight)
			Flip ();
	}
	public void FlipAwayFromPlayer()
	{
		if(PlayersPosition.transform.position.x < gameObject.transform.position.x && !facingRight)
			Flip();
		if(PlayersPosition.transform.position.x > gameObject.transform.position.x && facingRight)
			Flip();
	}










	// Using reflection this function goes through this and all the inherited scripts and finds all functions that have the AIAction attribute
	// These are all the possible actions a certain unit can do.
	// Depending on the AInumber int values it will create a delegate and populate that action slot
	public List<MethodInfo> GetFunctionList()
	{
		MonoBehaviour thisScript = GetComponent<BaseAI> ();
		MethodInfo[] allMethods = thisScript.GetType ().GetMethods ();
		List<MethodInfo> TA_Methods = new List<MethodInfo> ();
		
		for (int i = 0; i < allMethods.Length; i++)
		{
			foreach(Attribute a in allMethods[i].GetCustomAttributes(false))
			{
				if(a is AIActionAttribute)
				{
					TA_Methods.Add(allMethods[i]);
				}
			}
		}
		return TA_Methods;
	}
	public List<string> GetFunctionList_String(List<MethodInfo> theMethodList)
	{
		List<string> tempStringList = new List<string> ();
		string temp;

		for (int i = 0; i < theMethodList.Count; i++)
		{
			temp = theMethodList[i].Name;
			temp = temp.Remove(0,2);
			tempStringList.Add (temp);
		}

		return tempStringList;
	}








}

/*
	// OLD but the basis for using reflection and I dont want to delete this for a while
	void TestThis()
	{
		// WORKS
		MonoBehaviour thisScript = GetComponent<BaseAI> ();
		MethodInfo[] allMethods = thisScript.GetType ().GetMethods ();

		List<MethodInfo> TA_Methods = new List<MethodInfo> ();

//		print ("I HAVE FOUND "+ allMethods.Length + " METHODS IN THE BASE AI SCRIPT");

		for (int i = 0; i < allMethods.Length; i++)
		{
			foreach(Attribute a in allMethods[i].GetCustomAttributes(false))
			{
				if(a is AIAction)
				{
//					print (allMethods[i]);
					TA_Methods.Add(allMethods[i]);
//					PassiveAIAction = (CustomAIAction)Delegate.CreateDelegate(typeof(CustomAIAction), this, allMethods[i]);
				}
			}
		}

		print ("-------------------------------------------------------------------");
//		print (string.Format("I have {0} methods with the TestAttribute tag", TA_Methods.Count));
		for (int i = 0; i < TA_Methods.Count; i++)
		{
			print (string.Format ("Method Index: {0}, Method Name: {1}",i,TA_Methods[i].Name));
		}
		print ("-------------------------------------------------------------------");

		if(PassiveAINumber < 0 || PassiveAINumber > TA_Methods.Count-1)
			PassiveAINumber = 0;
		PassiveAIAction 	= (CustomAIAction)Delegate.CreateDelegate(typeof(CustomAIAction), this, TA_Methods[PassiveAINumber]);
		AlertAIAction 		= (CustomAIAction)Delegate.CreateDelegate(typeof(CustomAIAction), this, TA_Methods[AlertAINumber]);
		AggressiveAIAction 	= (CustomAIAction)Delegate.CreateDelegate(typeof(CustomAIAction), this, TA_Methods[AggressiveAINumber]);
//		print (string.Format("Seems you want me to {0}",TA_Methods[PassiveAINumber]));
	}
*/