using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : PanelBase
{
    private Text idText;
    private Text winText;
    private Text loseText;
    private Transform content;
    private GameObject roomPrefab;
    private Button closeBtn;
    private Button newBtn;
    private Button refreshBtn;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "Prefabs/UI/RoomListPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idText = skinTrans.Find("WinImage/comanderText").GetComponent<Text>();
        winText = skinTrans.Find("WinImage/winandlose/win").GetComponent<Text>();
        loseText = skinTrans.Find("WinImage/winandlose/lose").GetComponent<Text>();
        content = skinTrans.Find("ListImage/ScrollRect/Content");
        roomPrefab = content.Find("RoomPrefab").gameObject;
        roomPrefab.SetActive(false);
        closeBtn = skinTrans.Find("ListImage/closeBtn").GetComponent<Button>();
        newBtn = skinTrans.Find("ListImage/newBtn").GetComponent<Button>();
        refreshBtn = skinTrans.Find("ListImage/refreshBtn").GetComponent<Button>();
        newBtn.onClick.AddListener(OnNewClick);
        refreshBtn.onClick.AddListener(OnRefreshClick);
        closeBtn.onClick.AddListener(OnCloseClick);

        //监听
        NetMgr.servConn.msgDist.AddListener("GetAchieve", RecvGetAchieve);
        NetMgr.servConn.msgDist.AddListener("GetRoomList", RecvGetRoomList);
        //发送查询
        ProtocolBytes protoGetRoomList = new ProtocolBytes();
        protoGetRoomList.AddString("GetRoomList");
        NetMgr.servConn.Send(protoGetRoomList);
        ProtocolBytes protoGetAchieve = new ProtocolBytes();
        protoGetAchieve.AddString("GetAchieve");
        NetMgr.servConn.Send(protoGetAchieve);

        //测试用
        ProtocolBytes protoTest = new ProtocolBytes();
        protoTest.AddString("GetRoomList");
        protoTest.AddInt(2);

        protoTest.AddInt(2);
        protoTest.AddInt(1);

        protoTest.AddInt(4);
        protoTest.AddInt(2);
        RecvGetRoomList(protoTest);
    }

    public override void OnClosing()
    {
        base.OnClosing();
        NetMgr.servConn.msgDist.DelListener("GetAchieve", RecvGetAchieve);
        NetMgr.servConn.msgDist.DelListener("GetRoomList", RecvGetRoomList);
    }
    #endregion



    void OnNewClick() 
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("CreateRoom");
        NetMgr.servConn.Send(protocol, OnNewCallback);
    }

    void OnNewCallback(ProtocolBase proto) 
    {
        ProtocolBytes protocol = (ProtocolBytes)proto;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        int ret = protocol.GetInt(start, ref start);
        if (ret == 0)
        {
            Debug.Log("房间创建成功");
            PanelManager.instance.OpenPanel<RoomPanel>("");
            Close();
        }
        else
        { 
            PanelManager.instance.OpenPanel<TipPanel>("", "创建房间失败！");
        }
    }

    void OnRefreshClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomList");
        NetMgr.servConn.Send(protocol);
    }

    void OnCloseClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Logout");
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
            Debug.Log("退出登录成功！");
            PanelManager.instance.OpenPanel<LoginPanel>("");
            NetMgr.servConn.Close();
            Close();
        }
        else
        {
            Debug.Log("退出登录失败！");
        }
    }

    //收到GetAchieve协议
    public void RecvGetAchieve(ProtocolBase proto)
    {
        //解析协议
        ProtocolBytes protocol = (ProtocolBytes)proto;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        int win = protocol.GetInt(start, ref start);
        int lose = protocol.GetInt(start, ref start);
        //处理
        idText.text = "指挥官:" + GameMgr.instance.id;
        winText.text = win.ToString();
        loseText.text = lose.ToString();
    }

    //收到GetRoomList协议
    public void RecvGetRoomList(ProtocolBase proto)
    {
        ClearRoomUnit();
        ProtocolBytes protocol = (ProtocolBytes)proto;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        int count = protocol.GetInt(start, ref start);
        for (int i = 0; i < count; i++)
        {
            int num = protocol.GetInt(start, ref start);
            int status = protocol.GetInt(start, ref start);
            GenerateRoomUnit(i, num, status);
        }
    }

    //清空房间
    public void ClearRoomUnit()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            if (content.GetChild(i).name.Contains("Clone"))
                Destroy(content.GetChild(i).gameObject);
        }
    }

    //创建一个房间单元
    public void GenerateRoomUnit(int i, int num, int status)
    {
        GameObject o = Instantiate(roomPrefab);
        o.transform.SetParent(content);
        o.SetActive(true);
        //房间信息
        Transform trans = o.transform;
        Text nameText = trans.Find("nameText").GetComponent<Text>();
        Text countText = trans.Find("countText").GetComponent<Text>();
        Text statusText = trans.Find("statusText").GetComponent<Text>();
        nameText.text = "序号:" + (i + 1).ToString();
        countText.text = "人数:" + num.ToString();
        if (status == 1)
        {
            statusText.color = Color.black;
            statusText.text = "状态:准备中";
        }
        else
        {
            statusText.color = Color.red;
            statusText.text = "状态:战斗中";
        }
        Button btn = trans.Find("joinBtn").GetComponent<Button>();
        btn.name = i.ToString();
        btn.onClick.AddListener(delegate() {
            OnJoinBtnClick(btn.name);
        });
    }

    //加入房间
    public void OnJoinBtnClick(string name)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("EnterRoom");
        protocol.AddInt(int.Parse(name));
        Debug.Log("请求进入房间:" + name);
        NetMgr.servConn.Send(protocol, OnJoinCallnback);
    }

    //加入房间按钮返回
    public void OnJoinCallnback(ProtocolBase proto)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        int ret = protocol.GetInt(start, ref start);
        if (ret == 0)
        {
            Debug.Log("成功进入房间!");
            PanelManager.instance.OpenPanel<RoomPanel>("");
            Close();
        }
        else
        { 
            PanelManager.instance.OpenPanel<TipPanel>("", "进入房间失败!");
        }
    }
}
