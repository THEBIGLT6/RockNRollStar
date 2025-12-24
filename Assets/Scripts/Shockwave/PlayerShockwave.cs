using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerShockwave : MonoBehaviour
{

    [Header("References")]
    public GameObject m_ringPrefab;
    private Transform m_ringOrigin;
    [SerializeField] private Image m_abilityRing;
    private Coroutine blinkRoutine;
    
    [Header("Tune Settings")]    
    public float duration = 10f;  
    public float interval = 0.5f; 

    public void StartShockwaveEffect()
    {
        m_ringOrigin = GetComponent<Transform>();
        StartCoroutine(EmitRings());
    }

    private IEnumerator EmitRings()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if ( m_ringPrefab != null)
            {
                Vector3 pos = m_ringOrigin ? m_ringOrigin.position : transform.position;
                Instantiate(m_ringPrefab, pos, Quaternion.Euler(90, 0, 0));
            }

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }

    public void startBlinking()
    {
        if (m_abilityRing != null && blinkRoutine == null) blinkRoutine = StartCoroutine(BlinkRing());
    }

    public void stopBlinking()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        if (m_abilityRing != null)
        {
            Color c = Color.white;
            c.a = 1f;
            m_abilityRing.color = c;
        }
    }

    private IEnumerator BlinkRing()
    {
        Color c = Color.white;

        while (true)
        {
            // oscillate alpha between minAlpha and maxAlpha
            float blinkSpeed = 8f; // speed of blinking
            float minAlpha = 0.3f; // how transparent at lowest point
            float maxAlpha = 1f;   // how opaque at highest point
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f);
            c.a = alpha;
            m_abilityRing.color = c;

            yield return null;
        }
    }
}
