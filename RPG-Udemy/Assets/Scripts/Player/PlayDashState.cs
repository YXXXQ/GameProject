using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayDashState.cs摘要
/// 玩家冲刺状态，处理玩家冲刺技能的执行过程
/// 包括冲刺开始和结束时的特效、无敌帧和状态转换
/// </summary>
public class PlayDashState : PlayerState
{
    /// <summary>
    /// 构造函数，初始化冲刺状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 进入冲刺状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        // 创建冲刺开始时的分身特效
        player.skill.dash.CloneOnDash();
        // 设置冲刺持续时间
        stateTimer = player.dashDuration;
        // 冲刺期间玩家无敌
        player.stats.MakeInvincible(true);
    }

    /// <summary>
    /// 退出冲刺状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        // 创建冲刺结束时的分身特效
        player.skill.dash.CloneOnArrival();
        // 停止水平移动，保持垂直速度
        player.SetVelocity(0, rb.velocity.y);
        // 结束无敌状态
        player.stats.MakeInvincible(false);
    }

    /// <summary>
    /// 更新冲刺状态
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        // 如果在空中且碰到墙壁，切换到墙壁滑行状态
        if (!player.IsGroundDetected() && player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        // 设置冲刺速度和方向，垂直速度为0
        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        // 冲刺时间结束后，切换回待机状态
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
        
        // 创建残影特效
        player.fX.CreateAfterImage();
    }
}
