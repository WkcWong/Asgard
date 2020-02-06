using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;
    Camera sceneCam;

    //[SerializeField]
    //GameObject playerUIPrefab;
    //private GameObject playerUIInstance;

    //[SerializeField]
    //GameObject[] componentsToDisableMyself;

    [SerializeField]
    GameObject[] chooseList;

    private Animator anim;
    private int choose = 0;
    private bool chosen = false;
    private bool showed = false;

    void Start () {
        anim = GetComponent<Animator>();

        for (int i = 0; i < chooseList.Length; i++)
        {
            chooseList[i].layer = 12;
            int childNo = chooseList[i].transform.childCount;
            for (int x = 0; x < childNo; x++)
            {
                chooseList[i].transform.GetChild(x).gameObject.layer = 12;
            }
        }

        if (!isLocalPlayer)
        {
            for (int i = 0; i< componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
            //gameObject.layer = LayerMask.NameToLayer("Player");

        }
        else
        {
            Camera.main.gameObject.SetActive(false);
            //playerUIInstance = Instantiate(playerUIPrefab);
            //playerUIInstance.name = playerUIPrefab.name;
            //playerUIInstance.transform.position = new Vector3();
            //playerUIInstance.transform.parent = transform.Find("PlayerCam").transform;
            anim.SetBool(Animator.StringToHash("shoot"), true);
            /*
            for (int i = 0; i < componentsToDisableMyself.Length; i++)
            {
                //componentsToDisableMyself[i].SetActive(false);
                componentsToDisableMyself[i].layer = 12;
            }
            */

        }
        GetComponent<PlayerHealth>().Setup();
    }

    private void Update()
    {
        if (chosen && !showed && !isLocalPlayer)
        {
            showed = true;
            chooseList[choose].layer = 0;
            int childNo = chooseList[choose].transform.childCount;
            for (int x = 0; x < childNo; x++)
            {
                chooseList[choose].transform.GetChild(x).gameObject.layer = 0;
            }
        }
    }

    [ClientRpc]
    public void RpcChoosePlayer(int a)
    {
        choose = a;
        chosen = true;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerHealth health = GetComponent<PlayerHealth>();
        GameControl.RegisterPlayer(netID, health);
    }
    

    void OnDisable()
    {
        //Destroy(playerUIInstance);

        if (Camera.main != null)
        {
            Camera.main.gameObject.SetActive(true);
        }
        GameControl.UnRegisterPlayer(transform.name);
    }
}
