using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonScript : MonoBehaviour {
    
    public Slider healthBar;
    public SpriteRenderer sprite;
    public Animator cameraAnimator;
    public PolygonCollider2D col;
    [SerializeField] private float health = 10f;
    private float curHealth;
    [SerializeField] private float heartCollapseTime = 6f;
    private bool alive = true;
    
    // Essa funcao e chamada antes do primeiro Update
    void Start() {
        curHealth = health;
    }

    // Esta funcao e chamada a cada frame
    void Update() {
        if (alive) {
            if (curHealth <= 0) {
                // A dungeon morreu
                if (healthBar != null) Destroy(healthBar.gameObject, 0.2f); // Desativa sua barra de vida apos um certo tempo
                Destroy(this.gameObject, heartCollapseTime);   // Destroi o coracao de Yuku apos um certo tempo
                col.enabled = false;
                EventsManager.current.HeartDestroy();
                alive = false;
            }
            // Atualiza a barra de vida
            if (healthBar != null) healthBar.SetValueWithoutNotify(curHealth/health);
        }
    }

    // Funcao que sera chamada sempre que o boss tiver de receber dano
    public void TakeDamage(int amount) {
        if (curHealth > 0) curHealth -= amount;
        cameraAnimator.SetTrigger("Shake"); // Avisa a Camera para tremer
        StartCoroutine(SwitchColor(0.1f));
        if (curHealth > 0) AudioManager.instance.Play("YukuScream");
        else {
            Debug.Log("Should play YukuDeath");
            AudioManager.instance.Play("YukuDeath");
        }
        //animator.SetTrigger("HasTakenDamage");
    }

    public void SetColor(Color c) {
        if (sprite != null) sprite.color = c;
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
}
