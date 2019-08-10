using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public bool followPlayer = false;
    private GameObject player;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindWithTag("Player");
        offset = this.transform.position - player.transform.position;
    }

    // LateUpdate is called once per frame (after all Uptade things have been done)
    void LateUpdate() {
        if (followPlayer) this.transform.position = player.transform.position + offset;
    }
}