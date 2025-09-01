using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stat.cs摘要
// 属性类，用于管理游戏中各种数值属性（如力量、敏捷等）的基础值和修饰符

[System.Serializable]
public class Stat
{
    [SerializeField] private int baseValue;  // 属性基础值

    public List<int> modifiers;              // 属性修饰符列表
    
    // 获取属性最终值（基础值+所有修饰符）
    public int GetValue()
    {
        int finalValue = baseValue;
        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }
        return finalValue;
    }

    // 设置属性基础值
    public void SetDefaultValue(int _value)
    {
        baseValue = _value;
    }
    
    // 添加属性修饰符
    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }
    
    // 移除属性修饰符
    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }
}
