using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy.cs摘要
/// 敌人基类，定义所有敌人共有的属性和行为
/// 包括状态机管理、攻击逻辑、移动参数、眩晕机制等
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EntityFX))]
[RequireComponent(typeof(ItemDrop))]
public class Enemy : Entity
{
    [SerializeField] protected LayerMask isPlayer;       // 用于检测玩家的层掩码

    [Header("眩晕")]
    public float stunDuration = 1;                       // 眩晕持续时间
    public Vector2 stunDirection = new Vector2(10, 12);  // 眩晕击退方向和力度
    protected bool canBeStunned;                         // 是否可以被眩晕（反击窗口）
    [SerializeField] protected GameObject counterImage;  // 可反击提示图标

    [Header("移动")]
    public float moveSpeed = 1.5f;                       // 普通移动速度
    public float chaseSpeed = 3;                         // 追击玩家时的速度
    public float idleTime = 2;                           // 闲置状态持续时间
    public float battleTime = 7;                         // 战斗状态持续时间
    private float defaultMoveSpeed;                      // 默认移动速度（用于恢复）

    [Header("攻击")]
    public float attackDistance = 2;                     // 攻击距离
    public float attackCooldown = 2;                     // 攻击冷却时间
    public float minAttackCooldown = 1;                  // 最小攻击冷却时间
    public float maxAttackCooldown = 2;                  // 最大攻击冷却时间
    public float agroDistance = 3;                       // 仇恨距离（发现玩家的距离）
    [HideInInspector] public float lastTimeAttacked;     // 上次攻击时间

    public EnemyStateMachine stateMachine { get; private set; }  // 敌人状态机
    public EntityFX fX { get; private set; }                     // 特效控制器
    public string lastAnimBoolName { get; private set; }         // 上一个动画布尔参数名

    /// <summary>
    /// 初始化敌人状态机和默认速度
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
        defaultMoveSpeed = moveSpeed;
    }

    /// <summary>
    /// 获取特效控制器引用
    /// </summary>
    protected override void Start()
    {
        base.Start();
        fX = GetComponent<EntityFX>();
    }

    /// <summary>
    /// 更新当前状态
    /// </summary>
    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    /// <summary>
    /// 记录最后使用的动画参数名
    /// </summary>
    /// <param name="_animBoolName">动画布尔参数名</param>
    public virtual void AssignLastAnimName(string _animBoolName) => lastAnimBoolName = _animBoolName;

    /// <summary>
    /// 减速敌人
    /// </summary>
    /// <param name="_slowPercentage">减速百分比</param>
    /// <param name="_slowDuration">减速持续时间</param>
    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        chaseSpeed = chaseSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    /// <summary>
    /// 恢复默认速度
    /// </summary>
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
    }

    /// <summary>
    /// 冻结或解冻敌人
    /// </summary>
    /// <param name="_timeFrozen">是否冻结</param>
    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }

    /// <summary>
    /// 冻结敌人指定时间
    /// </summary>
    /// <param name="_duration">冻结持续时间</param>
    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimerCoroutine(_duration));

    /// <summary>
    /// 冻结计时协程
    /// </summary>
    /// <param name="_seconds">冻结秒数</param>
    protected virtual IEnumerator FreezeTimerCoroutine(float _seconds)
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
    }

    #region Counter Attack Window

    /// <summary>
    /// 打开反击窗口
    /// </summary>
    public virtual void OpenCountAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }

    /// <summary>
    /// 关闭反击窗口
    /// </summary>
    public virtual void CloseCountAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
    #endregion

    /// <summary>
    /// 检查敌人是否可以被眩晕
    /// </summary>
    /// <returns>如果可以被眩晕返回true</returns>
    public virtual bool CanbeStunned()
    {
        if (canBeStunned)
        {
            CloseCountAttackWindow();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 动画完成触发器
    /// </summary>
    public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    /// <summary>
    /// 特殊攻击动画触发器
    /// </summary>
    public virtual void AnimationSepcoalAttackTrigger()
    {
        // 子类可以重写此方法实现特殊攻击逻辑
    }

    /// <summary>
    /// 检测玩家是否在视线范围内
    /// </summary>
    /// <returns>射线检测结果</returns>
    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(transform.position, Vector2.right * facingDir, 50, isPlayer);

    /// <summary>
    /// 对玩家造成伤害
    /// </summary>
    protected virtual void DamagePlayer()
    {
        Player player = PlayerManager.instance.player;
        if (player != null)
        {
            // 设置伤害来源
            player.lastDamageSource = transform;
            Debug.Log($"Set player.lastDamageSource to {transform.name}");

            // 对玩家造成伤害
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(CalculateTotalDamage());
            }
        }
    }

    /// <summary>
    /// 计算敌人总伤害（物理+元素）
    /// </summary>
    /// <returns>总伤害值</returns>
    private int CalculateTotalDamage()
    {
        EnemyStats stats = GetComponent<EnemyStats>();
        if (stats == null) return 0;

        // 基础伤害 + 元素伤害
        return stats.damage.GetValue() + stats.fireDamage.GetValue() +
               stats.iceDamage.GetValue() + stats.lightningDamage.GetValue();
    }

    /// <summary>
    /// 绘制调试信息
    /// </summary>
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        // 绘制攻击距离
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }
}
