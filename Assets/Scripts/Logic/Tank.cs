using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour {

    //移动速度
    public float speed = 30f;
    //转向速度
    public float rotateSpeed = 30f;


    //炮塔
    public Transform turret;
    //炮塔旋转速度
    private float turretRotSpeed = 0.5f;
    //炮塔目标角度
    private float turretRotTarget = 0;



	// Use this for initialization
	void Start () {
		//获取炮塔
        turret = transform.FindChild("turret");
	}
	
	// Update is called once per frame
	void Update () {
        //旋转
        float x = Input.GetAxis("Horizontal");
        transform.Rotate(0, x * rotateSpeed * Time.deltaTime, 0);
        //移动
        float y = Input.GetAxis("Vertical");
        Vector3 s = y * transform.forward * speed * Time.deltaTime;
        transform.position += s;
	}
}
