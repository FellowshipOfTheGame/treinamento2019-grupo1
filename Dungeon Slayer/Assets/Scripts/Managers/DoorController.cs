using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {

    public Animator animator;
    private PlayerManager player;
    [SerializeField] private bool canBeOpen = true;
    [SerializeField] private float transitionTime = 1f;
    [SerializeField] private int goToScene = 1;

    void Start() {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && canBeOpen) {
            SceneController.instance.ChangeScene(goToScene);
            animator.SetTrigger("OpenTheDoor");
            StartCoroutine(StopPlayer());
        }
    }

    IEnumerator StopPlayer() {
        player.SetMovement(false);
        yield return new WaitForSeconds(transitionTime);
        player.SetMovement(true);
    }
}
