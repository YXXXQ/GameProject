using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerAirState.cs摘要
/// 玩家空中状态，处理玩家在空中（跳跃后下落或从平台掉落）的行为
/// 包括空中移动控制和状态转换逻辑
/// </summary>
public class PlayerAirState : PlayerState
{
    /// <summary>
    /// 构造函数，初始化空中状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 进入空中状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        // 无需额外操作
    }

    /// <summary>
    /// 退出空中状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        // 无需额外操作
    }

    /// <summary>
    /// 更新空中状态
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        // 检测是否接触地面，如果是则切换到待机状态
        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
        
        // 检测是否接触墙壁，如果是则切换到墙壁滑行状态
        if (player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        // 空中水平移动控制（速度为地面移动的80%）
        if (xInput != 0)
        {
            player.SetVelocity(player.moveSpeed * 0.8f * xInput, rb.velocity.y);
        }
    }
}
