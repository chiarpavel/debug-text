using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  DebugText needs to be attached to a Text GameObject in the scene
/// </summary>
public class DebugText : MonoBehaviour
{
    /// <summary>
    /// The ScrollRect used to manage the individual text message objects.
    /// </summary>
    public ScrollRect containerScrollRect;
    /// <summary>
    /// The container of the individual text message objects.
    /// </summary>
    public Transform container;
    /// <summary>
    /// The font of the text.
    /// </summary>
    public Font font;
    /// <summary>
    /// The color of the debug text.
    /// </summary>
    public Color debugColor = Color.white;
    /// <summary>
    /// The color of the warning text.
    /// </summary>
    public Color warningColor = Color.yellow;
    /// <summary>
    /// The color of the error text.
    /// </summary>
    public Color errorColor = Color.red;
    /// <summary>
    /// The number of messages to remember.
    /// </summary>
    public int maxEntryCount = 20;
    /// <summary>
    /// Whether or not to stack repeating messages on a single line.
    /// </summary>
    public bool stackRepeatingMessages = true;
    /// <summary>
    /// Whether or not to scroll to the bottom when a new message is logged.
    /// </summary>
    public bool scrollToBottom = true;

    static DebugText instance;

    private Queue<GameObject> entries = new Queue<GameObject>();
    private Text lastEntryText;
    private string lastMessageString = "";
    private int lastMessageCount = 1;

    void Awake()
    {
        instance = this;
        font = font ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private GameObject CreateEntry(string entryText, Color entryColor)
    {
        GameObject newEntry = new GameObject("LogEntry", typeof(RectTransform), typeof(Text));
        Text text = newEntry.GetComponent<Text>();
        text.text = entryText;
        text.font = font;
        text.color = entryColor;
        newEntry.transform.SetParent(container, false);
        lastEntryText = text;

        return newEntry;
    }

    private void _Log(object message, Color entryColor)
    {
        string messageString = message.ToString();

        if (stackRepeatingMessages && messageString == lastMessageString)
        {
            lastEntryText.text = "(" + ++lastMessageCount + ") " + messageString;
        }
        else
        {
            GameObject newEntry = CreateEntry(messageString, entryColor);
            entries.Enqueue(newEntry);
            if (entries.Count > maxEntryCount)
            {
                Destroy(entries.Dequeue());
            }

            lastMessageString = messageString;
            lastMessageCount = 1;
        }

        if (scrollToBottom)
        {
            StartCoroutine(ScrollToBottom());
        }
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        containerScrollRect.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// Use like Debug.Log
    /// </summary>
    /// <param name="message">String or object to be converted to string representation and displayed in the UI Text</param>
    public static void Log(object message)
    {
        if (instance != null)
            instance._Log(message, instance.debugColor);
    }

    /// <summary>
    /// Use like Debug.LogWarning
    /// </summary>
    /// <param name="message">String or object to be converted to string representation and displayed in the UI Text</param>
    public static void LogWarning(object message)
    {
        if (instance != null)
            instance._Log(message, instance.warningColor);
    }

    /// <summary>
    /// Use like Debug.LogError
    /// </summary>
    /// <param name="message">String or object to be converted to string representation and displayed in the UI Text</param>
    public static void LogError(object message)
    {
        if (instance != null)
            instance._Log(message, instance.errorColor);
    }

    /// <summary>
    /// Clear the current message history.
    /// </summary>
    public void Clear()
    {
        foreach (var entry in entries)
        {
            Destroy(entry);
        }
        entries.Clear();
        lastEntryText = null;
        lastMessageString = "";
        lastMessageCount = 1;
    }
}
