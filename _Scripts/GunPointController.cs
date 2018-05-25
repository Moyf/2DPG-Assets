using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPointController : MonoBehaviour {
    public GameObject gunPoint;
    BulletSpawner bulletSpw;
    Vector3 mousePos;

    public static void Msg(string msg) {
        Debug.Log(msg);
    }

    // Use this for initialization
    void Start () {
        bulletSpw = gunPoint.GetComponent<BulletSpawner>();
        bool isShottingActive = bulletSpw.active;
        mousePos = Input.mousePosition;


    }
	
	// Update is called once per frame
	void Update () {
        mousePos = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1"))
        {
            // 开始射击
            bulletSpw.SpawnON();
            
        }
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Fire1"))
        {
            // 停止射击
            bulletSpw.SpawnOFF();
        }
    }

}
