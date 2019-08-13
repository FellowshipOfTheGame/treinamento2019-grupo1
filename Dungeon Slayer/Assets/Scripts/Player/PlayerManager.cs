using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerManager : MonoBehaviour {
    
    public static PlayerManager instance;

    public PlayerMovement movementScript;
    public PlayerAttack attackScript;
    public Animator animator;
    public SpriteRenderer sprite;
    public Slider healthBar;
    [SerializeField] private float health = 30f;
    private float curHealth;

    // Essa funcao e chamada quando o objeto e instanciado pela primeira vez
    void Awake() {
        // So pode existir uma instancia dessa classe
        if (instance == null) {
            instance = this;
            //healthBar.enabled = true; // Achar uma forma de habilitar
        }
        else {
            // O jogador fica na posicao inicial da sua instancia naquela sala
            PlayerManager.instance.transform.position = this.transform.position;
            // A outra instancia e destruida
            Destroy(gameObject);
            return;
        }
        // Este objeto (Player) ira persistir por todas as cenas
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(healthBar.transform.parent.gameObject);
        // A vida do jogador e colocada
        curHealth = health;
    }

    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        //CinemachineVirtualCamera vcam = GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        //vcam.m_Follow = this.transform;
    }

    // Esta funcao e chamada a cada frame
    void Update() {
        if (curHealth <= 0) {
            // O jogador morreu
            Destroy(gameObject);    // Destroi o jogador
            // Chama a cena de Fim de Jogo
            Application.Quit();
        }
        // Atualiza a barra de vida
        healthBar.SetValueWithoutNotify(curHealth/health);
    }
    
    // Funcao que sera chamada sempre que o jogador tiver de receber dano
    public void TakeDamage(int amount) {
        if (curHealth > 0) curHealth -= amount;
        // animator.SetTrigger("HasTakenDamage");
    }

    public void SetMovement(bool canPlayerMove) {
        movementScript.canMove = canPlayerMove;
    }

    public void SetAttack(bool canPlayerAttack) {
        attackScript.canAttack = canPlayerAttack;
    }

    public void SetColor(Color c) {
        if (sprite != null) sprite.color = c;
    }
}
