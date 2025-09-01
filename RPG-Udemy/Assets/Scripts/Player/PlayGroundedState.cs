using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayGroundedState.cs摘要
/// 玩家地面状态基类，所有地面状态（如待机、移动）的基础实现
/// 处理玩家在地面上的通用行为和输入检测
/// </summary>
public class PlayGroundedState : PlayerState
{
    /// <summary>
    /// 构造函数，初始化地面状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 进入地面状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        // 无需额外操作
    }

    /// <summary>
    /// 退出地面状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        // 无需额外操作
    }

    /// <summary>
    /// 更新地面状态，处理各种输入和状态转换
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        // 检测黑洞技能输入 (R键)
        if (Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.blackHoleUnlocked)
        {
            // 检查技能冷却
            if (player.skill.blackhole.cooldownTimer > 0)
            {
                player.fX.CreatePopUpText("Cooldown");
                return;
            }
            // 切换到黑洞技能状态
            stateMachine.ChangeState(player.blackhole);
        }
        
        // 检测剑技能输入 (鼠标右键)
        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword.swordUnlocked)
        {
            // 切换到瞄准剑状态
            stateMachine.ChangeState(player.aimSowrd);
        }
        
        // 检测格挡/反击技能输入 (Q键)
        if (Input.GetKeyDown(KeyCode.Q) && player.skill.parry.parryUnlocked)
        {
            // 切换到反击状态
            stateMachine.ChangeState(player.counterAttackState);
        }
        
        // 检测普通攻击输入 (鼠标左键)
        if (Input.GetKey(KeyCode.Mouse0))
        {
            // 切换到普通攻击状态
            stateMachine.ChangeState(player.primaryAttackState);
        }
        
        // 检测是否离开地面
        if (!player.IsGroundDetected())
        {
            // 切换到空中状态
            stateMachine.ChangeState(player.airState);
        }

        // 检测跳跃输入 (空格键)
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
        {
            // 切换到跳跃状态
            stateMachine.ChangeState(player.jumpState);
        }
    }
    
    /// <summary>
    /// 检查玩家是否没有剑或需要召回剑
    /// </summary>
    /// <returns>如果没有剑返回true，否则召回剑并返回false</returns>
    private bool HasNoSword()
    {
        // 如果没有剑，返回true
        if (!player.sword)
        {
            return true;
        }
        
        // 如果有剑，召回剑并返回false
        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
