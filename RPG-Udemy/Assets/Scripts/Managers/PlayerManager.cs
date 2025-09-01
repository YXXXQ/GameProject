using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerManager.cs摘要
// 玩家管理器，负责管理玩家实例和货币系统
// 实现了ISaveManager接口，支持玩家数据的存档和读档

public class PlayerManager : MonoBehaviour, ISaveManager
{
    // 单例模式，提供全局访问点
    public static PlayerManager instance;
    
    // 玩家引用
    public Player player;

    // 玩家当前货币数量
    public int currency;

    // 初始化单例
    private void Awake()
    {
        // 确保场景中只有一个PlayerManager实例
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    // 检查玩家是否有足够的货币购买物品或技能
    // 参数：_price - 物品或技能的价格
    // 返回：如果有足够货币则返回true并扣除相应金额，否则返回false
    public bool HaveEnoughMoney(int _price)
    {
        if (_price > currency)
        {
            Debug.Log("没有足够的钱");
            return false;
        }
        else
        {
            currency -= _price;
            return true;
        }
    }

    // 获取当前货币数量
    public int GetCurrency() => currency;

    // 从存档加载玩家数据（ISaveManager接口实现）
    public void LoadData(GameData _data)
    {
        currency = _data.currency;
    }

    // 保存玩家数据到存档（ISaveManager接口实现）
    public void SaveData(ref GameData _data)
    {
        _data.currency = this.currency;
    }
}
