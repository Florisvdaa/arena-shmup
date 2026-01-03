using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public static Logger Instance {  get; private set; }

    [Header("Settings")]
    [SerializeField] private bool showLogs = true;              /// Enable or disable log printing
    [SerializeField] private string defaultPrefix = "LOG";      /// Default lable prefix for logs
    [SerializeField] private Color logColor = Color.white;      /// Color used for the prefix in the console

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else Instance = this;
    }

    public void Log(Color c, object message, Object sender = null, string customPrefix = null)
    {
        if (!showLogs)
        {
            return;
        }

        // Convert the Unity color to an HTML color code, used for coloring console output
        string hexColor = ColorUtility.ToHtmlStringRGB(c);

        // Use the custom prefix if it's not null, otherwise use the default prefix.
        string prefix = customPrefix ?? defaultPrefix;

        // Format the final message with a colored prefix
        string finalMessage = $"<color=#{hexColor}>[{prefix}]</color> {message}";

        // Print the message. sender lets you click the log and focus the object in the Inspector
        Debug.Log(finalMessage, sender);
    }
}
