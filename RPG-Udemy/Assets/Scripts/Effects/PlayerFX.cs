using Cinemachine;
using UnityEngine;

/// <summary>
/// PlayerFX.cs摘要
/// 玩家特效控制器，继承自EntityFX
/// 负责处理玩家特有的视觉效果，如屏幕震动、残影和尘土效果
/// </summary>
public class PlayerFX : EntityFX
{
    private CinemachineImpulseSource screenShake;    // Cinemachine屏幕震动源

    [Header("震动特效")]
    [SerializeField] private float shakeMultiplier;  // 震动强度乘数
    public Vector3 shakeSwordImpact;                 // 剑击中目标时的震动参数
    public Vector3 shakeHeightDamage;                // 高空坠落伤害的震动参数

    [Header("残影特效")]
    [SerializeField] private GameObject afterImagePrefab;  // 残影预制体
    [SerializeField] private float colorLooseRate;         // 残影颜色淡出速率
    [SerializeField] private float afterImageCooldown;     // 残影生成冷却时间
    private float afterImageCooldownTimer;                 // 残影冷却计时器

    [Space]
    [SerializeField] private ParticleSystem dustFx;        // 尘土粒子效果

    /// <summary>
    /// 初始化组件引用，调用基类Start方法
    /// </summary>
    protected override void Start()
    {
        base.Start();
        screenShake = GetComponent<CinemachineImpulseSource>();
    }

    /// <summary>
    /// 更新残影冷却计时器
    /// </summary>
    private void Update()
    {
        afterImageCooldownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// 创建玩家移动残影效果
    /// 只有在冷却时间结束后才会生成新残影
    /// </summary>
    public void CreateAfterImage()
    {
        if (afterImageCooldownTimer < 0)
        {
            // 重置冷却时间
            afterImageCooldownTimer = afterImageCooldown;
            
            // 创建残影实例并设置参数
            GameObject newAfterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);
            newAfterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLooseRate, sr.sprite);
        }
    }

    /// <summary>
    /// 触发屏幕震动效果
    /// </summary>
    /// <param name="_shakePower">震动强度向量</param>
    public void ScreenShake(Vector3 _shakePower)
    {
        // 根据玩家朝向调整X轴震动方向，应用震动乘数
        screenShake.m_DefaultVelocity = new Vector3(_shakePower.x * player.facingDir, _shakePower.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }

    /// <summary>
    /// 播放尘土粒子效果
    /// 通常在玩家着地或急停时调用
    /// </summary>
    public void PlayDustFx()
    {
        if (dustFx != null)
        {
            dustFx.Play();
        }
    }
}
