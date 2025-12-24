using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private GameObject m_pauseUI;
    [SerializeField] private GameObject m_hud;
    [SerializeField] private GameObject m_gameOverUI;
    [SerializeField] private GameObject m_gameWonUI;
    [SerializeField] private Text m_finalScoreLoss;
    [SerializeField] private Text m_finalScoreWin;
    [SerializeField] private Image[] m_gameWonRecords;
    [SerializeField] private Image[] m_gameLostRecords;
    [SerializeField] private GameObject m_settingsUI;
    private TimerUI m_timerUI;

    [Header("New High Score")]
    [SerializeField] private GameObject m_newHighScoreUI;
    [SerializeField] private TMP_InputField m_nameInput;
    private List<int> m_scores;
    private List<string> m_players;

    [Header("Sound FX")]
    [SerializeField] private AudioClip m_failFX;
    [SerializeField] private AudioClip m_scoreTallyFX;
    [SerializeField] private AudioClip m_selectFX;
    [SerializeField] private AudioClip m_recordAppear;

    private bool m_pausable;
    private Coroutine m_scoreTally;
    private float m_tallySFXCooldown = 0;
    private float m_recordRevealDelay = 0.5f;

    void Start()
    {
        changeGuiKeys();

        m_nameInput.characterLimit = 3;
        m_timerUI = m_hud.GetComponentInChildren<TimerUI>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_pausable = true;

        m_hud.SetActive(true);
        m_pauseUI.SetActive(false);
        m_gameOverUI.SetActive(false);
        m_gameWonUI.SetActive(false);
        m_settingsUI.SetActive(false);

        SettingsManager.Instance.m_OnSettingsChanged += changeGuiKeys;
    }

    private void OnDestroy()
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.m_OnSettingsChanged -= changeGuiKeys;
    }

    private void Update()
    {
        if (m_pausable && Input.GetKeyDown(KeyCode.Escape))
        {
            pauseGame(!m_pauseUI.activeInHierarchy);
        }

        else if( Input.GetKeyDown(KeyCode.Escape) && m_pauseUI.activeInHierarchy )
        {
            pauseGame(!m_pauseUI.activeInHierarchy);
        }

    }

    private void changeGuiKeys()
    {
        Language language = SettingsManager.Instance.GetSettings().gameLanguage;

        LocalizedText[] pauseTexts = m_pauseUI.GetComponentsInChildren<LocalizedText>(includeInactive: true);
        for( int i = 0; i < pauseTexts.Length; i++ )
        {
            Text text = pauseTexts[i].GetComponent<Text>();
            if (text == null) continue;

            string value = language.getValue(pauseTexts[i].key);
            if (value != "") text.text = value;
            else Debug.LogWarning("Could not find value for key: " + pauseTexts[i].key + " in language: " + language.name);
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

        LocalizedText[] gameoverTexts = m_gameOverUI.GetComponentsInChildren<LocalizedText>(includeInactive: true);
        for (int i = 0; i < gameoverTexts.Length; i++)
        {
            Text text = gameoverTexts[i].GetComponent<Text>();
            if (text == null) continue;

            string value = language.getValue(gameoverTexts[i].key);
            if (value != "") text.text = value;
            else Debug.LogWarning("Could not find value for key: " + gameoverTexts[i].key + " in language: " + language.name);
        }

        LocalizedText[] gameWonTexts = m_gameWonUI.GetComponentsInChildren<LocalizedText>(includeInactive: true);
        for (int i = 0; i < gameWonTexts.Length; i++)
        {
            Text text = gameWonTexts[i].GetComponent<Text>();
            if (text == null) continue;

            string value = language.getValue(gameWonTexts[i].key);
            if (value != "") text.text = value;
            else Debug.LogWarning("Could not find value for key: " + gameWonTexts[i].key + " in language: " + language.name);
        }
    }

    public void pauseGame(bool status)
    {
        MusicManager.Instance.TogglePause(status);
        m_hud.SetActive( !status );
        m_pauseUI.SetActive( status );
        m_pausable = !status;
        Time.timeScale = status ? 0f : 1f;

        Cursor.lockState = status ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = status ? true : false;
    }

    public void gameLoss()
    {
        MusicManager.Instance.stopMusic();
        MusicManager.Instance.playOneShot( m_failFX );

        m_hud.SetActive(false);
        m_gameOverUI.SetActive(true);
        foreach (Image image in m_gameLostRecords)
        {
            image.gameObject.SetActive(false);
        }

        Time.timeScale = 0f;
        addTimingScore();
        addHealthScore();

        int score = ScoreManager.Instance.getScore();
        if ( score != 0 ) animateScore( score );
        else m_finalScoreLoss.text = "00000000";

        m_pausable = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void gameWon()
    {
        loadScores();
        Time.timeScale = 0f;
        addTimingScore();
        addHealthScore();
        int score = ScoreManager.Instance.getScore();

        m_hud.SetActive(false);
        m_gameWonUI.SetActive(true);
        m_newHighScoreUI.SetActive( newHighScore( score ) );
        foreach (Image image in m_gameWonRecords)
        {
            image.gameObject.SetActive(false);
        }

        animateScore( score );
        
        m_pausable = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void respawn()
    {
        MusicManager.Instance.stopMusic();
        MusicManager.Instance.playOneShot(m_selectFX);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
        m_pausable = true;
        
        m_hud.SetActive(true);
        m_pauseUI.SetActive(false);
        m_gameOverUI.SetActive(false);
        m_gameWonUI.SetActive(false);
    }

    public void mainMenu()
    {
        MusicManager.Instance.stopMusic();
        MusicManager.Instance.playOneShot(m_selectFX);
        Time.timeScale = 1f;
        m_pausable = true;
        m_hud.SetActive(true);
        m_pauseUI.SetActive(false);
        m_gameOverUI.SetActive(false);
        m_gameWonUI.SetActive(false);

        SceneManager.LoadScene(0);
    }

    public void quitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void openSettings( bool status )
    {
        MusicManager.Instance.playOneShot(m_selectFX);
        m_pauseUI.SetActive( !status );
        m_settingsUI.SetActive( status );
    }


    // ----- SCORE TALLY -----
    private void animateScore( int finalScore )
    {
        if (m_scoreTally != null)
            StopCoroutine(m_scoreTally);

        Text scoreText;
        Image[] records;
        if (m_gameWonUI.activeInHierarchy)
        {
            scoreText = m_finalScoreWin;
            records = m_gameWonRecords;
        }
        else
        {
            scoreText = m_finalScoreLoss;
            records = m_gameLostRecords;
        }

        m_scoreTally = StartCoroutine( countUpScore(finalScore, scoreText, records) );
    }

    private IEnumerator countUpScore(int finalScore, Text textObj, Image[] records )
    {
        float duration = 5f;
        float timer = 0f;
        int startScore = 0;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(timer / duration);
            t = 1f - Mathf.Pow(1f - t, 3f);
            int currentScore = Mathf.RoundToInt(Mathf.Lerp(startScore, finalScore, t));
            string noisy = addNoiseToNumber(currentScore, 8);

            textObj.text = noisy;
            if (m_tallySFXCooldown <= 0f)
            {
                MusicManager.Instance.playOneShot(m_scoreTallyFX);
                m_tallySFXCooldown = 0.1f;
            }

            m_tallySFXCooldown -= Time.unscaledDeltaTime;

            yield return null;
        }

        textObj.text = finalScore.ToString("D8");

        m_scoreTally = null;

        StartCoroutine(revealRecords(ScoreManager.Instance.getRecordsCollected(), records));
    }

    private string addNoiseToNumber(int baseNumber, int digits)
    {
        string num = baseNumber.ToString("D" + digits);

        char[] chars = num.ToCharArray();

        // randomly replace a few digits with noise
        for (int i = 0; i < chars.Length; i++)
        {
            if (Random.value < 0.2f) chars[i] = (char)('0' + Random.Range(0, 10));
        }

        return new string(chars);
    }

    // ----- RECORD REVEAL -----
    private IEnumerator revealRecords(int collected, Image[] images)
    {

        for (int i = 0; i < collected; i++)
        {
            Image img = images[i];

            MusicManager.Instance.playOneShot(m_recordAppear);

            if (img != null) img.gameObject.SetActive(true);

            yield return new WaitForSecondsRealtime(m_recordRevealDelay);
        }
    }

    // ----- SCORING -----
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
    }

    public void saveScores()
    {
        string playerName = m_nameInput.text.ToUpper();
        int finalScore = ScoreManager.Instance.getScore();

        for (int i = 0; i < m_scores.Count; i++)
        {
            if (finalScore > m_scores[i])
            {
                m_scores.Insert(i, finalScore);
                m_players.Insert(i, playerName);
                break;
            }
        }

        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetInt("Score" + (i + 1), m_scores[i]);
            PlayerPrefs.SetString("Player" + (i + 1), m_players[i]);
        }

        m_newHighScoreUI.SetActive(false);
    }

    private bool newHighScore( int score )
    {
        foreach (float scoreStored in m_scores)
        {
            if (score > scoreStored) return true;
        }

        return false;
    }

    private void addTimingScore()
    {
        float time = m_timerUI.time();
        float bestTime = 30f;     // 30 seconds  - max score
        float worstTime = 300f;   // 5 minutes - no score
        int maxBonus = 5000;

        float t = Mathf.InverseLerp(worstTime, bestTime, time);
        int timeBonus = Mathf.RoundToInt(maxBonus * t);
        ScoreManager.Instance.addScore(timeBonus);
    }

    private void addHealthScore()
    {
        float health = m_hud.GetComponentInChildren<HealthBar>().playerHealthPercent();
        int healthBonus = Mathf.RoundToInt(1000 * health);
        ScoreManager.Instance.addScore(healthBonus);
    }


}
