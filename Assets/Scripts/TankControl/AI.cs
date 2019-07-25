using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

    //所控制的坦克
    public Tank tank;
    //状态枚举
    private Status status = Status.Patrol;
    
    //锁定的坦克
    private GameObject target;
    //视野范围
    private float sightDistance = 30;
    //上一次搜寻时间
    private float lastSearchTargetTime = 0;
    //搜寻间隔
    private float searchTargetInterval = 3;

    //路径
    private Path path = new Path();

    void Start() {
        InitWaypoint();
    }

	//状态处理
	void Update () {
        if (tank.ctrlType != CtrlType.COMPUTER)
            return;
        if (status == Status.Patrol)
            PatrolUpdate();
        if (status == Status.Attack)
            AttackUpdate();
        //目标更新
        TargetUpdate();
        //行走
        if (path.IsReach(transform)) {
            path.NextWaypoint();
        }
	}

    //初始化路径
    void InitWaypoint() {
        GameObject obj = GameObject.Find("WaypointContainer");
        if (obj)
            path.InitByObj(obj);
    }

    //更改状态
    public void ChangeStatus(Status status)
    {
        if (status == Status.Patrol)
            PatrolStart();
        else if (status == Status.Attack)
            AttackStart();
    }

    //巡逻开始
    void PatrolStart() { 
    
    }
    //攻击开始
    void AttackStart() { 
        
    }
    //巡逻中
    void PatrolUpdate() { 
        
    }
    //攻击中
    void AttackUpdate() { 
        
    }


    //搜寻目标
    void TargetUpdate()
    { 
        //cd 时间
        float interval = Time.time - lastSearchTargetTime;
        if (interval < searchTargetInterval)
            return;
        lastSearchTargetTime = Time.time;
        //已有目标的情况下判断是否丢失目标
        if (target != null)
            HasTarget();
        else
            NoTarget();
    }
    //已有目标，判断是否丢失目标
    void HasTarget()
    {
        Tank targetTank = target.GetComponent<Tank>();
        Vector3 pos = transform.position;
        Vector3 targetPos = target.transform.position;
        if (targetTank.ctrlType == CtrlType.NONE) {
            Debug.Log("目标死亡，丢失目标");
            target = null;
        }
        else if (Vector3.Distance(pos, targetPos) > sightDistance) {
            Debug.Log("距离过远，丢失目标");
            target = null;
        }
    }
    //没有目标的情况下，寻找视野中的坦克
    void NoTarget() { 
        //最小生命值
        float minHp = float.MaxValue;
        //遍历所有坦克
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Tank");
        for (int i = 0; i < targets.Length; i++) { 
            //Tank组件
            Tank tankTrans = targets[i].GetComponent<Tank>();
            if (tankTrans == null)
                continue;
            //如果是自己
            if (targets[i] == gameObject)
                continue;
            //死亡
            if (tankTrans.ctrlType == CtrlType.NONE)
                continue;
            //距离判断
            Vector3 pos = transform.position;
            Vector3 targetPos = targets[i].transform.position;
            if (Vector3.Distance(pos, targetPos) > sightDistance)
                continue;
            //判断生命值
            if (minHp > tankTrans.hp)
                target = tankTrans.gameObject;
        }
        if (target != null)
            Debug.Log("获取目标:" + target.name);
    }
    //被攻击
    public void OnAttacked(GameObject attackTank) {
        target = attackTank;
    }

    //获取炮管和炮塔的目标角度
    public Vector3 GetTurrentTarget()
    { 
        //没有目标，朝着炮塔的正方向
        if (target == null)
        {
            //坦克的正方向
            float y = transform.eulerAngles.y;
            Vector3 rot = new Vector3(0, y, 0);
            return rot;
        }
        else {
            Vector3 pos = transform.position;
            Vector3 targetPos = target.transform.position;
            Vector3 vec = targetPos - pos;
            return Quaternion.LookRotation(vec).eulerAngles;
        }
    }

    //是否发射炮弹
    public bool IsShoot() {
        if (target == null)
            return false;
        //目标角度差
        float turrentRoll = tank.turret.eulerAngles.y;
        float angle = turrentRoll - GetTurrentTarget().y;
        if (angle < 0) angle += 360;
        //30度以内发射炮弹
        if (angle < 30 || angle > 330)
            return true;
        else
            return false;
    }

    //获取转向角
    public float GetSteering() {
        if (tank == null)
            return 0;

        Vector3 itp = transform.InverseTransformPoint(path.waypoint);
        if (itp.x > path.deviation / 5) //右转
            return tank.maxSteeringAngle;
        else if (itp.x < -path.deviation / 5) //左转
            return -tank.maxSteeringAngle;
        else
            return 0; //直行
    }

    //获取马力
    public float GetMotor() {
        if (tank == null)
            return 0;
        Vector3 itp = transform.InverseTransformPoint(path.waypoint);
        float x = itp.x;
        float z = itp.z;
        float r = 6;
        if (z < 0 && Mathf.Abs(x) < -z && Mathf.Abs(x) < r)
            return -tank.maxMotorTorque;
        else
            return tank.maxMotorTorque;
    }

    //获取刹车
    public float GetBrakeTorque() {
        if (path.isFinish)
            return tank.maxBrakeTorque;
        else
            return 0;
    }

}
