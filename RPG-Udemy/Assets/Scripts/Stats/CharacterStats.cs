using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 角色属性类型枚举，用于标识不同的角色属性
/// </summary>
public enum StatType
{
    strength,           // 力量
    agility,            // 敏捷
    intelligence,       // 智力
    vitality,           // 体力
    damage,             // 伤害
    critChance,         // 暴击率
    critPower,          // 暴击伤害
    maxHealth,          // 最大生命值
    armor,              // 护甲
    evasion,            // 闪避
    magicResistance,    // 魔法抗性
    fireDamage,         // 火焰伤害
    iceDamage,          // 冰冻伤害
    lightningDamage     // 闪电伤害
}

/// <summary>
/// 伤害类型枚举，用于区分不同类型的伤害
/// </summary>
public enum DamageType
{
    Physical,           // 物理伤害
    Fire,               // 火焰伤害
    Ice,                // 冰冻伤害
    Lightning,          // 闪电伤害
    Heal                // 治疗
}

/// <summary>
/// CharacterStats.cs摘要
/// 角色属性系统核心类，管理所有角色的属性、状态效果和战斗计算
/// 包括物理和魔法伤害计算、状态效果应用、生命值管理等
/// </summary>
public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;                      // 特效控制器引用

    [Header("主要属性")]
    public Stat strength;                     // 力量：每点增加1点物理伤害，1%暴击伤害，1点物理抗性
    public Stat agility;                      // 敏捷：每点增加1%闪避率，1%暴击率
    public Stat intelligence;                 // 智力：每点增加1点魔法伤害，3点魔法抗性
    public Stat vitality;                     // 体力：每点增加10点最大生命值

    [Header("进攻属性")]
    public Stat damage;                       // 基础伤害
    public Stat critChance;                   // 暴击率
    public Stat critpower;                    // 暴击伤害倍率（默认150%）

    [Header("防御属性")]
    public Stat maxHealth;                    // 最大生命值
    public Stat armor;                        // 物理护甲
    public Stat evasion;                      // 闪避值
    public Stat magicResistance;              // 魔法抗性

    [Header("魔法伤害")]
    public Stat fireDamage;                   // 火焰伤害
    public Stat iceDamage;                    // 冰冻伤害
    public Stat lightningDamage;              // 闪电伤害

    // 状态效果标志
    public bool isIgnited;                    // 点燃状态：持续造成伤害
    public bool isChilled;                    // 冰冻状态：降低20%护甲
    public bool isShocked;                    // 感电状态：降低20%命中

    // 状态效果计时器
    [SerializeField] private float ailmentsDuration = 4;  // 状态效果持续时间
    private float ignitedTimer;               // 点燃持续时间
    private float chilledTimer;               // 冰冻持续时间
    private float shockedTimer;               // 感电持续时间

    // 点燃伤害相关
    private float igniteDamageCooldown = .3f; // 点燃伤害间隔
    private float igniteDamageTimer;          // 点燃伤害计时器
    private float igniteDamage;               // 点燃伤害值
    [SerializeField] private GameObject shockStrikePrefab;  // 电击特效预制体
    private int shockDamage;                  // 电击伤害值

    // 事件和生命值
    [SerializeField] public int currentHealth; // 当前生命值
    public System.Action onHealthChanged;      // 生命值改变事件，用于更新UI

    public bool isDead { get; protected set; }           // 角色是否已死亡
    public bool isInvincible { get; protected set; }     // 角色是否无敌
    private bool isVulnerable;                           // 角色是否处于易伤状态

    /// <summary>
    /// 初始化角色属性和状态
    /// </summary>
    protected virtual void Start()
    {
        critpower.SetDefaultValue(150);       // 设置默认暴击伤害倍率为150%
        currentHealth = GetMaxHealthValue();  // 初始化当前生命值为最大值
        fx = GetComponent<EntityFX>();        // 获取特效控制器引用
    }

    /// <summary>
    /// 更新状态效果计时器和处理持续伤害
    /// </summary>
    protected virtual void Update()
    {
        // 更新状态效果计时器
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;

        // 检查状态效果是否结束
        if (ignitedTimer < 0)
            isIgnited = false;
        if (chilledTimer < 0)
            isChilled = false;
        if (shockedTimer < 0)
            isShocked = false;

        // 处理点燃状态的持续伤害
        if (igniteDamageTimer < 0 && isIgnited)
        {
            DecreaseHealthBy((int)igniteDamage, false); // 使用统一方法，不显示受击效果
            igniteDamageTimer = igniteDamageCooldown;   // 重置伤害计时器
        }
    }

    /// <summary>
    /// 使角色在指定时间内处于易伤状态
    /// </summary>
    /// <param name="_duration">易伤持续时间</param>
    public void MakeVulnerableFor(float _duration) => StartCoroutine(VulnerableForCorutine(_duration));

    /// <summary>
    /// 易伤状态协程
    /// </summary>
    /// <param name="_duration">易伤持续时间</param>
    private IEnumerator VulnerableForCorutine(float _duration)
    {
        isVulnerable = true;
        yield return new WaitForSeconds(_duration);
        isVulnerable = false;
    }

    /// <summary>
    /// 临时增加指定属性值
    /// </summary>
    /// <param name="_modifier">属性增加值</param>
    /// <param name="_duration">持续时间</param>
    /// <param name="_staToModify">要修改的属性</param>
    public virtual void IncreaseStaBy(int _modifier, float _duration, Stat _staToModify)
    {
        StartCoroutine(StatModCorountine(_modifier, _duration, _staToModify));
    }

    /// <summary>
    /// 临时属性修改协程
    /// </summary>
    /// <param name="_modifier">属性修改值</param>
    /// <param name="_duration">持续时间</param>
    /// <param name="_staToModify">要修改的属性</param>
    private IEnumerator StatModCorountine(int _modifier, float _duration, Stat _staToModify)
    {
        _staToModify.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _staToModify.RemoveModifier(_modifier);
    }

    /// <summary>
    /// 计算并施加伤害给目标角色
    /// </summary>
    /// <param name="_targetStats">目标角色属性</param>
    public virtual void DoDamage(CharacterStats _targetStats)
    {
        // 检查目标是否无敌或已死亡
        if (_targetStats.isInvincible)
            return;

        if (_targetStats == null || _targetStats.isDead)
            return;

        // 如果目标是玩家，设置伤害来源
        Player targetPlayer = _targetStats.GetComponent<Player>();
        if (targetPlayer != null)
        {
            targetPlayer.lastDamageSource = transform;
        }

        // 检查目标是否闪避了攻击
        if (TargetCanAvoidAttack(_targetStats))
            return;
            
        // 设置击退方向
        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        // 计算总伤害
        int totalDamage = damage.GetValue() + strength.GetValue();
        bool isCritical = false;

        // 检查是否暴击
        if (Cancrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            isCritical = true;
        }
        
        // 创建命中特效
        fx.CreateFitFx(_targetStats.transform, isCritical);

        // 对目标造成物理伤害
        _targetStats.DecreaseHealthBy(totalDamage, true, DamageType.Physical, isCritical);

        // 处理魔法伤害
        DoMagicDamage(_targetStats);
    }

    #region  Magical damage and ailements

    /// <summary>
    /// 对目标角色造成魔法伤害
    /// </summary>
    /// <param name="_targetStats">目标角色属性</param>
    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        if (_targetStats == null || _targetStats.isDead)
            return;

        // 如果目标是玩家，设置伤害来源
        Player targetPlayer = _targetStats.GetComponent<Player>();
        if (targetPlayer != null)
        {
            targetPlayer.lastDamageSource = transform;
        }

        // 获取各类元素伤害值
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        // 计算总魔法伤害（包括智力加成）
        int totalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();

        // 对目标造成魔法伤害，使用最高的元素伤害类型
        _targetStats.DecreaseHealthBy(totalDamage, true, GetHighestDamageType(_fireDamage, _iceDamage, _lightningDamage));

        // 如果没有元素伤害，不尝试应用状态效果
        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
            return;

        // 尝试应用元素状态效果
        AttemptToApplyAilements(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
    }

    /// <summary>
    /// 获取最高的元素伤害类型
    /// </summary>
    /// <param name="fire">火焰伤害</param>
    /// <param name="ice">冰冻伤害</param>
    /// <param name="lightning">闪电伤害</param>
    /// <returns>最高伤害的元素类型</returns>
    private DamageType GetHighestDamageType(int fire, int ice, int lightning)
    {
        if (fire >= ice && fire >= lightning) return DamageType.Fire;
        if (ice >= fire && ice >= lightning) return DamageType.Ice;
        return DamageType.Lightning;
    }

    /// <summary>
    /// 尝试对目标应用元素状态效果
    /// </summary>
    /// <param name="_targetStats">目标角色属性</param>
    /// <param name="_fireDamage">火焰伤害</param>
    /// <param name="_iceDamage">冰冻伤害</param>
    /// <param name="_lightningDamage">闪电伤害</param>
    private void AttemptToApplyAilements(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        // 根据元素伤害值确定可以应用哪种状态效果
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        // 如果没有明显的主导元素，随机选择一种状态效果
        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            // 33%几率应用点燃
            if (UnityEngine.Random.value < .33f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
            }
            
            // 50%几率应用冰冻
            if (UnityEngine.Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
            }
            
            // 100%几率应用感电
            if (UnityEngine.Random.value < 1f && _lightningDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
            }
        }
        
        // 设置点燃伤害（火焰伤害的20%）
        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        // 设置电击伤害（闪电伤害的10%）
        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightningDamage * .1f));

        // 应用状态效果
        _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
    }


    /// <summary>
    /// 应用点燃、冰冻或电击效果到角色上
    /// </summary>
    /// <param name="_isIgnited">是否应用点燃效果</param>
    /// <param name="_isChilled">是否应用冰冻效果</param>
    /// <param name="_isShocked">是否应用电击效果</param>
    public void ApplyAilment(bool _isIgnited, bool _isChilled, bool _isShocked)
    {
        // 检查角色是否可以被点燃（不能同时有多种状态）
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        // 检查角色是否可以被冰冻（不能同时有多种状态）
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        // 检查角色是否可以被电击（可以与点燃共存）
        bool canApplyShock = !isIgnited && !isChilled;

        // 应用点燃效果
        if (_isIgnited && canApplyIgnite)
        {
            isIgnited = _isIgnited;
            ignitedTimer = ailmentsDuration;       // 设置点燃持续时间
            fx.IgniteFxFor(ailmentsDuration);      // 触发点燃视觉效果
        }

        // 应用冰冻效果
        if (_isChilled && canApplyChill)
        {
            isChilled = _isChilled;
            chilledTimer = ailmentsDuration;       // 设置冰冻持续时间
            fx.ChilFxFor(ailmentsDuration);        // 触发冰冻视觉效果

            // 冰冻效果使角色速度降低20%
            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
        }

        // 应用电击效果
        if (_isShocked && canApplyShock)
        {
            if (!isShocked)
            {
                // 首次电击
                ApplyShock(_isShocked);
            }
            else
            {
                // 已经处于电击状态，尝试电击链
                // 玩家不会触发电击链
                if (GetComponent<Player>() != null)
                    return;

                // 对周围敌人造成电击链伤害
                HieNearestTargetWithShockStrike();
            }
        }
    }

    /// <summary>
    /// 应用电击状态
    /// </summary>
    /// <param name="_isShocked">是否应用电击</param>
    public void ApplyShock(bool _isShocked)
    {
        // 如果已经处于电击状态，则不再重复触发
        if (isShocked)
            return;

        isShocked = _isShocked;
        shockedTimer = ailmentsDuration;       // 设置电击持续时间
        fx.ShockFxFor(ailmentsDuration);       // 触发电击视觉效果
    }

    /// <summary>
    /// 电击链效果：攻击周围的敌人
    /// </summary>
    private void HieNearestTargetWithShockStrike()
    {
        // 找到角色周围20单位距离内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 20);

        // 遍历所有碰撞体，检测并影响附近的敌人
        foreach (var hit in colliders)
        {
            // 检查碰撞体是否为敌方单位且不是自己
            if (hit.GetComponent<Enemy>() != null && hit.transform != transform)
            {
                // 实例化电击攻击预制体
                GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
                // 设置电击攻击的目标和伤害
                newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, hit.GetComponent<CharacterStats>());
            }
        }
    }

    /// <summary>
    /// 设置点燃伤害值
    /// </summary>
    /// <param name="_damage">点燃伤害值</param>
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    /// <summary>
    /// 设置电击伤害值
    /// </summary>
    /// <param name="_damage">电击伤害值</param>
    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

    #endregion


    /// <summary>
    /// 角色受到伤害时调用的方法
    /// </summary>
    /// <param name="_damage">伤害值</param>
    public virtual void TakeDamage(int _damage)
    {
        // 如果角色已死亡或处于无敌状态，不处理伤害
        if (isDead || isInvincible)
            return;

        // 使用统一的生命值减少方法，并显示受击效果
        DecreaseHealthBy(_damage, true);
    }

    /// <summary>
    /// 统一的生命值减少方法
    /// </summary>
    /// <param name="_damage">伤害值</param>
    /// <param name="showImpact">是否显示受击效果</param>
    /// <param name="damageType">伤害类型</param>
    /// <param name="isCritical">是否暴击</param>
    protected virtual void DecreaseHealthBy(int _damage, bool showImpact = false, DamageType damageType = DamageType.Physical, bool isCritical = false)
    {
        // 如果角色已死亡或处于无敌状态，不处理伤害
        if (isDead || isInvincible)
            return;
            
        // 如果角色处于易伤状态，增加10%伤害
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.1f);

        // 减少生命值
        currentHealth -= _damage;

        // 创建伤害数字
        if (_damage > 0)
        {
            fx.CreatePopUpText(_damage.ToString(), damageType, isCritical);
        }

        // 只在需要时显示受击效果
        if (showImpact)
        {
            Entity entity = GetComponent<Entity>();
            if (entity != null)
                entity.DamageImpact();  // 触发受击动画

            if (fx != null)
                fx.StartCoroutine("FlashFX");  // 触发受击闪烁效果
        }

        // 触发血量变化事件（用于更新UI等）
        onHealthChanged?.Invoke();

        // 检查是否死亡
        if (currentHealth <= 0 && !isDead)
        {
            currentHealth = 0;
            Die();  // 处理死亡
        }
    }

    /// <summary>
    /// 增加角色生命值
    /// </summary>
    /// <param name="_amount">增加的生命值</param>
    public virtual void IncreaseHealthBy(int _amount)
    {
        // 增加生命值
        currentHealth += _amount;
        
        // 确保不超过最大生命值
        if (currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();

        // 创建治疗数字
        fx.CreatePopUpText("+" + _amount.ToString(), DamageType.Heal);

        // 触发血量变化事件
        if (onHealthChanged != null)
            onHealthChanged();
    }

    /// <summary>
    /// 角色死亡处理
    /// </summary>
    protected virtual void Die()
    {
        // 防止重复调用
        if (isDead)
            return;

        // 设置死亡状态
        isDead = true;

        // 禁用组件，防止继续接收更新
        enabled = false;
    }

    /// <summary>
    /// 立即杀死角色
    /// </summary>
    public void KillEntity()
    {
        if (!isDead)
            Die();
    }

    /// <summary>
    /// 设置角色无敌状态
    /// </summary>
    /// <param name="_invincible">是否无敌</param>
    public void MakeInvincible(bool _invincible) => isInvincible = _invincible;


    #region Stat calculation
    /// <summary>
    /// 检查目标角色的护甲并计算最终物理伤害
    /// </summary>
    /// <param name="_targetStats">目标角色属性</param>
    /// <param name="totleDamage">初始伤害值</param>
    /// <returns>计算护甲后的最终伤害</returns>
    protected int CheckTargetArmor(CharacterStats _targetStats, int totleDamage)
    {
        // 如果目标被冰冻，护甲效果降低20%
        if (_targetStats.isChilled)
        {
            totleDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        }
        else
        {
            totleDamage -= _targetStats.armor.GetValue();
        }

        // 确保伤害值不会小于0（负数伤害会变成治疗）
        totleDamage = Math.Clamp(totleDamage, 0, int.MaxValue);

        return totleDamage;
    }
    
    /// <summary>
    /// 检查目标魔法抗性并计算最终魔法伤害
    /// </summary>
    /// <param name="_targetStats">目标角色属性</param>
    /// <param name="totleDamage">初始伤害值</param>
    /// <returns>计算魔法抗性后的最终伤害</returns>
    private int CheckTargetResistance(CharacterStats _targetStats, int totleDamage)
    {
        // 魔法抗性 = 基础魔法抗性 + 智力属性加成(每点智力增加3点魔法抗性)
        totleDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totleDamage = Math.Clamp(totleDamage, 0, int.MaxValue);
        return totleDamage;
    }
    
    /// <summary>
    /// 角色成功闪避攻击时调用
    /// 可被子类重写以添加额外效果
    /// </summary>
    public virtual void OnEvasion()
    {
        // 基类中为空，由子类实现具体闪避效果
    }

    /// <summary>
    /// 检查目标角色是否可以闪避攻击
    /// </summary>
    /// <param name="_targetStats">目标角色属性</param>
    /// <returns>是否成功闪避</returns>
    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        // 总闪避值 = 基础闪避值 + 敏捷属性加成
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        // 如果攻击者被电击，目标闪避值增加20点
        if (isShocked)
        {
            totalEvasion += 20;
        }

        // 根据闪避值判定是否闪避成功
        if (UnityEngine.Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();  // 触发闪避效果
            return true;  // 闪避成功
        }
        return false;  // 闪避失败
    }

    /// <summary>
    /// 检查角色是否可以暴击
    /// </summary>
    /// <returns>是否暴击</returns>
    protected bool Cancrit()
    {
        // 总暴击率 = 基础暴击率 + 敏捷属性加成
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        // 根据暴击率判定是否暴击成功
        if (UnityEngine.Random.Range(0, 100) < totalCriticalChance)
        {
            return true;  // 暴击成功
        }
        return false;  // 暴击失败
    }

    /// <summary>
    /// 计算暴击伤害
    /// </summary>
    /// <param name="_damage">基础伤害</param>
    /// <returns>暴击后的伤害</returns>
    protected int CalculateCriticalDamage(int _damage)
    {
        // 总暴击伤害倍率 = (基础暴击伤害倍率 + 力量属性加成) * 0.01
        float totalCritPower = (critpower.GetValue() + strength.GetValue()) * .01f;

        // 计算暴击伤害
        float critiDamage = _damage * totalCritPower;

        // 返回四舍五入后的整数伤害值
        return Mathf.RoundToInt(critiDamage);
    }

    /// <summary>
    /// 获取角色最大生命值
    /// </summary>
    /// <returns>最大生命值</returns>
    public int GetMaxHealthValue()
    {
        // 最大生命值 = 基础最大生命值 + 体力属性加成（每点体力增加10点生命值）
        return maxHealth.GetValue() + vitality.GetValue() * 10;
    }
    #endregion

    /// <summary>
    /// 根据属性类型获取对应的属性对象
    /// </summary>
    /// <param name="startType">属性类型</param>
    /// <returns>对应的属性对象</returns>
    public Stat GetStat(StatType startType)
    {
        switch (startType)
        {
            case global::StatType.strength:
                return strength;          // 力量
            case global::StatType.agility:
                return agility;           // 敏捷
            case global::StatType.intelligence:
                return intelligence;      // 智力
            case global::StatType.vitality:
                return vitality;          // 体力
            case global::StatType.damage:
                return damage;            // 伤害
            case global::StatType.critChance:
                return critChance;        // 暴击率
            case global::StatType.critPower:
                return critpower;         // 暴击伤害
            case global::StatType.maxHealth:
                return maxHealth;         // 最大生命值
            case global::StatType.armor:
                return armor;             // 护甲
            case global::StatType.evasion:
                return evasion;           // 闪避
            case global::StatType.magicResistance:
                return magicResistance;   // 魔法抗性
            case global::StatType.fireDamage:
                return fireDamage;        // 火焰伤害
            case global::StatType.iceDamage:
                return iceDamage;         // 冰冻伤害
            case global::StatType.lightningDamage:
                return lightningDamage;   // 闪电伤害
            default:
                return null;              // 未知属性类型
        }
    }
}
