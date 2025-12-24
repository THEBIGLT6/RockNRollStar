using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform m_orientation;
    private Rigidbody m_rigidBody;
    private Animator m_animator;
    [SerializeField] private CinemachineFreeLook m_camera;

    [Header("Movement Settings")]
    public float WALK_SPEED;
    public float RUN_SPEED;
    public float ROTATION_SPEED;

    private float m_horizontalInput;
    private float m_verticalInput; 
    private bool m_isWalking;
    private bool m_isRunning;
    private bool m_canMove = true;

    private void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_rigidBody.freezeRotation = true;

        m_animator = GetComponentInChildren<Animator>();

        SettingsManager.Instance.m_OnSettingsChanged += setMouseSensitvity;
        setMouseSensitvity();
    }

    private void Update()
    {
        m_horizontalInput = Input.GetAxisRaw("Horizontal");
        m_verticalInput = Input.GetAxisRaw("Vertical");
 
        handleAnimation();
    }

    private void FixedUpdate()
    {
        if (m_canMove)
            handleMovement();
    }

    private void handleMovement()
    {
        if( m_isWalking || m_isRunning )
        {
            Vector3 forward = m_orientation.forward;
            Vector3 right = m_orientation.right;
            forward.y = 0;
            right.y = 0;

            Vector3 moveDir = (forward * m_verticalInput + right * m_horizontalInput).normalized;

            // Pick the correct movement speed
            float currentSpeed = m_isRunning ? RUN_SPEED : WALK_SPEED;

            Vector3 newVelocity = moveDir * currentSpeed;
            newVelocity.y = m_rigidBody.velocity.y;
            m_rigidBody.velocity = newVelocity;

            if (moveDir.sqrMagnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                m_rigidBody.MoveRotation(Quaternion.Slerp(m_rigidBody.rotation, targetRot, ROTATION_SPEED * Time.fixedDeltaTime));
            }
        }
        
    }

    private void handleAnimation()
    {
        bool hasInput = Mathf.Abs(m_horizontalInput) > 0.1f || Mathf.Abs(m_verticalInput) > 0.1f;
        bool runPressed = Input.GetKey(KeyCode.LeftShift);

        if (hasInput && !m_isWalking)
        {
            m_animator.SetTrigger("idleToWalk");
            m_animator.SetBool("isWalking", true);
            m_isWalking = true;
        }
        else if (!hasInput && m_isWalking)
        {
            m_animator.SetBool("isWalking", false);
            m_animator.SetBool("isRunning", false);
            m_isWalking = false;
            m_isRunning = false;
        }

        if (hasInput && runPressed && !m_isRunning)
        {
            m_animator.SetBool("isRunning", true);
            m_isRunning = true;
        }
        else if ((!runPressed || !hasInput) && m_isRunning)
        {
            m_animator.SetBool("isRunning", false);
            m_isRunning = false;
        }
    }

    public void SuspendMovement(float duration)
    {
        StartCoroutine(SuspendMovementRoutine(duration));
    }

    private System.Collections.IEnumerator SuspendMovementRoutine(float duration)
    {
        m_canMove = false;
        m_rigidBody.velocity = Vector3.zero;
        yield return new WaitForSeconds(duration);
        m_canMove = true;
    }

    public void setMouseSensitvity()
    {
        GameSettings settings = SettingsManager.Instance.GetSettings();
        m_camera.m_XAxis.m_MaxSpeed = 300 * settings.mouseSensitivity;
        m_camera.m_YAxis.m_MaxSpeed = 2 * settings.mouseSensitivity;
    }

    public void triggerWin()
    {
        m_animator.SetTrigger("win");
        SuspendMovement(5f);
    }
}


