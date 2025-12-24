using UnityEngine;
using Cinemachine;

public class CameraOrientationSync : MonoBehaviour
{
    public CinemachineFreeLook m_freeLookCamera;

    void Update()
    {
        if ( m_freeLookCamera != null )
        {
            float cameraYaw = m_freeLookCamera.m_XAxis.Value;
            transform.rotation = Quaternion.Euler(0f, cameraYaw, 0f);
        }
    }
}
