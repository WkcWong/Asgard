using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private float lookspeed = 5f;
    private float jumpf = 570f;
    public Camera cam;
    public CapsuleCollider foot;
    public LayerMask ground;

    private Rigidbody body;
    private float camrot = 0;
    public Animator anim;

    public bool jump = false;
    public float jumpedTime = 0;

    public float h, v, x, y = 0;

    private ButtonControl btcon;
    public GameObject CamSet;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        btcon = GameObject.FindGameObjectWithTag("phone").GetComponent<ButtonControl>();

        if (btcon.use)
        {
            CamSet.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        } 
    }
    private void Update()
    {
        if (jump)
        {
            jumpedTime += Time.deltaTime;
            if (jumpedTime >= .2)
            {
                jump = false;
                jumpedTime = 0;
            }
        }
    }
    void FixedUpdate()
    {
        //move
        h = Input.GetAxisRaw("Horizontal");
        float _h = h + btcon.h;
        v = Input.GetAxisRaw("Vertical");
        float _v = v + btcon.v;
        Vector3 mov = new Vector3(_h, 0.0f, _v).normalized * speed;
        body.transform.Translate(Vector3.forward * mov.z * Time.fixedDeltaTime);
        body.transform.Translate(Vector3.right * mov.x * Time.fixedDeltaTime);
        if (_h != 0 || _v != 0)
        {
            anim.SetBool(Animator.StringToHash("move"), true);
        }
        else
        {
            anim.SetBool(Animator.StringToHash("move"), false);
        }

        //rotate & sight
        if (btcon.use)
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
        else
        {
            y = Input.GetAxisRaw("Mouse X");
            Vector3 rot = new Vector3(0, y, 0) * lookspeed;
            body.MoveRotation(body.rotation * Quaternion.Euler(rot));

            x = Input.GetAxisRaw("Mouse Y");
            camrot -= x * lookspeed;
            camrot = Mathf.Clamp(camrot, -85f, 70f);
            cam.transform.localEulerAngles = new Vector3(camrot, 0f, 0f);
        }

        //jump
        if (Input.GetButtonDown("Jump") || btcon.space)
        {
            if (onGround() && !jump)
            {
                body.AddForce(Vector3.up * jumpf);
                jump = true;
            }
        }
    }


    private bool onGround()
    {
        return Physics.CheckCapsule(foot.bounds.center, new Vector3(
        foot.bounds.center.x, foot.bounds.min.y, foot.bounds.center.z), foot.radius * 0.9f, ground);
    }
}