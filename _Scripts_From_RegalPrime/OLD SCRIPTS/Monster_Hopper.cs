using UnityEngine;
using System.Collections;

public class Monster_Hopper : MonoBehaviour
{

	public LayerMask whatIsGround;
	public float AggroRadius = 1f;
	public float JumpSpeed = 1f;
	public float JumpPower = 2f;

	Animator anim;
	public Transform groundCheck;
	bool grounded;
	bool facingRight = false;
	bool Active = true;

	void Start()
	{
		anim = GetComponent<Animator>();
	}


	void FixedUpdate()
	{
		grounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, whatIsGround);
		anim.SetBool ("Grounded", grounded);

		if(GameController.control.GuyLocation.transform.position.x > gameObject.transform.position.x && !facingRight && grounded)
			Flip ();
		if(GameController.control.GuyLocation.transform.position.x < gameObject.transform.position.x && facingRight && grounded)
			Flip ();

		if(Vector3.Distance(GameController.control.GuyLocation.transform.position, gameObject.transform.position) < AggroRadius && Active && grounded)
			Leap ();
	}
	
	
	public void Leap()
	{
		StartCoroutine (C_Leap ());
	}
	IEnumerator C_Leap()
	{
		Active = false;

		if(grounded)
		{
			if(facingRight)
				gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(JumpSpeed,JumpPower);
			else
				gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-JumpSpeed,JumpPower);
		}

		yield return new WaitForSeconds(JumpPower*0.6f);
		Active = true;
	}

	void Flip ()
	{
		facingRight = !facingRight;
		
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
