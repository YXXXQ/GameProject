using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerState.cs摘要
// 玩家状态基类，所有玩家状态的基础实现，包含状态进入、更新、退出的通用逻辑

public class PlayerState 
{
    protected PlayerStateMachine stateMachine; // 状态机引用
    protected Player player;                   // 玩家引用

    protected Rigidbody2D rb;                  // 刚体组件引用

    protected float xInput;                    // 水平输入值
    protected float yInput;                    // 垂直输入值
    private string animBoolName;               // 动画布尔参数名称

    protected float stateTimer;                // 状态计时器
    protected bool triggerCalled;              // 动画触发器是否已调用
    
    // 构造函数，初始化状态
    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    // 进入状态时调用
    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true); // 激活对应的动画状态
        rb = player.rb;                          // 获取刚体引用
        triggerCalled = false;                   // 重置触发器状态
    }
    
    // 状态更新时调用
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;            // 更新状态计时器
        xInput = Input.GetAxisRaw("Horizontal"); // 获取水平输入
        yInput = Input.GetAxisRaw("Vertical");   // 获取垂直输入
        player.anim.SetFloat("yVelocity", rb.velocity.y); // 更新垂直速度动画参数
    }
    
    // 退出状态时调用
    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false); // 关闭对应的动画状态
    }
    
    // 动画完成时的触发回调
    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true; // 标记触发器已调用
    }
}
