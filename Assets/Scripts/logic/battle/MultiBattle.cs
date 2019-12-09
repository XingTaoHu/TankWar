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
        NetMgr.servConn.msgDist.AddListener("UpdateUnitInfo", RecvUpdateUnitInfo);
        NetMgr.servConn.msgDist.AddListener("Shooting", RecvShooting);
        NetMgr.servConn.msgDist.AddListener("Hit", RecvHit);
        NetMgr.servConn.msgDist.AddListener("Result", RecvResult);
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
            Debug.Log("生成玩家坦克：id" + id + ", gameMgr id:" + GameMgr.instance.id);
        }
        else
        {
            bt.tank.ctrlType = CtrlType.NET;
            bt.tank.InitNetCtrl();
            Debug.Log("生成网络坦克:id:" + id);
        }
    }

    /// <summary>
    /// 网络同步更新位置
    /// </summary>
    /// <param name="protoBase"></param>
    public void RecvUpdateUnitInfo(ProtocolBase protoBase)
    {
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        string id = protocol.GetString(start, ref start);
        Vector3 nPos;
        Vector3 nRot;
        nPos.x = protocol.GetFloat(start, ref start);
        nPos.y = protocol.GetFloat(start, ref start);
        nPos.z = protocol.GetFloat(start, ref start);
        nRot.x = protocol.GetFloat(start, ref start);
        nRot.y = protocol.GetFloat(start, ref start);
        nRot.z = protocol.GetFloat(start, ref start);
        float turretY = protocol.GetFloat(start, ref start);
        float gunX = protocol.GetFloat(start, ref start);
        if (!list.ContainsKey(id))
        {
            Debug.Log("RecvUpdateUnitInfo id == null");
            return;
        }
        BattleTank bt = list[id];
        //跳过同步自己的信息
        if (id == GameMgr.instance.id)
            return;
        Debug.Log("网络同步位置：id:" + id + ", pos:" + nPos + ", rot:" + nRot);
        bt.tank.NetForecastInfo(nPos, nRot);
        bt.tank.NetTurretTarget(turretY, gunX);
    }

    /// <summary>
    /// 网络同步炮弹信息
    /// </summary>
    /// <param name="protoBase">Proto base.</param>
    public void RecvShooting(ProtocolBase protoBase)
    {
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        string id = protocol.GetString(start, ref start);
        Vector3 pos;
        Vector3 rot;
        pos.x = protocol.GetFloat(start, ref start);
        pos.y = protocol.GetFloat(start, ref start);
        pos.z = protocol.GetFloat(start, ref start);
        rot.x = protocol.GetFloat(start, ref start);
        rot.y = protocol.GetFloat(start, ref start);
        rot.z = protocol.GetFloat(start, ref start);
        if(!list.ContainsKey(id))
        {
            Debug.Log("RecvShooting id == null");
            return;
        }
        //找到炮弹发射的坦克，如果不是自己同步一下当前客户端其他的坦克
        BattleTank bt = list[id];
        if (id == GameMgr.instance.id)
            return;
        bt.tank.NetShoot(pos, rot);
    }

    /// <summary>
    /// 同步伤害信息 
    /// </summary>
    /// <param name="protoBase">Proto base.</param>
    public void RecvHit(ProtocolBase protoBase)
    {
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        string attId = protocol.GetString(start, ref start);
        string defId = protocol.GetString(start, ref start);
        float hurt = protocol.GetFloat(start, ref start);
        if(!list.ContainsKey(attId))
        {
            Debug.Log("RecvHit attBt == null " + attId);
            return;
        }
        BattleTank attBt = list[attId];
        if(!list.ContainsKey(defId))
        {
            Debug.Log("RecvHit defBt == null " + defId);
            return;
        }
        BattleTank defBt = list[defId];
        //被击中的坦克
        defBt.tank.NetBeAttacked(hurt, attBt.tank.gameObject);
    }

    /// <summary>
    /// 同步结果信息
    /// </summary>
    /// <param name="protoBase">Proto base.</param>
    public void RecvResult(ProtocolBase protoBase)
    {
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        int winTeam = protocol.GetInt(start, ref start);
        string id = GameMgr.instance.id;
        BattleTank bt = list[id];
        if(bt.camp == winTeam)
        {
            PanelManager.instance.OpenPanel<WinPanel>("", 1);
        }
        else
        {
            PanelManager.instance.OpenPanel<WinPanel>("", 0);
        }
        //取消监听
        NetMgr.servConn.msgDist.DelListener("UpdateUnitInfo", RecvUpdateUnitInfo);
        NetMgr.servConn.msgDist.DelListener("Shooting", RecvShooting);
        NetMgr.servConn.msgDist.DelListener("Hit", RecvHit);
        NetMgr.servConn.msgDist.DelListener("Result", RecvResult);
    }

}
