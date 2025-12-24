using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordCollectible : MonoBehaviour
{
    private Transform m_transform;
    [SerializeField] private AudioClip m_recordSong;
    [SerializeField] private AudioClip m_pickupSound;
    [SerializeField] private RecordUI m_recordUI;
    [SerializeField] private int m_value;
    [SerializeField] private bool m_winningRecord;

    private void Awake()
    {
        m_recordSong.LoadAudioData();
        m_transform = GetComponent<Transform>();
        m_transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
    }

    private void Update()
    {
        m_transform.Rotate(Vector3.up * 90f * Time.deltaTime, Space.World);
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            m_recordUI.collectRecord(m_recordSong);
            MusicManager.Instance.playOneShot(m_pickupSound);
            MusicManager.Instance.playSong(m_recordSong);
            ScoreManager.Instance.addScore(m_value);
            Destroy(gameObject);
        }

        if( m_winningRecord )
        {
            collision.GetComponentInParent<PlayerMovement>().triggerWin();

        }
    }

    public string recordName()
    {
        return m_recordSong.name;
    }
}
