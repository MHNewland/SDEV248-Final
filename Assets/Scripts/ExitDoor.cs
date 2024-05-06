using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ExitDoor : MonoBehaviour
{
    [SerializeField]
    SceneAsset nextScene;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")){
            PlayerController.Instance.UnsubscribeAllEvents();
            SceneManager.LoadScene(nextScene.name);
        }
    }
}
