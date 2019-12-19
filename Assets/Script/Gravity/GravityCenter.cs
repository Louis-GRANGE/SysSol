using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityCenter : Attractor
{
    public static GravityCenter Instance { get; private set; }

    private float mass = 1e5f;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            throw new Exception("[" + GetType().Name + "] Trying to instanciate a second instance");
        
        base.Awake();

        rigidbody.mass = mass;
    }

    private void OnTriggerEnter(Collider other)
    {
        ListAstre ListeAstreDontDestroyOnLoad = GameObject.Find("DontDestroyOnLoad").GetComponent<ListAstre>();
        
        ListeAstreDontDestroyOnLoad.RemoveAstre(other.gameObject);
        
        // if (other.transform.parent)
        //     Destroy(other.transform.parent.gameObject);
        // else
        //     Destroy(other.gameObject);
    }
}
