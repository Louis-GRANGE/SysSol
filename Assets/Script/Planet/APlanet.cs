using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APlanet : MonoBehaviour
{
    protected Attractor _attractor;
    
    private void Awake()
    {
        _attractor = gameObject.AddComponent<Attractor>();
    }


    public void SetGravityCenter(Attractor gravityCenter)
    {
        _attractor.orbitReference = gravityCenter;
    }
}
