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
    //炮塔的旋转点
    public Transform turretPoint;
    //炮塔旋转速度
    private float turretRotSpeed = 0.5f;
    //炮塔目标角度
    private float turretRotTarget = 0;
    //炮管
    public Transform gun;
    //炮管的旋转点
    public Transform gunPoint;
    //炮管的旋转范围
    private float maxRoll = 10f;
    private float minRoll = -5f;
    //炮管目标角度
    private float gunRollTarget = 0f;

	// Use this for initialization
	void Start () {
        //炮塔
        turret = transform.Find("turret");
		//获取炮塔旋转点
        turretPoint = transform.Find("turret/turretPoint");
        //炮管
        gun = transform.Find("turret/gun");
        //炮管旋转点
        gunPoint = transform.Find("turret/gunPoint");
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

        //炮塔旋转
        turretRotTarget = Camera.main.transform.eulerAngles.y;
        TurretRotation();
        //炮管旋转
        gunRollTarget = Camera.main.transform.eulerAngles.x;
        GunRotation();
	}


    //炮塔旋转
    public void TurretRotation()
    {
        if (Camera.main == null)
            return;
        if (turret == null)
            return;
        if (turretPoint == null)
            return;
        //归一化角度
        float angle = turretPoint.eulerAngles.y - turretRotTarget;
        if (angle < 0) angle += 360;
        if (angle > turretRotSpeed && angle < 180)
        {
            turret.Rotate(0f, -turretRotSpeed, 0f);
        }
        else if (angle > 180 && angle < 360 - turretRotSpeed)
        {
            turret.Rotate(0f, turretRotSpeed, 0f);
        }
    }

    //炮管旋转
    public void GunRotation()
    {
        if (Camera.main == null)
            return;
        if (gun == null)
            return;
        if (gunPoint == null)
            return;
        //旋转
        Vector3 worldEuler = gunPoint.eulerAngles;
        Vector3 localEuler = gunPoint.localEulerAngles;
        worldEuler.x = gunRollTarget;
        gunPoint.eulerAngles = worldEuler;
        Vector3 euler = gunPoint.localEulerAngles;
        if (euler.x > 180)
            euler.x -= 360;
        if (euler.x > maxRoll)
            euler.x = maxRoll;
        if (euler.x < minRoll)
            euler.x = minRoll;
        gun.localEulerAngles = new Vector3(euler.x, localEuler.y, localEuler.z);

        //Vector3 worldEuler = gun.eulerAngles;
        //Vector3 localEuler = gun.localEulerAngles;
        //worldEuler.x = gunRollTarget;
        //gun.eulerAngles = worldEuler;
        //Vector3 euler = gun.localEulerAngles;
        //if (euler.x > 180)
        //    euler.x -= 360;
        //if (euler.x > maxRoll)
        //    euler.x = maxRoll;
        //if (euler.x < minRoll)
        //    euler.x = minRoll;
        //gun.localEulerAngles = new Vector3(euler.x, localEuler.y, localEuler.z);
    }


}
