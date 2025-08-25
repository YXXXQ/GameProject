using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 游戏空闲管理器，负责处理游戏的经济系统和离线收益
public class IdleManager : MonoBehaviour
{
    [HideInInspector]
    public int length;              // 钓鱼深度

    [HideInInspector]
    public int strength;            // 钓鱼力量（可钓鱼数量）

    [HideInInspector]
    public int offlineEarnings;     // 离线收益率

    [HideInInspector]
    public int lengthCost;          // 升级深度的成本

    [HideInInspector]
    public int strengthCost;        // 升级力量的成本

    [HideInInspector]
    public int offlineEarningsCost; // 升级离线收益的成本

    [HideInInspector]
    public int wallet;              // 玩家钱包余额

    [HideInInspector]
    public int totalGain;           // 当前总收益

    // 各级升级的成本数组
    private int[] costs = new int[]
    {
        120, 151, 197, 250, 324, 414, 537, 687, 892, 1145,
        1484, 1911, 2479, 3196, 4148, 5359, 6954, 9000, 11687
    };

    // 单例实例
    public static IdleManager instance;
    
    // 初始化
    void Awake()
    {
        // 单例模式实现
        if (IdleManager.instance)
            UnityEngine.Object.Destroy(gameObject);
        else
            IdleManager.instance = this;

        // 从PlayerPrefs加载游戏数据
        length = -PlayerPrefs.GetInt("Length", 30);
        strength = PlayerPrefs.GetInt("Strength", 3);
        offlineEarnings = PlayerPrefs.GetInt("Offline", 3);
        
        // 计算各项升级的成本
        lengthCost = costs[-length / 10 - 3];
        strengthCost = costs[strength - 3];
        offlineEarningsCost = costs[offlineEarnings - 3];
        
        // 加载钱包余额
        wallet = PlayerPrefs.GetInt("Wallet", 0);
    }

    // 移除了Update方法，退出功能已集中到ScreensManager中

    // 处理游戏暂停和恢复（用于计算离线收益）
    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            // 游戏暂停时记录当前时间
            DateTime now = DateTime.Now;
            PlayerPrefs.SetString("Date", now.ToString());
            MonoBehaviour.print(now.ToString());
        }
        else
        {
            // 游戏恢复时计算离线收益
            string @string = PlayerPrefs.GetString("Date", string.Empty);
            if (@string != string.Empty)
            {
                DateTime d = DateTime.Parse(@string);
                // 根据离线时间和离线收益率计算总收益
                totalGain = (int)((DateTime.Now - d).TotalMinutes * offlineEarnings + 1.0);
                ScreensManager.instance.ChangeScreen(Screens.RETURN);
            }
        }
    }

    // 游戏退出时保存数据
    private void OnApplicationQuit()
    {
        OnApplicationPause(true);
    }

    // 购买/升级钓鱼深度
    public void BuyLength()
    {
        length -= 10;  // 深度值为负数，减小表示增加深度
        wallet -= lengthCost;
        lengthCost = costs[-length / 10 - 3];  // 计算新的升级成本
        PlayerPrefs.SetInt("Length", -length);
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreensManager.instance.ChangeScreen(Screens.MAIN);
    }

    // 购买/升级钓鱼力量
    public void BuyStrength()
    {
        strength++;
        wallet -= strengthCost;
        strengthCost = costs[strength - 3];  // 计算新的升级成本
        PlayerPrefs.SetInt("Strength", strength);
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreensManager.instance.ChangeScreen(Screens.MAIN);
    }

    // 购买/升级离线收益
    public void BuyOfflineEarnings()
    {
        offlineEarnings++;
        wallet -= offlineEarningsCost;
        offlineEarningsCost = costs[offlineEarnings - 3];  // 计算新的升级成本
        PlayerPrefs.SetInt("Offline", offlineEarnings);
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreensManager.instance.ChangeScreen(Screens.MAIN);
    }

    // 收集钱币
    public void CollectMoney()
    {
        wallet += totalGain;
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreensManager.instance.ChangeScreen(Screens.MAIN);
    }
}