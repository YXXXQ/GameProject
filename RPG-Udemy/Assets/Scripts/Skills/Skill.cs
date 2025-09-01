using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Skill.cs摘要
// 技能基类，所有技能的基础实现，包含冷却时间管理和基本技能使用逻辑

public class Skill : MonoBehaviour
{
    public float cooldown;      // 技能冷却时间
    public float cooldownTimer; // 当前冷却计时器

    protected Player player;    // 玩家引用

    // 初始化技能
    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
        CheckUnlock(); // 检查技能是否已解锁
    }

    // 更新冷却时间
    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    // 检查技能是否解锁，由子类实现具体逻辑
    protected virtual void CheckUnlock()
    {
        // 子类中实现具体解锁逻辑
    }

    // 检查技能是否可以使用
    public virtual bool CanUseSkill()
    {
        if (cooldownTimer < 0) // 冷却时间已过
        {
            UseSkill();         // 使用技能
            cooldownTimer = cooldown; // 重置冷却时间
            return true;
        }
        player.fX.CreatePopUpText("Cooldown"); // 显示冷却中提示
        return false;
    }

    // 使用技能的具体实现，由子类重写
    public virtual void UseSkill()
    {
        // 子类中实现具体技能效果
    }
    // 查找最近的敌人
    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        // 获取指定位置周围25单位范围内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closesEnemy = null;

        // 遍历所有碰撞体，找出最近的敌人
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closesEnemy = hit.transform;
                }
            }
        }

        return closesEnemy; // 返回最近的敌人变换
    }
}
