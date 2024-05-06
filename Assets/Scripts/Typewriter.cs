using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

[System.Serializable]
public class TypewriterMessage
{
    private float timer = 0;
    private int charIndex = 0;
    private float timePerChar = 0.05f;

    [SerializeField]
    public string currentMessage { get; private set; }
    public string displayMessage { get; private set; }

    private Action onActionCallback = null;

    public TypewriterMessage(string message, Action callback = null)
    {
        onActionCallback = callback;
        currentMessage = message;
    }

    public void CallBack()
    {
        if (onActionCallback != null) onActionCallback();
    }

    public string GetFullMessageAndCallback()
    {
        if (onActionCallback != null) onActionCallback();
        return currentMessage;
    }

    public void Update()
    {
        if (string.IsNullOrEmpty(currentMessage)) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer += timePerChar;
            charIndex++;

            displayMessage = currentMessage.Substring(0, charIndex);

            if (charIndex >= currentMessage.Length)
            {
                CallBack();
                currentMessage = null;
            }
        }
    }

    public bool IsActive()
    {
        if (string.IsNullOrEmpty(currentMessage)) return false;
        return charIndex < currentMessage.Length;
    }
}
public class Typewriter : MonoBehaviour
{
    public Text dialogueBox;

    private static Typewriter Instance;

    [SerializeField]
    private List<TypewriterMessage> messages = new List<TypewriterMessage>();

    private TypewriterMessage currentMessage = null;

    private int messageIndex = 0;

    public static void Add(string message, Action callBack = null)
    {
        TypewriterMessage typeMessage = new TypewriterMessage(message, callBack);
        Instance.messages.Add(typeMessage);
    }

    public static void Activate()
    {
        Instance.currentMessage = Instance.messages[0];

    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (messages.Count > 0 && currentMessage != null)
        {
            currentMessage.Update();
            dialogueBox.text = currentMessage.displayMessage;
        }
    }

    public void WriteNextMessageInQueue()
    {

        if (currentMessage != null && currentMessage.IsActive())
        {
            dialogueBox.text = currentMessage.GetFullMessageAndCallback();
            currentMessage = null;
            return;
        }

        messageIndex++;

        if (messageIndex >= messages.Count)
        {
            currentMessage = null;
            dialogueBox.text = "";
            messages.Clear();
            messageIndex = 0;
            return;
        }

        currentMessage = messages[messageIndex];
        Debug.Log(currentMessage.currentMessage);
    }

    public void ActivatePlayer()
    {
        PlayerController.Instance.canChangeColors = true;
        PlayerController.Instance.canFire = true;
    }

}