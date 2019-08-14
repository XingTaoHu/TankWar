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

/// <summary>
/// panel层级
/// </summary>
public enum PanelLayer{
    Panel,      //面板
    Tips,       //提示
}

//单场景加载完成后回调
public delegate void SingleSceneLoadedCallback();