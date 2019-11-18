using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : PanelBase {

    private Button closeBtn;
    private Button registBtn;
    private InputField userInput;
    private InputField pwInput;
    private InputField repeatPwInput;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "Prefabs/UI/RegisterPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        registBtn = skinTrans.Find("registBtn").GetComponent<Button>();
        userInput = skinTrans.Find("userInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("passwordInput").GetComponent<InputField>();
        repeatPwInput = skinTrans.Find("repeatPwInput").GetComponent<InputField>();
        closeBtn.onClick.AddListener(OnCloseClicked);
        registBtn.onClick.AddListener(OnRegistClicked);
    }
    #endregion

    private void OnCloseClicked()
    {
        Close();
    }

    private void OnRegistClicked() 
    {
        if(string.IsNullOrEmpty(userInput.text) || string.IsNullOrEmpty(pwInput.text))
        {
            PanelManager.instance.OpenPanel<TipPanel>("", "用户名或密码不能为空！");
            return;
        }
        if(string.IsNullOrEmpty(repeatPwInput.text) || !pwInput.text.Equals(repeatPwInput.text))
        {
            PanelManager.instance.OpenPanel<TipPanel>("", "两次密码不一致！");
            return;
        }
        if(NetMgr.servConn.status != Connection.Status.Connected)
        {
            if (!NetMgr.servConn.Connect("127.0.0.1", 1234))
                PanelManager.instance.OpenPanel<TipPanel>("", "连接服务器失败！");
        }
        //发送
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Register");
        protocol.AddString(userInput.text);
        protocol.AddString(pwInput.text);
        Debug.Log("发送:" + protocol.GetDesc());
        NetMgr.servConn.Send(protocol, OnRegistCallback);
    }

    private void OnRegistCallback(ProtocolBase proto)
    {
        ProtocolBytes protocol = (ProtocolBytes)proto;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        int ret = protocol.GetInt(start, ref start);
        if(ret == 0)
        {
            Debug.Log("注册成功！");
            Close();
        }
        else
        {
            PanelManager.instance.OpenPanel<TipPanel>("", "注册失败，请检查用户名或密码！");
        }
    }

}
