﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : PanelBase {

    private Button startBtn;
    private Button infoBtn;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "Prefabs/UI/TitlePanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        startBtn = skinTrans.Find("StartBtn").GetComponent<Button>();
        infoBtn = skinTrans.Find("InfoBtn").GetComponent<Button>();
        startBtn.onClick.AddListener(OnStartClick);
        infoBtn.onClick.AddListener(OnInfoClick);
    }
    #endregion

    public void OnStartClick()
    {
        //开始游戏
        //Battle.instance.StartTwoCampBattle(2, 2);
        PanelManager.instance.OpenPanel<LoginPanel>("");
        //关闭
        //Close();
    }

    public void OnInfoClick(){
        PanelManager.instance.OpenPanel<InfoPanel>("");
    }


}