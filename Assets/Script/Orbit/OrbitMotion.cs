﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitMotion : MonoBehaviour
{
    public Transform orbitingObject;
    public Ellipse orbitPath;

    [Range(0f,1f)]
    public float orbitProgress = 0f;
    public float orbitPeriod = 3f;
    public bool orbitActive = true;


    // Start is called before the first frame update
    void Start()
    {
        if(orbitingObject == null)
        {
            orbitActive = false;
            return;
        }

        //On set la position de l'objet en orbit et on demmare l'orbit
        setOrbitingObjectPosition();
        StartCoroutine(AnimateOrbit());
    }

    void setOrbitingObjectPosition()
    {
        Vector2 orbitPos = orbitPath.Evaluate(orbitProgress);
        orbitingObject.localPosition = new Vector3(orbitPos.x, 0, orbitPos.y);
    }

    IEnumerator AnimateOrbit()
    {
        if(orbitPeriod < 0.1f)
        {
            orbitPeriod = 0.1f;
        }

        float orbitSpeed = 1f / orbitPeriod;
        while(orbitActive)
        {
            orbitProgress += Time.deltaTime * orbitSpeed;
            orbitProgress %= 1f;
            setOrbitingObjectPosition();
            yield return null;
        }
    }
}
