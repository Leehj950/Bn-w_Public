using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class SoundManager : Singleton<SoundManager>
{
    [Header("BGM")]
    public AudioClip[] bgmClips;                                    // ����� ����� ����
    public float bgmVolume;                                         // ����� ũ��
    private AudioSource bgmAudioSource;                             // ������� ����� ����� �ҽ�

    [Header("SFX")]
    public float sfxVolume;                                         // ȿ���� ũ��

    [Header("Volume")]
    public AudioMixerGroup bgmmixerGroup;
    public AudioMixer audioMixer;

    public SoundObjectPool objectPool;

    public override void Awake()
    {
        base.Awake();
        Init();
        objectPool = GetComponent<SoundObjectPool>();
    }
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        bgmVolume = volume;

        if (bgmVolume <= 0f)
        {

            audioMixer.SetFloat("BGM", -80f); // ������ ���Ұ�
            return;
        }

        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }

    void Init()
    {
        // ����� �÷��̾� �ʱ� ����
        GameObject bgmPlayer = new GameObject("BGM Player");
        bgmPlayer.transform.SetParent(this.transform);
        bgmAudioSource = bgmPlayer.AddComponent<AudioSource>();
        bgmAudioSource.outputAudioMixerGroup = bgmmixerGroup;
        bgmAudioSource.playOnAwake = true;
        bgmAudioSource.loop = true;
        // bgmAudioSource.clip = bgmClips[0];
        // bgmVolume = bgmSlider.value * 1.5f;
        bgmAudioSource.volume = bgmVolume;


        // sfxVolume = sfxSlider.value * 0.3f;

        bgmAudioSource.Play();
    }

    public void PlayBGM(AudioClip clip, bool isLoop = false)
    {
        if (bgmAudioSource.isPlaying) bgmAudioSource.Stop();

        bgmAudioSource.loop = isLoop;
        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }

    public void PlaySFX(AudioClip clip, float soundEffectPitchVariance = 0)
    {
        if (clip == null) return;
        GameObject obj = objectPool.SpawnFromPool("SoundSource");
        obj.SetActive(true);
        SoundSource soundSource = obj.GetComponent<SoundSource>();
        soundSource.Play(clip, sfxVolume, soundEffectPitchVariance);
    }

    public void WaitCurrentBgmAndPlayNext(AudioClip clip, AudioClip nextClip)
    {
        StartCoroutine(WaitCurrentBgmAndPlayNextCor(clip, nextClip));
    }

    private IEnumerator WaitCurrentBgmAndPlayNextCor(AudioClip clip, AudioClip nextClip)
    {
        if (bgmAudioSource.isPlaying) bgmAudioSource.Stop();

        bgmAudioSource.loop = false;
        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();

        yield return new WaitForSeconds(clip.length);

        bgmAudioSource.loop = true;
        bgmAudioSource.clip = nextClip;
        bgmAudioSource.Play();
    }


}
