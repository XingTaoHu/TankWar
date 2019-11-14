using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationRegistPanel : PanelBase
{
    private InputField userInput;
    private InputField pwInput;
    private Button registBtn;
    private Button closeBtn;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "Prefabs/UI/LocationRegistPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        userInput = skinTrans.Find("userInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("pwInput").GetComponent<InputField>();
        registBtn = skinTrans.Find("registBtn").GetComponent<Button>();
        closeBtn = skinTrans.Find("closeBtn").GetComponent<Button>();
        registBtn.onClick.AddListener(OnRegistClick);
        closeBtn.onClick.AddListener(OnCloseClick);

        //增加tabcoll
        if (userInput.transform.GetComponent<Tabcoll>() == null)
        {
            userInput.gameObject.AddComponent<Tabcoll>();
        }
        if (pwInput.transform.GetComponent<Tabcoll>() == null)
        {
            pwInput.gameObject.AddComponent<Tabcoll>();
        }
    }
    #endregion

    private void OnRegistClick()
    {
        if(string.IsNullOrEmpty(userInput.text) || string.IsNullOrEmpty(pwInput.text))
        {
            Debug.Log("用户名或密码为空!");
            return;
        }

        if(NetMgr.servConn.status != Connection.Status.Connected)
        {
            NetMgr.servConn.Connect("127.0.0.1", 1234);
        }

        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Register");
        protocol.AddString(userInput.text);
        protocol.AddString(pwInput.text);
        Debug.Log("发送:" + protocol.GetDesc());
        NetMgr.servConn.Send(protocol, RegisterCallback);
    }

    private void OnCloseClick()
    {
        PanelManager.instance.OpenPanel<LocationLoginPanel>("");
        Close();
    }

    private void RegisterCallback(ProtocolBase proto)
    {
        ProtocolBytes protocol = (ProtocolBytes)proto;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        int ret = protocol.GetInt(start, ref start);
        if(ret == 0)
        {
            Debug.Log("注册成功!");
            PanelManager.instance.OpenPanel<LocationLoginPanel>("");
            Close();
        }
        else
        {
            Debug.Log("注册失败!");
        }
    }

}
