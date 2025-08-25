using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("教程界面设置")]
    public Button closeButton;        // 关闭按钮
    public Text tutorialText;         // 教程文本

    void Start()
    {
        // 设置关闭按钮的点击事件
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseTutorial);
        }

        // 设置教程文本内容
        if (tutorialText != null)
        {
            tutorialText.text =
                "钓鱼游戏操作指南：\n\n" +
                "1. 移动鼠标控制钓鱼钩的位置\n" +
                "2. 钓鱼钩会自动下沉，碰到鱼时会自动钩住\n" +
                "3. 当钓满指定数量的鱼或到达最大深度时，会自动收杆\n" +
                "4. 使用获得的金币升级钓鱼深度、钓鱼力量和离线收益\n" +
                "5. 按ESC键可以退出游戏\n\n" +
                "祝您游戏愉快！";
        }
    }

    // 关闭教程界面，返回主界面
    public void CloseTutorial()
    {
        // 先关闭教程界面
        gameObject.SetActive(false);
        // 然后切换到主界面
        ScreensManager.instance.ChangeScreen(Screens.MAIN);
    }
}