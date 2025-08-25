using UnityEngine;

/// <summary>
/// 相机宽高比适配器，确保游戏视图在不同设备上保持一致
/// </summary>
public class CameraAspectRatioHandler : MonoBehaviour
{
    [Tooltip("目标宽高比 (宽/高)")]
    public float targetAspect = 16.0f / 9.0f;
    
    [Tooltip("是否保持宽度不变")]
    public bool maintainWidth = true;
    
    [Tooltip("正交相机的参考大小")]
    public float referenceOrthographicSize = 5;
    
    private Camera cam;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraAspectRatioHandler需要附加到带有Camera组件的GameObject上");
            return;
        }
        
        AdjustCamera();
    }
    
    /// <summary>
    /// 调整相机以适应当前屏幕宽高比
    /// </summary>
    public void AdjustCamera()
    {
        // 获取当前屏幕宽高比
        float currentAspect = (float)Screen.width / Screen.height;
        
        // 计算宽高比差异
        float aspectRatio = targetAspect / currentAspect;
        
        // 创建调整矩形
        Rect rect = new Rect(0, 0, 1, 1);
        
        if (maintainWidth)
        {
            // 保持宽度，调整高度
            if (currentAspect < targetAspect)
            {
                // 当前屏幕太窄，需要裁剪顶部和底部
                rect.height = aspectRatio;
                rect.y = (1 - aspectRatio) / 2;
                
                // 调整正交相机大小
                if (cam.orthographic)
                {
                    cam.orthographicSize = referenceOrthographicSize;
                }
            }
            else
            {
                // 当前屏幕太宽，需要调整正交相机大小
                if (cam.orthographic)
                {
                    cam.orthographicSize = referenceOrthographicSize * (currentAspect / targetAspect);
                }
            }
        }
        else
        {
            // 保持高度，调整宽度
            if (currentAspect > targetAspect)
            {
                // 当前屏幕太宽，需要裁剪左右两侧
                rect.width = aspectRatio;
                rect.x = (1 - aspectRatio) / 2;
            }
            else
            {
                // 当前屏幕太窄，需要调整正交相机大小
                if (cam.orthographic)
                {
                    cam.orthographicSize = referenceOrthographicSize / (currentAspect / targetAspect);
                }
            }
        }
        
        // 应用视口矩形
        cam.rect = rect;
        
        Debug.Log($"相机适配: 目标宽高比={targetAspect:F2}, 当前宽高比={currentAspect:F2}, 正交大小={cam.orthographicSize:F2}");
    }
    
    // 在屏幕尺寸变化时重新调整
    void OnRectTransformDimensionsChange()
    {
        AdjustCamera();
    }
}