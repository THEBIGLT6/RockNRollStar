using UnityEngine;

public class RingShockwave : MonoBehaviour
{
    [Header("Tune Settings")]
    private float EXPAND_SPEED = 0.1f;  
    private float FADE_SPEED = 0.2f;    
    private float MAX_SCALE = 0.25f;       

    private Material m_material;
    private Color m_color;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            m_material = new Material(renderer.material);
            renderer.material = m_material;
            m_color = m_material.color;
        }

        transform.localScale = Vector3.one * 0.1f;
    }

    void Update()
    {
        transform.localScale += Vector3.one * EXPAND_SPEED * Time.deltaTime;

        if (m_material != null)
        {
            m_color.a -= FADE_SPEED * Time.deltaTime;
            m_material.color = m_color;
        }

        if (m_color.a <= 0f || transform.localScale.x >= MAX_SCALE)
            Destroy(gameObject);
    }
}