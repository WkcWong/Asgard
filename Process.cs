using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Process : MonoBehaviour {

    private GameObject player;
    private bool pro = false;
    public GameObject proUI;

    private GameObject obj;
    public Slider bar;

    public Text ProcessT;

    public Text lego;
    private int legoNo;

    private Vector2 resolution;
    public bool forPC = true;

    // Use this for initialization
    void Start () {
        player = transform.parent.transform.parent.transform.parent.gameObject;
        resolution = new Vector2(Screen.width, Screen.height);
    }
	
	// Update is called once per frame
	void Update () {
        if (player != null)
        {
            if (resolution.x != Screen.width || resolution.y != Screen.height)
            {
                if (forPC)
                {
                    resolution.x = Screen.width;
                    resolution.y = Screen.height;
                }
            }

            pro = player.GetComponent<PlayerAction>().process;
            proUI.SetActive(pro);

            legoNo = player.GetComponent<PlayerAction>().legoCount;
            lego.text = "Lego: "+ legoNo.ToString();

            obj = player.GetComponent<PlayerAction>().proObj;
        } 

        if (obj != null)
        {
            bar.value = obj.GetComponent<CheckpointControl>().process / obj.GetComponent<CheckpointControl>().process_done;
            if (bar.value >= 1)
            {
                ProcessT.text = "Done";
            }
            else
            {
                ProcessT.text = "Processing. . .";
            }
        }
    }
}
