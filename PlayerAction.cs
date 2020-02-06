using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerAction : NetworkBehaviour {

    public ParticleSystem particleLauncher;
    //private string setname = "ABC";
    private int dam = 10;
    private float range = 100f;
    private float timeBetweenBullets = .3f;

    public Camera cam;
    //public Light gunlight;
    public LayerMask mask;

    private float timer;

    public bool process = false;
    public GameObject proObj;
    public GameObject partner;

    //
    public GameObject selObj;
    private int redCol, greenCol, blueCol;
    private bool lookingAtObj = false;
    private bool flashingIn = true;
    private bool startedFlashing = false;

    public int legoCount;

    public Text hints;

    private int penCount = 3;
    private bool meet = false;

    private GameObject control;
    private NetworkManager networkManager;
    private int roomNo = 0;

    private bool end = false;
    private float endt = 6;
    public Text endtext;
    public GameObject endscene;

    private ButtonControl btcon;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        control = GameObject.FindGameObjectWithTag("LobbyCon");
        roomNo = control.GetComponent<LobbyControl>().roomNo;

        if (cam == null)
        {
            Debug.LogError("No camera");
            this.enabled = false;
        }

        btcon = GameObject.FindGameObjectWithTag("phone").GetComponent<ButtonControl>();
    }

    public void GainLego()
    {
        legoCount++;
    }

    public void GainPen(int n)
    {
        penCount -= n;
    }

    void Update()
    {
        //shooting
        timer += Time.deltaTime;
        if (btcon.use)
        {
            if (btcon.m1h)
            {
                if (timer >= timeBetweenBullets)
                {
                    Shoot();
                }
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (timer >= timeBetweenBullets)
                {
                    Shoot();
                }
            }
        }
        if (timer > 0.1)
        {
            //gunlight.enabled = false;
            particleLauncher.Stop();
        }

        if (lookingAtObj)
        {
            int mcol = selObj.GetComponent<Renderer>().materials.Length;
            for (int a = 0; a < mcol; a++)
            {
                selObj.GetComponent<Renderer>().materials[a].color = new Color32((byte)redCol, (byte)greenCol, (byte)blueCol, 255);
            }
        }

        if (Input.GetButtonDown("Create") || btcon.e)
        {
            if (legoCount > 0)
            {
                legoCount--;
                CmdSpawn();
            }
        }
        ////////////////////////
        /////Way to Victory/////
        ////////////////////////
        if (roomNo == 1)
        {
            hints.text = "Find & Meet the dinosaur!";
        }
        else if (roomNo == 2)
        {
            hints.text = "Get all the pens!" + "\n" + "Remaining Pen(s): " + penCount.ToString();  
        }
        else
        {
            hints.text = "* For Testing Only *";
        }

        if (penCount <= 0)
        {
            LeaveRoom();
        }

        ////////////////////////
        ////////////////////////
        if (end)
        {
            endscene.SetActive(true);
            endt -= Time.deltaTime;
            endtext.text = "~ Auto-closed in : " + (int)endt + "s ~";
            if (endt <= 0)
            {
                LeaveGame();
            }
        }
    }

    [Command]
    private void CmdSpawn()
    {
        GameObject spawn = Instantiate(partner);
        Vector3 place = new Vector3(transform.position.x, 1, transform.position.z);
        spawn.transform.position = place;
        NetworkServer.Spawn(spawn);
        Destroy(spawn, 15);
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Checkpoint")
        {
            selObj = other.gameObject;
            lookingAtObj = true;
            if (!startedFlashing)
            {
                startedFlashing = true;
                StartCoroutine(FlashObject());
            }

            if (Input.GetButtonDown("Action") || btcon.q)
            {
                if (!process)
                {
                    if (other.gameObject.tag == "Checkpoint")
                    {
                        CmdProcessing(other.gameObject);
                        process = true;
                        proObj = other.gameObject;
                        Debug.Log("Processing");
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            lookingAtObj = false;
            startedFlashing = false;
            StopCoroutine(FlashObject());
            //selObj.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
            selObj.GetComponent<CheckpointControl>().flashed();

            if (process)
            {
                process = false;
                CmdStopProcessing(other.gameObject);
                Debug.Log("EXIT Processing");
            }
        }
    }

    [Command]
    void CmdProcessing(GameObject chkpt)
    {
        chkpt.GetComponent<CheckpointControl>().RpcProcessing();
    }

    [Command]
    void CmdStopProcessing(GameObject chkpt)
    {
        chkpt.GetComponent<CheckpointControl>().RpcStopProcessing();
    }

    [Client]
    void Shoot ()
    {
        timer = 0;
        //gunlight.enabled = true;
        particleLauncher.Play();

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, mask))
        {
            Debug.Log("Debug: hit " + hit.collider.name);

            /*if (hit.collider.tag == "Player") //removed
            {
                //CmdPlayerShot(hit.collider.name);
                Debug.Log("You hit your teammate!!");
            }
            else*/
            if (hit.collider.tag == "Enemy")
            {
                GameObject going = hit.collider.transform.parent.transform.gameObject;
                //CmdShot(hit.collider.gameObject);
                CmdShot(going);
            }
            //else { Debug.Log(hit.collider.name + " Died."); }
        }
    }
    
    //[Command]
    //void CmdPlayerShot(string target)
    //{
       // PlayerHealth targetHealth = GameControl.GetPlayer(target);
        //targetHealth.RpcTakeDamage(dam, target);
   // }

    [Command]
    void CmdShot(GameObject target)
    {
        EnemyHealth targetHealth = target.GetComponent<EnemyHealth>();
        targetHealth.RpcTakeDamage(dam, target.name);
        //SendDam targetHealth = target.GetComponent<SendDam>();
        //targetHealth.GetDam(dam, target.name);
    }

    IEnumerator FlashObject()
    {
        while (lookingAtObj)
        {
            yield return new WaitForSeconds(0.05f);
            if (flashingIn)
            {
                if (greenCol <= 30)
                {
                    flashingIn = false;
                }
                else
                {
                    greenCol -= 25;
                    redCol -= 1;
                }
            }
            if (!flashingIn)
            {
                if (greenCol >= 250)
                {
                    flashingIn = true;
                }
                else
                {
                    greenCol += 25;
                    redCol += 1;
                }
            }
        }
    }

    public void LeaveRoom()
    {
        CmdLeaving();
    }

    [Command]
    void CmdLeaving()
    {
        RpcLeaving();
    }

    [ClientRpc]
    void RpcLeaving()
    {
        end = true;
    }


    public void LeaveGame()
    {
        MatchInfo matchInfo = networkManager.matchInfo;
        networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
        networkManager.StopHost();
        Application.Quit();
    }
}
