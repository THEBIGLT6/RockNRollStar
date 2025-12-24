using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordUI : MonoBehaviour
{
    [SerializeField] private Image[] m_recordImages;
    private int m_collectedRecords;
    private AudioClip[] m_audioClips;

    void Start()
    {
        for (int i = 0; i < m_recordImages.Length; i++)
        {
            m_recordImages[i].enabled = false;
        }   

        m_audioClips = new AudioClip[m_recordImages.Length];
    }

    public void collectRecord( AudioClip audioClip )
    {
        if (m_collectedRecords < m_recordImages.Length)
        {
            m_audioClips[m_collectedRecords] = audioClip;
            m_recordImages[m_collectedRecords].enabled = true;
            m_collectedRecords++;
            ScoreManager.Instance.addRecord();
        }
    }
}
