using UnityEngine;
using System.Collections;

public class NormalMapAnimation : MonoBehaviour
{
	public float Hspeed = 0.0f;
	public float Vspeed = 0.0f;


	Renderer rend;
	Vector2 start = Vector2.zero;

//	string main = "_MainTex";
	string normal = "_BumpMap";
//	string normal1 = "_DetailNormalMap";

	void Start()
	{
		rend = GetComponent<Renderer> ();
	}


	void FixedUpdate ()
	{
		start += (new Vector2(Hspeed, Vspeed) * Time.deltaTime);
//		rend.material.SetTextureOffset (main, start);
		rend.material.SetTextureOffset (normal, start);
//		rend.material.SetTextureOffset (normal1, start);
	}
}
