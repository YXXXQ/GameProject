using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EnemyState.cs摘要
// 敌人状态基类，所有敌人状态的基础实现，包含状态进入、更新、退出的通用逻辑

public class EnemyState
{
    protected EnemyStateMachine stateMachine; // 状态机引用
    protected Enemy enemyBase;                // 敌人基类引用
    protected Rigidbody2D rb;                 // 刚体组件引用

    protected bool triggerCalled;             // 动画触发器是否已调用
    private string animBoolName;              // 动画布尔参数名称

    protected float stateTimer;               // 状态计时器

    // 构造函数，初始化状态
    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        this.enemyBase = _enemyBase;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    // 状态更新时调用
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;         // 更新状态计时器
    }

    // 进入状态时调用
    public virtual void Enter()
    {
        triggerCalled = false;                // 重置触发器状态
        rb = enemyBase.rb;                    // 获取刚体引用
        enemyBase.anim.SetBool(animBoolName, true); // 激活对应的动画状态
    }
    
    // 退出状态时调用
    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false); // 关闭对应的动画状态
    }

    // 动画完成时的触发回调
    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;                 // 标记触发器已调用
    }
}
