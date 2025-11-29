using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class TerminalMenuManager : MonoBehaviour
{
    public string loadingSceneName;
    public TextMeshProUGUI terminalText;

    private AudioSource audioSource;
    private string displayText = "";
    private string userInput = "";
    private bool waitingForInput = false;
    private bool cursorVisible = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        Keyboard.current.onTextInput += OnTextInput;
        StartCoroutine(RunTerminalSequence());
    }

    void OnDestroy()
    {
        if (Keyboard.current != null)
            Keyboard.current.onTextInput -= OnTextInput;
    }

    void Update()
    {
        if (!waitingForInput) return;

        // Handle Enter and Backspace with new Input System
        if (Keyboard.current.enterKey.wasPressedThisFrame)
            ProcessChoice(userInput.Trim().ToLower());
        else if (Keyboard.current.backspaceKey.wasPressedThisFrame && userInput.Length > 0)
        {
            userInput = userInput.Substring(0, userInput.Length - 1);
            UpdateDisplay();
        }
    }

    void OnTextInput(char c)
    {
        if (!waitingForInput) return;
        if (!char.IsControl(c))
        {
            userInput += c;
            UpdateDisplay();
        }
    }

    IEnumerator RunTerminalSequence()
    {
        // Step 1: Show prompt
        displayText = "YourMaker@WeatheringTheStorms % ";
        UpdateDisplay();
        yield return new WaitForSeconds(1.5f);

        // Step 2: Typewriter effect for command
        string command = "run GPT_Simulator_3000.sh";
        if (audioSource != null) { audioSource.loop = true; audioSource.Play(); }
        foreach (char c in command)
        {
            displayText += c;
            UpdateDisplay();
            yield return new WaitForSeconds(Random.Range(0.03f, 0.07f));
        }
        if (audioSource != null) audioSource.Stop();

        // Step 3: Show menu
        yield return new WaitForSeconds(0.5f);
        displayText += "\nAre you sure you want to save the world?\n[y, n, -h]\n> ";
        UpdateDisplay();

        // Step 4: Enable input with blinking cursor
        waitingForInput = true;
        StartCoroutine(BlinkCursor());
    }

    IEnumerator BlinkCursor()
    {
        while (waitingForInput)
        {
            cursorVisible = !cursorVisible;
            UpdateDisplay();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void UpdateDisplay()
    {
        terminalText.text = displayText + userInput + (cursorVisible && waitingForInput ? "█" : "");
    }

    void ProcessChoice(string choice)
    {
        // =================================================================
        // TODO: CONTINUE YOUR CODE HERE
        // choice = user input (lowercase, trimmed). Examples: "y", "n", "-h"
        // =================================================================

        switch (choice)
        {
            case "y":
            case "-y":
            case "yes":
                SceneManager.LoadScene(loadingSceneName);
                break;

            case "n":
            case "-n":
            case "no":
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
                break;

            case "-h":
            case "--help":
            case "help":
            case "h":
                displayText += userInput + "\n  y - Start game\n  n - Exit game\n  -h - Display this text again\n  --easy - Start game in easy mode (default)\n  --medium - Start game in medium mode\n  --hard - Start game in hard mode\n  --credits - Display credits\n> ";
                userInput = "";
                break;

            default:
                displayText += userInput + $"\n\nError: Unknown command '{choice}'\n\n> ";
                userInput = "";
                break;
        }
    }
}
