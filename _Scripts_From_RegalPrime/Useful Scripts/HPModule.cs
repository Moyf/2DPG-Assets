// RegalPrime 5-21-15 - HPModule.cs

// A simple module that gives HP to an object.
// Max hp is set via the editor
// Objects with the Bullet tag will activate this trigger and decrement the HP by one (available damageDelay)
// Sound and damage delay effects are available options (but not required)

// Object will be disabled upon reaching 0 HP

// currentHP is a public variable because other scripts may reference it (also allows to see the current HP in the editor during runtime)

// ShowHpAboveObject = true - Will display the hp above the object
// public GameObject HPBarPrefab; This is a simple HP bar

// An event On_HPModule_Death has been added so other scripts can attach to this script and be told when this module's HP reaches zero

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HPModule : MonoBehaviour
{
	public int maxHP = 10;							// How much HP does this object have
	public int currentHP = 10;						// This shows the current HP - cannot edit this

	public float damageDelay = 1f;					// How often the object can be damaged
	private bool ableToDamage = true;				// Used to prevent damage during damage delay

	public float DeathTimer;						// Delay between hitting 0 and disabling the object (sets the "IsDead" bool of the animator to show a death animation)


	public AudioClip hitSound;						// sound effect when a bullet hits the object
	public AudioClip deathSound;					// sound effect when the object dies
	public GameObject shieldEffect;					// effect that plays when a bullet tries to damage the object but cannot (due to the damage delay)



	public bool ShowHpAboveObject = false;			// Shows the current HP above the object
	public bool ShowHPOnlyWhenHit = false;			// Will only show the hp right after being hit
	public float HPBarDelay = 1f;					// This delay is how long the hp bar will stay active when using ShowHPOnlyWhenHit = true
	public float HPBarLerpSpeed = 1f;				// 0 = no lerp. Negative numbers will instantly reduce the HP bar. Positive numbers will change how fast the bar lerps
	public GameObject HPBarPrefab;					// The HP bar to be created

	private GameObject HPBarClone;					// Reference of the HP bar
	private Slider HitPointBar;						// Slider on the HP bar - used to change the HP
	private float SliderbarOffset = 0;				// Used in calculating location of the HP bar
	private float ColliderWidth = 0;				// Used in calculating location of the HP bar



	private int CurrentLayerIndex;					// Backup of the starting layer index (used when resetting the object)
	private int IgnorePlayerLayerIndex;				// Index used to ignore the player (used when object dies)

	Animator animator;


	// Other scripts using the HP module can connect to this event to know when it has run out of HP (like a monster)
	public delegate void ObjectsDeathHandler();
	public event ObjectsDeathHandler On_HPModule_Death;
	public void HPModule_Death()
	{
		if(On_HPModule_Death != null)
		{
			On_HPModule_Death();
//			print (string.Format ("On_HPModule_Death - ANOTHER SCRIPT HAS BEEN ALERTED - No HP"));
		}
//		else
//			print ("On_HPModule_Death - NOBODY CARES THAT I DIED");
	}


	void Awake()
	{
		EventManager.resetObjects += Reset;
		CurrentLayerIndex = gameObject.layer;
		IgnorePlayerLayerIndex = LayerMask.NameToLayer("IgnorePlayer");

//		mainCamera = Camera.main;
		animator = gameObject.GetComponent<Animator> ();

		if(GetComponent<CircleCollider2D> ())
		{
			SliderbarOffset = GetComponent<CircleCollider2D> ().radius * gameObject.transform.localScale.y;
			ColliderWidth = GetComponent<CircleCollider2D> ().radius * gameObject.transform.localScale.x;
		}
		if(GetComponent<BoxCollider2D> ())
		{
			SliderbarOffset = GetComponent<BoxCollider2D> ().size.y/2 * gameObject.transform.localScale.y;;
			ColliderWidth = GetComponent<BoxCollider2D> ().size.x/2 * gameObject.transform.localScale.x;
		}

		if (HPBarPrefab == null)
			ShowHpAboveObject = false;


		if (ShowHpAboveObject && HPBarPrefab.activeInHierarchy)			// If a hp bar exists on the screen, use it
		{
			HPBarClone = HPBarPrefab;
			HitPointBar = HPBarClone.GetComponentInChildren<Slider> ();
		}
		else 										// else create one from the prefab
		if(ShowHpAboveObject)
		{
			HPBarClone = Instantiate (HPBarPrefab, gameObject.transform.position+(new Vector3(0,SliderbarOffset*2,0)), Quaternion.identity) as GameObject;
			HPBarClone.transform.SetParent (gameObject.transform);

			RectTransform cloneRectTransform = HPBarClone.transform as RectTransform;
			cloneRectTransform.sizeDelta = new Vector2(ColliderWidth*200, cloneRectTransform.sizeDelta.y);

			HitPointBar = HPBarClone.GetComponentInChildren<Slider> ();
//			HitPointBar.wholeNumbers = true;

			HitPointBar.gameObject.SetActive (gameObject.activeInHierarchy);
		}
	}

	void OnEnable()
	{
		currentHP = maxHP;

		if(ShowHpAboveObject)
		{
			HitPointBar.gameObject.SetActive (true);
			HitPointBar.maxValue = currentHP;
			HitPointBar.value = currentHP;

			if(ShowHPOnlyWhenHit)
				HPBarClone.SetActive (false);
			else
				HPBarClone.SetActive (true);
		}

		ableToDamage = true;
		gameObject.layer = CurrentLayerIndex;

		if(animator != null)
			animator.SetBool("IsDead", false);
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void Reset()
	{
		StopAllCoroutines ();
		gameObject.layer = CurrentLayerIndex;
		currentHP = maxHP;
		ableToDamage = true;
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Bullet" && currentHP > 0)			// A bullet hit me, -1 HP and play effects
		{
			if(ableToDamage)
			{
				StartCoroutine (C_DealDamage());
			}
			else
			{
				if(shieldEffect != null)
					Instantiate(shieldEffect, other.transform.position, Quaternion.identity);
			}

			other.gameObject.SetActive (false);
		}
	}

	IEnumerator C_DealDamage()
	{
		currentHP--;
		ableToDamage = false;
		StartCoroutine("ResetTrigger_DamageDelay");
		
		if(hitSound != null)
			MusicController.control.OneShotAudio(hitSound);
		
		if(ShowHpAboveObject)
		{
			StopCoroutine ("C_HPChange_Lerp");
			yield return StartCoroutine ("C_HPChange_Lerp", currentHP);
		}
		
		if(currentHP <= 0)
		{
			if(deathSound != null)
				MusicController.control.OneShotAudio(deathSound);
			
			if(ShowHpAboveObject)
				HitPointBar.gameObject.SetActive(false);
			
			if(DeathTimer > 0)
			{
				animator.SetBool("IsDead", true);
				StartCoroutine (C_PlayAndDie());
			}
			else
				gameObject.SetActive (false);

			HPModule_Death();	// Tell others that this object has died
		}

		yield return null;
	}


	IEnumerator C_HPChange_Lerp(int newHPValue)
	{
		float startingValue = HitPointBar.value;
		float endingValue = newHPValue;
		float Duration = 0;

		if(ShowHPOnlyWhenHit)
			HPBarClone.SetActive (true);

		while(Duration < 1 && HPBarLerpSpeed != 0)
		{
			HitPointBar.value = Mathf.Lerp(startingValue, endingValue, Duration);

			if(HPBarLerpSpeed > 0)
				Duration += Time.deltaTime * HPBarLerpSpeed;
			else
				Duration += Time.deltaTime * (1/damageDelay);

			yield return null;
		}
		HitPointBar.value = newHPValue;

		if(ShowHPOnlyWhenHit && newHPValue != 0)
		{
			yield return new WaitForSeconds(HPBarDelay);
			HPBarClone.SetActive (false);
		}

		yield return null;
	}

	IEnumerator C_PlayAndDie()
	{
		gameObject.layer = IgnorePlayerLayerIndex;
		yield return new WaitForSeconds (DeathTimer);
		gameObject.SetActive (false);
		
		//		Destroy (gameObject);
	}
	public IEnumerator ResetTrigger_DamageDelay()
	{
		yield return new WaitForSeconds(damageDelay);
		ableToDamage = true;
	}

	public void FlipHPBar()
	{
		if(HPBarClone != null)
		{
			Vector3 theScale = HPBarClone.transform.localScale;
			theScale.x *= -1;
			HPBarClone.transform.localScale = theScale;
		}
	}
}

/*
// RegalPrime 11-25-14 - HPModule.cs

// A simple module that gives HP to an object.
// Max hp is set via the editor
// Objects with the Bullet tag will activate this trigger and decrement the HP by one (available damageDelay)
// Sound and damage delay effects are available options (but not required)

// Object will be disabled upon reaching 0 HP

// currentHP is a public variable because other scripts may reference it (also allows to see the current HP in the editor during runtime)
// This is probably not very smart and I should make it a function, but I know somewhere down the road I will have to rewrite this script.

// ShowHpAboveObject = true - Will display the hp above the object
// Be aware this is very simplisticly done and needs to be update later

// public GameObject DisplayLabel;
// This is an object with a BasicLabel script attached to it.
// This also a styleGuide which is put on by the BasicLabel script
// For the most part, dont change what object is put at this location


// Resetting this script via the Reset() - This is used when reloading a screen
// does not remove the shield effects. I might put that code in later, or just wait for when I rewrite this script.


using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HPModule : MonoBehaviour
{
	public int maxHP = 10;							// How much HP does this object have
	public int currentHP = 10;						// This shows the current HP - cannot edit this

	public float damageDelay = 1f;					// How often the object can be damaged
	private bool ableToDamage = true;				// Used to prevent damage during damage delay

	public bool ShowHpAboveObject = false;			// Shows the current HP above the object in text form
	public float DeathTimer;						// Delay between hitting 0 and disabling the object (sets the "IsDead" bool of the animator to show a death animation)

	public AudioClip hitSound;						// sound effect when a bullet hits the object
	public AudioClip deathSound;					// sound effect when the object dies
	public GameObject shieldEffect;					// effect that plays when a bullet tries to damage the object but cannot (due to the damage delay)

	// Information used when displaying the hit points
	public GameObject DisplayLabel;					// The basic label used when displaying text
	public float OffsetOfLabel_Y = 0.5f;
	private BasicLabel basiclabel;
	private Vector2 pixelOffset;
	private float labelOffset_X;
	private float labelOffset_Y;

	private int CurrentLayerIndex;					// Backup of the starting layer index (used when resetting the object)
	private int IgnorePlayerLayerIndex;				// Index used to ignore the player (used when object dies)

	Animator animator;

//	private Camera mainCamera;

	void Awake()
	{
		ResetManager.resetObjects += Reset;
		CurrentLayerIndex = gameObject.layer;
		IgnorePlayerLayerIndex = LayerMask.NameToLayer("Ignore Player");

//		mainCamera = Camera.main;

		animator = gameObject.GetComponent<Animator> ();
		
		if(ShowHpAboveObject)
		{
			GameObject TEMP;
			TEMP = Instantiate(DisplayLabel, gameObject.transform.position, Quaternion.identity) as GameObject;
			TEMP.transform.parent = this.transform;
			TEMP.transform.localPosition += new Vector3 (0f, OffsetOfLabel_Y, 0);
			basiclabel = TEMP.GetComponent<BasicLabel> ();
			basiclabel.styleGuide.fontSize = 9;
		}
	}

	void OnEnable()
	{
		currentHP = maxHP;
		ableToDamage = true;
		gameObject.layer = CurrentLayerIndex;

		if(animator != null)
			animator.SetBool("IsDead", false);
	}
	void OnDestroy()
	{
		ResetManager.resetObjects -= Reset;
	}
	void Reset()
	{
		StopAllCoroutines ();
		gameObject.layer = CurrentLayerIndex;
		currentHP = maxHP;
		ableToDamage = true;
	}


	void FixedUpdate()
	{
		if(ShowHpAboveObject)
			basiclabel.TextToDisplay = "Hit Points: "+currentHP;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Bullet" && currentHP > 0)			// A bullet hit me, -1 HP and play effects
		{
			if(ableToDamage)
			{
				currentHP--;
				ableToDamage = false;
				StartCoroutine("ResetTrigger_DamageDelay");

				if(hitSound != null)
					MusicController.control.OneShotAudio(hitSound);

				if(currentHP <= 0)
				{
					if(deathSound != null)
						MusicController.control.OneShotAudio(deathSound);

					if(DeathTimer > 0)
					{
						animator.SetBool("IsDead", true);
						StartCoroutine (C_PlayAndDie());
					}
					else
						gameObject.SetActive (false);
				}
			}
			else
			{
				if(shieldEffect != null)
					Instantiate(shieldEffect, other.transform.position, Quaternion.identity);
			}

			other.gameObject.SetActive (false);
//			Destroy(other.gameObject);						// Destroy bullet
		}
	}

	IEnumerator C_PlayAndDie()
	{
		gameObject.layer = IgnorePlayerLayerIndex;
		yield return new WaitForSeconds (DeathTimer);
		gameObject.SetActive (false);
		
		//		Destroy (gameObject);
	}
	public IEnumerator ResetTrigger_DamageDelay()
	{
		yield return new WaitForSeconds(damageDelay);
		ableToDamage = true;
	}
}
*/