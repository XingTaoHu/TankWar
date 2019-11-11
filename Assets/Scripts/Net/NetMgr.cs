using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMgr 
{
    public static Connection servConn = new Connection();
    public static void Update()
    {
        servConn.Update();
    }

    //心跳
    public static ProtocolBase GetHeatBeatProtocol()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("HeatBeat");
        return protocol;
    }

}
