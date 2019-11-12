using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo 
{
    /// <summary>
    /// 代表某一轴上的两个车轮碰撞器
    /// </summary>
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    /// <summary>
    /// 指明是否将发动机的马力传送给轴上的轮子
    /// </summary>
    public bool motor;
    /// <summary>
    /// 指明轮子是否转向，汽车都是前轮转向，因此前轴为true，后轴为false
    /// </summary>
    public bool steering;
}
