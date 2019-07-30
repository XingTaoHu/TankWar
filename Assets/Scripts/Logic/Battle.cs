using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour {

	//单例
    public static Battle instance;
    //战场中的所有坦克
    public BattleTank[] battleTanks;

    //坦克预设
    public GameObject[] tankPrefabs;

    void Start()
    {
        //单例
        instance = this;
        //开始战斗
        StartTwoCampBattle(3, 3);
    }

    //开启一场两军对峙的游戏(分别为第一阵营数量和第二阵营数量)
    void StartTwoCampBattle(int camp1, int camp2)
    { 
        //获取出生点容器
        Transform sp = GameObject.Find("SwopPoints").transform;
        Transform spCamp1 = sp.GetChild(0);
        Transform spCamp2 = sp.GetChild(1);
        //判断
        if (spCamp1.childCount < camp1 || spCamp2.childCount < camp2)
        {
            Debug.LogError("出生点数量不够");
            return;
        }
        if (tankPrefabs.Length < 2) {
            Debug.LogError("坦克预设数量不够");
            return;
        }
        //清理场景
        ClearBattle();
        //产生坦克
        battleTanks = new BattleTank[camp1 + camp2];
        for (int i = 0; i < camp1; i++)
        {
            GenerateTank(1, i, spCamp1, i);
        }
        for (int j = 0; j < camp2; j++)
        {
            GenerateTank(2, j, spCamp2, j);
        }
        //将第一个坦克设为玩家操控
        Tank tankCmp = battleTanks[0].tank;
        tankCmp.ctrlType = CtrlType.PLAYER;
        //设置相机
        CameraFollow cf = Camera.main.gameObject.GetComponent<CameraFollow>();
        GameObject target = tankCmp.gameObject;
        cf.SetTarget(target);
    }

    /// <summary>
    /// 生成一辆坦克
    /// </summary>
    /// <param name="camp">坦克所在阵营</param>
    /// <param name="num">坦克在该阵营中的编号</param>
    /// <param name="spCamp">该阵营出生点</param>
    /// <param name="index">在战场中的编号，对应battleTanks的索引</param>
    public void GenerateTank(int camp, int num, Transform spCamp, int index)
    { 
        //获取出生点和预设
        Transform trans = spCamp.GetChild(num);
        Vector3 pos = trans.position;
        Quaternion rot = trans.rotation;
        GameObject prefab = tankPrefabs[camp - 1];
        //生成坦克
        GameObject tankObj = (GameObject)Instantiate(prefab, pos, rot);
        //设置属性
        Tank tankCmp = tankObj.GetComponent<Tank>();
        tankCmp.ctrlType = CtrlType.COMPUTER;
        //battleTanks
        battleTanks[index] = new BattleTank();
        battleTanks[index].tank = tankCmp;
        battleTanks[index].camp = camp;
    }

    //获取阵营，0为错误
    public int GetCamp(GameObject tankObj)
    {
        if(battleTanks == null)
            return 0;
        for (int i = 0; i < battleTanks.Length; i++)
        {
            BattleTank battleTank = battleTanks[i];
            if (battleTank.tank.gameObject == tankObj)
                return battleTank.camp;
        }
        return 0;
    }

    //判断是否是同一阵营 
    public bool IsSameCamp(GameObject tank1, GameObject tank2)
    {
        return GetCamp(tank1) == GetCamp(tank2);
    }

    //胜负判断
    public bool IsWin(int camp) {
        for (int i = 0; i < battleTanks.Length; i++)
        {
            Tank tank = battleTanks[i].tank;
            if (battleTanks[i].camp != camp)
            {
                if (tank.hp > 0)
                    return false;
            }
        }
        return true;
    }
    //胜负判断
    public bool IsWin(GameObject attTank) {
        int camp = GetCamp(attTank);
        return IsWin(camp);
    }

    //清理场景
    public void ClearBattle() {
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Tank");
        for (int i = 0; i < tanks.Length; i++)
            Destroy(tanks[i]);
    }
    
}
