// RegalPrime 6-20-15 - AnimatorTest.cs

// A simple test script to make sure my new animation controller is working alright
// There is a editor script that goes with this as well


using UnityEngine;
using System.Collections;

public class AnimatorTest : MonoBehaviour
{
	Animator anim;

	public float VSpeed = 0;

	void Start ()
	{
		anim = GetComponent<Animator> ();
		ResetToDefault();
	}

	public void WhatIsAnimator()
	{
		anim = GetComponent<Animator> ();
	}

	public void StartClimbing()
	{
		anim.SetBool ("Climbing", true);
		anim.SetTrigger ("ClimbingTrigger");
	}
	public void StopClimbing()
	{
		anim.SetBool ("Climbing", false);
	}

	public void Reset_Movement()
	{
		anim.SetFloat ("horizontalSpeed", 0);
		anim.SetFloat ("verticalSpeed", 0);
	}
	public void Move_Horizontal()
	{
		anim.SetFloat ("horizontalSpeed", 1);
	}
	public void Move_Vertical()
	{
		anim.SetFloat ("verticalSpeed", 1);
	}


	public void LandOnGround()
	{
		anim.SetBool ("Ground", true);
	}
	public void InAir()
	{
		anim.SetBool ("Ground", false);
		anim.SetFloat ("fallSpeed", VSpeed);
	}


	public void ResetToDefault()
	{
		Reset_Movement ();
		VSpeed = 0;
		anim.SetBool ("Ground", true);
		anim.SetBool ("Climbing", false);
	}





}
