// Used to test when an object actually touched each other

using UnityEngine;
using System.Collections;

public class CheckCollider : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D other)
	{
		print ("TOUCHING AN OBJECT");

		Vector3 SolidObject = other.gameObject.transform.position;
//		BoxCollider2D C_SolidObjet = other.gameObject.GetComponent<BoxCollider2D> ();

		Vector3 TriggerObject = gameObject.transform.position;
//		BoxCollider2D C_TriggerObject = gameObject.GetComponent<BoxCollider2D> ();




		print ("SolidObject " + SolidObject.x);
		print ("TriggerObject " + TriggerObject.x);

	}
}
