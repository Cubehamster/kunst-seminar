using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bounce : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] float multip = 1;

    Vector3 offset;

    private void Start()
    {
        offset = transform.position;
    }

    private void Update()
    {
        //transform.position = new Vector3(0,Mathf.Sin(Time.realtimeSinceStartup * speed) * multip, 0) + offset;


        transform.position = offset + transform.forward * Mathf.Sin(Time.realtimeSinceStartup * speed) * multip;
    }
}