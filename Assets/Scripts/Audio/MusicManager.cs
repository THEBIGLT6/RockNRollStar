using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    private AudioSource m_audioSourceMusic;
    private AudioSource m_audioSourceSoundFx;
    private Coroutine m_fadeRoutine;
    private float m_soundFXVolume;
    private float m_musicVolume;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        AudioSource[] sources = GetComponents<AudioSource>();
        m_audioSourceMusic = sources[0];
        m_audioSourceSoundFx = sources[1];
        setVolumes();

        SettingsManager.Instance.m_OnSettingsChanged += setVolumes;
    }

    public void playOneShot(AudioClip clip)
    {
        m_audioSourceSoundFx.PlayOneShot(clip, m_soundFXVolume);
    }

    public void playSong(AudioClip clip)
    {
        float fadeDuration = 1.0f;

        if (m_fadeRoutine != null)
            StopCoroutine(m_fadeRoutine);

        // Fade out current, then play new clip and fade in
        if (m_audioSourceMusic.isPlaying)
        {
            m_fadeRoutine = StartCoroutine(FadeOutAndPlay(clip, fadeDuration));
        }
        
        // No music playing, just start the clip and optionally fade in from 0
        else
        {
            float defaultVolume = 0.7f;
            m_audioSourceMusic.clip = clip;
            m_audioSourceMusic.volume = 0f;
            m_audioSourceMusic.Play();
            m_fadeRoutine = StartCoroutine(FadeVolume(0f, defaultVolume, fadeDuration));
        }
    }

    public void stopMusic()
    {
        if (m_fadeRoutine != null)
        {
            StopCoroutine(m_fadeRoutine);
            m_fadeRoutine = null;
        }

        m_audioSourceMusic.Stop();
        
    }

    public void TogglePause(bool pause)
    {
        if (pause && m_audioSourceMusic.isPlaying)
        {
            m_audioSourceMusic.Pause();
        }
        else if ( !pause && !m_audioSourceMusic.isPlaying && m_audioSourceMusic.clip != null)
        {
            m_audioSourceMusic.UnPause();
        }
    }

    private IEnumerator FadeOutAndPlay(AudioClip newClip, float duration)
    {
        float startVolume = m_audioSourceMusic.volume;

        yield return FadeVolume(startVolume, 0f, duration);

        m_audioSourceMusic.Stop();
        m_audioSourceMusic.clip = newClip;
        m_audioSourceMusic.Play();

        yield return FadeVolume(0f, m_musicVolume, duration);

        m_fadeRoutine = null;
    }

    private IEnumerator FadeVolume(float from, float to, float duration)
    {
        float elapsed = 0f;
        m_audioSourceMusic.volume = Mathf.Clamp01(from);

        if (duration <= 0f)
        {
            m_audioSourceMusic.volume = Mathf.Clamp01(to);
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            m_audioSourceMusic.volume = Mathf.Lerp(from, to, t);
            yield return null;
        }

        m_audioSourceMusic.volume = Mathf.Clamp01(to);
    }

    private void setVolumes()
    {
        m_soundFXVolume = SettingsManager.Instance.GetSettings().sfxVolume;
        m_musicVolume = SettingsManager.Instance.GetSettings().musicVolume;

        if(m_audioSourceMusic.isPlaying )
        {
            m_audioSourceMusic.volume = m_musicVolume;
        }
    }

}
