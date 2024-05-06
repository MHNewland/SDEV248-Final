using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class WorldColorShift : MonoBehaviour
{

    string layerName;

    [SerializeField]
    List<GameObject> colorTiles;

    private void Start()
    {
        PlayerController.Instance.OnColorShift += PlayerController_ChangeColor;
    }

    void PlayerController_ChangeColor(object sender, (Color prevColor, Color nextColor) e)
    {
        StartCoroutine(gameObject.GetComponent<ColorShiftSystem>().ChangeColor(e.prevColor, e.nextColor));

        //expects to receive "Blue", "Red", or "Yellow"
        layerName = PlayerController.Instance.layers[PlayerController.Instance.colorIndex];

        foreach (GameObject tile in colorTiles)
        {
            if (tile.TryGetComponent(out TilemapRenderer renderer))
            {
                {
                    if (LayerMask.LayerToName(tile.layer) == layerName)
                    {

                        StartCoroutine(GetComponent<ColorShiftSystem>().ChangeOpacity(renderer, 1));
                    }
                    else
                    {
                        StartCoroutine(GetComponent<ColorShiftSystem>().ChangeOpacity(renderer, 0));
                    }
                }
            }

        }
    }


    

  
    
}
