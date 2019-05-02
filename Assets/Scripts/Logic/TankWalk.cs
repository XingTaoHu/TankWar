using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankWalk : MonoBehaviour {

    public float MoveSpeed = 2.56f;
    public float RotateSpeed = 45.0f;

    private Transform mTrans;


	// Use this for initialization
	void Start () {
        mTrans = this.transform;
        mTrans.rotation = Quaternion.identity;
		Debug.Log("移动速度为：" + MoveSpeed + ", 转向速度为：" + RotateSpeed);
	}

    // 上下按键是控制移动的，左右按键是方向的
    void Update() {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            mTrans.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) 
        {
            mTrans.Translate(Vector3.back * MoveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            mTrans.Rotate(0, -RotateSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            mTrans.Rotate(0, RotateSpeed * Time.deltaTime, 0);
        }
	}
}
