// RegalPrime 5-21-15 - Machine.cs

// Abstract parent class of all machines
// Forces the implementation of the following below methods
// This allows the MachineController to have an easy way to manipluate machines


using UnityEngine;
using System.Collections;

public abstract class Machine : MonoBehaviour
{
	public abstract void Start_Machine();
	public abstract void Stop_Machine();
	public abstract void Reset_Machine();

	public abstract void Reverse_Machine();

	public abstract void SpeedUp_Machine(float speedChange);
	public abstract void SlowDown_Machine(float speedChange);
	public abstract void SetSpeed_Machine(float newSpeedMultiplier);
}
