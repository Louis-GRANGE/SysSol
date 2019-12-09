using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbitters : MonoBehaviour
{
    public int nPlanet = 100;
    public int maxRadius = 200;
    public List<GameObject> planets;
    public Material[] mats;
    public Material trailMat;

    private void Awake()
    {
        planets = new List<GameObject>();
    }

    private void Start()
    {
        for (int i = 0; i < nPlanet; ++i)
        {
            
        }
    }

    public void CreatePlanet()
    {
        
    }
    
}
