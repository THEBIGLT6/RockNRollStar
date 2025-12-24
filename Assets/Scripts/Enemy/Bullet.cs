using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int SHOTDAMAGE;
    private float m_lifeTime = 2f;

    private void Start()
    {
        Destroy(gameObject, m_lifeTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if ( other.gameObject.CompareTag("Player") ) other.gameObject.GetComponent<Health>().damage(SHOTDAMAGE, gameObject.transform.position);
        Destroy(gameObject);
    }
}
