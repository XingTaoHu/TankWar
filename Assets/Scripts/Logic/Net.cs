﻿using System.Collections;
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

    void Start()
    { 
        hostInput = transform.Find("HostInput").GetComponent<InputField>();
        portInput = transform.Find("PortInput").GetComponent<InputField>();
        textInput = transform.Find("TextInput").GetComponent<InputField>();
        recvText = transform.Find("RecvText").GetComponent<Text>();
        clientText = transform.Find("ClientText").GetComponent<Text>();
        connectBtn = transform.Find("ConnectBtn").GetComponent<Button>();
        sendBtn = transform.Find("SendBtn").GetComponent<Button>();
        connectBtn.onClick.AddListener(Connection);
        sendBtn.onClick.AddListener(Send);
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
        socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
    }

    //接收回调
    private void ReceiveCb(IAsyncResult ar) {
        try
        {
            //count是接收数据的大小
            int count = socket.EndReceive(ar);
            //数据处理
            buffCount += count;
            ProcessData();
            //继续接收
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
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
        if (buffCount > sizeof(Int32))
            return;
        //消息长度
        Array.Copy(readBuff, lenBytes, sizeof(Int32));
        msgLength = BitConverter.ToInt32(lenBytes, 0);
        if (buffCount < msgLength + sizeof(Int32))
            return;
        //处理消息
        string str = System.Text.Encoding.UTF8.GetString(readBuff, sizeof(Int32), msgLength);
        recvStr = str;
        //清除已处理的消息
        int count = buffCount - msgLength - sizeof(Int32);
        Array.Copy(readBuff, msgLength, readBuff, 0, count);
        buffCount = count;
        if (buffCount > 0)
            ProcessData();
    }

    public void Send() {
        string str = textInput.text;
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        byte[] length = BitConverter.GetBytes(bytes.Length);
        byte[] sendbuff = length.Concat(bytes).ToArray();
        try
        {
            socket.Send(sendbuff);
        }
        catch (Exception e){
            Debug.LogError("发送数据错误：" + e.Message);
        }
    }

}
