using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualComputer : MonoBehaviour
{
    object[] hardware;
    object[] softwareModules;
    object[] hardwareModules;

    VirtualOS os;

    void Awake()
    {
        SetProgram(currentProgram);
    }

    void Start()
    {
        os = GetComponent<VirtualOS>();
        if (os == null)
            os = gameObject.AddComponent<VirtualOS>();

        
    }

    void startRunning()
    {
        string code = ((VirtualScriptEditor)editor).code;
        if (useTestCodeForTasks)
        {
            code = currentTask.testCode.text;
            Debug.Log("Running task with test code");
        }
        currentTask.StartTask(code);
        ((VirtualScriptEditor)editor).SetTaskInfo(currentTask.taskInfo);

        SetProgram(Program.Console);
    }

    void Update()
    {
        this.HandleInput();

        if (needsUpdate && !Application.isPlaying)
        {
            needsUpdate = false;
            SetProgram(currentProgram);
        }
    }

    public object[] Hardware{ get; }
    public object[] SoftwareModules { get; }
    public object[] HardwareModules { get; }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public bool useTestCodeForTasks;

    public Camera viewCam;
    public VirtualProgram taskSelect;
    public VirtualProgram editor;
    public VirtualProgram console;
    public VirtualProgram instructions;

    public enum Program { None, TaskSelect, Editor, Console, Instructions }
    public Program currentProgram;

    bool needsUpdate;

    Task currentTask;

    //TODO: put modules classes on startup inhere
    public object[] availableModules;
    
    void RunTask()
    {
        string code = ((VirtualScriptEditor)editor).code;
        if (useTestCodeForTasks)
        {
            code = currentTask.testCode.text;
            Debug.Log("Running task with test code");
        }
        currentTask.StartTask(code);
        ((VirtualScriptEditor)editor).SetTaskInfo(currentTask.taskInfo);

        SetProgram(Program.Console);
    }

    void StopTask()
    {
        if (currentTask != null)
        {
            currentTask.StopTask();
        }
    }

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
            if (currentProgram == Program.Editor)
            {
                startRunning();
            }
        }

        // Open task menu
        if (Input.GetKeyDown(KeyCode.T) && ControlOperatorDown())
        {
            if (currentProgram != Program.TaskSelect)
            {
                StopTask();
                SetProgram(Program.TaskSelect);
            }
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

    public void RegisterTask(Task task)
    {
        this.currentTask = task;
        this.CopyTargetCamera();
        this.RunTask();
    }

    public void SetProgram(Program program)
    {
        currentProgram = program;
        if (taskSelect != null)
        {
            taskSelect.SetActive(currentProgram == Program.TaskSelect);
            editor.SetActive(currentProgram == Program.Editor);
            console.SetActive(currentProgram == Program.Console);
            instructions.SetActive(currentProgram == Program.Instructions);
        }
    }

    public void CopyTargetCamera()
    {
        if (FindObjectOfType<CameraTarget>())
        {
            Camera targetCam = FindObjectOfType<CameraTarget>().GetComponent<Camera>();
            // Copy settings
            viewCam.transform.position = targetCam.transform.position;
            viewCam.transform.rotation = targetCam.transform.rotation;
            viewCam.orthographic = targetCam.orthographic;
            viewCam.orthographicSize = targetCam.orthographicSize;
            viewCam.fieldOfView = targetCam.fieldOfView;
            // Disable
            targetCam.gameObject.SetActive(false);
        }
    }
}
