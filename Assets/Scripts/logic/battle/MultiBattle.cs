using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MultiBattle : MonoBehaviour
{
    //单例
    public static MultiBattle instance;
    //坦克预设
    public GameObject[] tankPrefabs;
    //战场中所有坦克
    public Dictionary<string, BattleTank> list = new Dictionary<string,BattleTank>();

    void Start()
    { 
        //单例模式
        instance = this;
    }

    /// <summary>
    /// 获取阵营，0为错误
    /// </summary>
    /// <param name="tankObj"></param>
    /// <returns></returns>
    public int GetCamp(GameObject tankObj)
    {
        foreach (BattleTank mt in list.Values)
        {
            if (mt.tank.gameObject == tankObj)
                return mt.camp;
        }
        return 0;
    }

    /// <summary>
    /// 判断是否是同一阵营
    /// </summary>
    /// <param name="tank1"></param>
    /// <param name="tank2"></param>
    /// <returns></returns>
    public bool IsSameCamp(GameObject tank1, GameObject tank2)
    {
        return GetCamp(tank1) == GetCamp(tank2);
    }

    /// <summary>
    /// 清理战场
    /// </summary>
    public void ClearBattle()
    {
        list.Clear();
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Tank");
        for (int i = 0; i < tanks.Length; i++) {
            Destroy(tanks[i]);
        }
    }

    /// <summary>
    /// 开始战斗协议
    /// </summary>
    /// <param name="proto"></param>
    public void StartBattle(ProtocolBytes proto)
    {
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        if (protoName != "Fight")
            return;
        //坦克总数
        int count = proto.GetInt(start, ref start);
        //清理战场
        ClearBattle();
        //生成坦克
        for (int i = 0; i < count; i++)
        {
            string id = proto.GetString(start, ref start);
            int team = proto.GetInt(start, ref start);
            int swopID = proto.GetInt(start, ref start);
            GenerateTank(id, team, swopID);
        }
        ////开启监听
        //NetMgr.servConn.msgDist.AddListener("UpdateUnitInfo", RecvUpdateUnitInfo);
        //NetMgr.servConn.msgDist.AddListener("Shooting", RecvShooting);
        //NetMgr.servConn.msgDist.AddListener("Hit", RecvHit);
        //NetMgr.servConn.msgDist.AddListener("Result", RecvResult);
    }

    /// <summary>
    /// 生成坦克
    /// </summary>
    /// <param name="id"></param>
    /// <param name="team"></param>
    /// <param name="swopID"></param>
    public void GenerateTank(string id, int team, int swopID)
    { 
        //获取出生点
        Transform sp = GameObject.Find("SwopPoints").transform;
        Transform swopTrans;
        if (team == 1)
        {
            Transform teamSwop = sp.GetChild(0);
            swopTrans = teamSwop.GetChild(swopID - 1);
        }
        else
        {
            Transform teamSwop = sp.GetChild(1);
            swopTrans = teamSwop.GetChild(swopID - 1);
        }
        if (swopTrans == null)
        {
            Debug.LogError("生成坦克，出生点错误！");
            return;
        }
        //预设
        if (tankPrefabs.Length < 2)
        {
            Debug.LogError("坦克预设数量不足！");
            return;
        }
        //生成坦克
        GameObject tankObj = (GameObject)Instantiate(tankPrefabs[team - 1]);
        tankObj.name = id;
        tankObj.transform.position = swopTrans.position;
        tankObj.transform.rotation = swopTrans.rotation;
        //列表处理
        BattleTank bt = new BattleTank();
        bt.tank = tankObj.GetComponent<Tank>();
        bt.camp = team;
        list.Add(id, bt);
        //玩家处理
        if (id == GameMgr.instance.id)
        {
            bt.tank.ctrlType = CtrlType.PLAYER;
            CameraFollow cf = Camera.main.gameObject.GetComponent<CameraFollow>();
            GameObject target = bt.tank.gameObject;
            cf.SetTarget(target);
        }
        else
        {
            //bt.tank.ctrlType = CtrlType.NET;
            //bt.tank.InitNetCtrl();
        }
    }

}
