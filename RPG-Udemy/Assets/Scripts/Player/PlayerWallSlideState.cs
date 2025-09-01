using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerWallSlideState.cs摘要
/// 玩家墙壁滑行状态，处理玩家贴墙滑行的行为
/// 包括墙壁滑行速度控制和墙跳转换逻辑
/// </summary>
public class PlayerWallSlideState : PlayerState
{
    /// <summary>
    /// 构造函数，初始化墙壁滑行状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 进入墙壁滑行状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        // 无需额外操作
    }

    /// <summary>
    /// 退出墙壁滑行状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        // 无需额外操作
    }

    /// <summary>
    /// 更新墙壁滑行状态
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        // 如果不再检测到墙壁，切换到空中状态
        if (player.IsWallDetected() == false)
            stateMachine.ChangeState(player.airState);

        // 确保只有在按住朝向墙壁的方向时才能蹬墙跳
        if (Input.GetKeyDown(KeyCode.Space) && xInput == player.facingDir)
        {
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }

        // 如果玩家输入与面向方向相反，离开墙壁
        if (xInput != 0 && player.facingDir != xInput)
            stateMachine.ChangeState(player.idleState);

        // 控制墙壁滑行速度
        if (yInput < 0)
            // 如果按下方向键，正常下落速度
            rb.velocity = new Vector2(0, rb.velocity.y);
        else
            // 否则减缓下落速度（墙壁滑行效果）
            rb.velocity = new Vector2(0, rb.velocity.y * 0.5f);

        // 如果检测到地面，切换到待机状态
        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
