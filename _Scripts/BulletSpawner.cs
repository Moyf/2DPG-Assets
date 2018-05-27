/* 这个脚本用来控制角色发射子弹。
 * 需要做到：
 * 1. 当角色按下射击键时，根据 Prefab 生成一个新的子弹对象，
 * 并给他赋予基础信息
 * 
 * 2. 限制角色设计的频率，即增加一个“冷却时间”
 * 
 * 3. （拓展）把子弹重复利用起来，避免无止境地创建对象
 * 这个概念是叫做 Object Pooling （对象池）
 * 可以在：https://unity3d.com/cn/learn/tutorials/topics/scripting/object-pooling
 * 页面上找到对应的介绍。
 * 利用对象池可以把像子弹这种重复生成的对象数量控制在一个上限内，
 * 并且避免了反复实例化/销毁造成的性能损耗。
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour {
    // 前置信息：预设对象、发射起点
    public GameObject[] prefabs;
    public GameObject gunPoint;

    // 子弹属性：数量限制、子弹延时、飞行速度、销毁计时、发射力度（？）
    public int maxBullets = 0;
    public float delay = .2f;
    public float flySpeed = 4f;
    public float deathTimer = 2f;
    public float power;

    public AudioClip shotSound;

    // 内部参数：允许自动开火、允许开火、起始位置
    public bool isAutoFireEnable = true;
    private bool ableToFire = true;
    private Transform startPos;
    public bool active;
    // 用来校正子弹的角度，与鼠标所在位置匹配
    Quaternion rotation;
    public Camera cam;

    private List<GameObject> BulletList = new List<GameObject>();
    private PlayerCharacter PlayerScript;


    // Use this for initialization
    void Start()
    {
        // 让生成的新子弹预设外观保持一致
        foreach (GameObject eachPrefab in prefabs)
        {
            eachPrefab.GetComponent<SpriteRenderer>().material = gameObject.GetComponent<SpriteRenderer>().material;
            eachPrefab.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder;
        }

        // 获取角色的脚本，并以此来得到诸如“禁止开枪”之类的角色属性
        PlayerScript = GetComponent<PlayerCharacter>();
        startPos = gunPoint.GetComponent<Transform>();


        
        // PlayerScript = GetComponent<>
    }

    IEnumerator BulletGenerator() {
        yield return new WaitForSeconds(delay);


        /*
        if (active) {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 5f;
            
            float randomInitVelX = 10.0f * Random.Range(0.8f, 1.2f);
            Vector2 initVelocity = new Vector2(randomInitVelX, .0f);
            
            // 旋转
            Vector3 objectPos = cam.WorldToScreenPoint(gunPoint.GetComponent<Transform>().position);
            mousePos.x = mousePos.x - objectPos.x;
            mousePos.y = mousePos.y - objectPos.y;

            float angle = Mathf.Atan2(mousePos.y, mousePos.x);// * Mathf.Rad2Deg;
            Debug.Log("ANGLE: " + angle + "");
            Vector2 force = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            force = force * 600.0f;
            // Debug.Log("FORCE: " + force + "\n");

            // 随机拿出一个 Prefab 来生成，并赋予初速度
            var newBullet = Instantiate(prefabs[Random.Range(0, prefabs.Length)], startPos.position, Quaternion.identity);
            // newBullet.GetComponent<Rigidbody2D>().velocity = initVelocity;
            newBullet.GetComponent<Rigidbody2D>().AddForce(force * power);
            
            // Ray2D mouseRay = Camera.main.ScreenPointToRay2D(Input.mousePosition);
            // Instantiate(prefabs[Random.Range(0, prefabs.Length)], newTransform.position * Random.Range(-2.0f, 2.0f), Quaternion.identity);
        }
        */

        StartCoroutine(BulletGenerator());
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void SpawnON()
    {
        active = true;
    }

    public void SpawnON(Vector3 mousePos) {
        active = true;


    }

    public void SpawnOFF()
    {
        active = false;
    }
}
