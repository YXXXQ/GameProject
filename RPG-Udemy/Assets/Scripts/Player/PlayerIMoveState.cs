using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerIMoveState.cs摘要
/// 玩家移动状态，继承自PlayGroundedState
/// 处理玩家在地面上移动的行为和状态转换
/// </summary>
public class PlayerIMoveState : PlayGroundedState
{
    /// <summary>
    /// 构造函数，初始化移动状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayerIMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 进入移动状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        // 播放移动音效
        AudioManager.instance.PlaySFX(14, null);
    }

    /// <summary>
    /// 退出移动状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        // 停止移动音效
        AudioManager.instance.StopSFX(14);
    }

    /// <summary>
    /// 更新移动状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        // 根据输入设置玩家水平速度，保持垂直速度不变
        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        // 如果没有水平输入或检测到墙壁，切换回待机状态
        if (xInput == 0 || player.IsWallDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
