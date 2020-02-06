using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour {

    public NetworkManager networkManager;
    public Text status;
    public GameObject roomBoxPre;
    public Transform roomListParent;

    public float delay = 1f;
    private bool waiting = false;

    List<GameObject> roomList = new List<GameObject>();

	void Start () {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
        //RefreshRoomList();
        status.text = "Loading...";
        ClearRoomList();
        networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
    }

    public void RefreshRoomList ()
    {
        if (waiting == false)
        {
        StartCoroutine(delayrefresh(delay));
        }

    }

    public void OnMatchList (bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        status.text = "Room Found";
        if (matches == null)
        {
            status.text = "No rooms...";
            return;
        }
        
        foreach (MatchInfoSnapshot match in matches)
        {
            GameObject roomBoxgo = Instantiate(roomBoxPre);
            roomBoxgo.transform.SetParent(roomListParent);
            roomBoxgo.transform.localScale = transform.localScale;
            RoomBox roomBoxx = roomBoxgo.GetComponent<RoomBox>();
            if (roomBoxx != null)
            {
                roomBoxx.Setup(match, JoinRoom);
            }

            roomList.Add(roomBoxgo);
        }
        if (roomList.Count == 0)
        {
            status.text = "No rooms here!";
        }
    }

    public void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }
        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot matchh)
    {
        networkManager.matchMaker.JoinMatch(matchh.networkId, "","","",0,0, networkManager.OnMatchJoined);
        ClearRoomList();
        status.text = "JOINING...";
    }

    public IEnumerator delayrefresh(float a)
    {
        waiting = true;
        status.text = "Loading...";
        ClearRoomList();
        yield return new WaitForSeconds(a);
        ClearRoomList();
        networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
        waiting = false;
    }
}
