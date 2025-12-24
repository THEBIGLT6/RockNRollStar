using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject m_mainMenuUI;
    [SerializeField] private GameObject m_leaderboardUI;
    [SerializeField] private GameObject m_settingsUI;
    [SerializeField] private GameObject m_loadInUI;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip m_selectSFX;
    [SerializeField] private AudioClip m_loadSong;

    [Header("Leaderboard")]
    [SerializeField] private TextMeshProUGUI[] m_leaderboardNames;
    [SerializeField] private TextMeshProUGUI[] m_leaderboardScores;
    private List<int> m_scores;
    private List<string> m_players;

    private void Start()
    {
        setGuiKeys();
        m_loadInUI.SetActive(true);
        m_mainMenuUI.SetActive(true);
        m_leaderboardUI.SetActive(false);
        m_settingsUI.SetActive(false);
        m_loadSong.LoadAudioData();

        SettingsManager.Instance.m_OnSettingsChanged += setGuiKeys;

        SceneManager.sceneLoaded += OnSceneLoaded;
        MusicManager.Instance.playSong(m_loadSong);
    }

    private void OnDestroy()
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.m_OnSettingsChanged -= setGuiKeys;
    }


    private void setGuiKeys()
    {
        Language language = SettingsManager.Instance.GetSettings().gameLanguage;

        LocalizedText[] menuTexts = m_mainMenuUI.GetComponentsInChildren<LocalizedText>(includeInactive: true);
        for (int i = 0; i < menuTexts.Length; i++)
        {
            Text text = menuTexts[i].GetComponent<Text>();
            if (text == null) continue;

            string value = language.getValue(menuTexts[i].key);
            if (value != "") text.text = value;
            else Debug.LogWarning("Could not find value for key: " + menuTexts[i].key + " in language: " + language.name);
        }

        LocalizedText[] settingsTexts = m_settingsUI.GetComponentsInChildren<LocalizedText>(includeInactive: true);
        for (int i = 0; i < settingsTexts.Length; i++)
        {
            Text text = settingsTexts[i].GetComponent<Text>();
            if (text == null) continue;

            string value = language.getValue(settingsTexts[i].key);
            if (value != "") text.text = value;
            else Debug.LogWarning("Could not find value for key: " + settingsTexts[i].key + " in language: " + language.name);
        }

        LocalizedText[] leaderboardTexts = m_leaderboardUI.GetComponentsInChildren<LocalizedText>(includeInactive: true);
        for (int i = 0; i < leaderboardTexts.Length; i++)
        {
            Text text = leaderboardTexts[i].GetComponent<Text>();
            if (text == null) continue;

            string value = language.getValue(leaderboardTexts[i].key);
            if (value != "") text.text = value;
            else Debug.LogWarning("Could not find value for key: " + leaderboardTexts[i].key + " in language: " + language.name);
        }
    }


    // Button Functions
    public void startGame()
    {
        MusicManager.Instance.playOneShot(m_selectSFX);
        SceneManager.LoadScene(1);
    }

    public void openLeaderboard( bool status )
    {
        MusicManager.Instance.playOneShot(m_selectSFX);
        
        if( status ) loadScores();

        m_mainMenuUI.SetActive( !status );
        m_leaderboardUI.SetActive(status);
    }

    public void openSettings(bool status)
    {
        MusicManager.Instance.playOneShot(m_selectSFX);
        m_mainMenuUI.SetActive( !status );
        m_settingsUI.SetActive( status );
    }

    public void quitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void resetScores()
    {
        PlayerPrefs.SetFloat("Score1", 0 );
        PlayerPrefs.SetFloat("Score2", 0 );
        PlayerPrefs.SetFloat("Score3", 0 );
        PlayerPrefs.SetFloat("Score4", 0 );
        PlayerPrefs.SetFloat("Score5", 0 );
        PlayerPrefs.SetString("Player1", "");
        PlayerPrefs.SetString("Player2", "");
        PlayerPrefs.SetString("Player3", "");
        PlayerPrefs.SetString("Player4", "");
        PlayerPrefs.SetString("Player5", "");
        loadScores();
    }

    private void loadScores()
    {
        m_scores = new List<int>();
        m_scores.Add(PlayerPrefs.GetInt("Score1"));
        m_scores.Add(PlayerPrefs.GetInt("Score2"));
        m_scores.Add(PlayerPrefs.GetInt("Score3"));
        m_scores.Add(PlayerPrefs.GetInt("Score4"));
        m_scores.Add(PlayerPrefs.GetInt("Score5"));

        m_players = new List<string>();
        m_players.Add(PlayerPrefs.GetString("Player1"));
        m_players.Add(PlayerPrefs.GetString("Player2"));
        m_players.Add(PlayerPrefs.GetString("Player3"));
        m_players.Add(PlayerPrefs.GetString("Player4"));
        m_players.Add(PlayerPrefs.GetString("Player5"));

        for (int i = 0; i < 5; i++)
        {
            if (m_players[i] != "") m_leaderboardNames[i].text = (i + 1) + ".   " + m_players[i];
            else m_leaderboardNames[i].text = (i + 1) + ".   " + "---";

            if (m_scores[i] != 0 ) m_leaderboardScores[i].text = m_scores[i].ToString("D8");
            else m_leaderboardScores[i].text = "--------";
        }

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            MusicManager.Instance.playSong(m_loadSong);
        }
    }

}
