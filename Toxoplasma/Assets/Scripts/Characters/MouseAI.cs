using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MouseAI : MonoBehaviour
{
    public GameManager gm;

    // Mouse properties
    [Range(0f, 10f)]
    public float fear = 0f;

    public bool isAlive;

    [Header("Movement")]
    public Animator animator;
    public NavMeshAgent agent;

    public LayerMask whatIsWall;

    public enum State { PATROL, CHASED, IDLE }
    public State state;

    // Patrol variables
    public GameObject[] waypoints;
    public int waypointInd;
    public float patrolSpeed = 15f;

    // Chased variables
    public float chasedSpeed = 20f;
    public GameObject chaser;

    // [Header("Toxoplasma")]


    private void Awake()
    {
        animator = GetComponent<Animator>();
        isAlive = true;
    }

    void Start()
    {
        gm = GameManager.instance;
        // Movement initialisation
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = true;
        agent.updateRotation = true;
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        waypointInd = Random.Range(0, waypoints.Length);
        state = MouseAI.State.PATROL;
        StartCoroutine("FSM", 2f);

        // Toxoplasma initialisation
        gm.infectedMice.Add(this.gameObject);
    }

    void Update()
    {
        animator.SetFloat("Speed", agent.speed);
        if (Input.GetKeyDown("space"))
        {
            state = MouseAI.State.IDLE;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Cat" && state != MouseAI.State.CHASED)
        {
            state = MouseAI.State.CHASED;
        }
    }

    // Movement functions
    IEnumerator FSM()
    {
        while (isAlive == true)
        {
            switch (state)
            {
                case State.PATROL:
                    Patrol();
                    break;
                case State.CHASED:
                    Chased();
                    break;
                case State.IDLE:
                    Idle();
                    break;
            }
            yield return null;
        }
    }
    void Patrol()
    {
        agent.speed = patrolSpeed;
        if (Vector3.Distance(this.transform.position, waypoints[waypointInd].transform.position) >= 2)
        {
            agent.SetDestination(waypoints[waypointInd].transform.position);
        }
        else if (Vector3.Distance(this.transform.position, waypoints[waypointInd].transform.position) <= 2)
        {
            waypointInd = Random.Range(0, waypoints.Length);
        }
        else
        {
            agent.speed = 0;
        }
    }
    void Chased()
    {
        if ((chaser == null) || (Vector3.Distance(transform.position, chaser.transform.position) > 3000))
        {
            state = MouseAI.State.PATROL;
        }

        else
        {
            agent.speed = chasedSpeed;
            Vector3 dirToTarget = transform.position - chaser.transform.position;
            Vector3 newPos = transform.position + dirToTarget;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 10, whatIsWall))
            {
                float wallAngle = Vector3.SignedAngle(transform.forward, hit.normal, Vector3.up);
                if (wallAngle > 0)
                {
                    dirToTarget = Quaternion.Euler(0, 100, 0) * dirToTarget;
                }
                else
                {
                    dirToTarget = Quaternion.Euler(0, -100, 0) * dirToTarget;
                }
                newPos = transform.position + dirToTarget / 100;
                Debug.Log(agent.destination);
                agent.SetDestination(newPos);
            }

            else
            {
                agent.SetDestination(newPos);
            }
        }

    }
    void Idle()
    {
        agent.speed = 0;
    }

    // Toxoplasma
    public void GetEaten()
    {
        isAlive = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
    }
}
