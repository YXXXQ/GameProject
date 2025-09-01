using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerWallJumpState.cs摘要
/// 玩家墙跳状态，处理玩家从墙壁蹬跳的行为
/// 包括墙跳力度、方向控制和状态转换逻辑
/// </summary>
public class PlayerWallJumpState : PlayerState
{
    private float wallJumpForce = 5f; // 蹬墙跳的水平力度
    private float jumpTime = 0.4f;    // 墙跳状态持续时间

    /// <summary>
    /// 构造函数，初始化墙跳状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayerWallJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 进入墙跳状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        // 设置墙跳状态持续时间
        stateTimer = jumpTime;
        
        // 计算墙跳方向（与面向墙壁的方向相反）
        float jumpDirection = -player.facingDir;
        // 设置墙跳速度（水平和垂直分量）
        player.SetVelocity(wallJumpForce * jumpDirection, player.jumpForce);
        
        // 翻转角色朝向（离开墙壁）
        player.Flip();
    }

    /// <summary>
    /// 退出墙跳状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        // 无需额外操作
    }

    /// <summary>
    /// 更新墙跳状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        // 墙跳状态持续时间结束后，切换到空中状态
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.airState);
        }

        // 允许玩家在墙跳过程中稍微控制移动方向
        if (xInput != 0)
        {
            // 空中移动速度减半，提供有限的控制
            float moveSpeed = player.moveSpeed * 0.5f;
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }

        // 如果检测到地面，切换到待机状态
        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
