using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class ProwlerController : EnemyClass
{
    [SerializeField]
    BoxCollider2D boxCollider;

    [SerializeField]
    Collider2D wallCollider;

    [SerializeField]
    Collider2D edgeCollider;

    [SerializeField]
    bool movingRight;

    [SerializeField]
    float colliderDistance;

    Animator prowlerAnimator;
    float speed;
    bool attacking;



    private void Awake()
    {
        speed = 1;
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        movingRight = true;
        ecRenderer = GetComponent<SpriteRenderer>();
        attackCooldown = 3;
        prowlerAnimator = GetComponent<Animator>();
        hp = 3;
        maxHP = 3;

    }

    private void Update()
    {
        UpdateHPBar();
        Move();
        cooldownTimer += Time.deltaTime;
        attacking = PlayerInSight();

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
        prowlerAnimator.SetTrigger("Attack");
    }

    public override void Move()
    {

        if (!attacking)
        {
            if (movingRight)
            {
                healthbar.transform.parent.localScale = Vector3.one;
                gameObject.transform.localScale = Vector3.one;
                rb.transform.Translate(Vector2.right * speed * Time.deltaTime);
            }
            else
            {
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
                healthbar.transform.parent.localScale = new Vector3(-1, 1, 1);
                rb.transform.Translate(Vector2.left * speed * Time.deltaTime);
            }
        }

    }

    private bool PlayerInSight()
    {

        RaycastHit2D attackBox = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * colliderDistance * range * transform.localScale.x,
                                                   new Vector3(boxCollider.size.x * range, .4f),
                                                   0,
                                                   Vector2.left,
                                                   0,
                                                   LayerMask.GetMask(LayerMask.LayerToName(currentLayer)));
        return (attackBox.collider != null && attackBox.collider.CompareTag("Player"));

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * colliderDistance * range * transform.localScale.x, new Vector3(boxCollider.size.x * range, .4f));
    }
    //called during animation
    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            PlayerController.Instance.TakeDamage();
        }
    }

    //wall collider hit wall
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("World"))
        {
            //if it's touching a world wall or same color wall
            if (wallCollider.IsTouchingLayers(LayerMask.GetMask(LayerMask.LayerToName(currentLayer))) || wallCollider.IsTouchingLayers(LayerMask.GetMask("World")))
            {
                if(gameObject.transform.localScale.x == 1)
                {
                    movingRight = false;
                }else if (gameObject.transform.localScale.x == -1)
                {
                    movingRight = true;
                }

            }
        }
    }

    //edge collider came out of ground
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(!edgeCollider.IsTouchingLayers() || collision.gameObject.layer == gameObject.layer)
        {
            if (collision.CompareTag("Bullet")) return;

            if (gameObject.transform.localScale.x == 1)
            {
                movingRight = false;
            }
            else if (gameObject.transform.localScale.x == -1)
            {
                movingRight = true;
            }
        }
    }

    public void TakeDamage()
    {
        hp--;
    }
}
