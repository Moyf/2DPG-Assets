/*  这个脚本用来控制子弹 Prefab 的基础行为
    需要做的事：
    1. 给自己设置一个死亡定时器（生命周期），保证在经过一定时间后子弹会消失
    2. 做碰撞检测判定，如果撞到了墙面或是敌人，触发对应的行为

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavious : MonoBehaviour {
    public float deathDelay;    // 生命周期

    // ------------------------------------------
    // 第一块：实现“等待一定时间后注销本对象”的功能
    // 使用了协程功能（StartCoroutine），调用一个 IEnumerator 类型函数
    // P.S. IEnumerator 不等于 IEnumerable！
    // ------------------------------------------

    // 当对象被 enable 或 activate 时，调用这个函数
    // 之所以用这个而不是 Start() 是为了保证可循环重利用
    // （再次 Active 时）
    private void OnEnable()
    {
        StartCoroutine(DisableAfter(deathDelay));
    }
    

    private IEnumerator DisableAfter(float Delay)
    {
        // 等待固定秒数
        // WaitForSeconds 是 Class，不是函数
        // 所以相当于用 yield return 来等待这个新的对象完成后执行下一步
        yield return new WaitForSeconds(Delay);
        gameObject.SetActive(false);
    }


    // ------------------------------------------
    // 第二块：对碰撞做检测
    // ------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "scene")
        {
            // 注意，这里需要停止所有协程，避免自带的生命周期产生干扰
            // ……大概吧
            StopAllCoroutines();
            gameObject.SetActive(false);
        } else if (other.gameObject.tag == "enemy")
        {
            // TODO:
            // 这里可以加播放一个爆炸的动画
            StopAllCoroutines();
            gameObject.SetActive(false);
        }


    }

}
