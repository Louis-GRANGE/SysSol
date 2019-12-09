using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGR_SolarSystem : MonoBehaviour
{
    public static MGR_SolarSystem Instance { get; private set; }

    private Attractor _sun;

    public Attractor Sun
    {
        get { return _sun; }
        private set { _sun = value; }
    }

    public List<Planet> Planets;

    private int _size;

    public int Size
    {
        get { return _size; }
        private set { _size = value; }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            throw new Exception("[" + GetType().Name + "] Trying to instanciate a second instance");

        Planets = new List<Planet>();
    }

    private void Start()
    {
        // Instancier les planètes ici

        foreach (var planet in Planets)
        {
//            planet.SetGravityCenter(_sun);
        }
    }
}
