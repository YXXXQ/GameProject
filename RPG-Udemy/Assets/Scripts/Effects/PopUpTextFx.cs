using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// PopUpTextFx.cs摘要
/// 弹出文本特效控制器，用于显示伤害数字、治疗量等临时文本
/// 文本会上升并在一段时间后淡出消失
/// </summary>
public class PopUpTextFx : MonoBehaviour
{
    private TextMeshPro myText;                          // TextMeshPro组件引用

    [SerializeField] private float speed;                // 文本上升速度
    [SerializeField] private float disappearanceSpeed;   // 文本消失阶段的上升速度（通常比正常速度慢）
    [SerializeField] private float colorDisappearanceSpeed; // 文本颜色淡出速度

    [SerializeField] private float lifeTime;             // 文本显示持续时间

    private float textTimer;                             // 文本生命周期计时器

    /// <summary>
    /// 初始化组件引用和计时器
    /// </summary>
    void Start()
    {
        // 获取TextMeshPro组件
        myText = GetComponent<TextMeshPro>();
        
        // 初始化生命周期计时器
        textTimer = lifeTime;
    }

    /// <summary>
    /// 每帧更新文本位置和透明度
    /// </summary>
    void Update()
    {
        // 文本沿Y轴方向上升
        transform.position = Vector2.MoveTowards(
            transform.position, 
            new Vector2(transform.position.x, transform.position.y + 1), 
            speed * Time.deltaTime
        );

        // 生命周期计时器递减
        textTimer -= Time.deltaTime;

        // 当生命周期结束后，开始淡出过程
        if (textTimer < 0)
        {
            // 文本的透明度逐渐减少
            float alpha = myText.color.a - colorDisappearanceSpeed * Time.deltaTime;
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, alpha);

            // 当透明度较低时，减慢文本的上升速度，创造渐渐消失的效果
            if (myText.color.a < 50)
                speed = disappearanceSpeed;

            // 当完全透明时，销毁文本对象
            if (myText.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
