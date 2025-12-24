using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecordLabel : MonoBehaviour
{

    private TextMeshPro m_textMesh;

    private void Start()
    {
        m_textMesh = GetComponent<TextMeshPro>();
        RecordCollectible recordObj = GetComponentInParent<RecordCollectible>();
        m_textMesh.text = recordObj.recordName();   
    }
}
