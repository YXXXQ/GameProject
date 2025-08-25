using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FisherController : MonoBehaviour
{
    private Animator animator;
    private bool hasTriggeredEndScreen = false; // 添加标志防止多次触发

    void Awake()
    {
        animator = GetComponent<Animator>();
        hasTriggeredEndScreen = false; // 初始化标志
    }

    public void PlayFishingEndAnimation()
    {
        if (animator != null)
        {
            // 重置标志
            hasTriggeredEndScreen = false;
            animator.SetTrigger("FishingEnd");
        }
    }

    public void OnAnimationComplete()
    {
        // 检查是否已经触发过结算屏幕
        if (!hasTriggeredEndScreen)
        {
            hasTriggeredEndScreen = true; 
            ScreensManager.instance.ChangeScreen(Screens.END);
        }
    }
}
