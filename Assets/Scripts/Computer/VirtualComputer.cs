using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualComputer : MonoBehaviour
{
    object[] hardware;
    object[] softwareModules;
    object[] hardwareModules;

    //TODO: save the code and compiled code on the computer and show the copy in the scripteditors
    string codeSavedOnComputer;


    VirtualScriptEditor editor;
    VirtualCompiler compiler;

    Task compiledUserCode;
    GameObject os;

    void Awake()
    {
        _GameMaster._GM.AddComputer(this);
    }

    void Start()
    {
        if (os == null)
            Debug.Log("Could not find a VirtualOS object!"); //TODO: instantiante a new one

        editor = FindObjectOfType<VirtualScriptEditor>();
    }

    void startRunning()
    {
        string code = editor.code;
        compiler = new VirtualCompiler(code);
        compiler.vComputer = this;
        compiler.Run();
    }

    void Update()
    {
        this.HandleInput();

        if (needsUpdate && !Application.isPlaying)
        {
            needsUpdate = false;
            //SetProgram(currentProgram);
        }
    }

    public object[] Hardware{ get; }
    public object[] SoftwareModules { get; }
    public object[] HardwareModules { get; }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public bool useTestCodeForTasks;

    bool needsUpdate;

    //TODO: put modules classes on startup inhere
    public object[] availableModules;

    void HandleInput()
    {
        // Open code editor
        /*if (Input.GetKeyDown(KeyCode.E) && ControlOperatorDown())
        {
            if (currentProgram == Program.Console)
            {
                StopTask();
                SetProgram(Program.Editor);
            }
        }*/

        // Run code
        if (Input.GetKeyDown(KeyCode.R) && ControlOperatorDown())
        {
            startRunning();
        }
    }

    bool ControlOperatorDown()
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        //bool cmd = Input.GetKey (KeyCode.LeftCommand) || Input.GetKey (KeyCode.RightCommand);
        return ctrl;// || cmd;
    }

    void OnValidate()
    {
        needsUpdate = true;
    }

    public bool Active
    {
        get
        {
            return this.gameObject.activeSelf;
        }
    }
}
