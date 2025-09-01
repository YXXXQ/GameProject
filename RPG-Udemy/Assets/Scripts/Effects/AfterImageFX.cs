using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AfterImageFX.cs摘要
/// 残影特效控制器，用于创建角色移动时的残影效果
/// 通过逐渐降低精灵透明度来实现淡出效果
/// </summary>
public class AfterImageFX : MonoBehaviour
{
    private SpriteRenderer sr;              // 精灵渲染器引用
    private float colorLooseRate;           // 颜色淡出速率
    
    /// <summary>
    /// 初始化组件引用
    /// </summary>
    private void Awake()
    {
        // 在 Awake 中获取组件，确保 sr 不为 null
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 设置残影参数
    /// </summary>
    /// <param name="_loosigSpeed">淡出速度</param>
    /// <param name="_spriteImage">残影使用的精灵图像</param>
    public void SetupAfterImage(float _loosigSpeed, Sprite _spriteImage)
    {
        // 确保有精灵渲染器引用
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();
            
        // 设置残影精灵和淡出速率
        sr.sprite = _spriteImage;
        colorLooseRate = _loosigSpeed;
    }

    /// <summary>
    /// 每帧更新残影透明度，实现淡出效果
    /// </summary>
    private void Update()
    {
        // 添加空检查，防止 null 引用异常
        if (sr == null)
            return;
            
        // 逐渐降低透明度
        float alpha = sr.color.a - colorLooseRate * Time.deltaTime;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        // 当完全透明时销毁残影对象
        if (sr.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
