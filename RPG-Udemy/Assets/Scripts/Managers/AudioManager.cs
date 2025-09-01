using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AudioManager.cs摘要
// 音频管理器，负责控制游戏中的所有音效和背景音乐
// 使用单例模式，提供全局访问点

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // 单例实例

    [SerializeField] private float sfxMinimumDistance; // 音效可听的最小距离
    [SerializeField] private AudioSource[] sfx;        // 音效音频源数组
    [SerializeField] private AudioSource[] bgm;        // 背景音乐音频源数组

    public bool playBgm;                // 是否播放背景音乐
    private int bgmIndex;               // 当前播放的背景音乐索引
    private bool canPlaySFX;            // 是否允许播放音效

    // 初始化单例
    private void Awake()
    {
        // 确保场景中只有一个AudioManager实例
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
            
        // 延迟1秒后允许播放音效，避免场景加载时的音效堆叠
        Invoke("AllowSFX", 1f);
    }

    // 更新背景音乐状态
    private void Update()
    {
        if (!playBgm)
            StopAllBGM(); // 如果不应播放背景音乐，停止所有背景音乐
        else
        {
            // 如果当前背景音乐没有播放，则开始播放
            if (!bgm[bgmIndex].isPlaying)
                PlayBGM(bgmIndex);
        }
    }

    // 播放指定索引的音效
    // 参数：_sfxIndex - 音效索引，_source - 音效来源位置
    public void PlaySFX(int _sfxIndex, Transform _source)
    {
        // 如果不允许播放音效，直接返回
        if (canPlaySFX == false)
        {
            return;
        }
        
        // 如果音效源存在且距离玩家太远，不播放音效
        if (_source != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMinimumDistance)
        {
            return;
        }

        // 检查索引是否有效
        if (_sfxIndex < sfx.Length)
        {
            // 随机调整音调，增加音效变化
            sfx[_sfxIndex].pitch = Random.Range(.85f, 1.15f);
            sfx[_sfxIndex].Play();
        }
    }

    // 立即停止指定索引的音效
    public void StopSFX(int _index) => sfx[_index].Stop();

    // 渐渐停止指定索引的音效（淡出效果）
    public void StopSFXWithTime(int _index)
    {
        // 检查索引是否有效
        if (_index < 0 || _index >= sfx.Length)
            return;

        // 检查音频源是否存在且正在播放
        if (sfx[_index] != null && sfx[_index].isPlaying)
        {
            StartCoroutine(DecreaseVolume(sfx[_index]));
        }
    }

    // 音量淡出协程
    private IEnumerator DecreaseVolume(AudioSource _audio)
    {
        // 检查音频源是否存在
        if (_audio == null)
            yield break;

        // 保存原始音量
        float defaultVolume = _audio.volume;
        float startVolume = _audio.volume;
        float fadeDuration = 1.5f;      // 淡出持续时间
        float startTime = Time.time;

        // 在指定时间内平滑降低音量
        while (_audio != null && Time.time < startTime + fadeDuration)
        {
            // 如果音频源被销毁，退出协程
            if (_audio == null)
                yield break;

            // 计算已经过去的时间比例
            float t = (Time.time - startTime) / fadeDuration;
            // 线性插值降低音量
            _audio.volume = Mathf.Lerp(startVolume, 0f, t);
            // 等待一帧
            yield return null;
        }

        // 再次检查音频源是否存在
        if (_audio != null)
        {
            // 确保音量为0并停止播放
            _audio.volume = 0f;
            _audio.Stop();
            // 恢复原始音量设置
            _audio.volume = defaultVolume;
        }
    }

    // 播放随机背景音乐
    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    // 播放指定索引的背景音乐
    public void PlayBGM(int _bgmIndex)
    {
        bgmIndex = _bgmIndex;

        StopAllBGM(); // 先停止所有背景音乐
        bgm[bgmIndex].Play();
    }

    // 停止所有背景音乐
    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }
    
    // 允许播放音效
    private void AllowSFX() => canPlaySFX = true;
}
