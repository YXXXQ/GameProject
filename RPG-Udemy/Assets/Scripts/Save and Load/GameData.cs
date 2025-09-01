using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameData.cs摘要
/// 游戏存档数据类，包含所有需要保存的游戏状态信息
/// 使用可序列化字典和列表存储各种游戏数据
/// </summary>
[System.Serializable]
public class GameData
{
    // 玩家资源
    public int currency;                                      // 玩家当前拥有的灵魂货币数量

    // 技能和物品数据
    public SerializableDictionary<string, bool> skillTree;    // 技能树解锁状态，键为技能ID，值为是否解锁
    public SerializableDictionary<string, int> inventory;     // 物品库存，键为物品ID，值为数量
    public List<string> equipmentId;                          // 已装备物品的ID列表

    // 检查点数据
    public SerializableDictionary<string, bool> checkpoints;  // 检查点激活状态，键为检查点ID，值为是否激活
    public string closestCheckpointId;                        // 玩家最近激活的检查点ID，用于重生点

    // 死亡掉落数据
    public float lostCurrencyX;                               // 玩家死亡掉落灵魂的X坐标
    public float lostCurrencyY;                               // 玩家死亡掉落灵魂的Y坐标
    public int lostCurrencyAmount;                            // 玩家死亡掉落的灵魂数量

    // 设置数据
    public SerializableDictionary<string, float> volumeSettings;  // 音量设置，键为音频类型，值为音量大小

    /// <summary>
    /// 创建新的游戏数据对象，初始化所有数据结构
    /// </summary>
    public GameData()
    {
        // 初始化死亡掉落数据
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;

        // 初始化玩家资源
        this.currency = 0;
        
        // 初始化技能和物品数据
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentId = new List<string>();

        // 初始化检查点数据
        closestCheckpointId = string.Empty;
        checkpoints = new SerializableDictionary<string, bool>();
        
        // 初始化设置数据
        volumeSettings = new SerializableDictionary<string, float>();
    }
}
