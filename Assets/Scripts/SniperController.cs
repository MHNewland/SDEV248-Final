using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SniperController : EnemyClass
{

    [SerializeField]
    Collider2D attackSpawnPoint;

    [SerializeField]
    GameObject bullet;

    Animator sniperAnimator;

    // Start is called before the first frame update
    void Awake()
    {
        ecRenderer = GetComponent<SpriteRenderer>();
        sniperAnimator = GetComponent<Animator>();
        attackCooldown = 3;
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

        Vector3 dir = PlayerController.Instance.gameObject.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.Euler(0f, 0f, angle);
        
        StartCoroutine("fireBullet", q);
    }

    IEnumerator fireBullet(Quaternion q)
    {
        yield return new WaitForSeconds(.5f);
        GameObject bulletGO = Instantiate(bullet, attackSpawnPoint.bounds.center, q, transform);
        bulletGO.GetComponent<BulletController>().parent = gameObject;
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

    public  void TakeDamage()
    {
        hp--;
    }
}
