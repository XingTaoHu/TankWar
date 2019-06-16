using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForce : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKey(KeyCode.Space))
        {
            Rigidbody rigi = gameObject.GetComponent<Rigidbody>();
            Vector3 force = Vector3.up * 50;
            rigi.AddForce(force);
        }
	}

    //当碰撞到物体
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("碰撞到:" + collision.gameObject.name);
    }

}
