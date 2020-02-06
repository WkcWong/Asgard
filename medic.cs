using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class medic : NetworkBehaviour {

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerHealth p = other.gameObject.GetComponent<PlayerHealth>();
            if (p != null)
            {
                CmdHeal(other.transform.gameObject);
            }
        }
    }

    [Command]
    void CmdHeal(GameObject a)
    {
        GameControl.GetPlayer(a.name).RpcTakeDamage(-50, a.name);
        RpcDestroy();
    }

    [ClientRpc]
    void RpcDestroy()
    {
        Destroy(gameObject);
    }
}
