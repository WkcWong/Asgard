using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartControl : MonoBehaviour {

    public GameObject rs;

    public void OpenRoomSet()
    {
        rs.SetActive(true);

    }
    public void CloseRoomSet()
    {
        rs.SetActive(false);
    }
    public void EndGame()
    {
        Application.Quit();
    }
}
