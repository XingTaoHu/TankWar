using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationLoginPanel : PanelBase
{
    private InputField userInput;
    private InputField pwInput;
    private Button loginBtn;
    private Button registBtn;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "Prefabs/UI/LocationLoginPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        userInput = skinTrans.Find("userInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("pwInput").GetComponent<InputField>();
        loginBtn = skinTrans.Find("loginBtn").GetComponent<Button>();
        registBtn = skinTrans.Find("registBtn").GetComponent<Button>();
        loginBtn.onClick.AddListener(OnLoginClick);
        registBtn.onClick.AddListener(OnRegistClick);
    }
    #endregion

    private void OnLoginClick()
    {
        if(string.IsNullOrEmpty(userInput.text) || string.IsNullOrEmpty(pwInput.text))
        {
            Debug.LogError("用户名或者密码为空!");
            return;
        }

        if(NetMgr.servConn.status != Connection.Status.Connected)
        {
            NetMgr.servConn.Connect("127.0.0.1", 1234);
        }

        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Login");
        protocol.AddString(userInput.text);
        protocol.AddString(pwInput.text);
        Debug.Log("发送:" + protocol.GetDesc());
        NetMgr.servConn.Send(protocol, LoginCallback);
    }

    private void OnRegistClick()
    {
        PanelManager.instance.OpenPanel<LocationRegistPanel>("");
        Close();
    }

    private void LoginCallback(ProtocolBase proto)
    {
        ProtocolBytes protocol = (ProtocolBytes)proto;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        int ret = protocol.GetInt(start, ref start);
        if(ret == 0)
        {
            Debug.Log("登录成功!");
            //开始游戏
            Walk.instance.StartGame(userInput.text);
            Close();
        }
        else
        {
            Debug.Log("登录失败!");
        }
    }

}
