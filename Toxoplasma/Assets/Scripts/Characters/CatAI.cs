using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatAI : MonoBehaviour
{
    public GameManager gm;
    private Animator animator;

    [Header("Movement")]
    private NavMeshAgent agent;
    public State state;
    // Patrol variables
    public GameObject[] waypoints;
    private int waypointInd;
    public float patrolSpeed = 5f;
    // Chase variables
    public float chaseSpeed = 15f;
    public GameObject target;
    public enum State { PATROL, CHASE, IDLE }
    public Transform trailPrefab;


    [Header("Toxoplasma")]
    public bool isInfected;
    private Transform catSkin;
    public int eatingTime = 3;
    public bool eating;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        isInfected = false;
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
        state = CatAI.State.PATROL;
        StartCoroutine("FSM", 2f);

        // Toxoplasma initialisation
        gm.cleanCats.Add(this.gameObject);
        catSkin = this.gameObject.transform.GetChild(0);
    }
    void Update()
    {
        animator.SetFloat("Speed", agent.speed);

        if (Input.GetKeyDown("space"))
        {
            if (state == CatAI.State.PATROL || state == CatAI.State.CHASE)
            {
                state = CatAI.State.IDLE;
            }

            else
            {
                state = CatAI.State.PATROL;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Mouse" && state != CatAI.State.CHASE && !eating)
        {
            state = CatAI.State.CHASE;
        }
    }

    // Movement functions
    IEnumerator FSM()
    {
        while (true)
        {
            switch (state)
            {
                case State.PATROL:
                    Patrol();
                    break;
                case State.CHASE:
                    Chase();
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
        agent.autoBraking = false;
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
    void Chase()
    {
        if (target.GetComponent<MouseAI>().isAlive == true && Vector3.Distance(transform.position, target.transform.position) < 50)
        {
            agent.autoBraking = true;
            if (Vector3.Distance(transform.position, target.transform.position) > 1)
            {
                agent.speed = chaseSpeed;
                agent.SetDestination(target.transform.position);

                // Line renderer from cat to chased mouse.
                Transform trail = Instantiate(trailPrefab, Vector3.zero, Quaternion.identity) as Transform;
                LineRenderer lr = trail.GetComponent<LineRenderer>();
                var points = new Vector3[2];
                points[0] = transform.position + new Vector3(0, 1, 0);
                points[1] = target.transform.position + new Vector3(0, 1, 0);
                if (lr != null)
                {
                    lr.SetPositions(points);
                }

                Destroy(trail.gameObject, 0.04f);
            }
            else
            {
                target.GetComponent<MouseAI>().GetEaten();
                StartCoroutine(Eat(eatingTime));
            }
        }

        else
        {
            state = CatAI.State.PATROL;
        }
    }
    void Idle()
    {
        agent.speed = 0;
    }

    // Toxoplasma functions

    public IEnumerator Eat(int eatingTime)
    {
        eating = true;
        GetComponent<CatFieldOfView>().visibleTargets.Clear();
        state = CatAI.State.IDLE;
        // Play eating sound.
        if (!isInfected)
        {
            GetInfected();
        }

        yield return new WaitForSeconds(eatingTime);
        eating = false;
        Destroy(target);
        target = null;
        state = CatAI.State.PATROL;

    }

    public void GetInfected()
    {
        isInfected = true;
        catSkin.GetComponent<Renderer>().material.color = Color.red;
    }
}
