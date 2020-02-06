using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RobotControl : NetworkBehaviour {

    public float speed = 8f;
    public float lookspeed = 5f;
    private float camrot = 0;

    public Camera cam;
    public Rigidbody body;

    [SyncVar]
    public bool used = false;
    private ButtonControl btcon;

	void Start ()
    {
        //Debug.Log("Client: "+ isClient);
        //Debug.Log("Server: " + isServer);
        //Debug.Log("LocalPlayer: " + isLocalPlayer);
        //Debug.Log("Authority: " + hasAuthority);
        btcon = GameObject.FindGameObjectWithTag("phone").GetComponent<ButtonControl>();
    }
	

    public void RobotMove(float h, float v)
    {
        //Vector3 mov = new Vector3(h, 0.0f, v).normalized * speed;
        //body.transform.Translate(Vector3.forward * mov.z * Time.fixedDeltaTime);
        //body.transform.Translate(Vector3.right * mov.x * Time.fixedDeltaTime);

        Vector3 mov = new Vector3(h, 0.0f, v).normalized * speed;
        body.transform.Translate(Vector3.forward * mov.z * Time.fixedDeltaTime);
        body.transform.Translate(Vector3.right * mov.x * Time.fixedDeltaTime);
    }

    public void RobotSight(float x, float y)
    {
        Vector3 rot = new Vector3(0, y, 0) * lookspeed;
        body.MoveRotation(body.rotation * Quaternion.Euler(rot));
        camrot -= x * lookspeed;
        camrot = Mathf.Clamp(camrot, -85f, 70f);
        cam.transform.localEulerAngles = new Vector3(camrot, 0f, 0f);
    }

    public void RobotSightapk()
    {
        Vector3 previousEulerAngles = body.transform.eulerAngles;
        Vector3 gyroInput = btcon.gyro.rotationRateUnbiased;
        Vector3 targetEulerAngles = previousEulerAngles - gyroInput * Time.deltaTime * Mathf.Rad2Deg;
        targetEulerAngles.x = 0.0f;
        targetEulerAngles.z = 0.0f;
        body.transform.eulerAngles = targetEulerAngles;

        Vector3 pEulerAngles = cam.transform.eulerAngles;
        Vector3 gInput = btcon.gyro.rotationRateUnbiased;
        Vector3 tEulerAngles = pEulerAngles - gInput * Time.deltaTime * Mathf.Rad2Deg;
        tEulerAngles.z = 0.0f;
        tEulerAngles.y = targetEulerAngles.y;
        cam.transform.eulerAngles = tEulerAngles;
    }

    [ClientRpc]
    public void RpcLockBot()
    {
        used = true;
    }

    [ClientRpc]
    public void RpcUnlockBot()
    {
        used = false;
    }
}
