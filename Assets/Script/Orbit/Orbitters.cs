using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbitters : MonoBehaviour
{
    public int sphereCount = 500;
    public int maxRadius = 200;
    public GameObject[] spheres;
    public Material[] mats;
    public Material trailMat;

    void Awake()
    {
        spheres = new GameObject[sphereCount];
    }

    void Start()
    {
       spheres = CreateSpheres(sphereCount, maxRadius);
    }

    public GameObject[] CreateSpheres(int count, int radius)
    {
        return spheres;
    }
}
