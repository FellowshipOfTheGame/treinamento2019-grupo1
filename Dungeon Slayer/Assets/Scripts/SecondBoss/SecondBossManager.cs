using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondBossManager : MonoBehaviour {

    public SecondBossMovement movementScript;
    //public SecondBossAttack attackScript;
    //public Animator animator;
    public Rigidbody2D bossRB;
    public BoxCollider2D col;
    public Slider healthBar;
    [SerializeField] private float health = 20f;
    private float curHealth;
    [SerializeField] private float delayToRise = 3f;
    [SerializeField] private float fallTime = 2f;
    [SerializeField] private float fallGravity = 100f;
    [SerializeField] private int fallDamage = 2;

    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        curHealth = health; // Inicializa a vida do boss
        EventsManager.current.onColumnDestroy += BossFall; // Inscreve o metodo "BossFall" como uma das acoes a serem tomadas quando o pilar quebrar
    }

    // Esta funcao e chamada a cada frame
    void Update() {
        if (curHealth <= 0) {
            // O boss morreu
            movementScript.canMove = false; // O boss nao pode mais se mexer
            //attackScript.canAttack = false; // O boss nao pode mais atacar
            //animator.SetTrigger("Died");    // Avisa ao Animator que ele morreu
            if (healthBar != null) Destroy(healthBar.gameObject, 0.5f); // Desativa sua barra de vida apos um certo tempo
            Destroy(gameObject, 0.5f);    // Destroi ele apos um certo tempo
        }
        // Atualiza a barra de vida
        if (healthBar != null) healthBar.SetValueWithoutNotify(curHealth/health);
    }

    // Funcao que sera chamada sempre que o boss tiver de receber dano
    public void TakeDamage(int amount) {
        if (curHealth > 0) curHealth -= amount;
        // animator.SetTrigger("HasTakenDamage");
    }

    public void SetMovement(bool canBossMove) {
        movementScript.canMove = canBossMove;
    }

    /*
    public void SetAttack(bool canBossAttack) {
        attackScript.canAttack = canBossAttack;
    }

    public Vector3 GetMovementVector() {
        return movementScript.movement;
    }
    */

    // Corotina que ira fazer o segundo boss intangivel enquanto estiver no alto
    private IEnumerator MakeIntangible() {
        yield return new WaitForSeconds(delayToRise);
        //bossRB.simulated = false;
        col.enabled = false;
        this.SetMovement(false);
    }

    // Funcao chamada quando o segundo boss entrar em contato com uma regiao em que ira crescer um novo pilar
    void OnTriggerEnter2D(Collider2D col) {
        // Desativa as colisoes entre o boss e o jogador
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("AirEnemy"), LayerMask.NameToLayer("Player"), true);
        StartCoroutine(MakeIntangible());
    }

    public void BossFall() {
        StartCoroutine(BossFallCor());
    }

    public IEnumerator BossFallCor() {
        //bossRB.simulated = true;
        bossRB.gravityScale = fallGravity;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("AirEnemy"), LayerMask.NameToLayer("Player"), false);
        yield return new WaitForSeconds(fallTime);
        col.enabled = true;
        bossRB.gravityScale = 0f;
        this.TakeDamage(fallDamage);
        yield return new WaitForSeconds(delayToRise/1.5f);
        this.SetMovement(true);
    }
}
