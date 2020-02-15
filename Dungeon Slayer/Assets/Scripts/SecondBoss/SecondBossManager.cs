using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondBossManager : MonoBehaviour {

    public SecondBossMovement movementScript;
    //public SecondBossAttack attackScript;
    public Animator animator;
    public SpriteRenderer sprite;
    public Rigidbody2D bossRB;
    public BoxCollider2D col;
    public Slider healthBar;
    [SerializeField] private float health = 20f;
    private float curHealth;
    [SerializeField] private float delayToRise = 1f;
    [SerializeField] private float timeToRise = 2f;
    [SerializeField] private float fallTime = 2f;
    [SerializeField] private float fallGravity = 100f;
    [SerializeField] private int fallDamage = 2;
    [SerializeField] private float stunnedTime = 2f;
    private bool alive = true;

    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        curHealth = health; // Inicializa a vida do boss
        EventsManager.current.onColumnDestroy += BossFall; // Inscreve o metodo "BossFall" como uma das acoes a serem tomadas quando o pilar quebrar
        EventsManager.current.onFirstBossHit += ShieldMagic;
    }

    // Esta funcao e chamada a cada frame
    void Update() {
        if (alive) {
            if (curHealth <= 0) {
                // O boss morreu
                SetMovement(false); // O boss nao pode mais se mexer
                //SetAttack(false); // O boss nao pode mais atacar
                animator.SetTrigger("HasDied");    // Avisa ao Animator que ele morreu
                if (healthBar != null) Destroy(healthBar.gameObject, 0.5f); // Desativa sua barra de vida apos um certo tempo
                Destroy(gameObject, 4f);    // Destroi ele apos um certo tempo
                alive = false;
            }
            // Atualiza a barra de vida
            healthBar.SetValueWithoutNotify(curHealth/health);
        }
    }

    // Funcao que sera chamada sempre que o boss tiver de receber dano
    public void TakeDamage(int amount) {
        if (curHealth > 0) curHealth -= amount;
        animator.SetTrigger("WasHit");
        StartCoroutine(SwitchColor(0.6f));
    }

    public void SetMovement(bool canBossMove) {
        movementScript.canMove = canBossMove;
    }

    /*
    public void SetAttack(bool canBossAttack) {
        attackScript.canAttack = canBossAttack;
    }
    */

    public void SetColor(Color c) {
        if (sprite != null) sprite.color = c;
    }

    // Funcao chamada quando o segundo boss entrar em contato com uma regiao em que ira crescer um novo pilar
    void OnTriggerEnter2D(Collider2D col) {
        // Desativa as colisoes entre o boss e o jogador
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("AirEnemy"), LayerMask.NameToLayer("Player"), true);
        sprite.sortingOrder = 1;
        StartCoroutine(MakeIntangible());
    }

    // Corotina que ira fazer o segundo boss intangivel enquanto estiver no alto
    private IEnumerator MakeIntangible() {
        yield return new WaitForSeconds(delayToRise);
        this.SetMovement(false);
        yield return new WaitForSeconds(timeToRise);
        col.enabled = false;
    }

    public void BossFall() {
        StartCoroutine(BossFallCor());
        if (alive) {
            animator.SetBool("IsTunned", true);
            animator.SetTrigger("HasFallen");
        }
    }

    public IEnumerator BossFallCor() {
        bossRB.gravityScale = fallGravity;
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("AirEnemy"), LayerMask.NameToLayer("Player"), false);
        yield return new WaitForSeconds(fallTime);
        sprite.sortingOrder = 0;
        bossRB.gravityScale = 0f;
        this.TakeDamage(fallDamage);
        if (alive) StartCoroutine(SwitchColor(stunnedTime));
        yield return new WaitForSeconds(stunnedTime);
        if (alive) {
            col.enabled = true;
            this.SetMovement(true);
            animator.SetBool("IsTunned", false);
        }
    }

    // Faz com que o boss fique trocando de cor para sinalizar que ele levou o hit
    IEnumerator SwitchColor(float timer) {
        bool turnBossColor = false;
        while (timer > 0) {
            timer -= 0.1f;
            turnBossColor = !turnBossColor;
            if (turnBossColor) SetColor(Color.red);
            else SetColor(Color.white);
            yield return new WaitForSeconds(0.1f);
        }
        SetColor(Color.white);
    }

    void ShieldMagic() {
        if (healthBar != null) animator.SetTrigger("HasAttacked");
    }

    void OnDestroy() {
        EventsManager.current.onColumnDestroy -= BossFall; // Desinscreve o metodo "BossFall" quando o segundo boss eh destruido
        EventsManager.current.onFirstBossHit -= ShieldMagic;
    }
}
