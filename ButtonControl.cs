using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonControl : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

    public bool use, q, e, m1, m2,space,m1h = false;
    public float h, v = 0;
    protected Joystick joystick;
    private ButtonControl btcon;
    private float timer = 0;
    public GameObject bt, js;

    public Gyroscope gyro;
    public Quaternion g;

    private void Start()
    {
        joystick = FindObjectOfType<Joystick>();
        btcon = GameObject.FindGameObjectWithTag("phone").GetComponent<ButtonControl>();
        if (!use)
        {
            bt.SetActive(false);
            js.SetActive(false);
        }
        else
        {
            gyro = Input.gyro;
            gyro.enabled = true;
        }
    }

    private void Update()
    {
        h = joystick.Horizontal;
        v = joystick.Vertical;
        if (use)
        {
            g = gyro.attitude;
        }  

        if (q || e || m1 || m2 || space)
        {
            if (timer < .1)
            {
                timer += Time.deltaTime;
            }
            else
            {
                q = false; e = false; m1 = false; m2 = false; space = false;
                timer = 0;
            }
        }
    }

    public void Press(string key)
    {
        switch (key)
        {
            case "q":
                q = true;
                break;
            case "e":
                e = true;
                break;
            case "m1":
                m1 = true;
                break;
            case "m2":
                m2 = true;
                break;
            default:
                space = true;
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        btcon.m1h = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        btcon.m1h = false;
    }
}
