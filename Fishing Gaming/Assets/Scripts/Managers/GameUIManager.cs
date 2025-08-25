using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    [Header("钓鱼深度和鱼钩信息")]
    public TextMeshProUGUI depthText;
    public TextMeshProUGUI fishCapacityText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // 深度显示
    public void UpdateDepthDisplay(float depth)
    {
        if (depthText != null)
            depthText.text = "Depth: " + Mathf.Abs(Mathf.RoundToInt(depth)) + "m";
    }

    // 钓鱼显示
    public void UpdateFishCapacityDisplay(int current, int max)
    {
        if (fishCapacityText != null)
            fishCapacityText.text = "HookCapacity " + current + "/" + max;
    }
}