using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System;
using System.Linq;

public class Net : MonoBehaviour {

	//与服务端的套接字
    Socket socket;
    //服务端的IP和端口+
    private InputField hostInput;
    private InputField portInput;
    private InputField textInput;
    //用户名密码登录按钮，获取添加分数
    private InputField idInput;
    private InputField pwInput;
    private Button loginBtn;
    private Button registerBtn;
    private Button setBtn;
    private Button getBtn;
    //文本框
    private Text recvText;
    private string recvStr;
    private Text clientText;
    //连接按钮
    private Button connectBtn;
    private Button sendBtn;
    //接收缓冲区
    const int BUFFER_SIZE = 1024;
    byte[] readBuff = new byte[BUFFER_SIZE];

    //增加的粘包分包处理
    int buffCount = 0;
    byte[] lenBytes = new byte[sizeof(UInt32)];
    Int32 msgLength = 0;

    //协议
    ProtocolBase proto = new ProtocolBase();

    void Start()
    { 
        hostInput = transform.Find("HostInput").GetComponent<InputField>();
        hostInput.text = "127.0.0.1";
        portInput = transform.Find("PortInput").GetComponent<InputField>();
        portInput.text = "1234";
        textInput = transform.Find("TextInput").GetComponent<InputField>();
        recvText = transform.Find("RecvText").GetComponent<Text>();
        clientText = transform.Find("ClientText").GetComponent<Text>();
        connectBtn = transform.Find("ConnectBtn").GetComponent<Button>();
        sendBtn = transform.Find("SendBtn").GetComponent<Button>();
        idInput = transform.Find("IDInput").GetComponent<InputField>();
        idInput.text = "hxt";
        pwInput = transform.Find("PWInput").GetComponent<InputField>();
        pwInput.text = "123456";
        loginBtn = transform.Find("LoginBtn").GetComponent<Button>();
        registerBtn = transform.Find("RegisterBtn").GetComponent<Button>();
        setBtn = transform.Find("SetBtn").GetComponent<Button>();
        getBtn = transform.Find("GetBtn").GetComponent<Button>();
        connectBtn.onClick.AddListener(Connection);
        sendBtn.onClick.AddListener(OnSendClick);
        loginBtn.onClick.AddListener(OnLoginClick);
        registerBtn.onClick.AddListener(OnRegisterClick);
        setBtn.onClick.AddListener(OnSetScoreClick);
        getBtn.onClick.AddListener(OnGetScoreClick);
        recvStr = "";
    }

    void Update()
    {
        recvText.text = recvStr;
    }

    public void Connection()
    {
        //清理text
        recvText.text = "";
        //Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Connect
        string host = hostInput.text;
        int port = int.Parse(portInput.text);
        Debug.Log("连接服务器：host:" + host + ", port:" + port);
        socket.Connect(host, port);
        clientText.text = socket.LocalEndPoint.ToString();
        //Recv
        socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
    }

    //接收回调
    private void ReceiveCb(IAsyncResult ar) {
        try
        {
            //count是接收数据的大小
            int count = socket.EndReceive(ar);
            Debug.Log("接收到的数据大小:" + count);
            //数据处理
            buffCount += count;
            ProcessData();
            //继续接收
            socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }
        catch (Exception e)
        {
            recvStr += "连接已断开。 \n";
            socket.Close();
        }
    }

    //数据处理
    private void ProcessData()
    {
        //小于字节长度
        if (buffCount < sizeof(Int32))
            return;
        //消息长度
        Array.Copy(readBuff, lenBytes, sizeof(Int32));
        msgLength = BitConverter.ToInt32(lenBytes, 0);
        if (buffCount < msgLength + sizeof(Int32))
            return;
        //处理消息
        string str = System.Text.Encoding.UTF8.GetString(readBuff, sizeof(Int32), msgLength);
        recvStr = str;
        //ProtocolBase protocol = proto.Decode(readBuff, sizeof(Int32), msgLength);
        //HandleMsg(protocol);
        //清除已处理的消息
        int count = buffCount - msgLength - sizeof(Int32);
        Array.Copy(readBuff, msgLength, readBuff, 0, count);
        buffCount = count;
        if (buffCount > 0)
            ProcessData();
    }

    private void HandleMsg(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        //获取参数 
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //显示
        Debug.Log("接收:" + proto.GetDesc());
        recvStr = "接收 " + protoName + " " + ret.ToString();
    }

    public void OnSendClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("HeatBeat");
        Debug.Log("发送:" + protocol.GetDesc());
        Send(protocol);
    }

    public void Send(ProtocolBase protocol) {
        byte[] bytes = protocol.Encode();
        byte[] length = BitConverter.GetBytes(bytes.Length);
        byte[] sendbuff = length.Concat(bytes).ToArray();
        Debug.Log("bytes length:" + bytes.Length + ", length length:" + length.Length + ", sendbuff length:" + sendbuff.Length);
        try
        {
            //socket.Send(sendbuff);
            //这里是异步发送，可以使用类似粘包分包处理方法确保sendbuff的全部内容被发送出去
            socket.BeginSend(sendbuff, 0, sendbuff.Length, SocketFlags.None, null, null);
        }
        catch (Exception e){
            Debug.LogError("发送数据错误：" + e.Message);
        }
    }

    /// <summary>
    /// 注册
    /// </summary>
    public void OnRegisterClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Register");
        protocol.AddString(idInput.text);
        protocol.AddString(pwInput.text);
        Debug.Log("发送 " + protocol.GetDesc());
        Send(protocol);
    }

    /// <summary>
    /// 登录
    /// </summary>
    public void OnLoginClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Login");
        protocol.AddString(idInput.text);
        protocol.AddString(pwInput.text);
        Debug.Log("发送 " + protocol.GetDesc());
        Send(protocol);
    }

    /// <summary>
    /// 增加分数
    /// </summary>
    public void OnSetScoreClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("AddScore");
        Debug.Log("发送 " + protocol.GetDesc());
        Send(protocol);
    }

    /// <summary>
    /// 获取分数
    /// </summary>
    public void OnGetScoreClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetScore");
        Debug.Log("发送 " + protocol.GetDesc());
        Send(protocol);
    }

   

}
