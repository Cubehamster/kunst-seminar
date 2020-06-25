using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ocelate : MonoBehaviour {

    [SerializeField] float multp = 100;
    [SerializeField] float speed = 1;

    void Update() {
        //transform.Rotate(0, multp * Mathf.Sin(speed * Time.deltaTime), 0);
        transform.rotation = Quaternion.Euler(0, Mathf.Sin(Time.realtimeSinceStartup * speed) * multp, 0);
    }
}