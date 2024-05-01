using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{

    private Rigidbody2D RB;
    private Animator animator;
    private SpriteRenderer sprite;
    private Vector2 rayVector;
    private RaycastHit2D rightRay;
    private RaycastHit2D leftRay;

    // Start is called before the first frame update
    void Start()
    {
        RB = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
        sprite = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();
    }

    void FlipSprite()
    {
        bool hasXVel = (RB.velocity.x < -0.1f || RB.velocity.x > 0.1f);
        if (hasXVel)
        {
            sprite.flipX = RB.velocity.x < -.1;
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }

    public void Run(float xMove, float speed)
    {
        rayVector = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        rightRay = Physics2D.Raycast(rayVector, Vector2.right, sprite.bounds.size.x / 2);
        leftRay = Physics2D.Raycast(rayVector, Vector2.left, sprite.bounds.size.x / 2);


        Debug.DrawRay(rayVector, Vector2.right * sprite.bounds.size.x / 2, Color.red);
        Debug.DrawRay(rayVector, Vector2.left * sprite.bounds.size.x / 2, Color.yellow);

        if (xMove == 0)
        {
            RB.velocity = new Vector2(0, RB.velocity.y);
        }

        if (xMove > 0)
        {
            if (rightRay.collider == null || (rightRay.collider != null && !rightRay.collider.CompareTag("World")))
            {
                RB.velocity = new Vector2(xMove * speed, RB.velocity.y);
            }
        }


        if (xMove < 0)
        {
            if (leftRay.collider == null || (leftRay.collider != null && !leftRay.collider.CompareTag("World")))
            {
                RB.velocity = new Vector2(xMove * speed, RB.velocity.y);
            }
        }
    }

    public void Jump(float power)
    {
        RB.velocity = new Vector2(RB.velocity.x, 0);
        RB.AddForce(Vector2.up * power, ForceMode2D.Impulse);
    }
}
