using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PlayerSwitch : NetworkBehaviour
{

    [SerializeField]
    public List<GameObject> Humanside = new List<GameObject>();
    [SerializeField]
    List<GameObject> Robotside = new List<GameObject>();

    public Rigidbody body;
    private PlayerMovement PlayerMovement;
    private PlayerAction PlayerAction;
    private float cockpitPos;
    private bool onBot = false;

    private GameObject botgo;
    //private Rigidbody botrb;
    private RobotControl botc;
    private float timer = 3f;
    private float delayt = 3f;

    private Vector3 rememberPos;

    private ButtonControl btcon;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerAction = GetComponent<PlayerAction>();
        cockpitPos = float.Parse(GetComponent<NetworkIdentity>().netId.ToString());
        btcon = GameObject.FindGameObjectWithTag("phone").GetComponent<ButtonControl>();
    }

    private void Update()
    {
        if (timer < 3) //for easy click
        {
            timer += Time.deltaTime;
        }

        float _h = Input.GetAxisRaw("Horizontal");
        float _v = Input.GetAxisRaw("Vertical");

        float h = btcon.h + _h;
        float v = btcon.v + _v;

        float y = Input.GetAxisRaw("Mouse X");
        float x = Input.GetAxisRaw("Mouse Y");

        if (onBot)
        {
            if (botc != null)
            {
                if (btcon.use)
                {
                    botc.RobotMove(btcon.h, btcon.v);
                    botc.RobotSightapk();
                }
                else
                {
                    botc.RobotMove(h, v);
                    botc.RobotSight(x, y);
                }
            }
            else { Debug.Log("NO ROBOT"); }

            if (Input.GetButtonDown("Jump") || btcon.space)
            {
                if (timer >= 1 && onBot)
                {
                    CmdDropControl(botgo);
                    changingMan();
                    StartCoroutine(fade());
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Robot") //if it is robot
        {
            botc = other.gameObject.GetComponent<RobotControl>();

            if (Input.GetButtonDown("Jump") || btcon.space)
            {
                if (botc.used == false && !onBot)
                {
                    if (other.gameObject.tag == "Robot")
                    {
                        botgo = other.gameObject;
                        //botrb = other.attachedRigidbody;

                        CmdGetControl(botgo);
                        changingBot();
                        timer = 0;

                        rememberPos = body.position - other.transform.position;
                        StartCoroutine(fade());
                    }
                }
            }
            else if (Input.GetButtonDown("Jump") && botc.used == true)
            {
                Debug.Log("The robot is in used!!");
            }
        }
    }

    IEnumerator delaytobot()
    {
        yield return new WaitForSeconds(delayt);

        onBot = true;
        for (int i = 0; i < Humanside.Count; i++)
        {
            Humanside[i].SetActive(false);
        }
        for (int i = 0; i < Robotside.Count; i++)
        {
            Robotside[i].SetActive(true);
        }
        body.isKinematic = true;

        body.position = new Vector3(cockpitPos * 10, -50, 0);
    }

    private void changingBot()
    {
        CmdLocking();
        PlayerMovement.enabled = false;
        PlayerAction.enabled = false;
        StartCoroutine("delaytobot");
    }

    IEnumerator delaytoman()
    {
        yield return new WaitForSeconds(delayt);

        PlayerMovement.enabled = true;
        PlayerAction.enabled = true;
        for (int i = 0; i < Humanside.Count; i++)
        {
            Humanside[i].SetActive(true);
        }
        for (int i = 0; i < Robotside.Count; i++)
        {
            Robotside[i].SetActive(false);
        }
        body.isKinematic = false;
        CmdUnlocking();
    }

    private void changingMan()
    {
        onBot = false;
        //body.position = new Vector3(botrb.position.x, 0.1f, botrb.position.z);
        body.position = botgo.transform.position + rememberPos;
        StartCoroutine("delaytoman");
    }

    [Command]
    void CmdLocking()
    {
        botc.RpcLockBot();
    }

    [Command]
    void CmdUnlocking()
    {
        botc.RpcUnlockBot();
    }

     [Command]
     void CmdGetControl (GameObject bot)
     {
         NetworkIdentity robotID = bot.GetComponent<NetworkIdentity>();
         robotID.AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
     }

     [Command]
     void CmdDropControl(GameObject bot)
     {
         NetworkIdentity robotID = bot.GetComponent<NetworkIdentity>();
         robotID.RemoveClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
     }

    IEnumerator fade()
    {
        float fadeTime = GetComponentInChildren<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime + 2.5f);
        fadeTime = GetComponentInChildren<Fading>().BeginFade(-1);
    }
}
