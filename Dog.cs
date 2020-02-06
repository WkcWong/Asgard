using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Dog : NetworkBehaviour
{
    private float chaseSpeed = 6f;
    private float attackWaitTime = 1.5f;
    //private float range = 7.5f;
    public int damage = 10;

    private float attackTimer = 0;
    private float dis;
    public NavMeshAgent nav;

    public bool inSight = false;
    [SerializeField]
    private GameObject[] targets;
    private GameObject target;
    //public float playerHP = 0;
    private GameObject enemy;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        targets = GameObject.FindGameObjectsWithTag("Enemy");
        nav.speed = chaseSpeed;
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
        targets = GameObject.FindGameObjectsWithTag("Enemy");

        if (targets != null && targets.Length > 0)
        {
            target = targets[0];
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

        if (inSight && enemy != null)
        {
            Attack(enemy);
        }
    }

    [Command]
    void CmdAttack(GameObject a)
    {
        EnemyHealth targetHealth = a.GetComponent<EnemyHealth>();
        targetHealth.RpcTakeDamage(damage, "a");
    }

    void Attack(GameObject a)
    {
        if (attackTimer >= attackWaitTime)
        {
            CmdAttack(a);
            attackTimer = 0;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            inSight = true;
            enemy = other.transform.parent.transform.gameObject;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            inSight = false;
        }
    }
}
