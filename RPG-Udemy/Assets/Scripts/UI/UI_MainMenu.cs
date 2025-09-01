using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//2024.11.27到冬天了
public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] UI_FadeScreen fadeScreen;//引用UI_FadeScreen脚本

    private void Start()
    {
        if (SaveManager.instance.HasSavedData() == false) //如果没有存档
            continueButton.SetActive(false);//不显示继续游戏按钮
    }


    public void ContinueGame()
    {
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void NewGame()
    {
        SaveManager.instance.DeleteSaveData();
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));

    }

    public void ExitGame()
    {
        Debug.Log("退出游戏");
    }


    IEnumerator LoadSceneWithFadeEffect(float _delay)//加载场景的协程
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(_delay);
        SceneManager.LoadScene(sceneName);
    }
}