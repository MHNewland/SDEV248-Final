using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class ColorShiftSystem : MonoBehaviour
{
    public static float shiftSpeed { get; private set; }
    float t;

    [SerializeField]
    Renderer csRenderer;

    // Start is called before the first frame update
    void Start()
    {
        shiftSpeed = 4f;
        t = 0;
    }


    public IEnumerator ChangeColor(Color prevColor, Color nextColor, float shiftSpeed = 4f)
    {
        if (csRenderer == null)
        {
            yield return null;
        }else  if (gameObject.TryGetComponent(out SpriteRenderer sr))
        {
            csRenderer = sr;
            Convert.ChangeType(csRenderer, typeof(SpriteRenderer));
        }
        else if (gameObject.TryGetComponent(out TilemapRenderer tm))
        {
            csRenderer = tm;
            Convert.ChangeType(csRenderer, typeof(TilemapRenderer));
        }
        else
        {
            Debug.Log("errored");
            yield return null;
        }

        t = 0;
        while (csRenderer.material.color != nextColor)
        {

            t += Time.deltaTime * shiftSpeed;
            csRenderer.material.color = Color.Lerp(prevColor, nextColor, t);
            yield return null;
        }
    }

    public IEnumerator ChangeOpacity(Renderer renderer, int opacity)
    {

        Color layerColor = renderer.material.color;
        Color newColor = new Color(layerColor.r, layerColor.g, layerColor.b, opacity);

        while (renderer.material.color.a != opacity)
        {

            t += Time.deltaTime * shiftSpeed;
            renderer.material.color = Color.Lerp(layerColor, newColor, t);
            yield return null;
        }
    }

    public Renderer GetCSRenderer() => csRenderer;
    
}
