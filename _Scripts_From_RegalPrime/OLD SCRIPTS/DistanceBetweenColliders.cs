using UnityEngine;
using System.Collections;

public class DistanceBetweenColliders : MonoBehaviour
{
	public GameObject Object1;
	public GameObject Object2;

	private float Ob1_Width;
	private float Ob1_Height;
	private float Ob1_OffsetX = 0;
	private float Ob1_OffsetY = 0;

	private float Ob2_Width;
	private float Ob2_Height;
	private float Ob2_OffsetX = 0;
	private float Ob2_OffsetY = 0;

	void Start ()
	{
		// Calculating the width / height based on which component the spike prefab has
		if(Object1.GetComponent<SpriteRenderer> () != null)
		{
			SpriteRenderer TEMP = Object1.GetComponent<SpriteRenderer> ();
			Ob1_Width = TEMP.sprite.bounds.size.x * Object1.transform.localScale.x;
			Ob1_Height = TEMP.sprite.bounds.size.y * Object1.transform.localScale.y;
		}
		if(Object1.GetComponent<PolygonCollider2D> () != null)
		{
			PolygonCollider2D Poly = Object1.GetComponent<PolygonCollider2D> ();
			Ob1_Width = Poly.bounds.size.x + (Ob1_Width - Poly.bounds.size.x);
			Ob1_Height = Poly.bounds.size.y + (Ob1_Height - Poly.bounds.size.y);
		}
		if(Object1.GetComponent<CircleCollider2D> () != null)
		{
			CircleCollider2D Poly = Object1.GetComponent<CircleCollider2D> ();
			Ob1_Width = Poly.bounds.size.x;
			Ob1_Height = Poly.bounds.size.y;
			Ob1_OffsetX = Poly.offset.x;
			Ob1_OffsetY = Poly.offset.y;

		}
		if(Object1.GetComponent<BoxCollider2D> () != null)
		{
			BoxCollider2D Poly = Object1.GetComponent<BoxCollider2D> ();
			Ob1_Width = Poly.bounds.size.x;
			Ob1_Height = Poly.bounds.size.y;
			Ob1_OffsetX = Poly.offset.x;
			Ob1_OffsetY = Poly.offset.y;
		}


		// Calculating the width / height based on which component the spike prefab has
		if(Object2.GetComponent<SpriteRenderer> () != null)
		{
			SpriteRenderer TEMP = Object2.GetComponent<SpriteRenderer> ();
			Ob2_Width = TEMP.sprite.bounds.size.x * Object2.transform.localScale.x;
			Ob2_Height = TEMP.sprite.bounds.size.y * Object2.transform.localScale.y;
		}
		if(Object1.GetComponent<PolygonCollider2D> () != null)
		{
			PolygonCollider2D Poly = Object2.GetComponent<PolygonCollider2D> ();
			Ob2_Width = Poly.bounds.size.x + (Ob2_Width - Poly.bounds.size.x);
			Ob2_Height = Poly.bounds.size.y + (Ob2_Height - Poly.bounds.size.y);
		}
		if(Object1.GetComponent<CircleCollider2D> () != null)
		{
			CircleCollider2D Poly = Object2.GetComponent<CircleCollider2D> ();
			Ob2_Width = Poly.bounds.size.x;
			Ob2_Height = Poly.bounds.size.y;
			Ob2_OffsetX = Poly.offset.x;
			Ob2_OffsetY = Poly.offset.y;
			
		}
		if(Object1.GetComponent<BoxCollider2D> () != null)
		{
			BoxCollider2D Poly = Object2.GetComponent<BoxCollider2D> ();
			Ob2_Width = Poly.bounds.size.x;
			Ob2_Height = Poly.bounds.size.y;
			Ob2_OffsetX = Poly.offset.x;
			Ob2_OffsetY = Poly.offset.y;
		}



		float DistanceX = Mathf.Abs(Object1.transform.position.x - Object2.transform.position.x);
		float DistanceY = Mathf.Abs(Object1.transform.position.y - Object2.transform.position.y);
		DistanceX = FourDecimalPlaces (DistanceX);
		DistanceY = FourDecimalPlaces (DistanceY);

		Ob1_Width = FourDecimalPlaces (Ob1_Width);
		Ob1_Height = FourDecimalPlaces (Ob1_Height);

		Ob2_Width = FourDecimalPlaces (Ob2_Width);
		Ob2_Height = FourDecimalPlaces (Ob2_Height);

		print ("Collider Width, Height");
		print ("Object 1 : "+Ob1_Width+ ", " +Ob1_Height);
		print ("Object 2 : "+Ob2_Width+ ", " +Ob2_Height);
		print ("DistanceX,Y : " + DistanceX+", "+DistanceY);

		print ("Gap X : " + (DistanceX - Ob1_Width/2 + Ob1_OffsetX - Ob2_Width/2 + Ob2_OffsetX));
		print ("Gap Y : " + (DistanceY - Ob1_Height/2 + Ob1_OffsetY - Ob2_Height/2 + Ob2_OffsetY));
	}

	float FourDecimalPlaces(float number)
	{
		number = (int)(number * 10000);
		number = number / 10000;
		return number;
	}

}


/*
 * 
 * using UnityEngine;
using System.Collections;

public class DistanceBetweenColliders : MonoBehaviour
{
	public GameObject Object1;
	public GameObject Object2;

	private float Ob1_Width;
	private float Ob1_Height;
	private float Ob1_OffsetX = 0;
	private float Ob1_OffsetY = 0;

	private float Ob2_Width;
	private float Ob2_Height;
	private float Ob2_OffsetX = 0;
	private float Ob2_OffsetY = 0;

	void Start ()
	{
		// Calculating the width / height based on which component the spike prefab has
		if(Object1.GetComponent<SpriteRenderer> () != null)
		{
			SpriteRenderer TEMP = Object1.GetComponent<SpriteRenderer> ();
			Ob1_Width = TEMP.sprite.bounds.size.x * Object1.transform.localScale.x;
			Ob1_Height = TEMP.sprite.bounds.size.y * Object1.transform.localScale.y;
		}
		if(Object1.GetComponent<PolygonCollider2D> () != null)
		{
			PolygonCollider2D Poly = Object1.GetComponent<PolygonCollider2D> ();
			Ob1_Width = Poly.bounds.size.x + (Ob1_Width - Poly.bounds.size.x);
			Ob1_Height = Poly.bounds.size.y + (Ob1_Height - Poly.bounds.size.y);
		}
		if(Object1.GetComponent<CircleCollider2D> () != null)
		{
			CircleCollider2D Poly = Object1.GetComponent<CircleCollider2D> ();
			Ob1_Width = Poly.bounds.size.x - Poly.center.x*2;
			Ob1_Height = Poly.bounds.size.y - Poly.center.y*2;
		}
		if(Object1.GetComponent<BoxCollider2D> () != null)
		{
			BoxCollider2D Poly = Object1.GetComponent<BoxCollider2D> ();
			Ob1_Width = Poly.bounds.size.x - Poly.center.x*2;
			Ob1_Height = Poly.bounds.size.y - Poly.center.y*2;
		}


		// Calculating the width / height based on which component the spike prefab has
		if(Object2.GetComponent<SpriteRenderer> () != null)
		{
			SpriteRenderer TEMP = Object2.GetComponent<SpriteRenderer> ();
			Ob2_Width = TEMP.sprite.bounds.size.x * Object2.transform.localScale.x;
			Ob2_Height = TEMP.sprite.bounds.size.y * Object2.transform.localScale.y;
		}
		if(Object2.GetComponent<PolygonCollider2D> () != null)
		{
			PolygonCollider2D Poly = Object2.GetComponent<PolygonCollider2D> ();
			Ob2_Width = Poly.bounds.size.x;
			Ob2_Height = Poly.bounds.size.y + (Ob2_Height - Poly.bounds.size.y);
		}
		if(Object2.GetComponent<CircleCollider2D> () != null)
		{
			CircleCollider2D Poly = Object2.GetComponent<CircleCollider2D> ();
			Ob2_Width = Poly.bounds.size.x - Poly.center.x*2;
			Ob2_Height = Poly.bounds.size.y - Poly.center.y*2;
		}
		if(Object2.GetComponent<BoxCollider2D> () != null)
		{
			BoxCollider2D Poly = Object2.GetComponent<BoxCollider2D> ();
			Ob2_Width = Poly.bounds.size.x - Poly.center.x*2;
			Ob2_Height = Poly.bounds.size.y - Poly.center.y*2;
		}

		float DistanceX = Mathf.Abs(Object1.transform.position.x - Object2.transform.position.x);
		float DistanceY = Mathf.Abs(Object1.transform.position.y - Object2.transform.position.y);
		DistanceX = FourDecimalPlaces (DistanceX);
		DistanceY = FourDecimalPlaces (DistanceY);

		Ob1_Width = FourDecimalPlaces (Ob1_Width);
		Ob1_Height = FourDecimalPlaces (Ob1_Height);

		Ob2_Width = FourDecimalPlaces (Ob2_Width);
		Ob2_Height = FourDecimalPlaces (Ob2_Height);

		print ("Collider Width, Height");
		print ("Object 1 : "+Ob1_Width+ ", " +Ob1_Height);
		print ("Object 2 : "+Ob2_Width+ ", " +Ob2_Height);
		print ("DistanceX,Y : " + DistanceX+", "+DistanceY);

		print ("Gap X : " + (DistanceX - Ob1_Width/2 - Ob2_Width/2));
		print ("Gap Y : " + (DistanceY - Ob1_Height/2 - Ob2_Height/2));	
	}

	float FourDecimalPlaces(float number)
	{
		number = (int)(number * 10000);
		number = number / 10000;
		return number;
	}

}


*/