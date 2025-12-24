using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{

    [Header("Cooldowns / Timers")]
    [SerializeField] private float SHOTCOOLDOWN;
    private float m_lastShotTime = 0f;

    [Header("Bullet")]
    [SerializeField] private GameObject m_bulletPrefab;
    [SerializeField] private Transform m_leftBulletSpawn;
    [SerializeField] private Transform m_rightBulletSpawn;
    private float m_bulletSpeed = 2f;

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
        if (!m_movement.canAttack()) return;

        triggerShoot();
    }

    private void triggerShoot()
    {
        if (Time.time - m_lastShotTime < SHOTCOOLDOWN)
            return;
        m_animator.SetTrigger("shoot");
        m_lastShotTime = Time.time;
    }

    public void shootBulletLeft()
    {
        GameObject bulletLeft = Instantiate(m_bulletPrefab, m_leftBulletSpawn.position, m_bulletPrefab.transform.rotation);
        bulletLeft.GetComponent<Rigidbody>().AddForce( m_leftBulletSpawn.forward * m_bulletSpeed, ForceMode.Impulse);
    }

    public void shootBulletRight()
    {
        GameObject bulletRight = Instantiate(m_bulletPrefab, m_rightBulletSpawn.position, m_bulletPrefab.transform.rotation);
        bulletRight.GetComponent<Rigidbody>().AddForce(m_rightBulletSpawn.forward * m_bulletSpeed, ForceMode.Impulse);

    }

}
