using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Enemy : NetworkBehaviour
{
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    //public float chaseWaitTime = 5f;
    public float patrolWaitTime = 3f;
    public float attackWaitTime = 3f;
    public Transform[] wayPoints;
    public float range = 6.5f; //must larger than 5.5f for shark;
    public int damage = 10;

    //private float chaseTimer = 0;
    private float patrolTimer = 0;
    private float attackTimer = 0;
    private int wayPointIndex = 0;
    private float dis;
    public NavMeshAgent nav;

    public bool inSight = false;
    public GameObject target;
    public float playerHP = 0;
    private GameObject player;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        target = wayPoints[0].gameObject;
    }

    void Update()
    {
        if (isServer)
        {
            CmdUpdate();
        }
    }

    [Command]
    void CmdUpdate()
    {
        RpcUpdate();
    }

    [ClientRpc]
    void RpcUpdate()
    {
        //transform.LookAt(target.transform);
        if (target != null)
        {
            Vector3 lookatplayer = target.transform.position - transform.position;
            lookatplayer.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookatplayer);
            transform.rotation = rotation;
            nav.SetDestination(target.transform.position);
        }

        if (attackTimer <= attackWaitTime)
        {
            attackTimer += Time.deltaTime;
        }

        if (inSight && playerHP > 0f && player != null)
        {
            target = player;
            dis = (target.transform.position - transform.position).magnitude;

            if (range < dis)
            {
                Chase();
            }
            else
            {
                Attack();
            }
        }
        else
        {
            Patrol();
        }
    }

    [Command]
    void CmdAttack()
    {
        GameControl.GetPlayer(target.name).RpcTakeDamage(damage, target.name);
    }

    void Attack()
    {
        if (attackTimer >= attackWaitTime)
        {
            nav.isStopped = true;
            CmdAttack();
            attackTimer = 0;
        }
    }

    void Chase()
    {
        nav.isStopped = false;
        nav.speed = chaseSpeed;
    }

    void Patrol()
    {

        nav.isStopped = false;
        nav.speed = patrolSpeed;
        target = wayPoints[wayPointIndex].gameObject;

        if (nav.remainingDistance <= nav.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                if (wayPointIndex == wayPoints.Length - 1)
                {
                    wayPointIndex = 0;
                }
                else
                {
                    wayPointIndex++;
                }
                patrolTimer = 0;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
                inSight = true;
                player = other.transform.gameObject;
                playerHP = other.GetComponent<PlayerHealth>().getHP();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inSight = false;
        }
    }
}
