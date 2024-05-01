using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EnemyClass : MonoBehaviour
{
    [SerializeField]
    protected int hp;

    protected int maxHP;

    [SerializeField]
    protected Image healthbar;
    protected Canvas canvas;

    protected Rigidbody2D rb;
    protected SpriteRenderer ecRenderer;

    protected LayerMask currentLayer;

    protected float attackCooldown;
    protected float cooldownTimer;

    [SerializeField]
    protected float range;

    public abstract void Attack();

    public abstract void Move();


    void Start()
    {
        PlayerController.Instance.OnColorShift += PlayerController_ChangeColor;
        ecRenderer = GetComponent<SpriteRenderer>();
        currentLayer = gameObject.layer;
        canvas = GetComponentInChildren<Canvas>();
        Debug.Log(canvas);
        Image[] images = GetComponentsInChildren<Image>();
        foreach(Image i in images)
        {
            if (i.name == "health") healthbar = i;
        }
    }

    void PlayerController_ChangeColor(object sender, (Color prevColor, Color nextColor) e)
    {

        if(LayerMask.LayerToName(currentLayer) == PlayerController.Instance.layers[PlayerController.Instance.colorIndex])
        {
            StartCoroutine(GetComponent<ColorShiftSystem>().ChangeOpacity(ecRenderer, 1));
        }
        else
        {
            StartCoroutine(GetComponent<ColorShiftSystem>().ChangeOpacity(ecRenderer, 0));
        }
    }

    protected void UpdateHPBar()
    {
        canvas.gameObject.SetActive(ecRenderer.material.color.a != 0);
        if (hp<=0)
        {
            Destroy(gameObject);
        }
        healthbar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (hp / (float)maxHP));
    }
}
