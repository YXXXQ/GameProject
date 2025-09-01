using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ISaveManager.cs摘要
/// 存档管理接口，定义了需要保存/加载数据的对象必须实现的方法
/// 所有需要参与存档系统的组件都应实现此接口
/// </summary>
public interface ISaveManager
{
    /// <summary>
    /// 从游戏数据中加载组件特定的数据
    /// </summary>
    /// <param name="_data">包含所有游戏数据的对象</param>
    void LoadData(GameData _data);
    
    /// <summary>
    /// 将组件特定的数据保存到游戏数据对象中
    /// </summary>
    /// <param name="_data">要保存数据的游戏数据对象</param>
    void SaveData(ref GameData _data);
}
