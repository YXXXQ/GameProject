using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Device;

// 钩子控制脚本，负责钓鱼的主要逻辑
public class Hook : MonoBehaviour
{
    public Transform hookedTransfrom;  // 鱼被钩住后的挂载点
    private Camera mainCamera;         // 主相机引用
    private Collider2D coll;           // 钩子的碰撞体

    [SerializeField] private int length;    // 钓鱼深度
    [SerializeField] private int strength;  // 可钓鱼数量
    [SerializeField] private int fishCount; // 已钓鱼数量

    private bool canMove;              // 是否可以移动钩子
    private List<Fish> hookedFishes;   // 已钓上的鱼列表
    private Tweener cameraTween;       // 相机动画控制器

    // 初始化组件
    void Awake()
    {
        mainCamera = Camera.main;
        coll = GetComponent<Collider2D>();
        hookedFishes = new List<Fish>();
    }

    // 处理钩子移动和UI更新
    void Update()
    {
        if (canMove)
        {
            Vector3 inputPosition;
            bool hasInput = false;
            
            // 同时支持鼠标和触摸输入
            if (Input.touchCount > 0)
            {
                // 触摸控制（手机）
                Touch touch = Input.GetTouch(0);
                inputPosition = mainCamera.ScreenToWorldPoint(touch.position);
                hasInput = true;
            }
            else if (Input.mousePresent)
            {
                // 鼠标控制（PC）
                inputPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                hasInput = true;
            }
            else
            {
                inputPosition = Vector3.zero;
            }
            
            if (hasInput)
            {
                Vector3 position = transform.position;
                
                // 获取屏幕边界的世界坐标
                float screenWidth = UnityEngine.Screen.width;
                Vector3 leftEdge = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
                Vector3 rightEdge = mainCamera.ScreenToWorldPoint(new Vector3(screenWidth, 0, 0));
                
                // 限制钩子在屏幕范围内
                float clampedX = Mathf.Clamp(inputPosition.x, leftEdge.x + 1f, rightEdge.x - 1f);
                position.x = clampedX;
                
                transform.position = position;
            }
        }
        
        // 更新UI显示
        if (GameUIManager.instance != null && canMove)
        {
            // 更新深度显示
            GameUIManager.instance.UpdateDepthDisplay(transform.position.y);
            
            // 更新鱼钩容量显示
            GameUIManager.instance.UpdateFishCapacityDisplay(fishCount, strength);
        }
    }

    // 开始钓鱼
    public void StartFishing()
    {
        // 初始化参数
        length = IdleManager.instance.length - 20;
        strength = IdleManager.instance.strength;
        fishCount = 0;
        float time = (-length) * 0.1f;

        // 隐藏鼠标光标
        Cursor.visible = false;

        // 相机移动动画
        cameraTween = mainCamera.transform.DOMoveY(length, 1 + time * 0.25f, false).OnUpdate(delegate
        {
            if (mainCamera.transform.position.y <= -11)
                transform.SetParent(mainCamera.transform);
        }).OnComplete(delegate
        {
            coll.enabled = true;
            cameraTween = mainCamera.transform.DOMoveY(0, time * 5, false).OnUpdate(delegate
            {
                if (mainCamera.transform.position.y >= -25f)
                    StopFishing();
            });
        });
        ScreensManager.instance.ChangeScreen(Screens.GAME);
        coll.enabled = false;
        canMove = true;
        hookedFishes.Clear();
    }

    // 停止钓鱼
    void StopFishing()
    {
        canMove = false;
        cameraTween.Kill(false);
        
        // 显示鼠标光标
        Cursor.visible = true;
        
        // 在停止钓鱼时更新一次UI显示，确保显示最终状态
        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.UpdateFishCapacityDisplay(fishCount, strength);
        }
        // 收杆动画在cameraTween的OnComplete回调中触发
        cameraTween = mainCamera.transform.DOMoveY(0, 2, false).OnUpdate(delegate
        {
            if (mainCamera.transform.position.y >= -11)
            {
                transform.SetParent(null);
                transform.position = new Vector2(transform.position.x, -6);
            }
            
            // 在相机移动过程中持续更新深度显示
            if (GameUIManager.instance != null)
            {
                GameUIManager.instance.UpdateDepthDisplay(transform.position.y);
            }
        }).OnComplete(delegate
        {
            
            transform.position = Vector2.down * 6;
            coll.enabled = true;
            int num = 0;

            // 计算钓到的鱼的总价值
            for (int i = 0; i < hookedFishes.Count; i++)
            {
                hookedFishes[i].transform.SetParent(null);
                int fishPrice = hookedFishes[i].Type.price;
                num += fishPrice;
                hookedFishes[i].ResetFish();
            }
            
            //Debug.Log("最终总收益: " + num);
            IdleManager.instance.totalGain = num;  
            // 获取Fisher对象并播放收杆动画
            GameObject fisher = GameObject.Find("Fisher");
            if (fisher != null)
            {
                FisherController fisherController = fisher.GetComponent<FisherController>();
                if (fisherController != null)
                {
                    fisherController.PlayFishingEndAnimation();
                }
                else
                {
                    // 如果没有找到FisherController组件，直接切换屏幕
                    ScreensManager.instance.ChangeScreen(Screens.END);
                }
            }
            else
            {
                // 如果没有找到Fisher对象，直接切换屏幕
                ScreensManager.instance.ChangeScreen(Screens.END);
            }
        });
    }
    
    // 检测鱼的碰撞
    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.CompareTag("Fish") && fishCount != strength)
        {
            fishCount++;
            Fish component = target.GetComponent<Fish>();
            component.Hooked();  
            hookedFishes.Add(component);
            target.transform.SetParent(transform);
            target.transform.position = hookedTransfrom.position;
            target.transform.rotation = hookedTransfrom.rotation;
            target.transform.localScale = Vector3.one;

            // 鱼被钩住时的抖动动画
            target.transform.DOShakeRotation(5, Vector3.forward * 45, 10, 90, false).SetLoops(1, LoopType.Yoyo).OnComplete(delegate
            {
                target.transform.rotation = Quaternion.identity;
            });
            if (fishCount == strength)
                StopFishing();
        }
    }
}