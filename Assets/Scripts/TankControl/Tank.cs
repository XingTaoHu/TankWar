using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour {

    //轮轴 
    public List<AxleInfo> axleInfos;
    //马力
    private float motor = 0f;
    //最大马力
    public float maxMotorTorque;
    //制动
    private float brakeTorque = 0f;
    //最大制动
    public float maxBrakeTorque = 100f;
    //转向角
    private float steering = 0f;
    //最大转向角
    public float maxSteeringAngle;


    //炮塔
    private Transform turret;
    //炮塔的旋转点
    private Transform turretPoint;
    //炮塔旋转速度
    private float turretRotSpeed = 0.5f;
    //炮塔目标角度
    private float turretRotTarget = 0;
    //炮管
    private Transform gun;
    //炮管的旋转点
    private Transform gunPoint;
    //炮管的旋转范围
    private float maxRoll = 10f;
    private float minRoll = -5f;
    //炮管目标角度
    private float gunRollTarget = 0f;

    //轮子
    private Transform wheels;


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

        //获取轮子
        wheels = transform.Find("wheels");
    }
	
	// Update is called once per frame
	void Update () {
        //玩家控制操控 
        PlayerCtrl();
        //遍历车轴
        foreach(AxleInfo axleInfo in axleInfos)
        {
            //转向
            if(axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            //马力
            if(axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            //制动
            if(true)
            {
                axleInfo.leftWheel.brakeTorque = brakeTorque;
                axleInfo.rightWheel.brakeTorque = brakeTorque;
            }
        }
        //炮塔炮管旋转
        TurretRotation();
        GunRotation();
    }

    /// <summary>
    /// 玩家控制
    /// </summary>
    public void PlayerCtrl()
    {
        //马力和转向角
        motor = maxMotorTorque * Input.GetAxis("Vertical");
        steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        //制动
        brakeTorque = 0;
        foreach(AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.leftWheel.rpm > 5 && motor < 0)
                brakeTorque = maxBrakeTorque;
            else if (axleInfo.leftWheel.rpm < -5 && motor > 0)
                brakeTorque = maxBrakeTorque;
            continue;
        }

        //炮塔旋转
        turretRotTarget = Camera.main.transform.eulerAngles.y;
        //炮管旋转
        gunRollTarget = Camera.main.transform.eulerAngles.x;
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
    }



}
