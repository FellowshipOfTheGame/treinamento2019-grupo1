using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnScript : MonoBehaviour {
    
    public EdgeCollider2D step;
    public CapsuleCollider2D idle;
    public SpriteRenderer sprite;
    public float timeToRise = 2f;
    public float delayToRise = 1f;
    public float distToRise = 12f;
    [SerializeField] private float health = 10f;
    private float curHealth;
    
    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        curHealth = health; // Inicializa sua vida com o valor padrao
        idle.enabled = false;
    }

    // Corotina para fazer o pilar se levantar do chao
    IEnumerator Rise() {
        // Faz com que o pilar demore um pouco ate comecar a se erguer do chao (para que o jogador possa bater no segundo boss)
        yield return new WaitForSeconds(delayToRise);
        // "Enrijece" o collider do step
        step.isTrigger = false;
        // Levanta o pilar do chao
        float timePassed = 0f;
        while (timePassed < timeToRise) {
            float delta = Time.deltaTime;
            this.transform.position += new Vector3(0f, delta*(distToRise/timeToRise), 0f);
            timePassed += delta;
            yield return null;
        }
        // Reativa as colisoes entre o pilar e o jogador/primeiro boss
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Column"), LayerMask.NameToLayer("Player"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Column"), LayerMask.NameToLayer("Enemy"), false);
        // Reativa o collider da base do pilar
        idle.enabled = true;
        // Desativa o collider do "step"
        step.enabled = false;
    }

    // Esta funcao e chamada a cada frame
    void Update() {
        if (curHealth <= 0) {
            Destroy(gameObject);    // A pilastra deve quebrar
            EventsManager.current.ColumnDestroy();  // Ativa os eventos em resposta a quebra do pilar
        }
    }

    // Funcao que sera chamada sempre que a pilastra receber dano
    public void TakeDamage(int amount) {
        if (curHealth > 0) curHealth -= amount;
        sprite.color -= new Color(0f, 0f, 0f, 0.09f*amount);    // Mostra ao jogador que a pilastra recebeu dano, deixando sua cor mais transparente
    }

    // Funcao chamada quando o segundo boss entrar em contato com uma regiao em que ira crescer um novo pilar
    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.name == "SecondBoss") {
            // Desativa as colisoes entre o pilar e o jogador/primeiro boss
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Column"), LayerMask.NameToLayer("Player"), true);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Column"), LayerMask.NameToLayer("Enemy"), true);
            StartCoroutine(Rise());
        }
    }
}
