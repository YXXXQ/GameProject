using UnityEngine;

/// <summary>
/// 移动设备性能优化器，根据设备性能自动调整游戏设置
/// </summary>
public class MobileOptimizer : MonoBehaviour
{
    [Tooltip("目标帧率")]
    public int targetFrameRate = 60;
    
    [Tooltip("是否启用垂直同步")]
    public bool vSyncEnabled = false;
    
    [Tooltip("是否根据设备性能自动调整质量")]
    public bool autoAdjustQuality = true;
    
    private int frameCounter = 0;
    private float timeCounter = 0.0f;
    private float lastFramerate = 0.0f;
    private float refreshTime = 0.5f;
    
    void Start()
    {
        // 设置目标帧率
        Application.targetFrameRate = targetFrameRate;
        
        // 设置垂直同步
        QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;
        
        // 在移动平台上禁用屏幕休眠
        #if UNITY_ANDROID || UNITY_IOS
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        #endif
        
        // 根据设备性能自动选择质量级别
        if (autoAdjustQuality)
        {
            AutoDetectAndSetQuality();
        }
    }
    
    void Update()
    {
        // 计算当前帧率
        if (timeCounter < refreshTime)
        {
            timeCounter += Time.deltaTime;
            frameCounter++;
        }
        else
        {
            lastFramerate = (float)frameCounter / timeCounter;
            frameCounter = 0;
            timeCounter = 0.0f;
            
            // 如果帧率过低，自动降低质量
            if (autoAdjustQuality && lastFramerate < targetFrameRate * 0.8f)
            {
                DecreaseQuality();
            }
        }
    }
    
    // 根据设备性能自动设置质量级别
    private void AutoDetectAndSetQuality()
    {
        // 获取系统内存大小（MB）
        int systemMemorySize = SystemInfo.systemMemorySize;
        
        // 获取处理器核心数
        int processorCount = SystemInfo.processorCount;
        
        // 根据设备性能设置质量
        if (systemMemorySize >= 4000 && processorCount >= 6)
        {
            // 高端设备
            QualitySettings.SetQualityLevel(2, true); // High
        }
        else if (systemMemorySize >= 2000 && processorCount >= 4)
        {
            // 中端设备
            QualitySettings.SetQualityLevel(1, true); // Medium
        }
        else
        {
            // 低端设备
            QualitySettings.SetQualityLevel(0, true); // Low
        }
        
        Debug.Log($"设备内存: {systemMemorySize}MB, 处理器核心数: {processorCount}, 设置质量级别: {QualitySettings.GetQualityLevel()}");
    }
    
    // 降低质量级别以提高性能
    private void DecreaseQuality()
    {
        int currentQuality = QualitySettings.GetQualityLevel();
        if (currentQuality > 0)
        {
            QualitySettings.SetQualityLevel(currentQuality - 1, true);
            Debug.Log($"帧率过低 ({lastFramerate:F1}), 降低质量级别至: {QualitySettings.GetQualityLevel()}");
        }
    }
}