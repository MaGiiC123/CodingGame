using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _GameMaster : MonoBehaviour
{
    List<VirtualComputer> Virtualcomputers = new List<VirtualComputer>();

    public static _GameMaster _GM;

    private void Awake()
    {
        if (_GM == null)
        {
            DontDestroyOnLoad(gameObject);
            _GM = this;
        }
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void AddComputer(VirtualComputer _virtualcomputer)
    {
        Virtualcomputers.Add(_virtualcomputer);
    }

    public void RemoveComputer(VirtualComputer _virtualcomputer)
    {
        Virtualcomputers.Remove(_virtualcomputer);
    }
}
