using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;





    public class PatrolAI : MonoBehaviour
    public List <Transform> patrolPoints; // Lista de puntos de destino
    private int currentPatrolIndex = 0; // Índice del punto de destino actual
    

    public float moveSpeed = 5f; // Velocidad de movimiento de la IA
    public float waitTime = 5f; // Tiempo de espera en segundos
    
    public float chaseRange = 10f; // Rango de detección del jugador
    public float attackRange = 2f; // Rango de ataque

    private enum AIState { Patrolling, Waiting, Chasing, Attacking }; // Enumeración para los estados de la IA
    private AIState currentState = AIState.Patrolling; // Estado actual de la IA
    private float waitTimer = 0f; // Temporizador de espera

    private Transform player; // Referencia al jugador

public class IAenemigo : MonoBehaviour


{
    enum State
    {
        Patrolling,

        Chasing,

        Searching,

        Attackin,

        Waiting
    }

    State currentState;

    NavMeshAgent enemyAgent;

    Transform playerTransform;

    [SerializeField] Transform patrolAreaCenter;
    [SerializeField] Vector2 patrolAreaSize;
    [SerializeField] float visionRange = 15;
    [SerializeField] float visionAngle = 90;
    Vector3 lastTargetPosition;

    float searchTimer;
    [SerializeField]float serachWaitTime = 15;
    [SerializeField]float searchRadius = 30;

    // Start is called before the first frame update
    void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        currentState = State.Patrolling;
        {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Buscar al jugador por la etiqueta "Player"
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.Patrolling:
                Patrol();
                break;
            case AIState.Waiting:
                Wait();
                break;
            case AIState.Chasing:
                Chase();
                break;
            case AIState.Attacking:
                Attack();
                break;
        }
    }

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                 Patrol();
            break;
            case State.Chasing:
                 Chase();
            break;
            case State.Searching:
                 Search();
            break;

        }
    }

  

    void Patrol()
    {
        if(OnRange())
        {
            currentState = State.Chasing;
        }

        if(enemyAgent.remainingDistance < 0.5f)
        {
            SetRandomPoint();
        }

        {
        if (player != null && Vector3.Distance(transform.position, player.position) < chaseRange)
        {
            currentState = AIState.Chasing; // Cambiar al estado de persecución si el jugador está dentro del rango de detección
            return;
        }

        if (patrolPoints.Count > 0)
        {
            Transform currentPatrolPoint = patrolPoints[currentPatrolIndex];
            transform.position = Vector3.MoveTowards(transform.position, currentPatrolPoint.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentPatrolPoint.position) < 0.1f)
            {
                currentState = AIState.Waiting;
                waitTimer = waitTime;
            }
        }
    }

    }

    void Wait()
    {
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0f)
        {
            currentPatrolIndex++;
            if (currentPatrolIndex >= patrolPoints.Count)
            {
                currentPatrolIndex = 0;
            }
            currentState = AIState.Patrolling;
        }
    }
 void Chase()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime); // Moverse hacia la posición del jugador

        if (Vector3.Distance(transform.position, player.position) > chaseRange)
        {
            currentState = AIState.Patrolling; // Volver a patrullar si el jugador está fuera del rango de detección
        }
        else if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            currentState = AIState.Attacking; // Cambiar al estado de ataque si el jugador está dentro del rango de ataque
        }
    }

void Attack()
    {
        // Simular el ataque (debug)
        Debug.Log("¡Ataque!");

        currentState = AIState.Chasing; // Volver al estado de persecución después del ataque
    }


    void Search()
    {
        if(OnRange() == true)
        {
            searchTimer = 0;
            currentState = State.Chasing;
        }
        searchTimer += Time.deltaTime;

        if(searchTimer < serachWaitTime)
        {
            if(enemyAgent.remainingDistance < 0.5f)
            {
                 Debug.Log("Buscando punto aleatorio");

                 Vector3 randomSearchPoint = lastTargetPosition + Random.insideUnitSphere * searchRadius;
                randomSearchPoint.y = lastTargetPosition.y;
                enemyAgent.destination = randomSearchPoint;
            }
          
        }

        else
        {
            currentState = State.Patrolling;

        }
    }

    void Chase()
    {
        enemyAgent.destination = playerTransform.position;

        if(OnRange() == false)
        {
            currentState = State.Searching;
        }

    }

    void SetRandomPoint()
    {
        float randomX = Random.Range(-patrolAreaSize.x / 2, patrolAreaSize.x / 2);
        float randomZ = Random.Range(-patrolAreaSize.y / 2, patrolAreaSize.y / 2);
        Vector3 randomPoint = new Vector3(randomX, 0f, randomZ) + patrolAreaCenter.position;

        enemyAgent.destination = randomPoint;
    }

    bool OnRange()
    {
        /*if(Vector3.Distance(transform.position, playerTransform.position) <= visionRange)
        {
            return true;
        }

        return false;*/

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if(distanceToPlayer <= visionRange && angleToPlayer < visionAngle * 0.5f)
        {
            if(playerTransform.position == lastTargetPosition)
            {
                return true;
            }
            //return true;
            RaycastHit hit;
            if(Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    lastTargetPosition = playerTransform.position;

                    return true;
                }
            }

            return false;
           
        }

        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(patrolAreaCenter.position, new Vector3(patrolAreaSize.x, 0, patrolAreaSize.y));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.green;
        Vector3  fovLine1 = Quaternion.AngleAxis(visionAngle * 0.5f, transform.up) * transform.forward * visionRange;

        Vector3 fovLine2 = Quaternion.AngleAxis(-visionAngle * 0.5f, transform.up) * transform.forward * visionRange;

        Gizmos.DrawLine(transform.position, transform.position + fovLine1); 

        Gizmos.DrawLine(transform.position, transform.position + fovLine2); 
    }


 

    
}
