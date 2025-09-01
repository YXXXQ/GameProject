using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerDeadState.cs摘要
/// 玩家死亡状态，处理玩家死亡后的行为和UI显示
/// 包括停止移动和显示游戏结束界面
/// </summary>
public class PlayerDeadState : PlayerState
{
    /// <summary>
    /// 构造函数，初始化死亡状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayerDeadState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 动画完成触发器
    /// </summary>
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        // 死亡动画完成后不需要执行特殊操作
    }

    /// <summary>
    /// 进入死亡状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        // 显示游戏结束界面
        GameObject.Find("Canvas").GetComponent<UI>().SwitchOnEndScreen();
    }

    /// <summary>
    /// 退出死亡状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        // 死亡状态通常不会退出，除非游戏重新开始
    }

    /// <summary>
    /// 更新死亡状态
    /// </summary>
    public override void Update()
    {
        base.Update();
        // 确保玩家完全停止移动
        player.SetZeroVelocity();
    }
}
