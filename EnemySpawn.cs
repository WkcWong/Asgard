using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawn : NetworkBehaviour {

    //public Transform x;
    public Transform[] spawnPoints;
    public GameObject enemy;
    public float spawnt= 10f;
    public bool LinkParent = false;

	void Start () {
        if (isServer)
        {
            Cmdinvoking();
        }
	}
    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
    }

    [Command]
    void Cmdinvoking()
    {
        InvokeRepeating("CmdSpawn", 1, spawnt);
    }

    [Command]
    void CmdSpawn()
    {
        int ran = Random.Range(0, spawnPoints.Length);
        GameObject go = Instantiate(enemy);
        go.transform.position = spawnPoints[ran].transform.position;
        if (LinkParent)
        {
            go.transform.parent = transform;
            Vector3 a = go.transform.position;
            a.z = 0;
            Quaternion rotation = Quaternion.LookRotation(a);
            transform.rotation = rotation;
        }
        NetworkServer.Spawn(go);
        //Debug.Log("SPAWNED");
    }
}
