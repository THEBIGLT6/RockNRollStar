using System;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager _instance;
    public static SettingsManager Instance => _instance;

    private GameSettings m_settings;

    public event Action m_OnSettingsChanged;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        m_settings = ScriptableObject.CreateInstance<GameSettings>();

        loadSettings();
        applySettings();
    }

    private void OnApplicationQuit()
    {
        saveSettings();
    }

    private void loadSettings()
    {
        m_settings.musicVolume = PlayerPrefs.GetFloat("MusicVolume", GameSettings.MUSIC_VOLUME_DEFAULT);
        m_settings.sfxVolume = PlayerPrefs.GetFloat("SFXVolume", GameSettings.SFX_VOLUME_DEFAULT);
        m_settings.mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", GameSettings.MOUSE_SENSITIVITY_DEFAULT);
        string language = PlayerPrefs.GetString("Language", GameSettings.LANGUAGE_DEFAULT);
        m_settings.gameLanguage = Resources.Load<Language>("Languages/" + language );
    }

    private void saveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", m_settings.musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", m_settings.sfxVolume);
        PlayerPrefs.SetFloat("MouseSensitivity", m_settings.mouseSensitivity);
        PlayerPrefs.SetString("Language", m_settings.gameLanguage.name );
        PlayerPrefs.Save();
    }

    public void applySettings()
    {
        m_OnSettingsChanged?.Invoke();
        saveSettings();
    }

    public GameSettings GetSettings()
    {
        return m_settings;
    }

}