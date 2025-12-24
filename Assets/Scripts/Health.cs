using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float MAX_HEALTH = 100f;
    private float m_currentHealth;
    private Animator m_animator;
    private AudioSource m_audioSource;
    private bool m_isDead;
    private bool m_isPlayer;
    private bool m_isImmune = false;
    private Coroutine m_immunityRoutine = null;

    [Header("SoundFX")]
    [SerializeField] private AudioClip[] m_hurtSounds;
    [SerializeField] private AudioClip m_deathSound1;
    [SerializeField] private AudioClip m_deathSound2;
    [SerializeField] private AudioClip m_deathSound3;
    private float m_soundFxVolume;

    private void Start()
    {
        m_currentHealth = MAX_HEALTH;
        m_audioSource = GetComponent<AudioSource>();

        m_isPlayer = CompareTag("Player");
        if( m_isPlayer ) m_animator = GetComponentInChildren<Animator>();
        else m_animator = GetComponent<Animator>();

        m_soundFxVolume = SettingsManager.Instance.GetSettings().sfxVolume;
        SettingsManager.Instance.m_OnSettingsChanged += () => { m_soundFxVolume = SettingsManager.Instance.GetSettings().sfxVolume; };
    }

    public void damage(int damage, Vector3 attackerPosition, bool drumSolo = false, bool stun = false)
    {
        if (m_isImmune) return;

        m_currentHealth = Mathf.Clamp(m_currentHealth - damage, 0, MAX_HEALTH);

        if( !m_isPlayer ) ScoreManager.Instance.addScore( damage );

        if ( m_currentHealth > 0 )
        {
            int index = Random.Range(0, m_hurtSounds.Length);
            m_audioSource.PlayOneShot(m_hurtSounds[index], m_soundFxVolume);

            Vector3 toAttacker = (attackerPosition - transform.position).normalized;
            float dotForward = Vector3.Dot(transform.forward, toAttacker);
            float dotRight = Vector3.Dot(transform.right, toAttacker);

            if( m_isPlayer )
            {
                if      (dotForward > 0.5f)  m_animator.SetTrigger("ReactionF"); 
                else if (dotForward < -0.5f) m_animator.SetTrigger("ReactionB"); 
                else if (dotRight > 0)       m_animator.SetTrigger("ReactionR");
                else                         m_animator.SetTrigger("ReactionL");
            }
            else
            {
                if( dotRight > 0 ) m_animator.SetTrigger("ReactionR");
                else               m_animator.SetTrigger("ReactionL");
            }
        }
        else 
        { 
            if( !m_isDead )
            {
                if( m_isPlayer ) m_audioSource.PlayOneShot( m_deathSound1, m_soundFxVolume );
                else
                {
                    int deathSoundIndex = Random.Range(1, 2);
                    switch( deathSoundIndex )
                    {
                        case 1:
                            m_audioSource.PlayOneShot(m_deathSound2, m_soundFxVolume);
                            break;
                        case 2:
                            m_audioSource.PlayOneShot(m_deathSound3, m_soundFxVolume);
                            break;
                    }
                }

                if( m_isPlayer ) m_animator.SetTrigger("death");
                else
                {
                    Vector3 toAttacker = (attackerPosition - transform.position).normalized;
                    float dotRight = Vector3.Dot(transform.right, toAttacker);

                    if( drumSolo )         m_animator.SetTrigger("deathDrumSolo");
                    else if (dotRight > 0) m_animator.SetTrigger("deathR");
                    else                   m_animator.SetTrigger("deathL");
                }
                m_isDead = true;
            }
        
        }

    }

    public void heal(int amount)
    {
        m_currentHealth = Mathf.Clamp(m_currentHealth + amount, 0, MAX_HEALTH);
    }

    public float currentHealth()
    {
        return m_currentHealth;
    }

    public void destroyGameObject()
    {
        Destroy(gameObject);
    }

    public void setImmunity(float time)
    {
        if (m_immunityRoutine != null)
            StopCoroutine(m_immunityRoutine);

        m_immunityRoutine = StartCoroutine(ImmunityTimer(time));
    }

    private IEnumerator ImmunityTimer(float duration)
    {
        m_isImmune = true;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        m_isImmune = false;
        m_immunityRoutine = null;
    }

}
