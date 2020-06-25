using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookat : MonoBehaviour{

    private Transform look;
    private Vector3 LookTarget;
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 0.3F;
    private Vector3 skeletonUp;


    void Start() {
        if (look == null) {
            look = GameObject.FindGameObjectWithTag("Player").transform;
        }

        skeletonUp = transform.up;
    }
    
    void Update() {

        LookTarget = Vector3.SmoothDamp(LookTarget, look.position, ref velocity, smoothTime);

        transform.LookAt(LookTarget, skeletonUp);

    }
}