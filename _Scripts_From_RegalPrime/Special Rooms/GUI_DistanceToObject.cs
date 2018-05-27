// Simple GUI to display the distance to wall / ceiling / ground on the screen
// Mostly for testing

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI_DistanceToObject : MonoBehaviour
{
	public Text GroundCheck_Distance;
	public Text WallCheck_Distance;
	public Text CeilingCheck_Distance;
	public Text TypeOfPlayer;

	private GameObject PlayerObject;
	private PlatformerCharacter2D PlayerScript;


	void Start()
	{
		StartCoroutine (DelayStart ());
	}

	IEnumerator DelayStart()
	{
		yield return new WaitForSeconds (1);
		PlayerObject = GameController.control.GuyLocation;
		
		if(PlayerObject.GetComponent<PlatformerCharacter2D>())
			PlayerScript = PlayerObject.GetComponent<PlatformerCharacter2D>();
		
	}

	void FixedUpdate ()
	{
		if(PlayerScript != null)
		{
			GroundCheck_Distance.text = ""+SetDecimal(PlayerScript.distanceToGround);
			WallCheck_Distance.text = ""+SetDecimal(PlayerScript.distanceToWall);
			CeilingCheck_Distance.text = ""+SetDecimal(PlayerScript.distanceToCeiling);
			TypeOfPlayer.text = ""+PlayerScript.name;
		}


	}


	float SetDecimal(float ANumber)
	{
		ANumber = (int)(ANumber * 100000);
		return ANumber / 1000;
	}

}
