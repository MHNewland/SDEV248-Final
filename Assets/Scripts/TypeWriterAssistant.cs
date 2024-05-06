using System.Collections.Generic;
using UnityEngine;

public class TypeWriterAssistant : MonoBehaviour
{

    [SerializeField]
    List<string> messages = new List<string>();
    private void Awake()
    {
        foreach(string s in messages)
        {
            Typewriter.Add(s);
        }
        Typewriter.Activate();
    }
}