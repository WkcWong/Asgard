using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

public class LobbyControl : NetworkBehaviour {

    [SerializeField]
    GameObject[] keep;

    //[SerializeField]
    public GameObject[] players;

    [SerializeField]
    GameObject[] bedroom;

    [SerializeField]
    GameObject[] keyroom;

    [SerializeField]
    GameObject spawnPlayer;
    [SerializeField]
    GameObject[] spawnPt;
    private bool starting = false;
    private bool spawning = false;
    [SyncVar]
    public int roomNo = 1;
    public int playerNo = 1;

    public GameObject UIHost;
    public GameObject UISel;
    public Text UIHostText;
    public Text UIRoomName;
    private int KindsOfRoom = 3;

    private bool a = false;
    private float time = 0;

    public ButtonControl btcon;

    void Start ()
    {
        for (int i = 0; i < keep.Length; i++)
        {
            DontDestroyOnLoad(keep[i]);
        }
    }

    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Select");
        spawnPt = GameObject.FindGameObjectsWithTag("SpawnPoint");

        if (a)
        {
            time += Time.deltaTime;
            if (time >= .5)
            {
                a = false;
                time = 0;
            }
        }
        
        if (!starting)
        {
            if (players.Length >= 1)
            {
                UISel.SetActive(true);
                if (isServer)
                {
                    UIHostText.text = "Press \"up\"&\"down\" to select room \nPress \"Q\" to start (Computer) \nPress \"A\" to start(Phone)";
                    if (Input.GetKeyDown("up"))
                    {
                        CmdGet(1);
                    }
                    if (Input.GetKeyDown("down"))
                    {
                        CmdGet(0);
                    }
                    //controller up down button 
                    float v = Input.GetAxis("UpDown");
                    float _v = v + btcon.v;

                    if (_v > 0 && !a)
                    {
                        //print("vertical movement" + verticalValue);
                        a = true;
                        CmdGet(1);
                    }
                    if (_v < 0 && !a)
                    {
                        //print("vertical movement" + verticalValue);
                        a = true;
                        CmdGet(0);
                    }

                    if (Input.GetButtonDown("Action") || Input.GetButtonDown("Jump") || btcon.space)
                    {
                        CmdStartGame();
                    }
                }
                else
                {
                    UIHost.SetActive(false);
                }
            }
            else
            {
                UIHostText.text = "    Waiting for Player. . . (1/2)";
                UISel.SetActive(false);
            }
        }

        if (spawnPt.Length > 0 && !spawning && isServer)
        {
            spawning = true;
            CmdSpawnEnvir();
            for (int i = 0; i < players.Length; i++)
            {
                CmdSpawnForStart(players[i], spawnPlayer, spawnPt[i]);
            }
        }
    }

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
            roomNo++;
        }
        else
        {
            roomNo--;
        }
        if (roomNo < 0) roomNo = KindsOfRoom -1;
        else if (roomNo >= KindsOfRoom) roomNo = 0;
        switch (roomNo)
        {
            case 1:
                UIRoomName.text = "Bedroom";
                break;

            case 2:
                UIRoomName.text = "Keyroom";
                break;

            default:
                UIRoomName.text = "TEST-ROOM";
                break;
        }
        
    }

    [Command]
    public void CmdStartGame()
    {
        RpcStartGame();
    }

    [ClientRpc]
    private void RpcStartGame()
    {
        starting = true;
        StartCoroutine(fade());
        for (int j = 0; j < players.Length; j++)
        {
            DontDestroyOnLoad(players[j]);
        }
    }

    IEnumerator fade()
    {
        yield return new WaitForSeconds(1f);
        float fadeTime = GameObject.Find("Main Camera").GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);

        switch (roomNo)
        {
            case 1:
                SceneManager.LoadScene("Bedroom");
                break;

            case 2:
                SceneManager.LoadScene("Keyroom");
                break;

            default:
                SceneManager.LoadScene("Testroom");
                break;
        }
    }

    public void CmdSpawnForStart(GameObject player, GameObject playerNew, GameObject place)
    {
        var conn = player.GetComponent<NetworkIdentity>().connectionToClient;
        var newPlayer = Instantiate<GameObject>(playerNew, place.transform.position, Quaternion.identity);
        NetworkServer.Spawn(newPlayer);
        NetworkServer.ReplacePlayerForConnection(conn, newPlayer, 0);

        playerNo = player.GetComponent<Selection>().num;
        newPlayer.GetComponent<PlayerSetup>().RpcChoosePlayer(playerNo);

        Destroy(player.gameObject);
    }

    [Command]
    private void CmdSpawnEnvir()
    {
        switch (roomNo)
        {
            case 1:
                for (int i = 0; i < bedroom.Length; i++)
                {
                    GameObject sp = Instantiate(bedroom[i]);
                    NetworkServer.Spawn(sp);
                }
                break;

            case 2:
                for (int i = 0; i < keyroom.Length; i++)
                {
                    GameObject sp = Instantiate(keyroom[i]);
                    NetworkServer.Spawn(sp);
                }
                break;

            default:
                break;
        }
    }
}
