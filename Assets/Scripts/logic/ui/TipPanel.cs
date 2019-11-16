using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : PanelBase
{

    private Text tips;
    private Button knowBtn;
    string str = "";

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "Prefabs/UI/TipPanel";
        layer = PanelLayer.Tips;
        if(args.Length == 1)
        {
            str = (string)args[0];
        }
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        tips = skinTrans.Find("tips").GetComponent<Text>();
        tips.text = str;
        knowBtn = skinTrans.Find("know").GetComponent<Button>();
        knowBtn.onClick.AddListener(OnKnowClick);
    }
    #endregion

    public void OnKnowClick()
    {
        Close();
    }

}
