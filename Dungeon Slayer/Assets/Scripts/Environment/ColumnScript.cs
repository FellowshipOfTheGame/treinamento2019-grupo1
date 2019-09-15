using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnScript : MonoBehaviour {
    
    public Animator animator;
    public EdgeCollider2D step;
    public CapsuleCollider2D idle;
    public SpriteRenderer sprite;
    [SerializeField] private float timeToRise = 2f;
    [SerializeField] private float health = 10f;
    private float curHealth;
    
    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        curHealth = health; // Inicializa sua vida com o valor padrao
        idle.enabled = false;
        StartCoroutine(enableFullCollider());
    }

    IEnumerator enableFullCollider() {
        yield return new WaitForSeconds(timeToRise);
        idle.enabled = true;
    }

    // Esta funcao e chamada a cada frame
    void Update() {
        if (curHealth <= 0) {
            // A pilastra deve quebrar
            sprite.color = Color.white;
            animator.SetTrigger("Broke");    // Avisa ao Animator que ela quebrou
            step.enabled = false;
            Destroy(gameObject, 3f);    // Destroi ela apos um certo tempo
        }
    }

    // Funcao que sera chamada sempre que a pilastra receber dano
    public void TakeDamage(int amount) {
        if (curHealth > 0) curHealth -= amount;
        Debug.Log("Taken " + amount + " of damage.");
        sprite.color -= new Color(0f, 0f, 0f, 0.09f*amount);
        // animator.SetTrigger("HasTakenDamage");
    }
}
