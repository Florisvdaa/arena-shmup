using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheatLog : MonoBehaviour
{
    private DebugInputActions debugInputActions;
    private bool showConsole = false;
    private string input;
    private Player player;

    public static DebugCommand INCREASE_SPEED;

    public List<object> commandList;

    private void Awake()
    {
        debugInputActions = new DebugInputActions();

        INCREASE_SPEED = new DebugCommand("speed_up", "Increases the speed of the player", "speed_up", () =>
        {
            player.IncreaseMovementSpeed(10f);
        });
    }

    private void Start()
    {
        debugInputActions.Debug.ToggleDebugConsole.performed += ctx => OnToggleDebug();
        debugInputActions.Debug.Return.performed += ctx => OnReturn();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    public void OnToggleDebug()
    {
        showConsole = !showConsole;
        player.ToggleMovement();
    }

    public void OnReturn()
    {
        if (showConsole)
        {
            OnToggleDebug();

            HandleInput();
            input = "";
        }
    }

    private void OnGUI()
    {
        if (!showConsole)
            return;

        Debug.Log("Console active");

        float y = 0f;

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
    }

    private void HandleInput()
    {
        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (input.Contains(commandBase.CommandId))
            {
                (commandList[i] as DebugCommand).Invoke();
            }
        }
    }

    private void OnEnable()
    {
        debugInputActions.Enable();
    }
}
