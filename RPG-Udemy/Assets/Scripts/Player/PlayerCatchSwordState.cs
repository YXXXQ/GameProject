using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerCatchSwordState.cs摘要
/// 玩家接剑状态，处理玩家接住投掷出去的剑的过程
/// 包括接剑时的视觉效果、冲击力和朝向调整
/// </summary>
public class PlayerCatchSwordState : PlayerState
{
    // 剑的Transform引用
    private Transform sword;
    
    /// <summary>
    /// 构造函数，初始化接剑状态
    /// </summary>
    /// <param name="_player">玩家引用</param>
    /// <param name="_stateMachine">状态机引用</param>
    /// <param name="_animBoolName">动画参数名</param>
    public PlayerCatchSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        // 继承基类构造函数
    }

    /// <summary>
    /// 进入接剑状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        // 获取剑的Transform引用
        sword = player.sword.transform;
        // 播放灰尘特效
        player.fX.PlayDustFx();
        // 触发屏幕震动效果
        player.fX.ScreenShake(player.fX.shakeSwordImpact);

        // 根据剑的位置调整玩家朝向
        // 如果玩家在剑的右侧但朝向右边，则翻转
        if (player.transform.position.x > sword.position.x && player.facingDir == 1)
            player.Flip();
        // 如果玩家在剑的左侧但朝向左边，则翻转
        else if (player.transform.position.x < sword.position.x && player.facingDir == -1)
            player.Flip();
            
        // 应用接剑时的冲击力，使玩家向后移动
        rb.velocity = new Vector2(player.swordReturnImpact * -player.facingDir, rb.velocity.y);
    }

    /// <summary>
    /// 退出接剑状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        // 设置短暂的忙碌状态，防止立即进入其他状态
        player.StartCoroutine("BusyFor", .1f);
    }

    /// <summary>
    /// 更新接剑状态
    /// </summary>
    public override void Update()
    {
        base.Update();
        // 当动画触发器被调用时（接剑动画完成），切换回待机状态
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
