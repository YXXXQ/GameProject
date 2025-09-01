using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

// UI.cs摘要
// 主UI管理器，负责控制游戏中所有UI界面的显示和切换
// 实现了ISaveManager接口，支持UI设置的存档和读档

public class UI : MonoBehaviour, ISaveManager
{
    [Header("End screens")]
    [SerializeField] private UI_FadeScreen fadeScreen;    // 淡入淡出效果控制器
    [SerializeField] private GameObject endText;          // 游戏结束文本
    [SerializeField] private GameObject restartButton;    // 重新开始按钮
    [Space]

    // 各种UI界面引用
    [SerializeField] private GameObject charcaterUI;      // 角色界面
    [SerializeField] private GameObject skillTreeUI;      // 技能树界面
    [SerializeField] private GameObject craftUI;          // 制作界面
    [SerializeField] private GameObject optionsUI;        // 选项界面
    [SerializeField] private GameObject inGameUI;         // 游戏内界面
    
    // 各种工具提示UI引用
    public UI_ItemTooltip itemTooltip;                    // 物品提示框
    public UI_StatToolTip statToolTip;                    // 属性提示框
    public UI_CraftWindow craftWindow;                    // 制作窗口
    public UI_SkillToolTip skillToolTip;                  // 技能提示框

    [SerializeField] private UI_VolumeSlider[] volumeSlider; // 音量滑块数组

    // 初始化UI
    private void Awake()
    {
        // 初始显示技能树UI（通常是为了测试）
        SwitchTo(skillTreeUI);
        // 激活淡入淡出效果
        fadeScreen.gameObject.SetActive(true);
    }
    
    // 开始时切换到游戏内UI
    void Start()
    {
        SwitchTo(inGameUI);

        // 确保提示框初始隐藏
        itemTooltip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
    }
    
    // 处理UI快捷键
    void Update()
    {
        // Z键：切换角色界面
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SwitchWithKeyTo(charcaterUI);
        }
        // X键：切换制作界面
        if (Input.GetKeyDown(KeyCode.X))
        {
            SwitchWithKeyTo(craftUI);
        }
        // C键：切换技能树界面
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchWithKeyTo(skillTreeUI);
        }
        // V键：切换选项界面
        if (Input.GetKeyDown(KeyCode.V))
        {
            SwitchWithKeyTo(optionsUI);
        }
    }
    
    // 切换到指定UI界面
    public void SwitchTo(GameObject _menu)
    {
        // 遍历并隐藏所有UI界面（除了淡入淡出效果）
        for (int i = 0; i < transform.childCount; i++)
        {
            bool isFadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;

            if (isFadeScreen == false)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        // 激活目标UI界面
        if (_menu != null)
        {
            // 播放UI切换音效
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(7, null);

            _menu.SetActive(true);
        }

        // 根据UI类型暂停或恢复游戏
        if (GameManager.instance != null)
        {
            if (_menu == inGameUI)
            {
                GameManager.instance.PauseGame(false); // 恢复游戏
            }
            else
            {
                GameManager.instance.PauseGame(true);  // 暂停游戏
            }
        }
    }
    
    // 使用快捷键切换UI界面（切换开关）
    public void SwitchWithKeyTo(GameObject _menu)
    {
        // 如果目标UI已激活，则关闭它并检查是否需要返回游戏内UI
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            checkForInGameUI();
            return;
        }
        // 否则切换到目标UI
        SwitchTo(_menu);
    }
    
    // 检查是否需要返回游戏内UI
    private void checkForInGameUI()
    {
        // 检查是否有任何非淡入淡出UI界面处于激活状态
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)
                return; // 如果有，则不需要切换
        }
        
        // 如果没有激活的UI界面，切换回游戏内UI
        SwitchTo(inGameUI);
    }

    // 显示游戏结束界面
    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut(); // 开始淡出效果
        StartCoroutine(EndScreenCorutione());
    }

    // 游戏结束界面显示协程
    IEnumerator EndScreenCorutione()
    {
        yield return new WaitForSeconds(1f); // 等待1秒
        endText.SetActive(true);             // 显示结束文本
        yield return new WaitForSeconds(1f); // 再等待1秒
        restartButton.SetActive(true);       // 显示重新开始按钮
    }

    // 重新开始游戏按钮回调
    public void RestartGameButton() => GameManager.instance.RestartScene();

    // 从存档加载UI设置（ISaveManager接口实现）
    public void LoadData(GameData _data)
    {
        // 加载音量设置
        foreach (KeyValuePair<string, float> pair in _data.volumeSettings)
        {
            foreach (UI_VolumeSlider item in volumeSlider)
            {
                // 匹配音量参数名称
                if (item.parametr == pair.Key)
                {
                    // 设置滑块值
                    item.LoadSlider(pair.Value);
                }
            }
        }
    }

    // 保存UI设置到存档（ISaveManager接口实现）
    public void SaveData(ref GameData _data)
    {
        // 清空现有音量设置
        _data.volumeSettings.Clear();
        
        // 保存所有音量滑块的值
        foreach (UI_VolumeSlider item in volumeSlider)
        {
            _data.volumeSettings.Add(item.parametr, item.slider.value);
        }
    }
}
