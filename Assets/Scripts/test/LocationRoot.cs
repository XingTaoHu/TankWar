using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationRoot : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PanelManager.instance.OpenPanel<LoginPanel>("");
    }
	
	// Update is called once per frame
	void Update () {
        NetMgr.Update();
    }
}
