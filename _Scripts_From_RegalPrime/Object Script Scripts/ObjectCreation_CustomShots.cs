// RegalPrime 5-22-15 - ObjectCreationExtras.cs

// This is a place to put custom shots for use mainly in the AI
// The shooting AI use this class


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectCreation_CustomShots : ObjectCreation
{

	// A test to see how making a custom shot would work for a monster AI
	IEnumerator C_CustomShot_ChargeShot()
	{
		GameObject newclone = CreateClone (false);
		List<GameObject> singleList = new List<GameObject> ();
		singleList.Add (newclone);

		MoveObjectToPoint_Local (0, newclone, (gameObject.transform.position + new Vector3 (0, 0.5f, 0)), 1);
		ChangeScale_GameObjectList(1f, singleList, 0, 3,3, 2);
		BreakToTarget_GameObjectList(3f, singleList, PlayersGameObject(), Vector2.zero, 0, 1);
		Destroy_GameObjectList_CircleBurst(5f, singleList, 0, 5, 2, 2);

		yield return null;
	}
	public void CustomShot_ChargeShot()
	{
		StartCoroutine (C_CustomShot_ChargeShot ());
	}

}
