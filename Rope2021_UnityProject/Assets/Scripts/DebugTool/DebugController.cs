using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugController : MonoBehaviour
{
    //References
    public Canvas consoleCanvas;
    public SaveSystem saveSystem;
    public TMP_Text text;
    public TMP_InputField inputField;
    public NewProtagonist player;
    public PlayerManager playerManager;

    public static DebugCommand<string> PRINT;
    public static DebugCommand<string> SAVE;
    public static DebugCommand<string> LOAD;
    public static DebugCommand HELP;
    public static DebugCommand SET_CHECKPOINT;

    public List<object> commandList;

    bool _showConsole;

    void Awake()
    {
        _showConsole = consoleCanvas.gameObject.activeSelf;

        PRINT = new DebugCommand<string>("print", "Print <text> on console.", "print <text>", (x) =>
        {
            PrintOnConsole(x);
        });

        SAVE = new DebugCommand<string>("save", "Save the level as <fileName>.", "save <fileName>", (x) =>
        {
            bool success;
            success = saveSystem.Save(x);
            if (success)
                PrintOnConsole("Level saved as <" + x + ">.");
            else
                PrintOnConsole("ERROR while saving.");
        });

        LOAD = new DebugCommand<string>("load", "Load the <fileName> level.", "load <fileName>", (x) =>
        {
            bool success;
            success = saveSystem.Load(x);
            if (success)
                PrintOnConsole("Loaded <" + x + ">.");
            else
                PrintOnConsole("ERROR while loading. Couldn't find <" + x + ">.");
        });

        SET_CHECKPOINT = new DebugCommand("set_checkpoint", "Set checkpoint at your current position.", "set_checkpoint", () =>
        {
            SetCheckpoint();
        });

        HELP = new DebugCommand("help", "Show all commands.", "help", () =>
        {
            PrintHelpText();
        });


        commandList = new List<object>
        {
            SAVE,
            LOAD,
            PRINT,
            SET_CHECKPOINT,
            HELP
        };
    }

    private void SetCheckpoint()
    {
        playerManager.SetCheckpointPosition(player.transform.position);
        PrintOnConsole("Checkpoint succesfully set at current position.");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _showConsole = !_showConsole;
            ShowConsole(_showConsole);
        }
    }

    void PrintHelpText()
    {
        string listOfCommands = "";

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase command = commandList[i] as DebugCommandBase;
            string label = $"{command.CommandFormat} - {command.CommandDescription}";
            listOfCommands = listOfCommands + label + "\n";
        }

        PrintOnConsole(listOfCommands);
    }


    void PrintOnConsole(string text)
    {
        string newText = text + "\n" + this.text.text;

        this.text.text = newText;
    }

    void ShowConsole(bool show)
    {
        consoleCanvas.gameObject.SetActive(show);

        if (show == true)
        {
            text.text = "";
            PrintHelpText();

            inputField.ActivateInputField();
        }        
    }

    public void OnConfirmInput(string input)
    {
        HandleInput(input);
    }

    void HandleInput(string input)
    {
        string[] properties = input.Split(' ');

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (input.Contains(commandBase.CommandId))
            {
                if (commandList[i] as DebugCommand != null)
                {
                    //Cast to this type and invoke the command
                    (commandList[i] as DebugCommand).Invoke();
                }
                else if (commandList[i] as DebugCommand<string> != null)
                {
                    string arg="";
                    for (int j=1;j<properties.Length;j++)
                    {
                        if (j != 1)
                            arg += ' '; 
                        arg += properties[j];
                    }
                    (commandList[i] as DebugCommand<string>).Invoke(arg);
                }
            }
        }
    }
}
