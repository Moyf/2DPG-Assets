// RegalPrime 12-01-14 - Shooting.cs

// 这个脚本用来控制角色射击
// This script is attached to the player to allow him to shoot via the fire button
// 利用对象池来循环子弹
// Uses object pooling to recycle the bullets

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shooting : MonoBehaviour
{
    // 需要附上你希望发射的子弹 Prefab （可以通过 Array 附加多个不同的对象）
	public GameObject bulletPrefab;						// Attach the bullet you wish to fire (possibly add multi bullet later via array)

    // 子弹的抵消值——它发射的地方，是个 Transform 俩口
	public Transform  bulletOffset;						// Where the bullet fires from
    // 最大子弹数限制，为 0 则不限制
	public int maxBullets = 0;							// Number of max bullets on the screen at a time (0 = infinite)
    // 开火速率限制，每发子弹之间间隔的时长
	public float fireRate = .1f;						// Delay inbetween firing、
    // 子弹飞行速度
	public float bulletSpeed = 4f;						// Speed of the bullet
    // 死亡倒计时
	public float deathTimer = 2f;

    // 是否自动开火（？）
	public bool AutoFire = false;						// Enables rapidfire if true

    // 是否允许开火——用来给发射之间做CD
	private bool ableToFire = true;						// Used in determining how often player can fire
	
    // 子弹音效
	public AudioClip bulletSound;						// Default bullet sound

    // 储存所有子弹的列表，用来生成对象池并且重用
	private List<GameObject> BulletObjects = new List<GameObject>();	// List of bullets to reuse via object pooling

	private PlatformerCharacter2D PlayerScript;							// Reference to the player script

	void Start ()
	{
		// Make the bullets material the same as this objects
		bulletPrefab.GetComponent<SpriteRenderer>().material = gameObject.GetComponent<SpriteRenderer>().material;
		bulletPrefab.GetComponent<SpriteRenderer> ().sortingOrder = gameObject.GetComponent<SpriteRenderer> ().sortingOrder;

		PlayerScript = GetComponent<PlatformerCharacter2D> ();

		if(bulletOffset == null)	bulletOffset = gameObject.transform;

		EventManager.CreateEventManagerIfNeeded();
		EventManager.resetObjects += Reset;
	}
	void OnDestroy()
	{
		EventManager.resetObjects -= Reset;
	}
	void OnEnable()
	{
		EventManager.attackButton += OnButtonPressed_Shoot;
		EventManager.attackButton_Held += OnButtonPressed_Shoot_Held;
	}
	void OnDisable()
	{
		EventManager.attackButton -= OnButtonPressed_Shoot;
		EventManager.attackButton_Held -= OnButtonPressed_Shoot_Held;
	}
	void Reset()
	{
		ableToFire = true;
		foreach (GameObject child in BulletObjects)
		{
			child.SetActive (false);
			child.GetComponent<WhenBulletDies>().StopAllCoroutines();
		}
	}


	void OnButtonPressed_Shoot()
	{
        // 检查是否允许发射
		if(ableToFire && !PlayerScript.disableCharacterShooting)
		{
            // 每次按下开火后，先禁用「允许发射」开关
			ableToFire = false;
            // Invoke：每隔 param2 的时间，调用一次 param1 的函数
            // 隔 fireRate 调用一次 ResetFIre 函数
            // 此处即为：在 CD 结束之后，再把开关打开
			Invoke ("ResetFire", fireRate);
			Shoot ();
		}
	}
	void OnButtonPressed_Shoot_Held()
	{
		if(ableToFire && !PlayerScript.disableCharacterShooting && AutoFire)
		{
			ableToFire = false;
			Invoke ("ResetFire", fireRate);
			Shoot ();
		}
	}


	public void Shoot()												// Create a bullet and make it move
	{
        // 创建一个子弹克隆体
		GameObject clone = GetBulletClone ();

        // 如果创建失败，那啥都没发生
        // 否则呢，开始射击的操作（让子弹移动）
		if(clone != null)		// If able to get a bullet, fire it
		{
            // 判断玩家朝向，并根据朝向进行射击
			if(PlayerScript.facingRight)
                // 子弹的运动倒是没啥特别的……就是施加一个水平方向的力
                // 子弹本身应该不受重力影响
				clone.GetComponent<Rigidbody2D>().velocity = new Vector2(bulletSpeed, 0);
			else
				clone.GetComponent<Rigidbody2D>().velocity = new Vector2(-bulletSpeed, 0);

            // 通过音乐（？）控制器来播放一个单发的子弹音效
			MusicController.control.OneShotAudio (bulletSound);
		}
	}

    // 即时生成一个子弹对象，并存入队列
	GameObject GetBulletClone()
	{
		foreach (GameObject child in BulletObjects)		// Check to see if there is a disabled bullet clone to reuse
		{
            // * 查看是否有可以重新利用的禁用的子弹，若有，则重新利用
            //  判定条件：如果在层级 Hierarchy 中为非激活状态
            // 这样可以避免生成过多的子弹对象，节约性能&资源
            if(!child.activeInHierarchy)
			{
                // （重要）停用所有协程
                // 如果是碰撞地面的话，本身就会停止一次，这里算是确保
				child.GetComponent<WhenBulletDies>().StopAllCoroutines();
                // 把位置挪回来
				child.transform.position = bulletOffset.transform.position;
                // 再重新设成激活
				child.SetActive (true);
                // 直接返回这个重新激活的对象
				return child;
			}
		}

        // 限制：只有当子弹数小于限制（或限制为0）才能发射
		if(BulletObjects.Count < maxBullets || maxBullets == 0)		// Create a new bullet clone if able
		{
			GameObject clone;   // 定义一个子弹的克隆
            // 将子弹对象  Prefab 实体化
			clone = Instantiate (bulletPrefab, bulletOffset.transform.position, bulletOffset.transform.rotation) as GameObject;
			
            // 增加 Tag
			clone.tag = "Bullet";

            // 判断……嗯……刚体？
            // 看来发射子弹的脚本只负责把这个 Prefab 生成+发射出去，其他的处理都是在别处
			if (!clone.GetComponent<Rigidbody2D> ())
                // 如果不存在2D刚体，则将2D刚体设为动力学
				clone.AddComponent<Rigidbody2D>().isKinematic = true;

            // 如果没有消亡，则将延迟消亡计时器（2秒） 后执行消亡
			if(!clone.GetComponent<WhenBulletDies>())
				clone.AddComponent<WhenBulletDies>().deathDelay = deathTimer;

            // # 以上两个其实都是相当于“初始时设置”
            // 看似判定了条件，就相当于「一开始的时候」（不存在对应数据）赋值

            // 把克隆出来的子弹加入子弹对象集（列表）
			BulletObjects.Add (clone);
			
            // 返回新生成的对象
			return clone;
		}

        // 如果两者都不满足，返回空值
		return null;
	}

	void ResetFire()
	{
        // 允许开火
        // （每次发射完都会关闭，然后需要等一定时间之后才开启）
        // 用这样：1. 设定开关 2. 执行完函数A后关闭 3. Invoke 函数 B 来再度开启开关
		ableToFire = true;
	}
}