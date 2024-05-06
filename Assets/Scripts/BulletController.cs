using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    
    float speed;
    ColorShiftSystem css;
    SpriteRenderer sprite;

    [SerializeField]
    bool checkCollision;

    public GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        speed = 5;
        gameObject.layer = transform.parent.gameObject.layer;
        transform.parent = null;
        css = GetComponent<ColorShiftSystem>();
        sprite = GetComponent<SpriteRenderer>();
        PlayerController.Instance.OnColorShift += PlayerController_ChangeColor;
        sprite.material.color = PlayerController.Instance.colors[PlayerController.Instance.layers.IndexOf(LayerMask.LayerToName(parent.gameObject.layer))];
        PlayerController_ChangeColor(this, (sprite.color, sprite.color));
        checkCollision = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime,Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (checkCollision && !collision.gameObject.CompareTag(parent.tag))
        {
            switch (collision.tag)
            {
                case "Player":
                    if (PlayerController.Instance.gameObject.layer == gameObject.layer) PlayerController.Instance.TakeDamage();
                    goto case "World";
                case "Enemy":
                    collision.gameObject.TryGetComponent(out ProwlerController pc);
                    collision.gameObject.TryGetComponent(out SniperController sniper);
                    if (pc != null) pc.TakeDamage();
                    if(sniper!= null) sniper.TakeDamage();
                    goto case "World";
                case "World":
                    PlayerController.Instance.OnColorShift -= PlayerController_ChangeColor;
                    Destroy(gameObject);
                    break;
                default:
                    break;
            }
        }
    }

    private void PlayerController_ChangeColor(object sender, (Color prevColor, Color nextColor) e)
    {

        if (LayerMask.LayerToName(gameObject.layer) == PlayerController.Instance.layers[PlayerController.Instance.colorIndex])
        {
            StartCoroutine(css.ChangeOpacity(sprite, 1));
        }
        else
        {
            StartCoroutine(css.ChangeOpacity(sprite, 0));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        checkCollision = true;
    }

}
