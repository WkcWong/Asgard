using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CheckpointControl : NetworkBehaviour
{

    public float ControlNo = 0;
    [SyncVar]
    public float process = 0;
    public int ppl = 0;
    public float process_done = 10;
    public float rot;

    public bool keyrm = false;
    private Vector3 location;

    private GameObject train;
    private GameObject block;
    private GameObject[] board;

    private float movetime;
    private float moved = 0;
    private int a = 0;

    Color32[] originCol;
    private int colorLength;

    public GameObject pen;
    // Use this for initialization
    void Start()
    {
        movetime = process_done / 3;
        train = GameObject.Find("train");
        block = GameObject.Find("trainBlock");

        GameObject[] getboard = new GameObject[3];
        getboard[0] = GameObject.Find("boardCube0");
        getboard[1] = GameObject.Find("boardCube1");
        getboard[2] = GameObject.Find("boardCube2");
        board = getboard;

        colorLength = GetComponentInChildren<Renderer>().materials.Length;
        originCol = new Color32[colorLength];
        for (int i = 0; i < colorLength; i++)
        {
            originCol[i] = GetComponent<Renderer>().materials[i].color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ppl > 0 && process <= process_done)
        {
            process += Time.deltaTime * ppl;
            clientside(0);

        }
        if (process >= process_done)
        {
            clientside(1);
        }
    }

    //[Command]
    public void clientside(float x)
    {
        if (x == 0)
        {
            if (ControlNo == 0)
            {
                TrainRotate();
            }
            if (ControlNo == 1)
            {
                Board();
            }
        }
        else if (x == 1)
        {
            if (ControlNo == 0)
            {
                TrainMove();
            }
            //else return;
        }
    }

    [ClientRpc]
    public void RpcProcessing()
    {
        ppl++;
    }

    [ClientRpc]
    public void RpcStopProcessing()
    {
        ppl--;
    }
   // [ClientRpc]
    public void TrainRotate()
    {
        rot = 90 / process_done;
        train.transform.Rotate(0, 0, rot * Time.deltaTime);
    }
    //[ClientRpc]
    public void TrainMove()
    {
        Vector3 trainpos = train.transform.position;
        if (!keyrm)
        {
            location = new Vector3(trainpos.x, trainpos.y, -10f);
            train.transform.position = Vector3.MoveTowards(trainpos, location, 5f * Time.deltaTime);
        }
        DestroyImmediate(block, true);
    }

    //[ClientRpc]
    public void Board()
    {
        if (moved < movetime)
        {
            board[a].transform.position += Vector3.forward * Time.deltaTime * 3f / movetime * (a + 1);
            moved += Time.deltaTime;
        }
        else if (a < board.Length - 1)
        {
            a++;
            moved = 0;
        }
    }

    public void flashed()
    {
        //GetComponentInChildren<Renderer>().material.color = originCol;
        for (int i = 0; i < colorLength; i++)
        {
           GetComponent<Renderer>().materials[i].color = originCol[i];
        }
    }
}
