using TMPro;
using UnityEngine;
using UnityEngine.UI;  
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private int m_score;
    private int m_recordsCollected;

    [Header("Text Fields")]
    private TextMeshProUGUI m_hudScore;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        m_hudScore = GetComponent<TextMeshProUGUI>();
        m_recordsCollected = 0;
        m_score = 0;
    }

    public void addRecord()
    {
        m_recordsCollected += 1;
    }

    public void addScore(int increment)
    {
        m_score += increment;
        m_hudScore.text = m_score.ToString("D8");
    }

    public int getScore()
    {
        return m_score;
    }

    public int getRecordsCollected()
    {
        return m_recordsCollected;
    }

}
