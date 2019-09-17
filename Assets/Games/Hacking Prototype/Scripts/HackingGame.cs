using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingGame : MonoBehaviour
{
    public TextAsset testCodeInitial;
    public TextAsset testCodeSolution;
    public bool useTestSolution;
    [TextArea(5, 20)]
    public string solution;

    HackingTurret turret;
    HackingPlayer player;
    HackingEnemy[] guards;
    bool active;
    public GameObject computer;
    VirtualScriptEditor editor;

    public GameObject levelPrefab;
    public HackingPlayer playerPrefab;
    public Transform spawnPos;
    GameObject level;

    void Start()
    {
        turret = FindObjectOfType<HackingTurret>();
        computer.SetActive(true);
        editor = FindObjectOfType<VirtualScriptEditor>();
        //editor.code = "setAim(intruderPosX, intruderPosY)";
        //editor.code = "setAim(guardPosX[0], guardPosY[0])";
        //editor.code = solution;
        editor.code = testCodeSolution.text;
        computer.SetActive(false);
        Restart();
    }

    void Restart()
    {
        turret.active = false;
        turret.Reset();
        active = false;
        if (level != null)
        {
            Destroy(level);
        }

        level = Instantiate(levelPrefab);
        if (player != null)
        {
            Destroy(player.gameObject);
        }
        player = Instantiate(playerPrefab, spawnPos.position, Quaternion.identity);

        var f = FindObjectsOfType<FragmentSpawner>();
        for (int i = f.Length - 1; i >= 0; i--)
        {
            Destroy(f[i].gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            computer.SetActive(true);
            editor.SetActive(true);
            player.gameObject.SetActive(false);
        }
        if (computer.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.C) && (Input.GetKey(KeyCode.LeftControl)))//|| Input.GetKey (KeyCode.LeftCommand)))
            {
                computer.SetActive(false);
                player.gameObject.SetActive(true);
            }
        }
        guards = FindObjectsOfType<HackingEnemy>();
        if (!active && player.transform.position.z > -3)
        {
            active = true;
            turret.active = true;
            foreach (var e in guards)
            {
                e.active = true;
            }
        }

        if (active && player != null)
        {
            string code = (useTestSolution) ? testCodeSolution.text : testCodeInitial.text;
            code = editor.code;
            VirtualCompiler compiler = new VirtualCompiler(code);
            VirtualComputer cc = computer.GetComponent<VirtualComputer>();
            cc.availableModules[0] = turret;
            compiler.vComputer = cc;

            float[] guardSpeed = new float[guards.Length];
            float[] guardPosX = new float[guards.Length];
            float[] guardPosY = new float[guards.Length];
            float[] guardDirX = new float[guards.Length];
            float[] guardDirY = new float[guards.Length];

            for (int i = 0; i < guards.Length; i++)
            {
                guardSpeed[i] = guards[i].speed;
                guardPosX[i] = guards[i].transform.position.x;
                guardPosY[i] = guards[i].transform.position.z;
                guardDirX[i] = guards[i].transform.forward.x;
                guardDirY[i] = guards[i].transform.forward.y;
            }
            compiler.AddInputArray(nameof(guardSpeed), guardSpeed);
            compiler.AddInputArray(nameof(guardPosX), guardPosX);
            compiler.AddInputArray(nameof(guardPosY), guardPosY);
            compiler.AddInputArray(nameof(guardDirX), guardDirX);
            compiler.AddInputArray(nameof(guardDirY), guardDirY);
            compiler.AddInput("numGuards", guards.Length);

            compiler.AddInput("intruderPosX", player.transform.position.x);
            compiler.AddInput("intruderPosY", player.transform.position.z);
            compiler.AddInput("intruderDirX", player.dir.x);
            compiler.AddInput("intruderDirY", player.dir.y);
            compiler.AddInput("intruderSpeed", player.speed);

            compiler.AddInput("missileSpeed", turret.missilePrefab.speed);
            compiler.AddInput("turretPosX", turret.muzzle.position.x);
            compiler.AddInput("turretPosY", turret.muzzle.position.z);
            compiler.AddInput("turretHeight", turret.muzzle.position.y);

            compiler.AddOutputFunction("setAim");
            compiler.AddOutputFunction("PowerUp");
            compiler.AddOutputFunction("setInactive");

            List<VirtualFunction> output = compiler.Run();
            ExecuteFunctions(output);

            
            //OLD
            /*if (output.Count > 0)
            {
                float aimX = output[1].values[0];
                float aimY = output[1].values[1];
                turret.setAim(aimX, aimY);
            }*/
        }
    }

    void ExecuteFunctions(List<VirtualFunction> outputs)
    {
        if (outputs.Count > 0)
        {
            foreach (VirtualFunction vFunc in outputs)
            {
                object[] mParams = new object[vFunc.values.Count];
                Array.Copy(vFunc.values.ToArray(), mParams, vFunc.values.Count);
                if (vFunc.delFunc != null)
                {
                    if(vFunc.values.Count > 1)
                    {
                        //TODO: return the return value if not null
                        var xxx = vFunc.delFunc.Invoke(turret, mParams);
                        //Debug.Log(xxx); 
                    }
                    else
                    {
                        vFunc.delFunc.Invoke(turret, null);
                    }
                }
                //Array.Clear(mParams, 0, mParams.Length);
            }
        }
    }
}