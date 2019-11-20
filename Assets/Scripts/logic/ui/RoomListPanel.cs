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
        
    }

    void OnRefreshClick()
    { 
        
    }

    void OnCloseClick()
    { 
        
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
    }

}
