using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {

    [SerializeField]
    private int health = 100;
    [SyncVar]
    private int nowHealth = 100;
    [SyncVar]
    public bool dying = false;
    public bool isDead {get { return dying; } protected set { dying = value; }}
    [SerializeField]
    private Behaviour[] disableOnDeath;

    private float timer = 0f;
    private float deadt = 2f;
    [SerializeField]
    private bool[] turnon;

    //[SerializeField]
    //GameObject[] hurteffect = new GameObject[4];
    public GameObject hurtUI1, hurtUI2, hurtUI3, hurtUIfin;
    //private bool hurt,hurt1,hurt2,hurt3 = true;

    public Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    } 

    public int getHP()
    {
        return nowHealth;
    }

    public void Update()
    {
        if (timer >= deadt)
        {
            if (isDead == true)
            {
                Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
                transform.position = spawnPoint.position;
                transform.rotation = spawnPoint.rotation;
                Setting();
                Debug.Log("timer Done");
            }
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    public void Setup ()
    {
        turnon = new bool[disableOnDeath.Length];
        for (int i =0; i < turnon.Length; i++)
        {
             turnon[i] = disableOnDeath[i].enabled;
        }
        Setting();
	}
    
    [ClientRpc]
    public void RpcTakeDamage (int damage, string n)
    {
        if (isDead)
           return;

            nowHealth -= damage;
            Debug.Log(n + " " + nowHealth + " / " + health);

        int h = health / 4;
        if (nowHealth <= h * 3 && isLocalPlayer)
        {
            //hurtUI1 = Instantiate(hurteffect[0]);
            //hurt1 = false;
            hurtUI1.SetActive(true);
        } else hurtUI1.SetActive(false);
        if (nowHealth <= h * 2 && isLocalPlayer)
        {
            //hurtUI2 = Instantiate(hurteffect[1]);
            //hurt2 = false;
            hurtUI2.SetActive(true);
        } else hurtUI2.SetActive(false);
        if (nowHealth <= h && isLocalPlayer)
        {
            //hurtUI3 = Instantiate(hurteffect[2]);
            //hurt3 = false;
            hurtUI3.SetActive(true);
        } else hurtUI3.SetActive(false);


        if (nowHealth <= 0)
            {
            if (isLocalPlayer)
            {
                hurtUIfin.SetActive(true);
            }
            gotohell();
            StartCoroutine(dieee());
            
            //gameObject.transform.position = Vector3.down * 10f;
        }
    }
    IEnumerator dieee()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool(Animator.StringToHash("die"), true);
        yield return new WaitForSeconds(1f);
        anim.SetBool(Animator.StringToHash("die"), false);
        yield return new WaitForSeconds(1f);
    }

    private void gotohell()
    {
        isDead = true;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        timer = 0f;
        anim.SetBool(Animator.StringToHash("die"), true);
        //play dead animation
        //StartCoroutine(Respawn());
    }

    //IEnumerator Respawn()
   // {
      //  yield return new WaitForSeconds(GameControl.usingManager.matchSetting.respawnT);
     //   gameObject.transform.rotation = new Quaternion(90, 90, 90, 0);
     //   Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
     //   transform.position = spawnPoint.position;
      //  transform.rotation = spawnPoint.rotation;
      //  Setting();
   // }
    public void Setting()
    {
        isDead = false;
        nowHealth = health;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = turnon[i];
        }
        //Destroy(hurtUI); Destroy(hurtUI1); Destroy(hurtUI2); Destroy(hurtUI3);
        //hurt = hurt1 = hurt2 = hurt3 = true;
        hurtUI1.SetActive(false);
        hurtUI2.SetActive(false);
        hurtUI3.SetActive(false);
        hurtUIfin.SetActive(false);
        anim.SetBool(Animator.StringToHash("die"), isDead);
    }
}
