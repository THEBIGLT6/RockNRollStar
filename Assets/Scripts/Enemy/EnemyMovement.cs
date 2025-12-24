using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform m_playerTransform;
    private NavMeshAgent m_navMeshAgent;
    private Animator m_animator;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] m_waypoints;
    private int m_currentWaypoint = 0;

    [Header("Detection Settings")]
    [SerializeField] private float DETECTIONRADIUS;
    [SerializeField] private float CHASESTOPDISTANCE;
    
    [Header("Movement Speeds")]
    [SerializeField] private float WALKSPEED;
    [SerializeField] private float RUNSPEED;
    [SerializeField] private float WAYPOINTWAIT;
    private bool m_movementPaused;

    private enum State { Patrol, Chase }
    private State m_currentState = State.Patrol;

    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_navMeshAgent.updateRotation = false;
        m_animator = GetComponent<Animator>();

        goToNextWaypoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, m_playerTransform.position);

        switch (m_currentState)
        {
            case State.Patrol:
                patrolBehaviour(distanceToPlayer);
                break;

            case State.Chase:
                chaseBehaviour(distanceToPlayer);
                break;
        }

        updateRotation();
        updateMovementAnimation();
    }

    private void patrolBehaviour(float distanceToPlayer)
    {
        m_navMeshAgent.speed = WALKSPEED;

        if (distanceToPlayer < DETECTIONRADIUS)
        {
            m_currentState = State.Chase;
            return;
        }

        if (!m_navMeshAgent.pathPending && m_navMeshAgent.remainingDistance < 0.5f)
        {
            StartCoroutine(pauseAtWaypoint());
        }
    }

    private void chaseBehaviour(float distanceToPlayer)
    {
        m_navMeshAgent.speed = RUNSPEED;
        
        if (distanceToPlayer > DETECTIONRADIUS)
        {
            m_currentState = State.Patrol;
            goToNextWaypoint();
            return;
        }

        if ( m_animator.GetBool("isWalking") )
        {

            if (distanceToPlayer > CHASESTOPDISTANCE)
            {
                m_navMeshAgent.SetDestination(m_playerTransform.position);
            }
            else
            {
                m_navMeshAgent.ResetPath();
            }
        }

        facePlayer();
    }

    private void facePlayer()
    {
        Vector3 direction = (m_playerTransform.position - transform.position).normalized;
        direction.y = 0f; 

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp( transform.rotation, targetRotation, Time.deltaTime * 8f);
        }
    }

    private void updateMovementAnimation()
    {
        float speed = m_navMeshAgent.velocity.magnitude;

        bool isWalking = speed > 0.1f;
        bool isRunning = (m_currentState == State.Chase) && speed > 0.1f;

        m_animator.SetBool("isWalking", isWalking);
        m_animator.SetBool("isRunning", isRunning);
    }

    private void updateRotation()
    {
        if (m_navMeshAgent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(m_navMeshAgent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    private void goToNextWaypoint()
    {
        if (m_waypoints.Length == 0) return;

        m_navMeshAgent.SetDestination(m_waypoints[m_currentWaypoint].position);
        m_currentWaypoint = (m_currentWaypoint + 1) % m_waypoints.Length;
    }

    private IEnumerator pauseAtWaypoint()
    {
        if (m_movementPaused) yield break;
        m_movementPaused = true;

        m_navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(WAYPOINTWAIT);

        m_navMeshAgent.isStopped = false;
        goToNextWaypoint();

        m_movementPaused = false;
    }

    public bool canAttack()
    {
        if (m_currentState != State.Chase) return false;
        float distanceToPlayer = Vector3.Distance(transform.position, m_playerTransform.position);
        return distanceToPlayer <= CHASESTOPDISTANCE && !m_animator.GetBool("isWalking");
    }

    bool isShooting()
    {
        AnimatorStateInfo info = m_animator.GetCurrentAnimatorStateInfo(0);
        return info.IsTag("Shooting");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DETECTIONRADIUS);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, CHASESTOPDISTANCE);
        
        Gizmos.color = Color.magenta;
        if (m_waypoints != null)
        {
            foreach (var wp in m_waypoints)
            {
                if (wp != null)
                    Gizmos.DrawSphere(wp.position, 0.2f);
            }
        }
    }
}
