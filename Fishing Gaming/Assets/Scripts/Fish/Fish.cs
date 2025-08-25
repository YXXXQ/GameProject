using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

/// <summary>
/// 鱼的行为控制脚本，负责鱼的移动、动画和被钩住的逻辑
/// </summary>
public class Fish : MonoBehaviour
{
    private Fish.FishType type;        // 鱼的类型数据

    private CircleCollider2D coll;     // 鱼的碰撞体组件
    private SpriteRenderer rend;       // 鱼的渲染器组件
    private float screenLeft;          // 屏幕左边缘的世界坐标

    private Tweener tweener;           // 控制鱼移动的DOTween动画
    private Coroutine swapSpriteCoroutine;  // 切换鱼精灵图的协程
    private float spriteSwapInterval = 0.3f;  // 精灵图切换间隔时间

    /// <summary>
    /// 鱼的类型属性，设置时会更新碰撞体半径和精灵图
    /// </summary>
    public Fish.FishType Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;
            coll.radius = type.collierRadius;  // 根据鱼类型设置碰撞体大小
            rend.sprite = type.sprite1;        // 设置初始精灵图
        }
    }

    /// <summary>
    /// 初始化组件引用和屏幕边界
    /// </summary>
    void Awake()
    {
        coll = GetComponent<CircleCollider2D>();
        rend = GetComponentInChildren<SpriteRenderer>();
        screenLeft = Camera.main.ScreenToWorldPoint(Vector3.zero).x;  // 获取屏幕左边缘位置
        
        // 设置渲染顺序，确保鱼显示在背景之上
        rend.sortingOrder = 5; 
    }

    /// <summary>
    /// 重置鱼的位置和状态，用于鱼被钓上来后重新生成
    /// </summary>
    public void ResetFish()
    {
        // 停止当前移动动画
        if (tweener != null)
            tweener.Kill(false);

        // 随机设置鱼的深度
        float num = UnityEngine.Random.Range(type.minLenght, type.maxLenght);
        coll.enabled = true;

        // 将鱼放置在屏幕左侧
        Vector3 position = transform.position;
        position.y = num;
        position.x = screenLeft;
        transform.position = position;

        // 翻转鱼的朝向，使其朝向正确方向
        Vector3 scale = transform.localScale;
        scale.x = -scale.x;
        transform.localScale = scale;

        // 设置鱼的目标位置（屏幕右侧）
        float num2 = 1;
        float y = UnityEngine.Random.Range(num - num2, num + num2);  // 在当前深度附近随机一个Y值
        Vector2 v = new Vector2(-position.x, y);  // 目标位置是屏幕右侧

        // 创建鱼的游动动画
        float num3 = 3;  // 移动时间
        float delay = UnityEngine.Random.Range(0, 2 * num3);  // 随机延迟开始移动
        tweener = transform.DOMove(v, num3, false)
            .SetLoops(-1, LoopType.Yoyo)  // 无限循环来回移动
            .SetEase(Ease.Linear)         // 线性移动
            .SetDelay(delay)
            .OnStepComplete(delegate      // 每次完成一个方向的移动时
            {
                // 翻转鱼的朝向
                Vector3 localScale = transform.localScale;
                localScale.x = -localScale.x;
                transform.localScale = localScale;
            });
            
        // 开始精灵图切换协程，实现鱼游动的动画效果
        swapSpriteCoroutine = StartCoroutine(SpriteSwapLoop());
    }

    /// <summary>
    /// 循环切换鱼的两个精灵图，实现游动动画
    /// </summary>
    IEnumerator SpriteSwapLoop()
    {
        bool useFirst = true;
        while (true)
        {
            // 在两个图之间切换实现动画效果
            rend.sprite = useFirst ? type.sprite1 : type.sprite2;
            useFirst = !useFirst;
            yield return new WaitForSeconds(spriteSwapInterval);  // 等待切换间隔
        }
    }

    /// <summary>
    /// 当鱼被钩住时调用，停止所有动画
    /// </summary>
    public void Hooked()
    {
        coll.enabled = false;  // 禁用碰撞体，防止重复触发
        tweener.Kill(false);   // 停止移动动画
        
        // 当鱼被钩住后需要停止动画，停止之前的图片切换协程，避免资源浪费或产生异常
        if (swapSpriteCoroutine != null)
            StopCoroutine(swapSpriteCoroutine);
            
        // 确保被钩住的鱼显示在背景之上
        rend.sortingOrder = 10;  // 设置更高的渲染顺序，确保显示在背景和其他鱼之上
    }

    /// <summary>
    /// 鱼的类型定义，包含价格、大小、深度范围等属性
    /// </summary>
    [Serializable]
    public class FishType
    {
        public int price;           // 鱼的价格
        public float fishCount;     // 鱼的数量权重
        public float minLenght;     // 最小深度
        public float maxLenght;     // 最大深度
        public float collierRadius; // 碰撞体半径
        public Sprite sprite1;      // 第一个图（游动动画帧1）
        public Sprite sprite2;      // 第二个图（游动动画帧2）
    }
}
