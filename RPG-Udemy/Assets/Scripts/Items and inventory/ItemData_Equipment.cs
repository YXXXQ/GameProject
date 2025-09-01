using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 装备类型枚举
public enum EquipmentType
{
    Weapon,     // 武器
    Armor,      // 护甲
    Amulet,     // 护符
    Flask       // 药水
}

// ItemData_Equipment.cs摘要
// 装备物品数据类，继承自ItemData，包含装备特有的属性和方法
// 定义了装备对玩家属性的加成以及特殊效果

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;  // 装备类型
    public float itemCooldown;           // 物品冷却时间（用于可激活的装备）
    public ItemEffect[] itemEffects;      // 装备特殊效果数组

    [Header("主要属性")]
    public int strength;      // 力量：增加物理伤害和暴击伤害
    public int agility;       // 敏捷：增加闪避和暴击率
    public int intelligence;  // 智力：增加魔法伤害和魔法抗性
    public int vitality;      // 体力：增加最大生命值

    [Header("攻击属性")]
    public int damage;        // 基础伤害
    public int critChance;    // 暴击率
    public int critPower;     // 暴击伤害倍率

    [Header("防御属性")]
    public int health;            // 最大生命值
    public int armor;             // 物理护甲值
    public int evasion;           // 闪避率
    public int magicResistance;   // 魔法抗性

    [Header("魔法属性")]
    public int fireDamage;        // 火焰伤害
    public int iceDamage;         // 冰霜伤害
    public int lightingDamage;    // 闪电伤害

    [Header("Craft requirements")]
    public List<InventoryItem> craftRequirements;  // 制作该装备所需的材料列表
    private int minDescriptionLength;              // 描述最小行数（用于UI显示）

    // 触发装备特殊效果
    public void Effect(Transform _enemyPosition)
    {
        // 遍历并执行所有特殊效果
        foreach (var item in itemEffects)
        {
            item.ExecuteEffect(_enemyPosition);
        }
    }

    // 装备时添加属性修饰符
    public void AddModifiers()
    {
        // 获取玩家属性组件
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        
        // 添加主要属性修饰符
        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        // 添加攻击属性修饰符
        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critpower.AddModifier(critPower);

        // 添加防御属性修饰符
        playerStats.maxHealth.AddModifier(health);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        // 添加魔法属性修饰符
        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightningDamage.AddModifier(lightingDamage);
    }

    // 卸下装备时移除属性修饰符
    public void RemoveModifiers()
    {
        // 获取玩家属性组件
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        // 移除主要属性修饰符
        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        // 移除攻击属性修饰符
        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critpower.RemoveModifier(critPower);

        // 移除防御属性修饰符
        playerStats.maxHealth.RemoveModifier(health);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        // 移除魔法属性修饰符
        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightningDamage.RemoveModifier(lightingDamage);
    }
    
    // 生成装备描述文本
    public override string GetDescription()
    {
        // 重置字符串构建器和描述长度计数
        sb.Length = 0;
        minDescriptionLength = 0;
        
        // 添加主要属性描述
        AddItemDescription(strength, "Strength");
        AddItemDescription(agility, "Agility");
        AddItemDescription(intelligence, "Intelligence");
        AddItemDescription(vitality, "Vitality");

        // 添加攻击属性描述
        AddItemDescription(damage, "Damage");
        AddItemDescription(critChance, "Crit Chance");
        AddItemDescription(critPower, "Crit Power");

        // 添加防御属性描述
        AddItemDescription(health, "Health");
        AddItemDescription(evasion, "Evasion");
        AddItemDescription(armor, "Armor");
        AddItemDescription(magicResistance, "Magic Resistance");

        // 添加魔法属性描述
        AddItemDescription(fireDamage, "Fire Damage");
        AddItemDescription(iceDamage, "Ice Damage");
        AddItemDescription(lightingDamage, "Lighting Damage");

        // 添加特殊效果描述
        if (itemEffects != null && itemEffects.Length > 0)
        {
            for (int i = 0; i < itemEffects.Length; i++)
            {
                if (itemEffects[i] != null && !string.IsNullOrEmpty(itemEffects[i].effectDescription))
                {
                    sb.AppendLine();
                    sb.AppendLine("Unique:" + itemEffects[i].effectDescription);
                    minDescriptionLength++;
                }
            }
        }

        // 确保描述至少有5行（为UI布局保持一致性）
        if (minDescriptionLength < 5)
        {
            for (int i = 0; i < 5 - minDescriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }

        return sb.ToString();
    }
    
    // 添加单个属性描述
    private void AddItemDescription(int _value, string _name)
    {
        // 只添加非零属性
        if (_value != 0)
        {
            // 如果已有内容，先添加换行
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }
            
            // 添加属性名称和值
            if (_value > 0)
            {
                sb.Append(_name + " :" + _value);
            }
            
            // 增加描述行数计数
            minDescriptionLength++;
        }
    }
}
