using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : PanelBase {

    private Button closeBtn;
    private Button loginBtn;
    private InputField usernameInput;
    private InputField passwordInput;

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
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        loginBtn = skinTrans.Find("LoginBtn").GetComponent<Button>();
        usernameInput = skinTrans.Find("UsernameInput").GetComponent<InputField>();
        passwordInput = skinTrans.Find("PasswordInput").GetComponent<InputField>();
        closeBtn.onClick.AddListener(OnCloseClicked);
        loginBtn.onClick.AddListener(OnLoginClicked);
    }
    #endregion

    private void OnCloseClicked()
    {
        Close();
    }

    private void OnLoginClicked() 
    {
        if (!string.IsNullOrEmpty(usernameInput.text) && !string.IsNullOrEmpty(passwordInput.text))
        {
            //Scenes.getInstance().SwitchScene("Battle");
            //PanelManager.instance.ClosePanel(typeof(TitlePanel).ToString());
            //Close();
            PanelManager.instance.OpenPanel<OptionPanel>("");
            Close();
        }
        else
        {
            Debug.LogError("用户名或者密码为空!!!");    
        }
    }

}
