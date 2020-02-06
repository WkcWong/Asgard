using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pen : NetworkBehaviour
{
    public GameObject[] players;
    public int no = 1;

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //other.GetComponent<PlayerAction>().GainPen();
            CmdDest();
        }
    }
    [Command]
    void CmdDest()
    {
        RpcDest();
    }
    [ClientRpc]
    void RpcDest()
    {
        for (int i =0 ; i < players.Length; i++)
        {
            players[i].GetComponent<PlayerAction>().GainPen(no);
        }
        Destroy(gameObject);
    }
}
