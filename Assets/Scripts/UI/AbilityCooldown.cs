using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldown : MonoBehaviour
{
    [Header("References")]
    public Image m_abilityRing;
    public PlayerCombat m_playerCombat;

    void Update()
    {
        float current = m_playerCombat.drumCooldownTimer();
        float max = m_playerCombat.drumCooldown();

        m_abilityRing.fillAmount = current / max;
    }
}
