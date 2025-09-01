using UnityEngine;

/// <summary>
/// PlayCounterAttackState.cs摘要
/// 玩家反击状态，处理玩家的格挡和反击机制
/// 包括箭矢反弹、敌人眩晕和分身生成
/// </summary>
public class PlayCounterAttackState : PlayerState
{
    // 是否可以创建分身（每次反击只能创建一个）
    private bool canCreateClone;
    
    /// <summary>
    /// 构造函数，初始化反击状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 进入反击状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        // 重置分身创建标志
        canCreateClone = true;
        // 设置反击持续时间
        stateTimer = player.counterAttackDuration;
        // 重置成功反击动画标志
        player.anim.SetBool("SuccesfulCounterAttack", false);
    }

    /// <summary>
    /// 退出反击状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        // 无需额外操作
    }

    /// <summary>
    /// 更新反击状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        // 反击时玩家保持静止
        player.SetZeroVelocity();

        // 检测攻击范围内的碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        foreach (var hit in colliders)
        {
            // 处理箭矢反弹
            if (hit.GetComponent<Arrow_Controller>() != null)
            {
                // 反转箭矢方向
                hit.GetComponent<Arrow_Controller>().FlipArrow();
                // 触发成功反击效果
                SuccesfulCounterAttack();
            }

            // 处理敌人反击
            if (hit.GetComponent<Enemy>() != null)
            {
                // 检查敌人是否可以被眩晕
                if (hit.GetComponent<Enemy>().CanbeStunned())
                {
                    // 触发成功反击效果
                    SuccesfulCounterAttack();
                    
                    // 使用格挡技能（可能触发冷却）
                    player.skill.parry.UseSkill();

                    // 在敌人位置创建分身（每次反击只创建一个）
                    if (canCreateClone)
                    {
                        canCreateClone = false;
                        player.skill.parry.MakeMirageOnParry(hit.transform);
                    }
                }
            }
        }
        
        // 反击时间结束或动画触发器被调用时，切换回待机状态
        if (stateTimer < 0 || triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    /// <summary>
    /// 处理成功反击的效果
    /// </summary>
    private void SuccesfulCounterAttack()
    {
        // 延长反击状态持续时间
        stateTimer = 10;
        // 设置成功反击动画标志
        player.anim.SetBool("SuccesfulCounterAttack", true);
    }
}
