// RegalPrime 7-27-15 - SpikeTreadmill.cs

// Takes a list of gameobjects which inherit from the Machine abstract class on them

// This script has an editor script attached to is so the following methods can be activated on button pressed
// This is an easy way to control a large set of machines from one object
// Mostly a proof of concept for how to control machines later (either through script or triggered objects)

// Work in progress

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MachineController : MonoBehaviour
{
	public List<Machine> MachineScripts;
	public float speedChange = 1;

//	void OnTriggerEnter2D()
//	{
//		foreach(Machine AMachine in MachineScripts)
//		{
//			AMachine.Start_Machine ();
//		}
//	}
//
//	void OnTriggerExit2D()
//	{
//		foreach(Machine AMachine in MachineScripts)
//		{
//			AMachine.Reset_Machine ();
//		}
//	}


	public void Start_Machine()
	{
		foreach(Machine AMachine in MachineScripts)
		{
			if(AMachine.isActiveAndEnabled) AMachine.Start_Machine ();
        }
    }
	public void Stop_Machine()
	{
		foreach(Machine AMachine in MachineScripts)
		{
			if(AMachine.isActiveAndEnabled) AMachine.Stop_Machine ();
        }
    }
	public void Reset_Machine()
	{
		foreach(Machine AMachine in MachineScripts)
		{
			if(AMachine.isActiveAndEnabled) AMachine.Reset_Machine ();
        }
    }
	public void Reverse_Machine()
	{
		foreach(Machine AMachine in MachineScripts)
		{
			if(AMachine.isActiveAndEnabled)	AMachine.Reverse_Machine ();
        }
    }
	public void SpeedUp_Machine()
	{
		foreach(Machine AMachine in MachineScripts)
		{
			if(AMachine.isActiveAndEnabled)	AMachine.SpeedUp_Machine (speedChange);
        }
    }
	public void SlowDown_Machine()
	{
		foreach(Machine AMachine in MachineScripts)
		{
			if(AMachine.isActiveAndEnabled)	AMachine.SlowDown_Machine (speedChange);
        }
    }
	public void SetSpeed_Machine()
	{
		foreach(Machine AMachine in MachineScripts)
		{
			if(AMachine.isActiveAndEnabled)	AMachine.SetSpeed_Machine (speedChange);
		}
	}

}
