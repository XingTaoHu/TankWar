using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : PanelBase 
{
    private List<Transform> prefabs = new List<Transform>();
    private Button closeBtn;
    private Button startBtn;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "Prefabs/UI/RoomPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        for (int i = 0; i < 6; i++)
        {
            string name = "PlayerPrefab" + i.ToString();
            Transform prefab = skinTrans.Find(name);
            prefabs.Add(prefab);
        }
        closeBtn = skinTrans.Find("closeBtn").GetComponent<Button>();
        startBtn = skinTrans.Find("startBtn").GetComponent<Button>();
        closeBtn.onClick.AddListener(OnCloseClick);
        startBtn.onClick.AddListener(OnStartClick);

        //监听
        NetMgr.servConn.msgDist.AddListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.servConn.msgDist.AddListener("Fight", RecvFight);
        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomInfo");
        NetMgr.servConn.Send(protocol);
    }

    public override void OnClosing()
    {
        base.OnClosing();
        NetMgr.servConn.msgDist.DelListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.servConn.msgDist.DelListener("Fight", RecvFight);
    }
    #endregion

    void OnCloseClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("LeaveRoom");
        NetMgr.servConn.Send(protocol, OnCloseCallback);
    }

    void OnCloseCallback(ProtocolBase proto)
    {
        ProtocolBytes protocol = (ProtocolBytes)proto;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        int ret = protocol.GetInt(start, ref start);
        if (ret == 0)
        {
            Debug.Log("退出房间成功！");
            PanelManager.instance.OpenPanel<RoomListPanel>("");
            Close();
        }
        else
        {
            PanelManager.instance.OpenPanel<TipPanel>("", "退出房间失败！");
        }
    }

    void OnStartClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("StartFight");
        NetMgr.servConn.Send(protocol, OnStartCallback);
    }

    void OnStartCallback(ProtocolBase proto)
    {
        ProtocolBytes protocol = (ProtocolBytes)proto;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        int ret = protocol.GetInt(start, ref start);
        if (ret != 0)
        {
            PanelManager.instance.OpenPanel<TipPanel>("", "开始游戏失败！两队至少都需要一名玩家，并且只有房主可以开始战斗！");
        }
    }


    void RecvGetRoomInfo(ProtocolBase proto)
    {
        ProtocolBytes protocol = (ProtocolBytes)proto;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        int count = protocol.GetInt(start, ref start);
        int i = 0;
        for (i = 0; i < count; i++)
        {
            string id = protocol.GetString(start, ref start);
            int team = protocol.GetInt(start, ref start);
            int win = protocol.GetInt(start, ref start);
            int fail = protocol.GetInt(start, ref start);
            int isOwner = protocol.GetInt(start, ref start);
            Transform trans = prefabs[i];
            Text idText = trans.Find("nameText").GetComponent<Text>();
            Text teamText = trans.Find("campText").GetComponent<Text>();
            Text winText = trans.Find("winText").GetComponent<Text>();
            Text failText = trans.Find("loseText").GetComponent<Text>();
            Text ownerText = trans.Find("player").GetComponent<Text>();
            idText.text = id;
            teamText.text = (team == 1) ? "红" : "蓝";
            winText.text = win.ToString();
            failText.text = fail.ToString();
            if (id == GameMgr.instance.id)
                ownerText.text += "【我自己】";
            if (isOwner == 1)
                ownerText.text += "【房主】";
            if (team == 1)
                trans.GetComponent<Image>().color = Color.red;
            else
                trans.GetComponent<Image>().color = Color.blue;
        }
        for (; i < 6; i++)
        {
            Transform trans = prefabs[i];
            Text idText = trans.Find("nameText").GetComponent<Text>();
            Text teamText = trans.Find("campText").GetComponent<Text>();
            Text winText = trans.Find("winText").GetComponent<Text>();
            Text failText = trans.Find("loseText").GetComponent<Text>();
            Text ownerText = trans.Find("player").GetComponent<Text>();
            idText.text = "";
            teamText.text = "";
            winText.text = "";
            failText.text = "";
            ownerText.text = "【等待玩家】";
            trans.GetComponent<Image>().color = Color.gray;
        }
    }

    void RecvFight(ProtocolBase proto)
    {
        ProtocolBytes protocol = (ProtocolBytes)proto;
        //开始战斗
        Scenes.getInstance().SwitchSceneWithCallback("Battle", delegate () {
            MultiBattle.instance.StartBattle((ProtocolBytes)proto);
            Close();
        });
    }

}
