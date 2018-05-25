using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 4);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Enemy")
        {
            // collision.gameObject.GetComponent<>().Hurt();
            
        }

        else if (collision.tag == "Ground")
        {
            // 
        }
    }
}
