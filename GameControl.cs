using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {

    public static GameControl usingManager;
    //public MatchSetting matchSetting;


    private void Awake()
    {
        if (usingManager != null)
        {
            //Debug.LogError("Error: GM > 1");
        }
        else
        {
            usingManager = this;
        }
    }

    #region aaa
    private static Dictionary<string, PlayerHealth> players = new Dictionary<string, PlayerHealth>();

    public static void RegisterPlayer (string netID, PlayerHealth health)
    {
        players.Add("Player: "+ netID, health);
        health.transform.name = "Player: " + netID;
    }

    public static void UnRegisterPlayer (string player)
    {
        players.Remove(player);
    }

    public static PlayerHealth GetPlayer(string id)
    {
        return players[id];
    }
    /*
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        GUILayout.BeginVertical();

        foreach(string player in players.Keys)
        {
            GUILayout.Label(player + " /// " + players[player].transform.name);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    */
    #endregion
}
