// RegalPrime 4-22-14 - ResetManager.cs

// Event Manager is automatically added when the gamecontroller is created
// There is no need to add this script yourself.

// This is an important script
// Other scripts that exist, connect to the event "resetObjects" via ResetManager.resetObjects += Reset;
// When the MasterReset function is activated, ALL other Reset() functions connected to this script will also activate
// This is how scripts reset themselves

// Most scripts have the Reset() ability added to them
// Take a look to see how they are setup

// A WORK IN PROGRESS
// I THINK THIS NEEDS TO BE REDONE AND MADE THE ONE STOP SHOP FOR EVENTS
// SINCE THIS SCRIPT IS ONLY ADDED TO THE GAMECONTROLLER THERE WILL ONLY BE ONE


// Quickly added in the ability for other scripts to see when the player has died
// Was used in the new avoidance boss so when the player died, the boss can stop its actions


using UnityEngine;
using System.Collections;
using System.IO;

public class EventManager : MonoBehaviour
{
//	private static EventManager _control;
//	public static EventManager control
//	{
//		get
//		{
//			if(_control == null)
//			{
////				print ("IM MAKING A NEW EVENT MANAGER");
//				GameObject temp = new GameObject();
//				temp.name = "Event Manager - AutoCreated";
//				_control = temp.AddComponent<EventManager>();
//			}
//			else
//				print ("ALREADY AN EVENT MANAGER");
//
//			return _control;
//		}
//
//		set
//		{
//			_control = value;
//		}
//	}
//	void Awake()
//	{
//		if (_control == null)				// If there are no control objects - this is the main one
//		{
//			_control = this;
//		}
//		else if(_control != this)
//		{
//			Destroy(this);					// If there is already a control object - delete the duplicate
//		}
//	}

	public static EventManager control;		// Reference to itself, so there is only one
	void Awake()
	{
		if (control == null)				// If there are no control objects - this is the main one
		{
			control = this;
		}
		else if(control != this)
		{
			Destroy(gameObject);			// If there is already a control object - delete the duplicate
		}
	}

	public static void CreateEventManagerIfNeeded()
	{
		if(control == null)
		{
			GameObject temp = new GameObject();
			temp.name = "Event Manager - AutoCreated";
			control = temp.AddComponent<EventManager>();
		}
	}


	// Used to tell others the player has died
	public delegate void PlayerDeathHandler (GameObject TEMP);
	public static event PlayerDeathHandler PlayerIsDead;
	public void OnPlayerDeath(GameObject ObjectThatKilledPlayer)
	{
		if(PlayerIsDead != null)
		{
			PlayerIsDead(ObjectThatKilledPlayer);
			print (string.Format ("OnPlayerDeath has triggered {0} event call(s)", PlayerIsDead.GetInvocationList().Length));
		}
		else
		{
			print (string.Format("OnPlayerDeath has triggered no event calls. Player Died by {0} at location {1}", ObjectThatKilledPlayer.name, ObjectThatKilledPlayer.transform.position));
		}
	}

	// Used to tell others to reset themselves
	public delegate void ResetObject();
	public static event ResetObject resetObjects;
	public void MasterReset()
	{
		if(resetObjects != null)
		{
			resetObjects();
			print (string.Format ("MasterReset has triggered {0} event call(s)", resetObjects.GetInvocationList().Length));
		}
		else
		{
			print ("MasterReset has triggered no event calls");
		}
	}

	public delegate void ButtonPressHandler ();
	public static event ButtonPressHandler jumpButton;

	public static event ButtonPressHandler attackButton;
	public static event ButtonPressHandler attackButton_Held;

	public static event ButtonPressHandler actionButton;

	public static event ButtonPressHandler resetButton;
	public static event ButtonPressHandler resetButton_Hard;
	public static event ButtonPressHandler resetGameButton;


	bool pressed_JumpButton = false;

	void FixedUpdate()
	{
		if(pressed_JumpButton)	OnButtonPressed_Jump ();
	}
	void Update()
	{
		if (Input.GetButtonDown("Jump") && !pressed_JumpButton)
			pressed_JumpButton = true;

		if(Input.GetKeyDown(KeyCode.X))
			OnButtonPressed_actionButton();

		if(Input.GetButtonDown ("Fire1"))
			OnButtonPressed_Shoot();

		if(Input.GetButton ("Fire1"))
			OnButtonPressed_Shoot_Held();

		if(Input.GetKeyDown(KeyCode.R))
			OnButtonPressed_ResetScreen();

		if(Input.GetKeyDown(KeyCode.Q))
			OnButtonPressed_HardResetScreen ();

		if (Input.GetKeyDown (KeyCode.F2))
			OnButtonPressed_ResetGame ();
	}

	void OnButtonPressed_Jump()
	{
//		print ("JUMPING");
		if(jumpButton != null) jumpButton();
		pressed_JumpButton = false;
	}

	void OnButtonPressed_actionButton()
	{
//		print ("ACTION BUTTON PRESSED");
		if(actionButton != null) actionButton();
	}

	void OnButtonPressed_Shoot()
	{
//		print ("SHOOTING");
		if(attackButton != null) attackButton();
	}
	void OnButtonPressed_Shoot_Held()
	{
//		print ("SHOOTING HELD");
		if(attackButton_Held != null) attackButton_Held();
	}
	void OnButtonPressed_ResetScreen()
	{
//		print ("RESET THE SCREEN");
		if(resetButton != null) resetButton();
	}
	void OnButtonPressed_HardResetScreen()
	{
//		print ("HARD RESET THE SCREEN");
		if(resetButton_Hard != null) resetButton_Hard();
	}
	void OnButtonPressed_ResetGame()
	{
//		print ("BACK TO THE TITLESCREEN");
		if(resetGameButton != null) resetGameButton();
	}



















}
