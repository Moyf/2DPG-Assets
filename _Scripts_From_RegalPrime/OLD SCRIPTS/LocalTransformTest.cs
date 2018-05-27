// Testing the TransformPoint call

using UnityEngine;
using System.Collections;

public class LocalTransformTest : MonoBehaviour
{
	public GameObject VisualObject;

	void FixedUpdate()
	{
		Transform test = gameObject.transform;
		Vector3 test2 = gameObject.transform.TransformPoint(0.20f, -0.20f, 0);

		VisualObject.transform.position = test2;
		print (test.localPosition + "  -  " + test2);


	}
}
