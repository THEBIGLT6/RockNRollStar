using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private Slider m_musicSlider;
    [SerializeField] private Slider m_fxSlider;
    [SerializeField] private Slider m_sensitivitySlider;
    [SerializeField] private Dropdown m_languageDropdown;

    void Start()
    {
        populateLanguageDropdown();
        setSliderPositions();
    }

    private void populateLanguageDropdown()
    {
        m_languageDropdown.ClearOptions();
        List<string> optionsLanguages = new List<string>();
        Language[] languages = Resources.LoadAll<Language>("Languages");

        foreach (var language in languages)
        {
            optionsLanguages.Add(language.name);
        }

        m_languageDropdown.AddOptions(optionsLanguages);

        // Select current language
        GameSettings settings = SettingsManager.Instance.GetSettings();
        for (int i = 0; i < m_languageDropdown.options.Count; i++)
        {
            if (m_languageDropdown.options[i].text == settings.name)
            {
                m_languageDropdown.value = i;
                m_languageDropdown.RefreshShownValue();
                return;
            }
        }
    }

    private void setSliderPositions()
    {
        GameSettings settings = SettingsManager.Instance.GetSettings();
        m_musicSlider.value = settings.musicVolume;
        m_fxSlider.value = settings.sfxVolume;


        m_sensitivitySlider.minValue = GameSettings.MIN_MOUSE_SENSITVITY;
        m_sensitivitySlider.maxValue = GameSettings.MAX_MOUSE_SENSITVITY;
        m_sensitivitySlider.value = settings.mouseSensitivity;
    }

    public void saveSettings()
    {
        GameSettings settings = SettingsManager.Instance.GetSettings();
        settings.musicVolume = m_musicSlider.value;
        settings.sfxVolume = m_fxSlider.value;
        settings.mouseSensitivity = m_sensitivitySlider.value;
        settings.gameLanguage = Resources.Load<Language>("Languages/" + m_languageDropdown.options[m_languageDropdown.value].text);
        SettingsManager.Instance.applySettings();
    }

}
