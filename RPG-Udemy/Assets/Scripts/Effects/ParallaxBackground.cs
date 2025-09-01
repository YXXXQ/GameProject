using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ParallaxBackground.cs摘要
/// 视差背景控制器，用于创建2D游戏中的视差滚动效果
/// 通过调整背景移动速度相对于相机移动速度的比例来实现深度感
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;                      // 相机引用
    
    [SerializeField] private float parallaxEffect;  // 视差效果强度（0-1之间）
                                                   // 值越小，背景移动越慢，看起来越远
                                                   // 值为1时，背景与相机同步移动
                                                   // 值为0时，背景完全静止

    private float xPosition;                     // 背景初始X坐标
    private float length;                        // 背景精灵宽度
    
    /// <summary>
    /// 初始化组件引用和背景参数
    /// </summary>
    private void Start()
    {
        // 获取主相机引用
        cam = GameObject.Find("Main Camera");

        // 获取背景精灵的宽度和初始位置
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        xPosition = transform.position.x;
    }
    
    /// <summary>
    /// 每帧更新背景位置，实现视差效果
    /// </summary>
    private void Update()
    {
        // 计算相机移动的总距离
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);
        
        // 计算背景应该移动的距离
        float distanceToMove = cam.transform.position.x * parallaxEffect;

        // 更新背景位置，只改变X坐标
        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

        // 无限滚动逻辑：当相机移动足够远时，重新定位背景
        if(distanceMoved > xPosition + length)
            xPosition = xPosition + length;  // 向右重新定位
        else if(distanceMoved < xPosition - length)
            xPosition = xPosition - length;  // 向左重新定位
    }
}
