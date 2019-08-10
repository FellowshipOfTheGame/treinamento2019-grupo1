using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

    [SerializeField] private bool goBack = false;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            if (goBack) SceneController.instance.PreviousScene();
            else SceneController.instance.NextScene();
        }
    }
}
