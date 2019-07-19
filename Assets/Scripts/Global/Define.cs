using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 操控类型
/// </summary>
public enum CtrlType
{
    NONE,
    PLAYER,
    COMPUTER,
}

/// <summary>
/// 状态枚举
/// </summary>
public enum Status
{ 
    Patrol,
    Attack,
}