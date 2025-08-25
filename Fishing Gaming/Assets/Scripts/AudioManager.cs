using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("������������")]
    public AudioClip backgroundMusic; // ֻʹ��һ����������

    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    private AudioSource musicSource;

    private void Awake()
    {
        // ����ģʽ
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �����л�ʱ������

            // �������ֲ�����
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true; // ѭ������
            musicSource.volume = musicVolume;

            // ���ű�������
            PlayBackgroundMusic();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ���ű�������
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null) return;

        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    // ������������
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
    }

    // ��ͣ��������
    public void PauseMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Pause();
    }

    // �ָ���������
    public void ResumeMusic()
    {
        if (!musicSource.isPlaying)
            musicSource.UnPause();
    }

    // ������ScreensManager�ļ���
    public void PlayMusicForScreen(Screens screen)
    {
        // �������û�в��ţ���ʼ����
        if (!musicSource.isPlaying)
        {
            PlayBackgroundMusic();
        }
    }
}