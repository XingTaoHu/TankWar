using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginPanel : PanelBase {

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
        closeBtn = skinTrans.FindChild("CloseBtn").GetComponent<Button>();
        loginBtn = skinTrans.FindChild("LoginBtn").GetComponent<Button>();
        usernameInput = skinTrans.FindChild("UsernameInput").GetComponent<InputField>();
        passwordInput = skinTrans.FindChild("PasswordInput").GetComponent<InputField>();
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
            SceneManager.LoadScene("Battle");
            PanelManager.instance.ClosePanel(typeof(TitlePanel).ToString());
            Close();
        }
        else
        {
            Debug.LogError("用户名或者密码为空!!!");    
        }
    }

}
