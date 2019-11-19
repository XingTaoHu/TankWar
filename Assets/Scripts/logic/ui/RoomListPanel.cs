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

}
