using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerBlackholeState.cs摘要
/// 玩家黑洞技能状态，处理玩家使用黑洞技能的过程
/// 包括上升、悬浮和技能释放的全过程
/// </summary>
public class PlayerBlackholeState : PlayerState
{
    // 上升飞行的持续时间
    private float flyTime = .4f;
    // 技能是否已使用的标志
    private bool skillUsed;
    // 保存默认重力值，以便退出状态时恢复
    private float defaultGravity;
    
    /// <summary>
    /// 构造函数，初始化黑洞状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 动画完成触发器
    /// </summary>
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        // 黑洞技能不需要在动画结束时执行特殊操作
    }

    /// <summary>
    /// 进入黑洞状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        // 保存默认重力值
        defaultGravity = player.rb.gravityScale;

        // 初始化状态变量
        skillUsed = false;
        stateTimer = flyTime;
        // 取消重力影响，使玩家可以悬浮
        rb.gravityScale = 0;
    }

    /// <summary>
    /// 退出黑洞状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        
        // 强制恢复玩家属性
        player.rb.gravityScale = defaultGravity;
        // 取消玩家透明效果
        player.fX.MakeTransprent(false);
        
        // 确保结束黑洞技能，防止技能效果残留
        if (player.skill.blackhole != null)
            player.skill.blackhole.ForceCompleteSkill();
    }

    /// <summary>
    /// 更新黑洞状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        // 在初始飞行时间内，玩家快速上升
        if (stateTimer > 0)
        {
            rb.velocity = new Vector2(0, 15);
        }

        // 飞行时间结束后，玩家缓慢下降并释放技能
        if (stateTimer < 0)
        {
            // 设置极慢的下降速度，使玩家看起来像悬浮
            rb.velocity = new Vector2(0, -.1f);

            // 如果技能尚未使用，尝试使用黑洞技能
            if (!skillUsed)
            {
                if (player.skill.blackhole.CanUseSkill())
                    skillUsed = true;
            }
        }
        
        // 增加空值保护，当黑洞技能完成时切换到空中状态
        if (player.skill.blackhole != null && player.skill.blackhole.SkillCompleted())
        {
            stateMachine.ChangeState(player.airState);
        }
    }
}
