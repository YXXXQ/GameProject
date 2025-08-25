using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 屏幕管理器，负责UI界面切换和分辨率适配
/// </summary>
public class ScreensManager : MonoBehaviour
{
    public static ScreensManager instance;

    private GameObject currentSceen;
    private Screens currentScreenType; // 当前屏幕类型

    public GameObject endScreen;
    public GameObject gameScreen;
    public GameObject mainScreen;
    public GameObject returnScreen;
    public GameObject tutorialScreen; // 教程屏幕
    public GameObject exitConfirmationScreen; // 退出确认对话框

    public Button lengthButton;
    public Button strengthButton;
    public Button offlineButton;

    public Text gameScreenMoney;
    public Text lengthCostText;
    public Text lengthValueText;
    public Text strengthCostText;
    public Text strengthValueText;
    public Text offlineCostText;
    public Text offlineValueText;
    public Text endScreenMoney;
    public Text returnScreenMoney;

    private int gameCount;

// 分辨率适配相关
[Header("分辨率适配")]
public bool enableResolutionAdaptation = true;
public float referenceAspectRatio = 16f/9f; // 参考宽高比
public Camera mainCamera; // 主相机引用
public RectTransform safeAreaRect; // 安全区域的RectTransform

void Awake()
{
    if (ScreensManager.instance)
        Destroy(base.gameObject);
    else
    {
        ScreensManager.instance = this;
        // 使ScreensManager在场景切换时不被销毁，确保在所有场景中都存在
        DontDestroyOnLoad(gameObject);
    }

    // 强制设置为横屏模式
    Screen.orientation = ScreenOrientation.LandscapeLeft;

    currentSceen = mainScreen;
    currentScreenType = Screens.MAIN;
    
    
}

    void Update()
    {
        // 检测Esc键是否被按下，如果按下则直接退出游戏
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
    
    // 退出游戏的方法
    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void Start()
    {
        CheckIdles();
        UpdateTexts();
        
        // 如果是第一次运行游戏，显示教程屏幕
        if (tutorialScreen != null && !PlayerPrefs.HasKey("TutorialShown"))
        {
            ChangeScreen(Screens.TUTORIAL);
            PlayerPrefs.SetInt("TutorialShown", 1);
            PlayerPrefs.Save();
        }
        
       
    }
    
   
    
    
    
   
    
    
    
    // 获取当前屏幕类型
    public Screens GetCurrentScreen()
    {
        return currentScreenType;
    }
    
   
    
   
    
    public void ChangeScreen(Screens screen)
    {
        currentSceen.SetActive(false);
        currentScreenType = screen; // 记录当前屏幕类型
        
        switch (screen)
        {
            case Screens.MAIN:
                currentSceen = mainScreen;
                UpdateTexts();
                CheckIdles();
                break;

            case Screens.GAME:
                currentSceen = gameScreen;
                gameCount++;
                break;

            case Screens.END:
                currentSceen = endScreen;
                SetEndScreenMoney();
                break;

            case Screens.RETURN:
                currentSceen = returnScreen;
                SetReturnScreenMoney();
                break;
                
            case Screens.TUTORIAL:
                currentSceen = tutorialScreen;
                break;
        }
        currentSceen.SetActive(true);
        
        // 如果AudioManager存在，切换相应的背景音乐
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusicForScreen(screen);
        }
    }
    
    public void SetEndScreenMoney()
    {
        endScreenMoney.text = "$" + IdleManager.instance.totalGain;
    }

    public void SetReturnScreenMoney()
    {
        returnScreenMoney.text = "$" + IdleManager.instance.totalGain + " gained while waiting!";
    }

    public void UpdateTexts()
    {
        gameScreenMoney.text = "$" + IdleManager.instance.wallet;
        lengthCostText.text = "$" + IdleManager.instance.lengthCost;
        lengthValueText.text = -IdleManager.instance.length + "m";
        strengthCostText.text = "$" + IdleManager.instance.strengthCost;
        strengthValueText.text = IdleManager.instance.strength + " fishes.";
        offlineCostText.text = "$" + IdleManager.instance.offlineEarningsCost;
        offlineValueText.text = "$" + IdleManager.instance.offlineEarnings + "/min";
    }
    
    public void CheckIdles()
    {
        int lengthCost = IdleManager.instance.lengthCost;
        int strengthCost = IdleManager.instance.strengthCost;
        int offlineEarningsCost = IdleManager.instance.offlineEarningsCost;
        int wallet = IdleManager.instance.wallet;

        if (wallet < lengthCost)
            lengthButton.interactable = false;
        else
            lengthButton.interactable = true;

        if (wallet < strengthCost)
            strengthButton.interactable = false;
        else
            strengthButton.interactable = true;

        if (wallet < offlineEarningsCost)
            offlineButton.interactable = false;
        else
            offlineButton.interactable = true;
    }
}