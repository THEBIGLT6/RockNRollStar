using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health m_playerHealth;
    [SerializeField] private Image m_currentHealthBar;

    private void Update()
    {
        m_currentHealthBar.fillAmount = m_playerHealth.currentHealth() / 100f;
    }

    public float playerHealthPercent()
    {
        return m_playerHealth.currentHealth() / 100f;
    }   

}
