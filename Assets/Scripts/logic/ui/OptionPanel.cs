using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : PanelBase {

    private Button closeBtn;
    private Button startBtn;
    private Dropdown dropdown1;
    private Dropdown dropdown2;
    private int camp1;
    private int camp2;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "Prefabs/UI/OptionPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        closeBtn.onClick.AddListener(OnCloseClick);
        startBtn = skinTrans.Find("StartBtn").GetComponent<Button>();
        startBtn.onClick.AddListener(OnStartClick);
        dropdown1 = skinTrans.Find("Camp1/Dropdown").GetComponent<Dropdown>();
        dropdown2 = skinTrans.Find("Camp2/Dropdown").GetComponent<Dropdown>();
    }
    #endregion

    public void OnStartClick() {
        camp1 = dropdown1.value + 1;
        camp2 = dropdown2.value + 1;

        Scenes.getInstance().SwitchSingleScene("Battle", sceneLoaded);
        PanelManager.instance.ClosePanel(typeof(TitlePanel).ToString());  
    }

    public void OnCloseClick() {
        Close();
    }


    public void sceneLoaded()
    {
        Close();
        Battle.instance.StartTwoCampBattle(camp1, camp2);
    }

}
