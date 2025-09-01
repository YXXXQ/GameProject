using System.Collections;
using UnityEngine;

/// <summary>
/// Player.cs摘要
/// 玩家角色控制器，继承自Entity基类
/// 管理玩家状态机、技能、移动和战斗逻辑
/// </summary>
public class Player : Entity
{
    [Header("攻击")]
    public Vector2[] attackMovement;                // 攻击时的位移数组
    public float counterAttackDuration = 0.2f;      // 反击持续时间
    public bool isBusy { get; private set; }        // 玩家是否正忙（无法接收输入）
    [HideInInspector] public Transform lastDamageSource; // 上次受到伤害的来源
    
    [Header("移动")]
    public float moveSpeed = 12f;                   // 移动速度
    public float jumpForce;                         // 跳跃力度
    public float swordReturnImpact;                 // 剑回收时的冲击力
    private float defualtMoveSpeed;                 // 默认移动速度
    private float defualtJumpForce;                 // 默认跳跃力度

    [Header("冲刺")]
    public float dashSpeed;                         // 冲刺速度
    public float dashDuration;                      // 冲刺持续时间
    private float defaultDashSpeed;                 // 默认冲刺速度
    public float dashDir { get; private set; }      // 冲刺方向

    public SkillManager skill { get; private set; } // 技能管理器引用
    public GameObject sword { get; private set; }   // 玩家剑的引用
    public PlayerFX fX { get; private set; }        // 玩家特效控制器

    #region States
    // 玩家状态机和各种状态
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }           // 待机状态
    public PlayerIMoveState moveState { get; private set; }          // 移动状态
    public PlayerJumpState jumpState { get; private set; }           // 跳跃状态
    public PlayerAirState airState { get; private set; }             // 空中状态
    public PlayDashState dashState { get; private set; }             // 冲刺状态
    public PlayerWallJumpState wallJumpState { get; private set; }   // 墙跳状态
    public PlayerWallSlideState wallSlideState { get; private set; } // 墙滑状态
    public PlayerPrimaryAttackState primaryAttackState { get; private set; } // 普通攻击状态
    public PlayCounterAttackState counterAttackState { get; private set; }   // 反击状态
    public PlayerAimSwordState aimSowrd { get; private set; }        // 瞄准剑状态
    public PlayerCatchSwordState catchSword { get; private set; }    // 接剑状态
    public PlayerBlackholeState blackhole { get; private set; }      // 黑洞技能状态
    public PlayerDeadState deadState { get; private set; }           // 死亡状态
    #endregion

    /// <summary>
    /// 初始化玩家状态机和所有状态
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        
        // 创建状态机
        stateMachine = new PlayerStateMachine();
        
        // 初始化基础移动状态
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerIMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");

        // 初始化战斗状态
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayCounterAttackState(this, stateMachine, "CounterAttack");

        // 初始化技能状态
        aimSowrd = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackhole = new PlayerBlackholeState(this, stateMachine, "Jump");
        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    /// <summary>
    /// 获取组件引用并初始化状态机
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // 获取组件引用
        fX = GetComponent<PlayerFX>();
        skill = SkillManager.instance;

        // 初始化状态机为待机状态
        stateMachine.Initialize(idleState);
        
        // 保存默认参数值
        defualtMoveSpeed = moveSpeed;
        defualtJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

    /// <summary>
    /// 更新玩家状态和处理输入
    /// </summary>
    protected override void Update()
    {
        // 游戏暂停时不处理
        if (Time.timeScale == 0)
        {
            return;
        }

        base.Update();

        // 更新当前状态
        stateMachine.currentState.Update();

        // 检查冲刺输入
        checkForDashInput();
        
        // 检查水晶技能输入
        if (Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
        {
            skill.crystal.CanUseSkill();
        }
        
        // 检查药水使用输入
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Inventory.instance.UseFlask();
        }
    }

    /// <summary>
    /// 减速玩家
    /// </summary>
    /// <param name="_slowPercentage">减速百分比</param>
    /// <param name="_slowDuration">减速持续时间</param>
    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    /// <summary>
    /// 恢复默认速度
    /// </summary>
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defualtMoveSpeed;
        jumpForce = defualtJumpForce;
        dashSpeed = defaultDashSpeed;
    }

    /// <summary>
    /// 分配新的剑对象
    /// </summary>
    /// <param name="_newSword">新剑的游戏对象</param>
    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    /// <summary>
    /// 接住剑并销毁剑对象
    /// </summary>
    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    /// <summary>
    /// 设置玩家为忙碌状态一段时间
    /// </summary>
    /// <param name="_seconds">忙碌持续时间</param>
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    /// <summary>
    /// 动画触发器回调
    /// </summary>
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    /// <summary>
    /// 检查冲刺输入并执行冲刺
    /// </summary>
    private void checkForDashInput()
    {
        // 如果技能未解锁，直接返回
        if (skill.dash.dashUnlocked == false)
        {
            return;
        }

        // 如果检测到墙壁，直接返回
        if (IsWallDetected())
        {
            return;
        }

        // 检测冲刺输入并执行
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            // 获取冲刺方向
            dashDir = Input.GetAxisRaw("Horizontal");

            // 如果没有水平输入，使用面朝方向
            if (dashDir == 0)
            {
                dashDir = facingDir;
            }
            
            // 切换到冲刺状态
            stateMachine.ChangeState(dashState);
        }
    }

    /// <summary>
    /// 玩家死亡处理
    /// </summary>
    public override void Die()
    {
        base.Die();
        // 在玩家死亡时立即保存游戏状态
        SaveManager.instance.SaveGame();
        stateMachine.ChangeState(deadState);
    }

    /// <summary>
    /// 设置零击退力
    /// </summary>
    protected override void SetupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0, 0);
        // 确保在击退结束后重置玩家速度
        if (!isKnocked)
            rb.velocity = Vector2.zero;
    }
}
