using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour {

    //[SerializeField]
    private uint rmsize = 2;
    private string rmname;
    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName (string a)
    {
        rmname = a;
    }
    
    public void CreateRoom ()
    {
        if (rmname != "" && rmname != null)
        {
            Debug.Log("Creating Room: " + rmname + "with room for " + rmsize + "players.");
            networkManager.matchMaker.CreateMatch(rmname, rmsize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
        }
        else
        {
            networkManager.matchMaker.CreateMatch("EmptyRoomName", rmsize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
        }
    }
}
