using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityCenter : MonoBehaviour
{
    public static GravityCenter Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            throw new Exception("[" + GetType().Name + "] Trying to instanciate a second instance");
    }
}
