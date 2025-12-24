using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_timerText;
    private float m_elapsedTime = 0f;
    private bool m_isCounting = true;

    void Update()
    {
        if (!m_isCounting) return;

        m_elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(m_elapsedTime / 60);
        int seconds = Mathf.FloorToInt(m_elapsedTime % 60);

        m_timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void stopTimer()
    {
        m_isCounting = false;
    }

    public void startTimer()
    {
        m_isCounting = true;
    }

    public void resetTimer()
    {
        m_elapsedTime = 0f;
        m_timerText.text = "00:00";
    }

    public float time()
    {
        return m_elapsedTime;
    }

}
