using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGR_SolarSystem : MonoBehaviour
{
    public static MGR_SolarSystem Instance { get; private set; }

    private GravityCenter _sun;
    public GravityCenter Sun
    {
        get { return _sun;}
        private set { _sun = value; }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            throw new Exception("[" + GetType().Name + "] Trying to instanciate a second instance");
    }
}
