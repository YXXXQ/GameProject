using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SkillManager.cs摘要
// 技能管理器，负责管理和提供对所有玩家技能的访问
// 使用单例模式，允许从游戏中的任何地方访问玩家技能

public class SkillManager : MonoBehaviour
{
    // 单例实例
    public static SkillManager instance;
    
    // 所有可用技能的属性
    public Dash_Skill dash { get; private set; }         // 冲刺技能
    public Clone_Skill clone { get; private set; }       // 分身技能
    public Sword_Skill sword { get; private set; }       // 剑技能
    public Blackhole_Skill blackhole { get; private set; } // 黑洞技能
    public Crystal_Skill crystal { get; private set; }   // 水晶技能
    public Parry_SKill parry { get; private set; }       // 格挡技能
    public Dodge_Skill dodge { get; private set; }       // 闪避技能

    // 初始化单例
    private void Awake()
    {
        // 确保场景中只有一个SkillManager实例
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    // 获取所有技能组件引用
    private void Start()
    {
        // 从当前游戏对象获取所有技能组件
        dash = GetComponent<Dash_Skill>();
        clone = GetComponent<Clone_Skill>();
        sword = GetComponent<Sword_Skill>();
        blackhole = GetComponent<Blackhole_Skill>();
        crystal = GetComponent<Crystal_Skill>();
        parry = GetComponent<Parry_SKill>();
        dodge = GetComponent<Dodge_Skill>();
    }
}
