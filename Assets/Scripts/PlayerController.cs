using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Unity.Mathematics;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Security.Cryptography;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    GameObject bodyGO;

    [SerializeField]
    Image healthbar;
    Canvas canvas;

    [SerializeField]
    Camera mainCamera;

    [SerializeField]
    GameObject reticle;

    [SerializeField]
    GameObject bullet;

    [SerializeField]
    BoxCollider2D feetCollider;

    public static PlayerController Instance { get; private set; }
    public event EventHandler<(Color prevColor, Color nextColor)> OnColorShift;

    Color red = Color.red;
    Color yellow = Color.yellow;
    Color blue = new Color(0, (200f / 255f), 1);
    Color prevColor;
    Color nextColor;
    SpriteRenderer sr;
    Rigidbody2D rb;

    Animator playerAnimator;
    Animator bodyAnimator;

    public int colorIndex { get; private set; }

    public bool changingColor { get; private set; }

    public List<Color> colors { get; private set; }
    public List<string> layers { get; private set; }

    float moveX;
    float speed;
    float jumpForce;
    [SerializeField]
    bool grounded;
    [SerializeField]
    Transform spawnPoint;

    public bool cutscenePlaying;

    int hp;
    int maxHP;

    public bool canChangeColors;

    public bool canFire;

    bool spawning;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one PlayerController" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        colorIndex = 0;
        colors = new List<Color> { red, blue, yellow };
        layers = new List<string> { "Red", "Blue", "Yellow" };
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        spawning = true;
        transform.position = spawnPoint.position;

        speed = 5f;
        jumpForce = 5f;


        prevColor = red;
        nextColor = blue;

        changingColor = false;
        sr = bodyGO.GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        bodyAnimator = bodyGO.GetComponent<Animator>();
        canvas = GetComponentInChildren<Canvas>();
        rb = GetComponent<Rigidbody2D>();

        sr.material.color = colors[0];
        gameObject.layer = LayerMask.NameToLayer(layers[colorIndex]);
        grounded = true;

        canChangeColors = false;

        Cursor.visible = false;

        Instance.OnColorShift += ChangeColor;

        hp = 10;
        maxHP = 10;
        canFire = false;
        OnColorShift?.Invoke(this, (prevColor, colors[colorIndex]));
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        transform.position = spawnPoint.position;
        spawning = true;
        Instance.OnColorShift += ChangeColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawning)
        {
            OnColorShift?.Invoke(this, (prevColor, colors[colorIndex]));
            spawning = false;
        }
        HandleMovement();
        if (canFire) CaptureMouse();
        UpdateHPBar();
        CheckColorChange();
    }

    void HandleMovement()
    {
        if (!cutscenePlaying)
        {
            moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

            playerAnimator.SetBool("movingRight", moveX > 0);
            bodyAnimator.SetBool("movingRight", moveX > 0);

            playerAnimator.SetBool("movingLeft", moveX < 0);
            bodyAnimator.SetBool("movingLeft", moveX < 0);

            gameObject.transform.Translate(moveX, 0, 0);


            if (grounded && Input.GetKeyDown(KeyCode.W))
            {
                grounded = false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    void ChangeColor(object sender, (Color prevColor, Color nextColor) e)
    {
        StartCoroutine(bodyGO.GetComponent<ColorShiftSystem>().ChangeColor(e.prevColor, e.nextColor));
    }

    public void TakeDamage()
    {
        hp--;
    }

    void CaptureMouse()
    {

        Vector3 aimPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        aimPoint.z = 0;
        reticle.transform.position = aimPoint;

        Vector3 dir = aimPoint - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.Euler(0f, 0f, angle);

        if (!changingColor && Input.GetMouseButtonDown(0))
        {
            fireBullet(q);
        }
    }

    void fireBullet(Quaternion q)
    {
        GameObject bulletGO = Instantiate(bullet, gameObject.transform.position, q, transform);
        bulletGO.GetComponent<BulletController>().parent = gameObject;
    }

    protected void UpdateHPBar()
    {
        if (hp <= 0)
        {
            transform.position = spawnPoint.position;
            hp = maxHP;
        }
        healthbar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (hp / (float)maxHP));
    }

    void CheckColorChange()
    {
        if (canChangeColors && !changingColor)
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
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                colorIndex--;
                if (colorIndex < 0)
                {
                    colorIndex = colors.Count - 1;
                }
                changingColor = true;
            }

            if (changingColor)
            {
                //set layer to player while changing color
                //gameObject.layer = LayerMask.NameToLayer("Player");
                nextColor = colors[colorIndex];
                OnColorShift?.Invoke(this, (prevColor, colors[colorIndex]));
            }
        }
        else if (sr.material.color == nextColor)
        {
            changingColor = false;
            gameObject.layer = LayerMask.NameToLayer(layers[colorIndex]);
            prevColor = nextColor;

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("World"))
        {
            if (feetCollider.IsTouching(collision))
            {
                grounded = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Bounds"))
        {
            Death();
        }
    }

    private void Death()
    {
        UnsubscribeAllEvents();
        hp = maxHP;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UnsubscribeAllEvents()
    {
        foreach(EventHandler<(Color prevColor, Color nextColor)> d in OnColorShift.GetInvocationList())
        {
            OnColorShift -= d;
        }
    }
}
