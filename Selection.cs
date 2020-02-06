using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Selection : NetworkBehaviour {

    [SerializeField]
    GameObject[] players; //get players to select
    public int num = 1; //for select

    
    //private bool checkcon = false; // check where it is going to the next scene
    //public LobbyControl con;
    public GameObject spawnPlayer; //for spawning

    //private int playerNo;
    //private GameObject[] spawnpt;
    //for report: failed coding
    //private GameObject go;
    //private bool assigned = false;
    //private bool spawned = false;

    private bool a = false;
    private float time = 0;

    private int playerCount;

    private ButtonControl btcon;

    void Start ()
    {
        //con = GameObject.Find("Controller").GetComponent<LobbyControl>();
        //playerNo = con.getNo();
        btcon = GameObject.FindGameObjectWithTag("phone").GetComponent<ButtonControl>();
    }

    void Update()
    {
        playerCount = GameObject.FindGameObjectWithTag("LobbyCon").GetComponent<LobbyControl>().players.Length;
        if (playerCount >= 2)
        {
            if (Input.GetKeyDown("right"))
            {
                CmdGet(1);
            }
            if (Input.GetKeyDown("left"))
            {
                CmdGet(0);
            }
        }

        if (a)
        {
            time += Time.deltaTime;
            if (time >= .5)
            {
                a = false;
                time = 0;
            }
        }

        //controller left right button 
        float h = Input.GetAxis("LeftRight");
        float _h = h + btcon.h;

        if (_h ==1 && !a)
        {
            CmdGet(1);
            a = true;
        }

        if (_h == -1 &&  !a)
        {
            CmdGet(0);
            a = true;
        }

        

        //con = GameObject.Find("Controller").GetComponent<LobbyControl>();

        /*
        checkcon = con.started;
        if (checkcon)
        {
            spawnpt = GameObject.FindGameObjectsWithTag("SpawnPoint");
            if (spawnpt.Length != 0)
            {
                    CmdSpawn();
            }
        }*/
    }
    //for select player
    [Command]
    void CmdGet(int a)
    {
        RpcGet(a);
    }
    [ClientRpc]
    void RpcGet(int a)
    {
        if (a == 1)
        {
            num++;
            for (int i = 0; i < players.Length; i++)
            {
                players[i].SetActive(false);
            }
        }
        else
        {
            num--;
            for (int i = 0; i < players.Length; i++)
            {
                players[i].SetActive(false);
            }
        }
        if (num < 0) num = players.Length - 1;
        else if (num >= players.Length) num = 0;
        players[num].SetActive(true);
    }
    //useless coding
    /*[Command]
    void CmdSpawn()
    {
        if (!spawned)
        {
            con.SpawnForStart(gameObject, spawnPlayer, spawnpt[playerNo]);
            spawned = true;
        }
        
        if (!spawned)
        {
            go = (GameObject)Instantiate(spawnPlayer);
            go.transform.position = spawnpt[playerNo].transform.position;

            NetworkServer.Spawn(go);
            spawned = true;
        }
        else
        {
            if (!assigned)
            {
                NetworkIdentity id = go.GetComponent<NetworkIdentity>();
                id.AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
                assigned = true;

            } 
        }
     }*/
}
