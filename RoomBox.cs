using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class RoomBox : MonoBehaviour {

    public delegate void JoinRoomDelegate(MatchInfoSnapshot matchh);
    private JoinRoomDelegate joinRoomCallBack;

    public Text roomname;
    private MatchInfoSnapshot match;

    void Start () {
		
	}
	
	public void Setup(MatchInfoSnapshot matchh, JoinRoomDelegate joinRoomCallBackk)
    {
        match = matchh;
        joinRoomCallBack = joinRoomCallBackk;
        roomname.text = "(" + match.currentSize + "/" + match.maxSize + ") " + match.name;
    }

    public void JoinRoom ()
    {
        joinRoomCallBack.Invoke(match);
    }
}
