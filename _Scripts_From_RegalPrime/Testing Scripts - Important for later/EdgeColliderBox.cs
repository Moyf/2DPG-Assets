using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EdgeColliderBox : MonoBehaviour
{
	public List<Vector2> newVerticies = new List<Vector2>();
	private EdgeCollider2D edgeCollider;

	void Start ()
	{
		edgeCollider = gameObject.GetComponent<EdgeCollider2D> ();

		newVerticies.Add(new Vector2 (-0.16f, 0.16f));
		newVerticies.Add(new Vector2 ( 0.16f, 0.16f));
		newVerticies.Add(new Vector2 ( 0.16f, -0.16f));
		newVerticies.Add(new Vector2 (-0.16f, -0.16f));
		newVerticies.Add(new Vector2 (-0.16f, 0.16f));

		edgeCollider.points = newVerticies.ToArray ();

//		foreach(Vector2 value in edgeCollider.points)
//		{
//			print (value.x + ", "+value.y);
//		}
	}
	
}
