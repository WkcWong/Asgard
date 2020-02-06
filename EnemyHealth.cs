using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class EnemyHealth : NetworkBehaviour
{

    [SerializeField]
    private int health = 50;
    [SyncVar]
    public int nowHealth = 50;
    private bool dead = false;

    private bool bleed = false;
    private float red = 255;
    Color32 originCol;

    //[SerializeField]
    //private GameObject spawnpt;
    //private NavMeshAgent nav;

    public void Awake()
    {
        //nav = GetComponent<NavMeshAgent>();
        originCol = GetComponentInChildren<Renderer>().material.color;
    }

    private void Update()
    {
        if (dead)
        {
            Destroy(gameObject);
        }
        if (bleed)
        {
            GetComponentInChildren<Renderer>().material.color = new Color32((byte)red, 0, 0, 0);
        }

    }

    [ClientRpc]
    public void RpcTakeDamage(int damage, string n)
    {
        nowHealth -= damage;
        Debug.Log(n + " " + nowHealth + " / " + health);

        bleed = true;
        StartCoroutine(bleeding());

        if (nowHealth <= 0)
        {
            dead = true;
        }
    }
    /*
    [Command]
    void CmdtryingtoDestroy()
    {
        RpcDestroy();
    }
    [ClientRpc]
    void RpcDestroy()
    {
        Destroy(gameObject);
    }
    */

    [Command]
    public void CmdSendDam(int damage, string n)
    {
        RpcTakeDamage(damage, n);
    }

    IEnumerator bleeding()
    {
        while (bleed)
        {
            yield return new WaitForSeconds(0.05f);
            if (red > 0)
            {
                red -= 100;
            }
            else
            {
                StopCoroutine(bleeding());
                bleed = false;
                GetComponentInChildren<Renderer>().material.color = originCol;
                red = 255;
            }
        }

    }
}
