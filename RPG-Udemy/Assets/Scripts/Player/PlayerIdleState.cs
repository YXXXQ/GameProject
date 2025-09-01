using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerIdleState.cs摘要
/// 玩家待机状态，继承自PlayGroundedState
/// 处理玩家站立不动时的行为和状态转换
/// </summary>
public class PlayerIdleState : PlayGroundedState
{
    /// <summary>
    /// 构造函数，初始化待机状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 进入待机状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        // 停止玩家移动
        player.SetZeroVelocity();
    }

    /// <summary>
    /// 退出待机状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        // 无需额外操作
    }

    /// <summary>
    /// 更新待机状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        // 如果检测到墙壁且玩家试图向墙壁方向移动，不执行状态切换
        if (player.IsWallDetected() && xInput * player.facingDir > 0)
            return;
            
        // 如果有水平输入且玩家不处于忙碌状态，切换到移动状态
        if(xInput != 0 && !player.isBusy)
            stateMachine.ChangeState(player.moveState);
    }
}
