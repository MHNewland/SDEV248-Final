using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SniperController : EnemyClass
{

    [SerializeField]
    BoxCollider2D attackSpawnPoint;
 

    Animator sniperAnimator;

    // Start is called before the first frame update
    void Awake()
    {
        ecRenderer = GetComponent<SpriteRenderer>();
        sniperAnimator = GetComponent<Animator>();
        attackCooldown = 4;
        hp = 2;
        maxHP = 2;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHPBar();
        cooldownTimer += Time.deltaTime;
        sniperAnimator.SetBool("PlayerInSight", PlayerInSight());
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                //attack
                cooldownTimer = 0;
                Attack();
            }
        }
    }
    public override void Attack()
    {
        sniperAnimator.SetTrigger("Attack");
    }

    public override void Move()
    {
        throw new System.NotImplementedException();
    }

    private bool PlayerInSight()
    {

        float distanceToPlayer = Vector3.Distance(gameObject.transform.position, PlayerController.Instance.gameObject.transform.position);

        return distanceToPlayer < range && PlayerController.Instance.gameObject.layer == gameObject.layer;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, range);
    }
}
