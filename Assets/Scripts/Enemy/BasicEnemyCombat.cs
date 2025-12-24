using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyCombat : MonoBehaviour
{
    [Header("DAMAGE")]
    [SerializeField] private int PUNCHDAMAGE;
    [SerializeField] private int PUSHDAMAGE;
    
    [Header("Cooldowns / Timers")] 
    private float m_lastAttackTime = 0f;
    private float m_comboTimer = 0;
    private int m_comboStep = 0;
    [SerializeField] private float ATTACKCOOLDOWN;
    [SerializeField] private float COMBOWINDOW;

    [Header("Hitbox Settings")]
    private Vector3 m_punch1Offset = new Vector3(0, 1.5f, 0.8f);
    private Vector3 m_punch1BoxSize = new Vector3(0.3f, 0.5f, 0.7f);
    private Vector3 m_punch2Offset = new Vector3(-0.3f, 1.5f, 0.8f);
    private Vector3 m_punch2BoxSize = new Vector3(0.3f, 0.5f, 0.4f);
    private Vector3 m_pushOffset = new Vector3(0, 1.5f, 0.6f);
    private Vector3 m_pushBoxSize = new Vector3(0.9f, 0.7f, 0.9f);

    private Animator m_animator;
    private Transform m_enemyTransform;
    private EnemyMovement m_movement;

    void Start()
    {
        m_movement = GetComponent<EnemyMovement>();
        m_animator = GetComponent<Animator>();
        m_enemyTransform = GetComponent<Transform>();
    }

    void Update()
    {
        if( !m_movement.canAttack() ) return;
        
        if( m_comboTimer > 0f ) m_comboTimer -= Time.deltaTime;
        else m_comboStep = 0;

        promptAttack();
    }

    private void promptAttack()
    {
        if (Time.time - m_lastAttackTime < ATTACKCOOLDOWN)
            return;

        if (m_comboStep == 1 && m_comboTimer > 0f)
        {
            m_animator.SetTrigger("Punch2");
            m_comboStep = 2;

            m_comboTimer = COMBOWINDOW;
        }
        else if (m_comboStep == 2 && m_comboTimer > 0f)
        {
            m_animator.SetTrigger("Push");
            m_comboStep = 0;

            m_comboTimer = 0f;
        }
        else
        {
            m_animator.SetTrigger("Punch1");
            m_comboStep = 1;
            m_comboTimer = COMBOWINDOW;
        }

        m_lastAttackTime = Time.time;
    }

    private void hitboxAttack(Vector3 offset, Vector3 size, int damage)
    {
        Vector3 worldCenter = m_enemyTransform.position + m_enemyTransform.rotation * offset;
        Vector3 halfExtents = size * 0.5f;
        Quaternion rotation = m_enemyTransform.rotation;

        Collider[] hits = Physics.OverlapBox(
            worldCenter,
            halfExtents,
            rotation
        );

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Health enemy = hit.GetComponentInParent<Health>();

                if (enemy != null)
                {
                    enemy.damage(damage, m_enemyTransform.position);
                }
            }
        }
    }

    public void punch1()
    {
        hitboxAttack(m_punch1Offset, m_punch1BoxSize, PUNCHDAMAGE);
    }

    public void punch2()
    {
        hitboxAttack(m_punch2Offset, m_punch2BoxSize, PUNCHDAMAGE);
    }

    public void push()
    {
        hitboxAttack(m_pushOffset, m_pushBoxSize, PUSHDAMAGE);
    }


    private void OnDrawGizmos()
    {
        //Gizmos.color = gizmoColor;
        //Gizmos.matrix = Matrix4x4.TRS(m_enemyTransform.position, m_enemyTransform.rotation, Vector3.one);
        //Gizmos.DrawWireCube(m_punch1Offset, m_punch1BoxSize);
        //Gizmos.DrawWireCube(m_punch2Offset, m_punch2BoxSize);
        //Gizmos.DrawWireCube(m_pushOffset, m_pushBoxSize);
    }
}
