// RegalPrime 12-01-14 - WhenBulletDies.cs

//　添加到子弹对象上（Prefab内置），让它在一段时间后 or 碰到地面的时候消失
// Added to the bullet so that it disables itself after a set period of time or when it hits a Ground tagged object

using UnityEngine;
using System.Collections;

public class WhenBulletDies : MonoBehaviour
{
    // 存活周期（死亡延迟）
	public float deathDelay;		// How long this object stays alive for

    // 一旦创建这个对象，就开始一个协程
    // 这个协程对应的是一个函数：DisableAfter
    // DisableAfter 同样是个协程，等待 Delay 后，反激活游戏对象
	void OnEnable()
	{
		StartCoroutine (DisableAfter(deathDelay));
	}

	IEnumerator DisableAfter(float Delay)
	{
		yield return new WaitForSeconds (Delay);
		gameObject.SetActive (false);
	}

    // 在碰撞到其他对象的时候，准确的说，碰到 Tag 为 Ground 的对象后
    // （重要）停止所有协程
    // 然后直接注销对象
	void OnTriggerEnter2D(Collider2D other)
	{
		
		if (other.gameObject.tag == "Ground")	// If ground object hit, disable this bullet
		{
			StopAllCoroutines ();
			gameObject.SetActive (false);
		}
	}
}
