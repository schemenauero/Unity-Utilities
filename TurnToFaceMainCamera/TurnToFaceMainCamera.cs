using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToFaceMainCamera : MonoBehaviour {
    Camera c;
    
    void Start() {
        c = Camera.main;
    }

    
    void Update() {
        Vector3 pos = new Vector3(c.transform.position.x, transform.position.y, c.transform.position.z);
        transform.LookAt(pos);
    }
}