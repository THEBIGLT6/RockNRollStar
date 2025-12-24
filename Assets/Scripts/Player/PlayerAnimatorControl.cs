using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorControl : MonoBehaviour
{
    [Header("References")]
    private PlayerCombat m_playerCombat;
    private PlayerMovement m_playerMovement;
    [SerializeField] private UIManager m_uiManager;

    private void Start()
    {
        m_playerCombat = GetComponentInParent<PlayerCombat>();
        m_playerMovement = GetComponentInParent<PlayerMovement>();
    }


    public void onDeath()
    {
        m_uiManager.gameLoss();
    }

    public void OnPunch1()
    {
        m_playerCombat.punch1();
    }

    public void OnPunch2()
    {
        m_playerCombat.punch2();
    }

    public void OnKick()
    {
        m_playerCombat.kick();
    }

    public void OnDrumSolo()
    {
        m_playerCombat.drumSoloEvent();
    }

    public void OnGameWon() 
    {
        m_uiManager.gameWon();
    }

}
