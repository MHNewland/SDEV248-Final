using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance { get; private set; }
    public event EventHandler<(Color prevColor, Color nextColor)> OnColorShift;

    Color red = Color.red;
    Color yellow = Color.yellow;
    Color blue = new Color(0, (200f / 255f), 1);
    Color prevColor;
    Color nextColor;
    SpriteRenderer sr;
    Rigidbody2D rb;


    [SerializeField]
    GameObject bodyGO;

    Animator playerAnimator;
    Animator bodyAnimator;

    public int colorIndex { get; private set; }

    public bool changingColor { get; private set; }

    public List<Color> colors { get; private set; }
    public List<string> layers { get; private set; }

    float moveX;
    float speed;
    float jumpForce;
    bool grounded;

    int hp;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one PlayerController" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = 5f;
        jumpForce = 5f;
        colorIndex = 0;
        colors = new List<Color> { blue, red, yellow };
        layers = new List<string> { "Blue", "Red", "Yellow" };

        prevColor = blue;
        nextColor = red;

        changingColor = false;
        sr = bodyGO.GetComponent<SpriteRenderer>();
        Instance.OnColorShift += ChangeColor;
        playerAnimator = GetComponent<Animator>();
        bodyAnimator = bodyGO.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        grounded = true;


    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        if (!changingColor)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                colorIndex++;
                if (colorIndex >= colors.Count)
                {
                    colorIndex = 0;
                }
                changingColor = true;
            }
            else if(Input.GetKeyDown(KeyCode.LeftShift)) {
                colorIndex--;
                if (colorIndex < 0)
                {
                    colorIndex = colors.Count-1;
                }
                changingColor = true;
            }

            if (changingColor)
            {
                //set layer to player while changing color
                gameObject.layer = LayerMask.NameToLayer("Player");
                nextColor = colors[colorIndex];
                OnColorShift?.Invoke(this, (prevColor, colors[colorIndex]));
            }
        }
        else if(sr.material.color == nextColor)
        {
            changingColor = false;
            gameObject.layer = LayerMask.NameToLayer(layers[colorIndex]);
            prevColor = nextColor;
        }
    }

    void HandleMovement()
    {
        moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        
        playerAnimator.SetBool("movingRight", moveX>0);
        bodyAnimator.SetBool("movingRight", moveX>0);

        playerAnimator.SetBool("movingLeft", moveX < 0);
        bodyAnimator.SetBool("movingLeft", moveX < 0);

        gameObject.transform.Translate(moveX, 0, 0);

        if (grounded && Input.GetKeyDown(KeyCode.W))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

    }

    void ChangeColor(object sender, (Color prevColor, Color nextColor) e)
    {
        StartCoroutine(bodyGO.GetComponent<ColorShiftSystem>().ChangeColor(e.prevColor, e.nextColor));
    }

    public void TakeDamage()
    {
        hp--;
        Debug.Log("Damage Taken");
    }

}
