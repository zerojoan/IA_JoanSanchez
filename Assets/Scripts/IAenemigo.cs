using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading;

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
