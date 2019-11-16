using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Scenes.getInstance().SwitchScene("Start");
        //PanelManager.instance.OpenPanel<LoginPanel>("");
        PanelManager.instance.OpenPanel<TipPanel>("", "用户名或者密码错误！");
    }
	
	// Update is called once per frame
	void Update () {
        NetMgr.Update();
	}
}
