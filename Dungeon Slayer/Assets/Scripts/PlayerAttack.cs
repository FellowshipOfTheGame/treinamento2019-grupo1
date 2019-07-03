using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attack;
    public LayerMask enemyLayer;
    public float attackRange;
    public int attackDamage;
    private float attackDelay;
    public float initAttackDelay;

    // Update is called once per frame
    void Update()
    {
        if (attackDelay <= 0) {
            // The player can attack
            if (Input.GetAxisRaw("Attack") == 1) {
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attack.position, attackRange, enemyLayer);
                
                foreach (Collider2D enemy in enemiesToDamage) {
                    Debug.Log(enemy);
                    // Fazer o inimigo sofrer dano
                }

                Debug.Log("Attacked!!");
                attackDelay = initAttackDelay;
            }
        }
        else {
            attackDelay -= Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attack.position, attackRange);
    }
}
