using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Hitbox Settings")]
    public Transform m_playerTransform;
    private Vector3 m_punch1Offset = new Vector3(0, 1.5f, 0.8f);
    private Vector3 m_punch1BoxSize = new Vector3(0.5f, 1.0f, 0.8f);
    private Vector3 m_punch2Offset = new Vector3(0, 1.5f, 0.8f);
    private Vector3 m_punch2BoxSize = new Vector3(0.9f, 0.8f, 0.8f);
    private Vector3 m_kickOffset = new Vector3(0, 1.5f, 1.0f);
    private Vector3 m_kickBoxSize = new Vector3(1.9f, 0.7f, 1.1f);
    private Color gizmoColor = new Color(1, 0, 0, 0.3f);
    private float m_drumSoloAttakRadius = 2.2f;

    [Header("Damage")]
    [SerializeField] private const int PUNCH1_DAMAGE = 10;
    [SerializeField] private const int PUNCH2_DAMAGE = 15;
    [SerializeField] private const int KICK_DAMAGE = 25;
    [SerializeField] private const int DRUMSHOT_DAMAGE = 25;
    [SerializeField] private const int DRUMSOLO_DAMAGE = 50;

    [Header("Cooldowns/Timers")]
    private float COMBO_RESET_TIME = 0.8f;
    private float KICK_COOLDOWN = 1.0f;
    private float m_kickCooldownTimer = Mathf.Infinity;
    private int m_comboStep = 0;
    private float m_lastStrikeTime = 0f;
    private float DRUMSHOT_COOLDOWN = 2.0f;
    private float m_drumShotCooldownTimer = Mathf.Infinity;
    private float DRUMSOLO_COOLDOWN = 30.0f;
    private float m_drumSoloCooldownTimer = Mathf.Infinity;
    private bool m_isDrumSoloActive = false;

    [Header("Instrument GOs")]
    [SerializeField] private GameObject m_drums;
    [SerializeField] private GameObject m_leftStick;
    [SerializeField] private GameObject m_rightStick;

    [Header("SoundFX/Audio")]
    AudioSource m_audioSource;
    [SerializeField] private AudioClip m_punch1SFX;
    [SerializeField] private AudioClip m_punch2SFX;
    [SerializeField] private AudioClip m_kickSFX;
    [SerializeField] private AudioClip m_drumSolo;
    private float m_soundFxVolume;

    // Private variables
    private Animator m_animator;
    private PlayerMovement m_playerMovement;
    private Health m_playerHealth;


    // -------------- UNITY METHODS -------------- 
    private void Awake()
    {
        m_drums.SetActive(false);
        m_leftStick.SetActive(false);
        m_rightStick.SetActive(false);

        m_playerHealth = GetComponent<Health>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_animator = GetComponentInChildren<Animator>();
        m_audioSource = GetComponent<AudioSource>();

        m_soundFxVolume = SettingsManager.Instance.GetSettings().sfxVolume;
        SettingsManager.Instance.m_OnSettingsChanged += () => { m_soundFxVolume = SettingsManager.Instance.GetSettings().sfxVolume; };
    }

    private void Update()
    {
        if (Time.timeScale >= 1f)
        {
            if (Input.GetMouseButtonDown(0) && !m_isDrumSoloActive)
            {
                handleStrike();
            }

            if (Input.GetMouseButtonDown(1) && m_kickCooldownTimer > KICK_COOLDOWN && !m_isDrumSoloActive)
            {
                handleKick();
            }

            if (Input.GetKeyDown(KeyCode.F) && m_drumShotCooldownTimer > DRUMSHOT_COOLDOWN && !m_isDrumSoloActive)
            {
                drumShot();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) && m_drumSoloCooldownTimer > DRUMSOLO_COOLDOWN && !m_isDrumSoloActive)
            {
                drumSolo();
            }
        }

        m_kickCooldownTimer += Time.deltaTime;
        m_drumShotCooldownTimer += Time.deltaTime;
        m_drumSoloCooldownTimer += Time.deltaTime;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(m_playerTransform.position, m_playerTransform.rotation, Vector3.one);
        Gizmos.DrawWireCube(m_punch1Offset, m_punch1BoxSize);
        Gizmos.DrawWireCube(m_punch2Offset, m_punch2BoxSize);
        Gizmos.DrawWireCube(m_kickOffset, m_kickBoxSize);
        //Gizmos.DrawWireSphere(transform.position, m_drumSoloAttakRadius);
    }


    // -------------- HANDLE MOVEMENTS --------------
    private void handleStrike()
    {
        float currentTime = Time.time;

        if (currentTime - m_lastStrikeTime <= COMBO_RESET_TIME) m_comboStep++;
        else m_comboStep = 1;

        m_lastStrikeTime = currentTime;

        if (m_comboStep == 1)
        {
            m_playerMovement.SuspendMovement(1f);
            m_animator.SetTrigger("punch1");
            m_audioSource.PlayOneShot(m_punch1SFX, m_soundFxVolume);
        }
        else if (m_comboStep == 2)
        {
            m_playerMovement.SuspendMovement(1f);
            m_animator.SetTrigger("punch2");
            m_audioSource.PlayOneShot(m_punch2SFX, m_soundFxVolume);
            m_comboStep = 0;
        }
    }

    private void handleKick()
    {
        m_playerMovement.SuspendMovement(1f);
        m_animator.SetTrigger("kick");
        m_audioSource.PlayOneShot(m_kickSFX, m_soundFxVolume);
        m_comboStep = 0;
        m_kickCooldownTimer = 0f;
    }

    private void drumSolo()
    {
        m_audioSource.PlayOneShot(m_drumSolo, m_soundFxVolume);
        m_playerMovement.SuspendMovement(11f);
        m_playerHealth.setImmunity(11f);
        m_animator.SetTrigger("drumSolo");
        m_isDrumSoloActive = true;

        m_drums.SetActive(true);
        m_leftStick.SetActive(true);
        m_rightStick.SetActive(true);
    }

    private void drumShot()
    {
        m_playerMovement.SuspendMovement(3f);
        m_animator.SetTrigger("drumShot");
        m_drumShotCooldownTimer = 0f;

        m_drums.SetActive(true);
        m_leftStick.SetActive(true);
        m_rightStick.SetActive(true);
    }

    private void hideDrums()
    {
        m_drums.SetActive(false);
        m_leftStick.SetActive(false);
        m_rightStick.SetActive(false);
    }

    public void OnAnimationFinished()
    {
        hideDrums();
    }

    public void resetSoloTimer()
    {
        m_drumSoloCooldownTimer = 0f;
        m_isDrumSoloActive = false;
    }

    public float drumCooldownTimer()
    {
        return m_drumSoloCooldownTimer;
    }

    public float drumCooldown()
    {
        return DRUMSOLO_COOLDOWN;
    }


    // -------------- ANIMATION EVENTS --------------

    private void hitboxAttack(Vector3 offset, Vector3 size, int damage)
    {
        Vector3 worldCenter = m_playerTransform.position + m_playerTransform.rotation * offset;
        Vector3 halfExtents = size * 0.5f;
        Quaternion rotation = m_playerTransform.rotation;

        // Detect everything (or a physics layer of your choice)
        Collider[] hits = Physics.OverlapBox(
            worldCenter,
            halfExtents,
            rotation
        );

        foreach (Collider hit in hits)
        {
            // Only accept objects tagged "Enemy"
            if (hit.CompareTag("Enemy"))
            {
                if (hit.TryGetComponent(out Health enemy))
                {
                    enemy.damage(damage, m_playerTransform.position);
                }
            }
        }
    }

    public void punch1()
    {
        hitboxAttack(m_punch1Offset, m_punch1BoxSize, PUNCH1_DAMAGE);
    }

    public void punch2()
    {
        hitboxAttack(m_punch2Offset, m_punch2BoxSize, PUNCH2_DAMAGE);
    }

    public void kick()
    {
        hitboxAttack(m_kickOffset, m_kickBoxSize, KICK_DAMAGE);
    }

    public void drumShotEvent()
    {
    }

    public void drumSoloEvent()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, m_drumSoloAttakRadius);

        foreach (Collider hit in hits)
        {
            if( hit.CompareTag("Enemy") )

                if (hit.TryGetComponent(out Health enemy))
                {
                    enemy.damage(DRUMSOLO_DAMAGE, m_playerTransform.position, true );
                }
        }
    }

}
