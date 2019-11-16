using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : PanelBase {

    private InputField idInput;
    private InputField pwInput;
    private Button loginBtn;
    private Button registBtn;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "Prefabs/UI/LoginPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("idInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("pwInput").GetComponent<InputField>();
        loginBtn = skinTrans.Find("loginBtn").GetComponent<Button>();
        registBtn = skinTrans.Find("registBtn").GetComponent<Button>();
        loginBtn.onClick.AddListener(OnLoginClick);
        registBtn.onClick.AddListener(OnRegistClick);

        //增加tabcoll
        if (idInput.transform.GetComponent<Tabcoll>() == null)
        {
            idInput.gameObject.AddComponent<Tabcoll>();
        }
        if (pwInput.transform.GetComponent<Tabcoll>() == null)
        {
            pwInput.gameObject.AddComponent<Tabcoll>();
        }
    }
    #endregion

    public void OnLoginClick()
    {

    }

    public void OnRegistClick()
    {

    }


}
