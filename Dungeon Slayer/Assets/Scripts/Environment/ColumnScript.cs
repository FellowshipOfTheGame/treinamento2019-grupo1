using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnScript : MonoBehaviour {
    
    public EdgeCollider2D step;
    public CapsuleCollider2D idle;
    public SpriteRenderer sprite;
    public float timeToRise = 2f;
    public float delayToRise = 5f;
    public float distToRise = 12f;
    [SerializeField] private float health = 10f;
    private float curHealth;
    
    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        curHealth = health; // Inicializa sua vida com o valor padrao
        idle.enabled = false;
        StartCoroutine(Rise());
    }

    // Corotina para fazer o pilar se levantar do chao
    IEnumerator Rise() {
        // Faz com que o pilar demore um pouco ate comecar a se erguer do chao (para que o jogador possa bater no segundo boss)
        yield return new WaitForSeconds(delayToRise);
        // Levanta o pilar do chao
        float timePassed = 0f;
        while (timePassed < timeToRise) {
            float delta = Time.deltaTime;
            this.transform.position += new Vector3(0f, delta*(distToRise/timeToRise), 0f);
            timePassed += delta;
            yield return null;
        }
        // Reativa o collider da base do pilar
        idle.enabled = true;
    }

    // Esta funcao e chamada a cada frame
    void Update() {
        if (curHealth <= 0) {
            Destroy(gameObject);    // A pilastra deve quebrar
        }
    }

    // Funcao que sera chamada sempre que a pilastra receber dano
    public void TakeDamage(int amount) {
        if (curHealth > 0) curHealth -= amount;
        sprite.color -= new Color(0f, 0f, 0f, 0.09f*amount);    // Mostra ao jogador que a pilastra recebeu dano, deixando sua cor mais transparente
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.otherCollider.GetType() == typeof(EdgeCollider2D) && col.gameObject.name != "SecondBoss") {
            Physics2D.IgnoreCollision(col.collider, col.otherCollider);
        }
    }
}
