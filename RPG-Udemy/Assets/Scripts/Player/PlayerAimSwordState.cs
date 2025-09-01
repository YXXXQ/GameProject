using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerAimSwordState.cs摘要
/// 玩家瞄准剑状态，处理玩家瞄准和投掷剑的准备阶段
/// 允许玩家通过鼠标位置确定剑的投掷方向
/// </summary>
public class PlayerAimSwordState : PlayerState
{
    /// <summary>
    /// 构造函数，初始化瞄准剑状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 进入瞄准剑状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        // 激活剑技能的瞄准点显示
        player.skill.sword.DotsActive(true);
    }

    /// <summary>
    /// 退出瞄准剑状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        
        // 设置短暂的忙碌状态，防止立即进入其他状态
        player.StartCoroutine("BusyFor", .2f);
    }

    /// <summary>
    /// 更新瞄准剑状态
    /// </summary>
    public override void Update()
    {
        base.Update();
        // 瞄准时玩家保持静止
        player.SetZeroVelocity();

        // 松开鼠标右键时退出瞄准状态
        if(Input.GetKeyUp(KeyCode.Mouse1))
            stateMachine.ChangeState(player.idleState);

        // 根据鼠标位置调整玩家朝向
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // 如果鼠标在玩家左侧但玩家朝右，则翻转
        if (player.transform.position.x > mousePosition.x && player.facingDir == 1)
            player.Flip();
        // 如果鼠标在玩家右侧但玩家朝左，则翻转
        else if(player.transform.position.x < mousePosition.x && player.facingDir == -1)
            player.Flip();
    }
}
