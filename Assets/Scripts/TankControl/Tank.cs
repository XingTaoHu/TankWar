using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 操控类型
/// </summary>
//public enum CtrlType
//{
//    NONE,
//    PLAYER,
//    COMPUTER
//}

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
    public Transform turret;
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
    //履带
    private Transform tracks;
    //马达音源
    private AudioSource motorAudioSource;
    //马达音效
    public AudioClip motorClip;

    //炮弹预设
    public GameObject bullet;
    //上一次开炮时间
    private float lastShootTime = 0;
    //开炮时间间隔
    private float shootInterval = 0.5f;

    //最大生命值
    private float maxHp = 100;
    //当前生命值
    public float hp = 100;

    //操控类型
    public CtrlType ctrlType = CtrlType.PLAYER;

    //焚烧特效
    public GameObject destoryEffect;


    //中心准心
    public Texture2D centerSight;
    //坦克准心
    public Texture2D tankSight;
    //血条
    public Texture2D hpBarBg;
    public Texture2D hpBar;
    //击杀提示图标
    public Texture2D killUI;
    //击杀图标开始显示的时间
    private float killUIStartTime = float.MinValue;

    //发射炮弹音源
    private AudioSource shootAudioSource;
    //发射音效
    public AudioClip shootClip;

    //人工智能
    private AI ai;

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
        //获取履带
        tracks = transform.Find("tracks");
        //马达音源
        motorAudioSource = gameObject.AddComponent<AudioSource>();
        motorAudioSource.spatialBlend = 1;
        //发射音源
        shootAudioSource = gameObject.AddComponent<AudioSource>();
        shootAudioSource.spatialBlend = 1;
        //人工智能
        if (ctrlType == CtrlType.COMPUTER)
        { 
            ai = gameObject.AddComponent<AI>();
            ai.tank = this;
        }
    }
	
	// Update is called once per frame
	void Update () {
        //玩家控制操控 
        PlayerCtrl();
        ComputerCtrl();
        NoneCtrl();
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
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            //制动
            if (true)
            {
                axleInfo.leftWheel.brakeTorque = brakeTorque;
                axleInfo.rightWheel.brakeTorque = brakeTorque;
            }
            //转动轮子
            if (axleInfos[1] != null && axleInfo == axleInfos[1])
            {
                WheelsRotation(axleInfos[1].leftWheel);
                TrackMove();
            }
        }
        //炮塔炮管旋转
        TurretRotation();
        GunRotation();
        //马达音效
        MotorSound();
    }

    void OnGUI()
    {
        if (ctrlType != CtrlType.PLAYER)
            return;
        DrawSight();
        DrawHp();
        DrawKillUI();
    }

    /// <summary>
    /// 玩家控制
    /// </summary>
    public void PlayerCtrl()
    {
        //只有为玩家的时候才生效
        if (ctrlType != CtrlType.PLAYER)
            return;

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

        //计算目标角度（炮管转向角度,沿着屏幕中心点）
        TargetSignPos();

        //发射炮弹
        if (Input.GetMouseButton(0))
            Shoot();
    }

    //电脑控制
    public void ComputerCtrl()
    {
        if (ctrlType != CtrlType.COMPUTER)
            return;
        //炮塔目标角度(由AI获取炮塔和炮管的目标角度)
        Vector3 rot = ai.GetTurrentTarget();
        turretRotTarget = rot.y;
        gunRollTarget = rot.x;
        //发射炮弹
        if (ai.IsShoot())
            Shoot();
        //移动
        steering = ai.GetSteering();
        motor = ai.GetMotor();
        brakeTorque = ai.GetBrakeTorque();
    }

    //无人控制(对应于坦克被摧毁，马力和旋转角为0，有一定的制动，炮塔维持原角度)
    public void NoneCtrl()
    {
        if (ctrlType != CtrlType.NONE)
            return;
        motor = 0;
        steering = 0;
        brakeTorque = maxBrakeTorque / 2;
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

    //轮子旋转
    public void WheelsRotation(WheelCollider collider)
    {
        if (wheels == null)
            return;
        //获取旋转信息
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        //旋转每个轮子
        foreach (Transform wheel in wheels)
        {
            wheel.rotation = rotation;
        }
    }

    //履带滚动
    public void TrackMove()
    {
        if (tracks == null)
            return;
        float offset = 0;
        if (wheels.GetChild(0) != null)
            offset = wheels.GetChild(0).localEulerAngles.x / 90;
        foreach (Transform track in tracks)
        {
            MeshRenderer mr = track.gameObject.GetComponent<MeshRenderer>();
            if (mr == null) continue;
            Material mtl = mr.material;
            mtl.mainTextureOffset = new Vector2(0, offset);
        }
    }

    //马达音效
    void MotorSound()
    {
        if (motor != 0 && !motorAudioSource.isPlaying)
        {
            motorAudioSource.loop = true;
            motorAudioSource.clip = motorClip;
            motorAudioSource.Play();
        }
        else if (motor == 0)
        {
            motorAudioSource.Pause();
        }
    }

    /// <summary>
    /// 发射炮弹
    /// </summary>
    public void Shoot()
    { 
        //发射间隔
        if (Time.time - lastShootTime < shootInterval)
            return;
        //子弹
        if (bullet == null)
            return;
        //发射
        Vector3 pos = gun.position + gun.forward * 5;
        GameObject bulletObj = (GameObject)Instantiate(bullet, pos, gun.rotation);
        Bullet bulletCmp = bulletObj.GetComponent<Bullet>();
        if (bulletCmp != null)
            bulletCmp.attackTank = this.gameObject;
        lastShootTime = Time.time;
        shootAudioSource.PlayOneShot(shootClip);
    }

    /// <summary>
    /// 被攻击
    /// </summary>
    /// <param name="att"></param>
    public void BeAttacked(float att, GameObject attackTank)
    {
        if (hp <= 0)
            return;
        if (hp > 0)
        {
            hp -= att;    
            //AI处理
            if (ai != null) {
                ai.OnAttacked(attackTank);
            }
        }
        if (hp <= 0)
        {
            GameObject destroyObj = (GameObject)Instantiate(destoryEffect);
            destroyObj.transform.SetParent(transform, false);
            destroyObj.transform.localPosition = Vector3.zero;
            ctrlType = CtrlType.NONE;

            //显示击杀提示
            if (attackTank != null)
            { 
                Tank tankCmp = attackTank.GetComponent<Tank>();
                if (tankCmp != null && tankCmp.ctrlType == CtrlType.PLAYER)
                    tankCmp.StartDrawKill();
            }
        }
    }

    //计算目标角度
    public void TargetSignPos()
    { 
        //碰撞信息和碰撞点
        Vector3 hitPoint = Vector3.zero;
        RaycastHit raycastHit;
        Vector3 centerVec = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        //射线检测
        Ray ray = Camera.main.ScreenPointToRay(centerVec);
        if (Physics.Raycast(ray, out raycastHit, 400f))
        {
            hitPoint = raycastHit.point;
        }
        else {
            hitPoint = ray.GetPoint(400);
        }
        //计算目标角度
        Vector3 dir = hitPoint - turret.position;
        Quaternion angle = Quaternion.LookRotation(dir);
        turretRotTarget = angle.eulerAngles.y;
        gunRollTarget = angle.eulerAngles.x;
    }

    //计算爆炸位置
    public Vector3 CalExplodePoint()
    { 
        //碰撞信息和碰撞点
        Vector3 hitPoint = Vector3.zero;
        RaycastHit hit;
        //沿炮管方向的射线
        Vector3 pos = gun.position + gun.forward * 5;
        Ray ray = new Ray(pos, gun.forward);
        //射线检测
        if (Physics.Raycast(ray, out hit, 400f))
        {
            hitPoint = hit.point;
        }
        else {
            hitPoint = ray.GetPoint(400);
        }
        return hitPoint;
    }

    //绘制准心
    public void DrawSight()
    { 
        //计算实际射击位置
        Vector3 explodePoint = CalExplodePoint();
        //获取坦克准心的屏幕坐标
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(explodePoint);
        //绘制坦克准心
        Rect tankRect = new Rect(screenPoint.x - tankSight.width / 2,
            Screen.height - screenPoint.y - tankSight.height / 2,
            tankSight.width, tankSight.height);
        GUI.DrawTexture(tankRect, tankSight);
        //绘制中心准心
        Rect centerRect = new Rect(Screen.width / 2 - centerSight.width /2,
            Screen.height / 2 - centerSight.height / 2,
            centerSight.width, centerSight.height);
        GUI.DrawTexture(centerRect, centerSight);
    }

    //绘制血条
    public void DrawHp()
    { 
        //底框
        Rect bgRect = new Rect(30, Screen.height - hpBarBg.height - 15, hpBarBg.width, hpBarBg.height);
        GUI.DrawTexture(bgRect, hpBarBg);
        //血条
        float width = hp * 102 / maxHp;
        Rect hpRect = new Rect(bgRect.x + 29, bgRect.y + 9, width, hpBar.height);
        GUI.DrawTexture(hpRect, hpBar);
        //文字
        string text = Mathf.Ceil(hp).ToString() + "/" + Mathf.Ceil(maxHp).ToString();
        Rect textRect = new Rect(bgRect.x + 80, bgRect.y - 10, 50, 50);
        GUI.Label(textRect, text);
    }

    //绘制击杀图标
    public void StartDrawKill()
    {
        killUIStartTime = Time.time;
    }
    private void DrawKillUI()
    {
        if (Time.time - killUIStartTime < 1f) {
            Rect rect = new Rect(Screen.width / 2 - killUI.width / 2, 30, killUI.width, killUI.height);
            GUI.DrawTexture(rect, killUI);
        }
    }

}
