using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;

// EntityFX.cs摘要
// 实体视觉效果控制器，负责处理所有实体（玩家、敌人等）的视觉特效
// 包括伤害文本、受击闪烁、状态效果（点燃、冰冻、感电）和攻击特效

public class EntityFX : MonoBehaviour
{
    protected Player player;                           // 玩家引用
    protected SpriteRenderer sr;                       // 精灵渲染器引用
    
    [Header("弹出文本")]
    [SerializeField] private GameObject popUpTextPrefab; // 伤害数字预制体

    [Header("闪光特效")]
    [SerializeField] private float flashDuration;      // 受击闪烁持续时间
    [SerializeField] private Material hitMat;          // 受击材质
    private Material originalMat;                      // 原始材质

    [Header("异常状态颜色")]
    [SerializeField] private Color[] chillColor;       // 冰冻状态颜色
    [SerializeField] private Color[] igniteColor;      // 点燃状态颜色
    [SerializeField] private Color[] shockColor;       // 感电状态颜色

    [Header("异常状态粒子")]
    [SerializeField] private ParticleSystem igniteFX;  // 点燃粒子效果
    [SerializeField] private ParticleSystem chillFX;   // 冰冻粒子效果
    [SerializeField] private ParticleSystem shockFX;   // 感电粒子效果

    [Header("攻击特效")]
    [SerializeField] private GameObject hitFx;         // 普通攻击特效
    [SerializeField] private GameObject criticalHitFx; // 暴击攻击特效

    [Header("弹出文本颜色")]
    [SerializeField] private Color physicalDamageColor = Color.white;                  // 物理伤害颜色
    [SerializeField] private Color fireDamageColor = new Color(1f, 0.5f, 0f);          // 火焰伤害颜色（橙红色）
    [SerializeField] private Color iceDamageColor = new Color(0.5f, 0.8f, 1f);         // 冰冻伤害颜色（淡蓝色）
    [SerializeField] private Color lightningDamageColor = new Color(1f, 1f, 0f);       // 闪电伤害颜色（黄色）
    [SerializeField] private Color criticalDamageColor = new Color(1f, 0f, 0f);        // 暴击伤害颜色（红色）
    [SerializeField] private Color healColor = new Color(0f, 1f, 0f);                  // 治疗颜色（绿色）
    
    private GameObject myHealthBar;                    // 生命值条引用

    // 初始化组件引用
    protected virtual void Start()
    {
        // 获取精灵渲染器
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError($"SpriteRenderer not found on {gameObject.name}");
            enabled = false;  // 禁用脚本避免后续错误
            return;
        }

        // 获取玩家引用
        if (PlayerManager.instance != null)
            player = PlayerManager.instance.player;
        else
            Debug.LogError("PlayerManager instance is null");

        // 保存原始材质
        originalMat = sr.material;

        // 获取生命值条引用
        var healthBar = GetComponentInChildren<UI_HealthBar>();
        if (healthBar != null)
            myHealthBar = healthBar.gameObject;
        else
            Debug.LogWarning($"UI_HealthBar not found on {gameObject.name}");
    }

    // 创建伤害或治疗弹出文本
    public void CreatePopUpText(string _text, DamageType _damageType = DamageType.Physical, bool _isCritical = false)
    {
        // 随机位置偏移，使文本不重叠
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(1.5f, 3f);
        Vector3 positionOffset = new Vector3(randomX, randomY, 0);

        // 创建弹出文本实例
        GameObject newText = Instantiate(popUpTextPrefab, transform.position + positionOffset, Quaternion.identity);
        TextMeshPro textMesh = newText.GetComponent<TextMeshPro>();
        textMesh.text = _text;

        // 根据伤害类型和是否暴击设置颜色和大小
        if (_isCritical)
        {
            textMesh.color = criticalDamageColor;
            textMesh.fontSize += 2; // 暴击伤害文字稍大
        }
        else
        {
            // 根据伤害类型选择颜色
            switch (_damageType)
            {
                case DamageType.Physical:
                    textMesh.color = physicalDamageColor;
                    break;
                case DamageType.Fire:
                    textMesh.color = fireDamageColor;
                    break;
                case DamageType.Ice:
                    textMesh.color = iceDamageColor;
                    break;
                case DamageType.Lightning:
                    textMesh.color = lightningDamageColor;
                    break;
                case DamageType.Heal:
                    textMesh.color = healColor;
                    break;
            }
        }
    }

    // 设置实体透明度（用于隐身或显示）
    public void MakeTransprent(bool _transprent)
    {
        // 添加null检查
        if (sr == null)
        {
            Debug.LogWarning("SpriteRenderer is null in MakeTransprent");
            return;
        }
        
        if (_transprent)
        {
            // 隐藏生命值条和精灵
            if (myHealthBar != null)
                myHealthBar.SetActive(false);
            sr.color = Color.clear;  // 完全透明
        }
        else
        {
            // 显示生命值条和精灵
            if (myHealthBar != null)
                myHealthBar.SetActive(true);
            sr.color = Color.white;  // 完全不透明
        }
    }
    
    // 受击闪烁效果
    private IEnumerator FlashFX()
    {
        // 切换到受击材质并设为白色
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;

        // 等待闪烁持续时间
        yield return new WaitForSeconds(flashDuration);

        // 恢复原始颜色和材质
        sr.color = currentColor;
        sr.material = originalMat;
    }
    
    // 红色闪烁效果
    private void RedColorBlink()
    {
        // 在白色和红色之间交替
        if (sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }
    
    // 取消所有颜色变化和粒子效果
    private void CancelColorChange()
    {
        // 取消所有定时调用
        CancelInvoke();
        // 恢复默认颜色
        sr.color = Color.white;

        // 停止所有状态粒子效果
        igniteFX.Stop();
        chillFX.Stop();
        shockFX.Stop();
    }
    
    // 应用点燃效果指定时间
    public void IgniteFxFor(float _seconds)
    {
        // 播放点燃粒子效果
        igniteFX.Play();
        // 开始颜色闪烁
        InvokeRepeating("igniteColorFx", 0, 0.3f);
        // 设置效果持续时间
        Invoke("CancelColorChange", _seconds);
    }
    
    // 应用冰冻效果指定时间
    public void ChilFxFor(float _seconds)
    {
        // 播放冰冻粒子效果
        chillFX.Play();
        // 开始颜色闪烁
        InvokeRepeating("ChillColorFx", 0, 0.3f);
        // 设置效果持续时间
        Invoke("CancelColorChange", _seconds);
    }
    
    // 应用感电效果指定时间
    public void ShockFxFor(float _seconds)
    {
        // 播放感电粒子效果
        shockFX.Play();
        // 开始颜色闪烁
        InvokeRepeating("shockColorFx", 0, 0.3f);
        // 设置效果持续时间
        Invoke("CancelColorChange", _seconds);
    }
    
    // 点燃状态颜色闪烁
    private void igniteColorFx()
    {
        // 在两种点燃颜色之间交替
        if (sr.color != igniteColor[0])
        {
            sr.color = igniteColor[0];
        }
        else
        {
            sr.color = igniteColor[1];
        }
    }
    
    // 冰冻状态颜色闪烁
    private void ChillColorFx()
    {
        // 在两种冰冻颜色之间交替
        if (sr.color != chillColor[0])
        {
            sr.color = chillColor[0];
        }
        else
        {
            sr.color = chillColor[1];
        }
    }
    
    // 感电状态颜色闪烁
    private void shockColorFx()
    {
        // 在两种感电颜色之间交替
        if (sr.color != shockColor[0])
        {
            sr.color = shockColor[0];
        }
        else
        {
            sr.color = shockColor[1];
        }
    }

    // 创建攻击命中特效
    public void CreateFitFx(Transform _target, bool _critical)
    {
        // 随机旋转和位置偏移，增加视觉变化
        float zRotation = Random.Range(-90, 90);
        float xPosition = Random.Range(-0.5f, 0.5f);
        float yPosition = Random.Range(-0.5f, 0.5f);

        Vector3 hitFxRotion = new Vector3(0, 0, zRotation);
        GameObject hitPrefab = hitFx;  // 默认使用普通攻击特效

        // 如果是暴击，使用暴击特效并调整旋转
        if (_critical)
        {
            hitPrefab = criticalHitFx;

            float yRotation = 0;
            zRotation = Random.Range(-45, 45);

            // 根据实体朝向调整Y轴旋转
            if (GetComponent<Entity>().facingDir == -1)
                yRotation = 180;

            hitFxRotion = new Vector3(0, yRotation, zRotation);
        }

        // 创建特效实例并设置位置、旋转
        GameObject newHitFx = Instantiate(hitPrefab, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity, _target);
        newHitFx.transform.Rotate(hitFxRotion);
        
        // 0.5秒后销毁特效
        Destroy(newHitFx, 0.5f);
    }
}
