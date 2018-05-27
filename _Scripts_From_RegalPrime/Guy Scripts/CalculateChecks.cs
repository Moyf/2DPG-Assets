// RegalPrime 1-21-15 - CalculateChecks.cs

// This script is used to calculate the distance between two colliders (only 2d box collider)
// It checks by creating a area collider check at a certain ground / ceiling / wall position (based on collider size / etc)
// This takes into consideration size / scale / offsets of this box collider and ones it bumps into (hopefully things are working right)

// Due to the nature of edge colliders, this scripts sets AllowPreCheck = false to disable using this information to change movement
// If an edge collider if found to be the ground object, a raycast on either edge of the object is sent out to calculate the correct distance
// Certain jaggy edge colliders might create inaccurate results (might need another raycast in the middle if things are not accurate enough)

// This script can be used on other objects and is used on the AI controlled objects as well
// Also used in PlatformerCharacter2D scripts

// This objects stats are calculated on runtime and wont be updated unless you tell it to via function
// It seems your character wont be changing size and scale a lot during the game and its just making less calculations per call
// If the need arises to have the stats polled every frame, just add the recheck function to each main function call

// Added ColliderIndent variable.
// Normally the AoE checks would be based on this objects collider size. Increasing the ColliderIndent will shrink the edges by that amount
// This can be used to prevent false positives. Anyways, it is in there in case the need arises.


using UnityEngine;
using System.Collections;

public class CalculateChecks : MonoBehaviour
{
	public LayerMask whatIsGround;						// A mask determining what is ground to the character

	public float DistanceToGround = 1;					// Calculated distance to the ground
	public float DistanceToWall = 1;					// Calculated distance to the wall
	public float DistanceToCeiling = 1;					// Calculated distance to the ceiling
	public float DistanceToPit = 1;						// Calculated distance to the pit

	private float ColliderWidth;						// Gameobjects collider stats
	private float ColliderHeight;						// Gameobjects collider stats
	private float ColliderOffsetX;						// Gameobjects collider stats
	private float ColliderOffsetY;						// Gameobjects collider stats

	private float ColliderIndent = 0.009f;				// The slight indent of AoE checks to prevent false positives (if needed)

	private Vector2 TopLeft;							// Top left position of the area check
	private Vector2 BottomRight;						// Bottom right position of the area check

	private PlatformerCharacter2D CharacterScript;

	private bool BackwardsSprite_X = false;

	void Start()
	{
		// Shape of the players collider
		BoxCollider2D Poly = gameObject.GetComponent<BoxCollider2D> ();
		ColliderWidth = Poly.bounds.size.x;
		ColliderHeight = Poly.bounds.size.y;
		ColliderOffsetX = Poly.offset.x * Mathf.Abs(gameObject.transform.localScale.x);
		ColliderOffsetY = Poly.offset.y * Mathf.Abs(gameObject.transform.localScale.y);

		if(GetComponent<PlatformerCharacter2D> ())
			CharacterScript = GetComponent<PlatformerCharacter2D> ();
	}

	public void ChangeLayerMask(LayerMask newLayerMask)
	{
		whatIsGround = newLayerMask;
	}
	public void Recheck_ThisObjectsStats()
	{
		BoxCollider2D Poly = gameObject.GetComponent<BoxCollider2D> ();
		ColliderWidth = Poly.bounds.size.x;
		ColliderHeight = Poly.bounds.size.y;
		ColliderOffsetX = Poly.offset.x * Mathf.Abs(gameObject.transform.localScale.x);
		ColliderOffsetY = Poly.offset.y * Mathf.Abs(gameObject.transform.localScale.y);
	}
	public void Set_ColliderIndent(float new_Indent)
	{
		ColliderIndent = new_Indent;
	}
	public void Set_BackwardsSprite_X(bool flipit)
	{
		BackwardsSprite_X = flipit;
	}

	// Check below the character and calculate the distance to the ground (relative to Y scale)
	// If not in range then distance to ground = 1
	public float GroundCheck(bool isUpright)
	{
		Collider2D GroundObject = null;

		float X_Flip = gameObject.transform.localScale.x / Mathf.Abs (gameObject.transform.localScale.x);
		float Y_Flip = gameObject.transform.localScale.y / Mathf.Abs (gameObject.transform.localScale.y);

		TopLeft.x = gameObject.transform.position.x - ColliderWidth/2 + ColliderOffsetX*X_Flip + ColliderIndent;
		TopLeft.y = gameObject.transform.position.y - ColliderHeight/2*Y_Flip + ColliderOffsetY*Y_Flip + 0.05f*Y_Flip;
		BottomRight.x = gameObject.transform.position.x + ColliderWidth/2 + ColliderOffsetX*X_Flip - ColliderIndent;
		BottomRight.y = gameObject.transform.position.y - ColliderHeight/2*Y_Flip + ColliderOffsetY*Y_Flip - 0.30f*Y_Flip;

		GroundObject = Physics2D.OverlapArea (TopLeft, BottomRight, whatIsGround);

//		print ("("+TopLeft.x + ", " + TopLeft.y + ")   (" + BottomRight.x + ", " + BottomRight.y+")");

		if(GroundObject != null)
		{
			if(GroundObject.GetComponent<BoxCollider2D>() && GroundObject.transform.rotation == Quaternion.identity)
			{
				float objectOffset = 0;
				objectOffset = GroundObject.GetComponent<BoxCollider2D>().offset.y;
				objectOffset *= GroundObject.transform.localScale.y;

				DistanceToGround = (Mathf.Abs(gameObject.transform.position.y - GroundObject.gameObject.transform.position.y + ColliderOffsetY*Y_Flip - objectOffset) - ColliderHeight/2 - GroundObject.bounds.size.y/2);

				if(CharacterScript)
					CharacterScript.AllowPreCheck_Ground = true;
			}
			else
				// This uses a raycast check from both sides of the character pointing downwards
				// If either are grounded, then the character is grounded
//			if(GroundObject.GetComponent<EdgeCollider2D>() || GroundObject.GetComponent<CircleCollider2D>())	// I found something but its not a box collider. Use raycasting
			{
				Vector3 TEMP = transform.position + new Vector3(ColliderWidth/2 + ColliderOffsetX*X_Flip - ColliderIndent, ColliderOffsetY*Y_Flip, 0);
				Vector3 TEMP2 = transform.position + new Vector3(-ColliderWidth/2 + ColliderOffsetX*X_Flip + ColliderIndent, ColliderOffsetY*Y_Flip, 0);

				DistanceToGround = 1;
				float DistanceToGround2 = 1;
				
				RaycastHit2D hit = Physics2D.Raycast(TEMP, -Vector2.up*Y_Flip, 0.5f, whatIsGround);
				RaycastHit2D hit2 = Physics2D.Raycast(TEMP2, -Vector2.up*Y_Flip, 0.5f, whatIsGround);

				if(hit.collider != null)
				{
					DistanceToGround  = (Mathf.Abs (hit.point.y - transform.position.y - ColliderOffsetY*Y_Flip) - ColliderHeight / 2);
				}
				if(hit2.collider != null)
				{
					DistanceToGround2 = (Mathf.Abs (hit2.point.y - transform.position.y - ColliderOffsetY*Y_Flip) - ColliderHeight / 2);
				}

				if(Mathf.Abs(DistanceToGround) > Mathf.Abs (DistanceToGround2))
					DistanceToGround = DistanceToGround2;

				if(CharacterScript)
					CharacterScript.AllowPreCheck_Ground = false;
			}
		}
		else
		{
			DistanceToGround = 1;
		}

		return DistanceToGround;
	}

	// Check below the character and calculate the distance to the ceiling (relative to Y scale)
	// If not in range then distance to ceiling = 1
	public float CeilingCheck(bool isUpright)
	{
		Collider2D CeilingObject = null;

		float X_Flip = gameObject.transform.localScale.x / Mathf.Abs (gameObject.transform.localScale.x);
		float Y_Flip = gameObject.transform.localScale.y / Mathf.Abs (gameObject.transform.localScale.y);

		TopLeft.x = gameObject.transform.position.x - ColliderWidth/2 + ColliderOffsetX*X_Flip + ColliderIndent;
		TopLeft.y = gameObject.transform.position.y + ColliderHeight/2*Y_Flip + ColliderOffsetY*Y_Flip - 0.02f*Y_Flip;
		BottomRight.x = gameObject.transform.position.x + ColliderWidth/2 + ColliderOffsetX*X_Flip - ColliderIndent;
		BottomRight.y = gameObject.transform.position.y + ColliderHeight/2*Y_Flip + ColliderOffsetY*Y_Flip + 0.15f*Y_Flip;

		CeilingObject = Physics2D.OverlapArea (TopLeft, BottomRight, whatIsGround);

		if(CeilingObject != null)
		{
			if(CeilingObject.GetComponent<BoxCollider2D>() && CeilingObject.transform.rotation == Quaternion.identity)
			{
				float objectOffset = 0;
				objectOffset = CeilingObject.GetComponent<BoxCollider2D>().offset.y;
				objectOffset *= CeilingObject.transform.localScale.y;

				DistanceToCeiling = (Mathf.Abs(gameObject.transform.position.y - CeilingObject.gameObject.transform.position.y + ColliderOffsetY*Y_Flip - objectOffset) - ColliderHeight/2 - CeilingObject.bounds.size.y/2);

				if(CharacterScript)
					CharacterScript.AllowPreCheck_Ceiling = true;
			}
			else
//			if(CeilingObject.GetComponent<EdgeCollider2D>() || CeilingObject.GetComponent<CircleCollider2D>())	// I found something but its not a box collider. Use raycasting
			{
				Vector3 TEMP = transform.position + new Vector3(ColliderWidth/2 + ColliderOffsetX*X_Flip - ColliderIndent, ColliderOffsetY*Y_Flip, 0);
				Vector3 TEMP2 = transform.position + new Vector3(-ColliderWidth/2 + ColliderOffsetX*X_Flip + ColliderIndent, ColliderOffsetY*Y_Flip, 0);
//				print ("("+TEMP.x + ", " + TEMP.y + "   " + TEMP2.x + ", " + TEMP2.y+")");

				DistanceToCeiling = 1;
				float DistanceToCeiling2 = 1;
				
				RaycastHit2D hit = Physics2D.Raycast(TEMP, Vector2.up*Y_Flip, 0.5f, whatIsGround);
				RaycastHit2D hit2 = Physics2D.Raycast(TEMP2, Vector2.up*Y_Flip, 0.5f, whatIsGround);
				
				if(hit.collider != null)
				{
					DistanceToCeiling  = (Mathf.Abs (hit.point.y - transform.position.y - ColliderOffsetY*Y_Flip) - ColliderHeight / 2);
				}
				if(hit2.collider != null)
				{
					DistanceToCeiling2 = (Mathf.Abs (hit2.point.y - transform.position.y - ColliderOffsetY*Y_Flip) - ColliderHeight / 2);
				}
				
				if(Mathf.Abs(DistanceToCeiling) > Mathf.Abs (DistanceToCeiling2))
					DistanceToCeiling = DistanceToCeiling2;

				if(CharacterScript)
					CharacterScript.AllowPreCheck_Ceiling = false;
			}
		}
		else
		{
			DistanceToCeiling = 1;
		}

		return DistanceToCeiling;
	}

	// Check below the character and calculate the distance to the wall (relative to x scale)
	// If not in range then distance to wall = 1
	public float WallCheck(bool isUpright, bool facingRight)
	{
		Collider2D WallObject = null;

		float X_Flip = gameObject.transform.localScale.x / Mathf.Abs (gameObject.transform.localScale.x);
		float Y_Flip = gameObject.transform.localScale.y / Mathf.Abs (gameObject.transform.localScale.y);

		if(BackwardsSprite_X)
			X_Flip *= -1;

		TopLeft.x = gameObject.transform.position.x + ColliderWidth/2*X_Flip + ColliderOffsetX*X_Flip - 0.05f*X_Flip;
		TopLeft.y = gameObject.transform.position.y + ColliderHeight/2 + ColliderOffsetY*Y_Flip - ColliderIndent;
		BottomRight.x = gameObject.transform.position.x + ColliderWidth/2*X_Flip + ColliderOffsetX*X_Flip + 0.10f*X_Flip;
		BottomRight.y = gameObject.transform.position.y - ColliderHeight / 2 + ColliderOffsetY*Y_Flip + ColliderIndent;

		WallObject = Physics2D.OverlapArea (TopLeft, BottomRight, whatIsGround);

//		print ("("+TopLeft.x + ", " + TopLeft.y + ")   (" + BottomRight.x + ", " + BottomRight.y+")");

		if(WallObject != null)
		{
			if(WallObject.GetComponent<BoxCollider2D>() && WallObject.transform.rotation == Quaternion.identity)
			{
				float objectOffset = 0;
				objectOffset = WallObject.GetComponent<BoxCollider2D>().offset.x;
				objectOffset *= WallObject.transform.localScale.x;

				DistanceToWall = (Mathf.Abs(gameObject.transform.position.x - WallObject.gameObject.transform.position.x + ColliderOffsetX*X_Flip - objectOffset) - ColliderWidth/2 - WallObject.bounds.size.x/2);

				if(CharacterScript)
					CharacterScript.AllowPreCheck_Wall = true;
			}
			else
//			if(WallObject.GetComponent<EdgeCollider2D>() || WallObject.GetComponent<CircleCollider2D>())	// I found something but its not a box collider. Use raycasting
			{
				Vector3 TEMP = transform.position + new Vector3(ColliderOffsetX*X_Flip, ColliderHeight/2 + ColliderOffsetY*Y_Flip - ColliderIndent, 0);
				Vector3 TEMP2 = transform.position + new Vector3(ColliderOffsetX*X_Flip, -ColliderHeight/2 + ColliderOffsetY*Y_Flip + ColliderIndent, 0);

//				print ("("+TEMP.x + ", " + TEMP.y + "   " + TEMP2.x + ", " + TEMP2.y+")");

				DistanceToWall = 1;
				float DistanceToWall2 = 1;
				
				RaycastHit2D hit = Physics2D.Raycast(TEMP, Vector2.right*X_Flip, 0.5f, whatIsGround);
				RaycastHit2D hit2 = Physics2D.Raycast(TEMP2, Vector2.right*X_Flip, 0.5f, whatIsGround);
				
				if(hit.collider != null)
				{
					DistanceToWall  = (Mathf.Abs (hit.point.x - transform.position.x - ColliderOffsetX*X_Flip) - ColliderWidth / 2);
				}
				if(hit2.collider != null)
				{
					DistanceToWall2 = (Mathf.Abs (hit2.point.x - transform.position.x - ColliderOffsetX*X_Flip) - ColliderWidth / 2);
				}

//				print (DistanceToWall + "  " + DistanceToWall2);

				if(Mathf.Abs(DistanceToWall) > Mathf.Abs (DistanceToWall2))
					DistanceToWall = DistanceToWall2;

				if(CharacterScript)
					CharacterScript.AllowPreCheck_Wall = false;
			}
		}
		else
		{
			DistanceToWall = 1;
		}

		return DistanceToWall;
	}

	// Checks for ground in front and below the character (relative to x/y scale)
	// Used mostly to detect pits for the AI script when it is patrolling
	public float PitCheck(bool isUpright, bool facingRight)
	{
		Collider2D PitObject = null;

		float X_Flip = gameObject.transform.localScale.x / Mathf.Abs (gameObject.transform.localScale.x);
		float Y_Flip = gameObject.transform.localScale.y / Mathf.Abs (gameObject.transform.localScale.y);

		if(BackwardsSprite_X)
			X_Flip *= -1;

		TopLeft.x = gameObject.transform.position.x + ColliderWidth/2*X_Flip + ColliderOffsetX*X_Flip;
		TopLeft.y = gameObject.transform.position.y - ColliderHeight/2*Y_Flip + ColliderOffsetY*Y_Flip + 0.01f*Y_Flip;
		BottomRight.x = gameObject.transform.position.x + ColliderWidth*X_Flip + ColliderOffsetX*X_Flip;
		BottomRight.y = gameObject.transform.position.y - ColliderHeight/2*Y_Flip + ColliderOffsetY*Y_Flip - 0.16f*Y_Flip;

		PitObject = Physics2D.OverlapArea (TopLeft, BottomRight, whatIsGround);

//		print ("("+TopLeft.x + ", " + TopLeft.y + ")   (" + BottomRight.x + ", " + BottomRight.y+")");

		if(PitObject != null)
		{
			if(PitObject.GetComponent<BoxCollider2D>() && PitObject.transform.rotation == Quaternion.identity)
			{
				float objectOffset = 0;
				objectOffset = PitObject.GetComponent<BoxCollider2D>().offset.y;
				objectOffset *= PitObject.transform.localScale.y;

				DistanceToPit = (Mathf.Abs(gameObject.transform.position.y - PitObject.gameObject.transform.position.y + ColliderOffsetY*Y_Flip - objectOffset) - ColliderHeight/2 - PitObject.bounds.size.y/2);
			}
			else
				//			if(PitObject.GetComponent<EdgeCollider2D>() || PitObject.GetComponent<CircleCollider2D>())	// NOT TESTED SO IT MIGHT BE TOTALLY BROKEN - FIX LATER // I found something but its not a box collider. Use raycasting
			{
				Vector3 TEMP = transform.position + new Vector3(ColliderWidth/2*X_Flip + ColliderOffsetX*X_Flip - ColliderIndent, ColliderOffsetY*Y_Flip, 0);
				Vector3 TEMP2 = transform.position + new Vector3(ColliderWidth*1.5f*X_Flip + ColliderOffsetX*X_Flip + ColliderIndent, ColliderOffsetY*Y_Flip, 0);

				DistanceToPit = 1;
				float DistanceToPit2 = 1;
				
				RaycastHit2D hit = Physics2D.Raycast(TEMP, -Vector2.up*Y_Flip, 0.5f, whatIsGround);
				RaycastHit2D hit2 = Physics2D.Raycast(TEMP2, -Vector2.up*Y_Flip, 0.5f, whatIsGround);
				
				if(hit.collider != null)
				{
					DistanceToPit   = (Mathf.Abs (hit.point.y - transform.position.y - ColliderOffsetY*Y_Flip) - ColliderHeight / 2);
				}
				if(hit2.collider != null)
				{
					DistanceToPit2 = (Mathf.Abs (hit2.point.y - transform.position.y - ColliderOffsetY*Y_Flip) - ColliderHeight / 2);
				}

				if(Mathf.Abs(DistanceToPit) > Mathf.Abs (DistanceToPit2))
					DistanceToPit = DistanceToPit2;
			}
		}
		else
		{
			DistanceToPit = 1;
		}

		return DistanceToPit;
	}


	// Overridden functions that take NO parameters
	// None of the functions use them anways. This is just a stopgap between me changing all the ingame references and writting new code that wont break later.
	public float GroundCheck()
	{
		return GroundCheck (true);
	}
	public float CeilingCheck()
	{
		return CeilingCheck (true);
	}
	public float WallCheck()
	{
		return WallCheck (true, true);
	}
	public float PitCheck()
	{
		return PitCheck (true, true);
	}

}