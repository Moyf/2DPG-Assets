// RegalPrime 12-19-14 - SavePoint.cs

// Save the games data when this script is triggered
// Information is saved via the GameController script (SaveFile function)

// By default, save points will be triggered by both the Player and Bullet tagged objects
// noBullets = true - To disable bullet triggering (this will create the noBulletShield gameobect to show you cannot shoot it)
// pressStoSave = true - Will disable bullet triggering and only allow the player to save when pressing S while in the trigger
// There is a very simple prefab that appears when on the object, telling the player to press S to save.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SavePoint : MonoBehaviour
{
	public float saveBuffer = 1f;		// Speed at which you can save
	public bool noBullets = false;		// Bullets wont trigger the save

	public bool pressStoSave = false;	// Only allows to save if touching and S is pressed (player / bullets wont work)
	public GameObject pressStoSave_Prefab;
	private GameObject pressStoSave_Clone;

	public GameObject saveEffect;		// Added effect when saving the game
	public GameObject noBulletShield;	// Graphical representation that you cannot shoot the save
	public AudioClip  saveSound;		// Added effect when saving the game

	public bool SetNewCameraDefaultPosition = false;	// This will set the current camera location as the new default camera position (this is useful in multi screen / camera movement scenes)

	private bool canSave = true;				// This is used in the delay buffer
	private bool abletosave = false;			// This is used in press S to save
	private GameObject effectClone;
	Animator anim;


	void Awake ()
	{
		EventManager.resetObjects += Reset;
		anim = GetComponent<Animator> ();

		if(noBullets && noBulletShield!=null)	// Places a prefab at the save location to tell the player they cant shoot the save
		{
			GameObject TEMP = Instantiate(noBulletShield, transform.position, Quaternion.identity) as GameObject;
			TEMP.transform.parent = this.transform;
			TEMP.GetComponent<SpriteRenderer>().material = gameObject.GetComponent<SpriteRenderer>().material;
		}

		if(pressStoSave)
		{
			RectTransform TEST = pressStoSave_Prefab.transform as RectTransform;
			float X = gameObject.transform.position.x;
			float Y = gameObject.transform.position.y + TEST.sizeDelta.y/2/100;
			float Z = gameObject.transform.position.z + 1;

			pressStoSave_Clone = Instantiate(pressStoSave_Prefab, new Vector3(X, Y, Z), Quaternion.identity) as GameObject;
			pressStoSave_Clone.transform.SetParent(this.transform);
			pressStoSave_Clone.SetActive (false);
		}

	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		if(effectClone != null)
			Destroy(effectClone);

		canSave = true;

		if(pressStoSave)
			pressStoSave_Clone.SetActive (false);

		gameObject.GetComponent<BoxCollider2D> ().enabled = false;
		gameObject.GetComponent<BoxCollider2D> ().enabled = true;
	}
	void OnEnable()
	{
		anim.SetBool ("IsActive", false);
	}
	

	void OnTriggerEnter2D(Collider2D other)		// Saves if able when collider is hit
	{
		if(canSave)
		{
			if(other.gameObject.tag == ("Player") || (other.gameObject.tag == ("Bullet")&&noBullets==false))
			{
				if(pressStoSave==true)
				{
					pressStoSave_Clone.SetActive (true);
					abletosave=true;
				}
				else
					Saving();
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if(Input.GetKeyDown(KeyCode.S)&&abletosave&&canSave)	// If player is in the collider and able to save
		{
			Saving();
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.tag == ("Player") && pressStoSave)
		{
			pressStoSave_Clone.SetActive(false);
			abletosave=false;
		}
	}



	void Saving()
	{
		GameController.control.Save (GameController.control.SaveFile);			// Saves the game through the gamecontroller
		anim.SetBool ("IsActive", true);										// Sets the animation

		if(SetNewCameraDefaultPosition)
			CameraController.control.SetNewCameraResetPosition();

		if(saveEffect != null)													// If a save effect - play it
			effectClone = Instantiate(saveEffect, transform.position, Quaternion.identity) as GameObject;
		
		if(saveSound != null)													// If a sound effect - play it
			MusicController.control.OneShotAudio(saveSound);

		canSave = false;														// Unable to save until ResetSave function resets the bool
		StartCoroutine (ResetSave ());
	}

	IEnumerator ResetSave()
	{
		yield return new WaitForSeconds (saveBuffer);
		canSave = true;
		anim.SetBool ("IsActive", false);
	}
}