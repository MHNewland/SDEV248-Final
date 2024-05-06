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
    [SerializeField]
    protected Canvas canvas;

    protected Rigidbody2D rb;
    protected SpriteRenderer ecRenderer;

    protected LayerMask currentLayer;

    protected float attackCooldown;
    protected float cooldownTimer;

    [SerializeField]
    protected float range;

    ColorShiftSystem css;

    public abstract void Attack();

    public abstract void Move();


    void Start()
    {
        ecRenderer = GetComponent<SpriteRenderer>();
        currentLayer = gameObject.layer;
        canvas = GetComponentInChildren<Canvas>();
        Image[] images = GetComponentsInChildren<Image>();
        css = GetComponent<ColorShiftSystem>();
        foreach (Image i in images)
        {
            if (i.name == "health") healthbar = i;
        }
        PlayerController.Instance.OnColorShift += PlayerController_ChangeColor;
        ecRenderer.material.color = PlayerController.Instance.colors[PlayerController.Instance.layers.IndexOf(LayerMask.LayerToName(gameObject.layer))];
    }



    void PlayerController_ChangeColor(object sender, (Color prevColor, Color nextColor) e)
    {
        if(LayerMask.LayerToName(currentLayer) == PlayerController.Instance.layers[PlayerController.Instance.colorIndex])
        {
            StartCoroutine(css.ChangeOpacity(ecRenderer, 1));
        }
        else
        {
            StartCoroutine(css.ChangeOpacity(ecRenderer, 0));
        }
    }

    protected void UpdateHPBar()
    {
        canvas.gameObject.SetActive(ecRenderer.material.color.a != 0);
        if (hp<=0)
        {
            PlayerController.Instance.OnColorShift-= PlayerController_ChangeColor;
            Destroy(gameObject);
        }
        healthbar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (hp / (float)maxHP));
    }
}
