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
        if(string.IsNullOrEmpty(idInput.text) || string.IsNullOrEmpty(pwInput.text))
        {
            PanelManager.instance.OpenPanel<TipPanel>("", "用户名或者密码为空！");
            return;
        }
        if(NetMgr.servConn.status != Connection.Status.Connected)
        {
            if(!NetMgr.servConn.Connect("127.0.0.1", 1234))
                PanelManager.instance.OpenPanel<TipPanel>("", "连接服务器失败！");
        }
        //发送登录协议
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Login");
        protocol.AddString(idInput.text);
        protocol.AddString(pwInput.text);
        Debug.Log("发送:" + protocol.GetDesc());
        NetMgr.servConn.Send(protocol, OnLoginCallback);
    }

    public void OnRegistClick()
    {
        PanelManager.instance.OpenPanel<RegisterPanel>("");
    }

    private void OnLoginCallback(ProtocolBase proto)
    {
        ProtocolBytes protocol = (ProtocolBytes)proto;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        int ret = protocol.GetInt(start, ref start);
        if(ret == 0)
        {
            Debug.Log("登录成功，开始游戏！");
            //开始游戏

            Close();
        }
        else
        {
            PanelManager.instance.OpenPanel<TipPanel>("", "登录失败，请检查用户名或密码！");
        }
    }


}
