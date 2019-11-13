using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Walk : MonoBehaviour{
    //预设
    public GameObject prefab;
    //玩家列表
    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    //self
    string playerID = "";
    //上一次移动的时间
    public float lastMoveTime;
    //单例
    public static Walk instance;

    private void Start()
    {
        instance = this;
    }

    //添加玩家
    void AddPlayer(string id, Vector3 pos, int score)
    {
        GameObject player = (GameObject)Instantiate(prefab, pos, Quaternion.identity);
        TextMesh textMesh = player.GetComponentInChildren<TextMesh>();
        textMesh.text = id + ":" + score;
        players.Add(id, player);
    }

    //删除玩家
    void DelPlayer(string id)
    {
        //已经初始化该玩家
        if(players.ContainsKey(id))
        {
            Destroy(players[id]);
            players.Remove(id);
        }
    }

    //更新分数
    public void UpdateScore(string id, int score)
    {
        GameObject player = players[id];
        if (player == null)
            return;
        TextMesh textMesh = player.GetComponentInChildren<TextMesh>();
        textMesh.text = id + ":" + score;
    }

    //更新信息
    public void UpdateInfo(string id, Vector3 pos, int score)
    {
        //只更新自己的分数
        if(id == playerID)
        {
            UpdateScore(id, score);
            return;
        }
        //其他人
        if(players.ContainsKey(id))
        {
            players[id].transform.position = pos;
            UpdateScore(id, score);
        }
        else
        {
            AddPlayer(id, pos, score);
        }
    }

    public void StartGame(string id)
    {
        playerID = id;
        //生成自己
        UnityEngine.Random.seed = (int)DateTime.Now.Ticks;
        float x = 100 + UnityEngine.Random.Range(-30, 30);
        float y = 0;
        float z = 100 + UnityEngine.Random.Range(-30, 30);
        Vector3 pos = new Vector3(x, y, z);
        AddPlayer(id, pos, 0);
        //同步

        //获取列表
    }

    //同步位置
    void SendPos()
    {

    }

}

//public class Walk : MonoBehaviour {

//    //Socket
//    Socket socket;
//    const int BUFFER_SIZE = 1024;
//    private byte[] readBuff = new byte[BUFFER_SIZE];
//    //玩家列表
//    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
//    //消息列表
//    List<string> msgList = new List<string>();
//    //Player预设
//    public GameObject prefab;
//    //自己的IP和端口
//    string id;

//    string ServerIP = "127.0.0.1";
//    int ServerPort = 1234;

//	// Use this for initialization
//	void Start () {
//        Connect();
//        //请求其他玩家列表
//        //随机位置
//        UnityEngine.Random.seed = (int)DateTime.Now.Ticks;
//        float x = 100 + UnityEngine.Random.Range(-30, 30);
//        float y = 0;
//        float z = 100 + UnityEngine.Random.Range(-30, 30);
//        Vector3 pos = new Vector3(x, y, z);
//        AddPlayer(id, pos);
//        //同步
//        SendPos();
//	}
	
//	// Update is called once per frame
//	void Update () {
//        for (int i = 0; i < msgList.Count; i++)
//        {
//            HandleMsg();
//        }
//        //移动
//        Move();
//	}

//    //连接
//    void Connect()
//    {
//        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//        socket.Connect(ServerIP, ServerPort);
//        id = socket.LocalEndPoint.ToString();
//        socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
//    }

//    //接收回调
//    private void ReceiveCb(IAsyncResult ar)
//    {
//        try
//        {
//            int count = socket.EndReceive(ar);
//            string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
//            msgList.Add(str);
//            //继续接收
//            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
//        }
//        catch (Exception e) {
//            socket.Close();
//        }
//    }

//    //处理消息列表
//    void HandleMsg()
//    {
//        if (msgList.Count <= 0)
//            return;
//        string str = msgList[0];
//        msgList.RemoveAt(0);
//        string[] args = str.Split(' ');
//        if (args[0] == "POS")
//        {
//            OnRecvPos(args[1], args[2], args[3], args[4]);
//        }
//        else if (args[0] == "LEAVE")
//        {
//            OnRecvLeave(args[1]);    
//        }
//    }

//    //处理更新位置协议
//    public void OnRecvPos(string id, string xStr, string yStr, string zStr)
//    {
//        if (id == this.id)
//            return;
//        float x = float.Parse(xStr);
//        float y = float.Parse(yStr);
//        float z = float.Parse(zStr);
//        Vector3 pos = new Vector3(x, y, z);
//        if (players.ContainsKey(id))
//        {
//            players[id].transform.position = pos;
//        }
//        else
//        {
//            AddPlayer(id, pos);
//        }
//    }

//    //处理玩家离开的协议
//    public void OnRecvLeave(string id)
//    {
//        if (players.ContainsKey(id))
//        {
//            Destroy(players[id]);
//            players[id] = null;
//        }
//    }

//    //添加Player
//    void AddPlayer(string id, Vector3 pos)
//    {
//        GameObject player = (GameObject)Instantiate(prefab, pos, Quaternion.identity);
//        TextMesh textMesh = player.GetComponentInChildren<TextMesh>();
//        textMesh.text = id;
//        players.Add(id, player);
//    }

//    //发送位置协议
//    void SendPos()
//    { 
//        GameObject player = players[id];
//        Vector3 pos = player.transform.position;
//        //组装协议
//        string str = "POS ";
//        str += id + " ";
//        str += pos.x.ToString() + " ";
//        str += pos.y.ToString() + " ";
//        str += pos.z.ToString() + " ";
//        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
//        socket.Send(bytes);
//        Debug.Log("发送：" + str);
//    }

//    //发送离开协议
//    void SendLeave()
//    { 
//        //组装协议
//        string str = "LEAVE ";
//        str += id + " ";
//        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
//        socket.Send(bytes);
//        Debug.Log("发送:" + str);
//    }

//    void Move()
//    {
//        if (string.IsNullOrEmpty(id))
//            return;
//        GameObject player = players[id];
//        if (Input.GetKey(KeyCode.UpArrow)) {
//            player.transform.position += new Vector3(0, 0, 1);
//            SendPos();
//        }
//        else if (Input.GetKey(KeyCode.DownArrow))
//        {
//            player.transform.position += new Vector3(0, 0, -1);
//            SendPos();
//        }
//        else if (Input.GetKey(KeyCode.LeftArrow))
//        {
//            player.transform.position += new Vector3(-1, 0, 0);
//            SendPos();
//        }
//        else if (Input.GetKey(KeyCode.RightArrow))
//        {
//            player.transform.position += new Vector3(1, 0, 0);
//            SendPos();
//        }
//    }

//    //离开
//    void OnDestory()
//    {
//        SendLeave();
//    }
//}
