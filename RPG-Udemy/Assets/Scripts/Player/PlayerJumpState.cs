using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerJumpState.cs摘要
/// 玩家跳跃状态，处理玩家跳跃的初始阶段
/// 当玩家开始下落时，会切换到空中状态
/// </summary>
public class PlayerJumpState : PlayerState
{
    /// <summary>
    /// 构造函数，初始化跳跃状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 进入跳跃状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        // 应用跳跃力，保持水平速度不变
        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);
    }

    /// <summary>
    /// 退出跳跃状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        // 无需额外操作
    }

    /// <summary>
    /// 更新跳跃状态
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        // 当垂直速度变为负值（开始下落）时，切换到空中状态
        if (rb.velocity.y < 0)
        {
            stateMachine.ChangeState(player.airState);
        }
    }
}
