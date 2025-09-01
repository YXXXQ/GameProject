using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerPrimaryAttackState.cs摘要
/// 玩家普通攻击状态，处理玩家的基础攻击连招系统
/// 包括连招计数、攻击方向控制和攻击位移
/// </summary>
public class PlayerPrimaryAttackState : PlayerState
{
    // 当前连招计数器
    public int comboCounter { get; private set; }

    // 上次攻击的时间戳
    private float lastTimeAttacked;
    // 连招窗口时间（秒）
    private float comboWindow = 2;
    
    /// <summary>
    /// 构造函数，初始化普通攻击状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 进入普通攻击状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        // 播放攻击音效（当前已注释）
        // AudioManager.instance.PlaySFX(2);

        // 清除水平输入，防止攻击时滑动
        xInput = 0;

        // 检查连招计数器是否需要重置
        // 如果连招计数超过2或超出连招窗口时间，重置为0
        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
        {
            comboCounter = 0;
        }
        
        // 设置动画连招计数器参数
        player.anim.SetInteger("ComboCounter", comboCounter);

        #region Choose attack direction
        // 确定攻击方向
        float attackDir = player.facingDir;
        if (xInput != 0)
        {
            // 如果有水平输入，使用输入方向
            attackDir = xInput;
        }
        else
        {
            // 否则使用玩家当前面向
            attackDir = player.facingDir;
        }
        #endregion

        // 根据当前连招设置攻击位移
        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);

        // 设置状态计时器（控制攻击位移持续时间）
        stateTimer = .1f;
    }

    /// <summary>
    /// 退出普通攻击状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        
        // 设置短暂的忙碌状态，防止立即进行下一次攻击
        player.StartCoroutine("BusyFor", .12f);
        
        // 增加连招计数
        comboCounter++;
        
        // 更新上次攻击时间
        lastTimeAttacked = Time.time;
    }

    /// <summary>
    /// 更新普通攻击状态
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        // 攻击位移时间结束后停止移动
        if (stateTimer < 0)
        {
            player.SetZeroVelocity();
        }
        
        // 当动画触发器被调用（攻击动画结束）时，切换回待机状态
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
