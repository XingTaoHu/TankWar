using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Scenes.getInstance().SwitchScene("Start");
        PanelManager.instance.OpenPanel<TitlePanel>("");
	}
	
	// Update is called once per frame
	void Update () {
        NetMgr.Update();
	}
}
