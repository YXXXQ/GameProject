using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("背景音乐设置")]
    public AudioClip backgroundMusic; // 只使用一个背景音乐

    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    private AudioSource musicSource;

    private void Awake()
    {
        // 单例模式
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 场景切换时不销毁

            // 创建音乐播放器
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true; // 循环播放
            musicSource.volume = musicVolume;

            // 播放背景音乐
            PlayBackgroundMusic();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 播放背景音乐
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null) return;

        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    // 设置音乐音量
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
    }

    // 暂停背景音乐
    public void PauseMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Pause();
    }

    // 恢复背景音乐
    public void ResumeMusic()
    {
        if (!musicSource.isPlaying)
            musicSource.UnPause();
    }

    // 保持与ScreensManager的兼容
    public void PlayMusicForScreen(Screens screen)
    {
        // 如果音乐没有播放，则开始播放
        if (!musicSource.isPlaying)
        {
            PlayBackgroundMusic();
        }
    }
}